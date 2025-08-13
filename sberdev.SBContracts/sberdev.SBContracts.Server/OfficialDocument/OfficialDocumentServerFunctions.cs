using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using System.Reflection;
using System.Collections;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;
using System.IO;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Server
{
  partial class OfficialDocumentFunctions
  {
    
    #region Публичный комментарий к документу
    /// <summary>
    /// Сбрасываем статус соглаосвания (комментарий) в документе
    /// </summary>
    [Remote, Public]
    public void ClearPublicComment()
    {
      if (_obj != null && _obj.PublicCommentSberDev != null)
      {
        PublicFunctions.Module.Remote.UnblockCardByDatabase(_obj);
        _obj.PublicCommentSberDev = null;
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Функция устанавливает комментарий в документе (статус)
    /// </summary>
    [Remote]
    public void SetPublicComment(string text)
    {
      if (text != "")
      {
        _obj.PublicCommentSberDev = "Статус по задаче: " + text;
        _obj.Save();
      }
    }
    #endregion

    #region Перенос тел и подписей
    /// <summary>
    /// Переносит тело с подписями по ид
    /// </summary>
    [Public, Remote]
    public void TransferBodyWithSignatures(long idDoc)
    {
      var doc = Sungero.Docflow.OfficialDocuments.GetAll(d => d.Id == idDoc).First();
      TransferBody(doc);
      TransferExternalSignatures(doc);
    }
    
    /// <summary>
    /// Переносит тело документа
    /// </summary>
    /// <param name="idDoc"></param>
    [Remote, Public]
    public void TransferBody(Sungero.Docflow.IOfficialDocument doc)
    {
      Stream strmCommon = _obj.LastVersion.Body.Read();
      doc.CreateVersionFrom(strmCommon, _obj.LastVersion.AssociatedApplication.Extension);
      if (_obj.LastVersion.PublicBody.Size > 0)
      {
        Stream strmPublic = _obj.LastVersion.PublicBody.Read();
        doc.LastVersion.PublicBody.Write(strmPublic);
        strmPublic.Close();
      }
      doc.Save();
      strmCommon.Close();
    }
    
    /// <summary>
    /// Переносит внешние подписи последней версии документа
    /// </summary>
    /// <param name="id"></param>
    [Remote, Public]
    public void TransferExternalSignatures(Sungero.Docflow.IOfficialDocument doc)
    {
      var signInfos = Signatures.Get(_obj.LastVersion);
      foreach(var signInfo in signInfos)
      {
        var signaturesBytes = signInfo.GetDataSignature();
        if (signInfo.IsExternal.HasValue && signInfo.IsExternal.Value)
          Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, signInfo.SigningDate, doc.LastVersion);
      }
    }
    #endregion

    #region Разное
    /// <summary>
    /// Обнуляет флаг ручной проверки
    /// </summary>
    [Remote, Public]
    public void NullingManuallyChecked()
    {
      _obj.InternalApprovalState = null;
      _obj.ExternalApprovalState = null;
    }
    #endregion
    
    #region Функции фиксации изменения свойств (с кешами)

    #region Заготовки
    private static readonly System.Collections.Generic.HashSet<string> SkipProps =
      new System.Collections.Generic.HashSet<string>(new[] {
                                                       "Modified","ModifiedBy","Created","CreatedBy","ModifiedSberDev",
                                                       "LastVersion","VersionId","LocationState"
                                                     });
    
    private static readonly System.Collections.Generic.HashSet<string> SkipPropsForModifiedSberDev =
      new System.Collections.Generic.HashSet<string>(new[] {
                                                       "Modified","ModifiedBy","Created","CreatedBy","ModifiedSberDev",
                                                       "LastVersion","VersionId","LocationState"
                                                     });

    // ==== КЭШИ ====
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, Sungero.Metadata.EntityMetadata> _mdCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, Sungero.Metadata.EntityMetadata>();

    // (Type-of-StateProperties, PropertyName) -> PropertyInfo
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Tuple<System.Type,string>, System.Reflection.PropertyInfo> _statePropCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Tuple<System.Type,string>, System.Reflection.PropertyInfo>();

    // PropertyState<T> caches
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo> _propStateIsChangedCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo>();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo> _propStateOriginalCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo>();

    // Collection state caches
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo> _collStateIsChangedCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo>();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo> _collStateAddedCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo>();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo> _collStateRemovedCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo>();

    // Enumerator caches
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.MethodInfo> _getEnumeratorCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.MethodInfo>();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.MethodInfo> _moveNextCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.MethodInfo>();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo> _currentPropCache =
      new System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Reflection.PropertyInfo>();

    // Настройка усечения длинных значений
    private const int TRUNCATE_LEN = 120;
    private const bool LOG_PER_LINE = true;
    private const bool SUPPRESS_SCALARS_ON_CREATE = true; // true — не логировать скаляры при создании
    private const bool SHOW_CREATE_AS_FROM_NULL   = false; // если SUPPRESS=false: false => "Field = New", true => "null -> New"
    #endregion
    
    #region Основные функции

    public virtual void LoggingPropertyChanges(System.Collections.Generic.List<string> changes)
    {
      if (changes == null || changes.Count == 0) return;

      var header = $"CHANGE LOG: {_obj.GetType().GetFinalType().FullName} Id={_obj.Id}";

      if (LOG_PER_LINE)
      {
        Logger.Debug(header);
        foreach (var line in changes)
          Logger.Debug("  " + line);
      }
      else
      {
        var sb = new System.Text.StringBuilder().AppendLine(header);
        foreach (var line in changes) sb.AppendLine(line);
        Logger.Debug(sb.ToString());
      }
    }
    
    /// <summary>
    /// Если в объекте есть изменения, не попадающие в исключения, проставить ModifiedSberDev = Calendar.Now.
    /// </summary>
    public virtual void UpdateModifiedSberDevIfAnyChange()
    {
      if (HasMeaningfulChanges(_obj, SkipPropsForModifiedSberDev))
        _obj.ModifiedSberDev = Calendar.Now;
    }

    /// <summary>
    /// Быстрая проверка: изменилось ли хоть одно свойство (скаляр/коллекция), не входящее в skipProps.
    /// </summary>
    private static bool HasMeaningfulChanges(
      Sungero.Domain.Shared.IEntity entity,
      System.Collections.Generic.ISet<string> skipProps)
    {
      if (entity == null)
        return false;

      // Ранняя отсечка на уровне всей сущности
      if (!entity.State.IsChanged)
        return false;

      var objType = entity.GetType().GetFinalType();
      var md = _mdCache.GetOrAdd(objType, t => t.GetEntityMetadata());

      foreach (var pm in md.Properties)
      {
        // --- Коллекции ---
        var colMeta = pm as Sungero.Metadata.CollectionPropertyMetadata;
        if (colMeta != null)
        {
          // State коллекции
          var stateProps = entity.State.Properties;
          var spi = SafeGetPropertyByName(stateProps, colMeta.Name);
          if (spi == null)
            continue;

          var collState = spi.GetValue(stateProps);
          if (collState == null)
            continue;

          // Любой из быстрых признаков — достаточно, чтобы считать "что-то изменилось"
          var collIsChangedPi = GetCollStateIsChanged(collState.GetType());
          if (collIsChangedPi != null && (bool)collIsChangedPi.GetValue(collState))
            return true;

          if (HasAny(collState, GetCollStateAdded(collState.GetType())))
            return true;

          if (HasAny(collState, GetCollStateRemoved(collState.GetType())))
            return true;

          // Изменения внутри строк коллекции
          var current = colMeta.GetValue(entity) as System.Collections.Generic.IEnumerable<object>;
          if (HasAnyChangedItemsFast(current))
            return true;

          continue;
        }

        // --- Скаляры ---
        if (skipProps != null && skipProps.Contains(pm.Name))
          continue;

        var statePropsScalar = entity.State.Properties;
        var spScalar = SafeGetPropertyByName(statePropsScalar, pm.Name);
        if (spScalar == null)
          continue;

        var pstate = spScalar.GetValue(statePropsScalar);
        if (pstate == null)
          continue;

        // Быстрый флаг IsChanged у state свойства
        var isChangedPi = GetPropStateIsChanged(pstate.GetType());
        if (isChangedPi != null && (bool)isChangedPi.GetValue(pstate))
          return true;
      }

      return false;
    }
    
    public virtual System.Collections.Generic.List<string> GetChangedPropertiesList()
    {
      if (!_obj.State.IsChanged)
        return new System.Collections.Generic.List<string>(0);

      var result = new System.Collections.Generic.List<string>();
      var objType = _obj.GetType().GetFinalType();
      var objMetadata = _mdCache.GetOrAdd(objType, t => t.GetEntityMetadata());

      foreach (var propertyMetadata in objMetadata.Properties)
      {
        // Коллекции
        var colMeta = propertyMetadata as Sungero.Metadata.CollectionPropertyMetadata;
        if (colMeta != null)
        {
          var colChanges = GetCollectionChanges(_obj, colMeta);
          if (colChanges.Count > 0)
            result.AddRange(colChanges);
          continue;
        }

        // Скаляры
        var check = CheckScalarPropertyChange(_obj, propertyMetadata);
        if (!string.IsNullOrEmpty(check))
          result.Add(check);
      }

      return result;
    }

    /// <summary>
    /// Обычные (не коллекционные) свойства.
    /// </summary>
    public static string CheckScalarPropertyChange(Sungero.Domain.Shared.IEntity entity, Sungero.Metadata.PropertyMetadata propertyMetadata)
    {
      if (propertyMetadata is Sungero.Metadata.CollectionPropertyMetadata)
        return string.Empty;

      if (SkipProps.Contains(propertyMetadata.Name))
        return string.Empty;

      var stateProperties = entity.State.Properties;
      var statePropInfo = SafeGetPropertyByName(stateProperties, propertyMetadata.Name);
      if (statePropInfo == null)
        return string.Empty;

      var propertyState = statePropInfo.GetValue(stateProperties);
      if (propertyState == null)
        return string.Empty;

      // Подсказка: IsChanged (не жёсткий фильтр!)
      bool? isChangedFlag = null;
      var isChangedPi = GetPropStateIsChanged(propertyState.GetType());
      if (isChangedPi != null)
        isChangedFlag = (bool)isChangedPi.GetValue(propertyState);

      var newValue = propertyMetadata.GetValue(entity);

      object originalValue;
      try
      {
        var originalProp = GetPropStateOriginal(propertyState.GetType());
        if (originalProp == null)
          return string.Empty;

        originalValue = originalProp.GetValue(propertyState);
      }
      catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException is System.NotSupportedException)
      {
        // Для таких полей трекинг оригинала не поддерживается — пропускаем.
        return string.Empty;
      }

      // === Поведение на первом сохранении ===
      if (entity.State.IsInserted)
      {
        if (SUPPRESS_SCALARS_ON_CREATE)
          return string.Empty;

        if (newValue == null)
          return string.Empty;

        // Показать скаляр на создании человекочитаемо
        if (SHOW_CREATE_AS_FROM_NULL)
          return string.Format("{0} : {1} -> {2}",
                               propertyMetadata.Name,
                               "null",
                               FormatValue(newValue));
        else
          return string.Format("{0} = {1}",
                               propertyMetadata.Name,
                               FormatValue(newValue));
      }

      // Если IsChanged == false, всё ещё проверим «на всякий случай» реальное отличие
      if (isChangedFlag == false && AreValuesEqual(originalValue, newValue))
        return string.Empty;

      // Нормальный случай: выводим "оригинал -> новое"
      if (AreValuesEqual(originalValue, newValue))
        return string.Empty;

      return string.Format("{0} : {1} -> {2}",
                           propertyMetadata.Name,
                           FormatValue(originalValue),
                           FormatValue(newValue));
    }


    /// <summary>
    /// Коллекции: Added/Removed и изменения внутри строк (только скаляры).
    /// </summary>
    private static System.Collections.Generic.List<string> GetCollectionChanges(
      Sungero.Domain.Shared.IEntity entity,
      Sungero.Metadata.CollectionPropertyMetadata colMeta)
    {
      var lines = new System.Collections.Generic.List<string>();

      // Текущее значение коллекции
      var current = colMeta.GetValue(entity) as System.Collections.Generic.IEnumerable<object>;
      if (current == null)
        return lines;

      // State коллекции
      var stateProps = entity.State.Properties;
      var statePropInfo = SafeGetPropertyByName(stateProps, colMeta.Name);
      if (statePropInfo == null)
        return lines;

      var state = statePropInfo.GetValue(stateProps);
      if (state == null)
        return lines;

      // Быстрые проверки без материализации
      var collIsChangedPi = GetCollStateIsChanged(state.GetType());
      var collIsChanged = collIsChangedPi != null && (bool)collIsChangedPi.GetValue(state);

      bool hasAddedFast   = HasAny(state, GetCollStateAdded(state.GetType()));
      bool hasRemovedFast = HasAny(state, GetCollStateRemoved(state.GetType()));
      bool hasInnerChangesFast = HasAnyChangedItemsFast(current);

      if (!collIsChanged && !hasAddedFast && !hasRemovedFast && !hasInnerChangesFast)
        return lines;

      // Детализация
      var added   = ToList(state, GetCollStateAdded(state.GetType()));
      var removed = ToList(state, GetCollStateRemoved(state.GetType()));

      lines.Add(colMeta.Name + ":");

      for (int i = 0; i < added.Count; i++)
      {
        var ent = added[i] as Sungero.Domain.Shared.IEntity;
        lines.Add("  [+] " + ItemCaption(ent));
      }

      for (int i = 0; i < removed.Count; i++)
      {
        var ent = removed[i] as Sungero.Domain.Shared.IEntity;
        lines.Add("  [-] " + ItemCaption(ent));
      }

      // Изменённые элементы (кроме только что добавленных)
      foreach (var item in current)
      {
        var ent = item as Sungero.Domain.Shared.IEntity;
        if (ent == null)
          continue;
        
        // Если это новая строка (до сохранения Id == 0) — считаем как добавленную.
        if (ent.Id == 0)
        {
          lines.Add("  [+] " + ItemCaption(ent));
          continue;
        }

        if (ContainsEntity(added, ent))
          continue;

        if (ent.State.IsChanged)
        {
          var diff = GetChildEntityScalarDiff(ent);
          if (diff.Count > 0)
          {
            lines.Add("  [~] " + ItemCaption(ent));
            for (int i = 0; i < diff.Count; i++)
              lines.Add("     • " + Trunc(diff[i]));
          }
        }
      }

      return lines;
    }

    /// <summary>
    /// Дифф по скалярным свойствам элемента коллекции (без вложенных коллекций).
    /// </summary>
    private static System.Collections.Generic.List<string> GetChildEntityScalarDiff(
      Sungero.Domain.Shared.IEntity child)
    {
      var diffs = new System.Collections.Generic.List<string>();

      var type = child.GetType().GetFinalType();
      var md = _mdCache.GetOrAdd(type, t => t.GetEntityMetadata());

      foreach (var prop in md.Properties)
      {
        if (prop is Sungero.Metadata.CollectionPropertyMetadata)
          continue;

        if (SkipProps.Contains(prop.Name))
          continue;

        var stateProps = child.State.Properties;
        var spi = SafeGetPropertyByName(stateProps, prop.Name);
        if (spi == null) continue;

        var pstate = spi.GetValue(stateProps);
        if (pstate == null) continue;

        var pIsChangedPi = GetPropStateIsChanged(pstate.GetType());
        if (pIsChangedPi != null && !(bool)pIsChangedPi.GetValue(pstate))
          continue;

        var newValue = prop.GetValue(child);
        object originalValue;
        try
        {
          var op = GetPropStateOriginal(pstate.GetType());
          if (op == null)
            continue;

          originalValue = op.GetValue(pstate);
        }
        catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException is System.NotSupportedException)
        {
          continue;
        }

        if (!AreValuesEqual(newValue, originalValue))
          diffs.Add(string.Format("{0} : {1} -> {2}",
                                  prop.Name, FormatValue(originalValue), FormatValue(newValue)));
      }

      return diffs;
    }

    #endregion

    #region Хелперы

    private static System.Reflection.PropertyInfo SafeGetPropertyByName(object statePropsObj, string name)
    {
      var type = statePropsObj.GetType();
      var key = System.Tuple.Create(type, name);

      if (_statePropCache.TryGetValue(key, out var cached))
        return cached;

      // Ищем однозначно по иерархии
      var t = type;
      while (t != null)
      {
        var pi = t.GetProperty(name,
                               System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        if (pi != null)
        {
          _statePropCache[key] = pi;
          return pi;
        }
        t = t.BaseType;
      }

      _statePropCache[key] = null;
      return null;
    }

    private static System.Reflection.PropertyInfo GetPropStateIsChanged(System.Type propStateType)
    {
      if (_propStateIsChangedCache.TryGetValue(propStateType, out var pi))
        return pi;
      pi = propStateType.GetProperty("IsChanged",
                                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _propStateIsChangedCache[propStateType] = pi;
      return pi;
    }

    private static System.Reflection.PropertyInfo GetPropStateOriginal(System.Type propStateType)
    {
      if (_propStateOriginalCache.TryGetValue(propStateType, out var pi))
        return pi;
      pi = propStateType.GetProperty("OriginalValue",
                                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _propStateOriginalCache[propStateType] = pi;
      return pi;
    }

    private static System.Reflection.PropertyInfo GetCollStateIsChanged(System.Type collStateType)
    {
      if (_collStateIsChangedCache.TryGetValue(collStateType, out var pi))
        return pi;
      pi = collStateType.GetProperty("IsChanged",
                                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _collStateIsChangedCache[collStateType] = pi;
      return pi;
    }

    private static System.Reflection.PropertyInfo GetCollStateAdded(System.Type collStateType)
    {
      if (_collStateAddedCache.TryGetValue(collStateType, out var pi))
        return pi;
      pi = collStateType.GetProperty("Added",
                                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _collStateAddedCache[collStateType] = pi;
      return pi;
    }

    private static System.Reflection.PropertyInfo GetCollStateRemoved(System.Type collStateType)
    {
      if (_collStateRemovedCache.TryGetValue(collStateType, out var pi))
        return pi;
      pi = collStateType.GetProperty("Removed",
                                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _collStateRemovedCache[collStateType] = pi;
      return pi;
    }

    private static bool HasAny(object hostObj, System.Reflection.PropertyInfo collectionPi)
    {
      if (collectionPi == null || hostObj == null) return false;
      var val = collectionPi.GetValue(hostObj);
      if (val == null) return false;

      // Count — O(1), если есть
      var cntPi = val.GetType().GetProperty("Count",
                                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      if (cntPi != null)
      {
        var cntObj = cntPi.GetValue(val);
        if (cntObj is int cnt) return cnt > 0;
      }

      // Fallback: один шаг по Enumerator
      var enumerator = GetEnumerator(val);
      if (enumerator == null) return false;

      var moveNext = GetMoveNext(enumerator.GetType());
      if (moveNext == null) return false;

      return (bool)moveNext.Invoke(enumerator, null);
    }

    private static System.Collections.Generic.List<object> ToList(object hostObj, System.Reflection.PropertyInfo collectionPi)
    {
      var list = new System.Collections.Generic.List<object>();
      if (collectionPi == null || hostObj == null) return list;

      var val = collectionPi.GetValue(hostObj);
      if (val == null) return list;

      var enumerator = GetEnumerator(val);
      if (enumerator == null) return list;

      var moveNext = GetMoveNext(enumerator.GetType());
      var currentProp = GetCurrentProp(enumerator.GetType());
      if (moveNext == null || currentProp == null) return list;

      while ((bool)moveNext.Invoke(enumerator, null))
        list.Add(currentProp.GetValue(enumerator));

      return list;
    }

    private static object GetEnumerator(object enumerable)
    {
      if (enumerable == null) return null;
      var t = enumerable.GetType();

      if (_getEnumeratorCache.TryGetValue(t, out var mi))
        return mi != null ? mi.Invoke(enumerable, null) : null;

      mi = t.GetMethod("GetEnumerator",
                       System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _getEnumeratorCache[t] = mi;
      return mi != null ? mi.Invoke(enumerable, null) : null;
    }

    private static System.Reflection.MethodInfo GetMoveNext(System.Type enumeratorType)
    {
      if (_moveNextCache.TryGetValue(enumeratorType, out var mi))
        return mi;

      mi = enumeratorType.GetMethod("MoveNext",
                                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _moveNextCache[enumeratorType] = mi;
      return mi;
    }

    private static System.Reflection.PropertyInfo GetCurrentProp(System.Type enumeratorType)
    {
      if (_currentPropCache.TryGetValue(enumeratorType, out var pi))
        return pi;

      pi = enumeratorType.GetProperty("Current",
                                      System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      _currentPropCache[enumeratorType] = pi;
      return pi;
    }

    private static bool HasAnyChangedItemsFast(System.Collections.Generic.IEnumerable<object> current)
    {
      if (current == null) return false;
      foreach (var item in current)
      {
        var ent = item as Sungero.Domain.Shared.IEntity;
        if (ent != null && ent.State.IsChanged)
          return true;
      }
      return false;
    }

    private static bool ContainsEntity(System.Collections.Generic.List<object> list, Sungero.Domain.Shared.IEntity target)
    {
      if (list == null || target == null) return false;
      for (int i = 0; i < list.Count; i++)
      {
        var ent = list[i] as Sungero.Domain.Shared.IEntity;
        if (ent != null && ent.Id == target.Id)
          return true;
      }
      return false;
    }

    private static bool AreValuesEqual(object a, object b)
    {
      if (a == null && b == null) return true;
      if (a == null || b == null) return false;

      var aEnt = a as Sungero.Domain.Shared.IEntity;
      var bEnt = b as Sungero.Domain.Shared.IEntity;
      if (aEnt != null && bEnt != null)
        return aEnt.Id == bEnt.Id;

      return object.Equals(a, b);
    }

    private static string ItemCaption(Sungero.Domain.Shared.IEntity ent)
    {
      if (ent == null) return "null";
      var caption = ent.ToString();
      if (string.IsNullOrWhiteSpace(caption))
        caption = ent.GetType().GetFinalType().Name;
      return string.Format("{0} (Id={1})", caption, ent.Id);
    }

    private static string FormatValue(object v)
    {
      if (v == null) return "null";

      var ent = v as Sungero.Domain.Shared.IEntity;
      if (ent != null) return ItemCaption(ent);

      if (v is System.DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
      if (v is bool bv) return bv ? "true" : "false";

      var s = v.ToString() ?? "";
      s = s.Replace(System.Environment.NewLine, " ");
      if (s.Length > TRUNCATE_LEN)
        s = s.Substring(0, TRUNCATE_LEN) + "...";
      return s;
    }

    private static string Trunc(string s)
    {
      if (string.IsNullOrEmpty(s)) return s ?? "";
      return s.Length <= TRUNCATE_LEN ? s : s.Substring(0, TRUNCATE_LEN) + "...";
    }

    #endregion
    
    #endregion
  }
}