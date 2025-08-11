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
    
    #region Функции фиксации изменения свойств
    
    private System.Collections.Generic.List<string> GetPropetyExceptionList()
    {
      List<string> list = new List<string>{};
      list.Add("LocationState");
      list.Add("Modified");
      return list;
    }
    
    public virtual void LoggingPropertyChanges(System.Collections.Generic.List<string> changeList)
    {
      string log = "";
      foreach (string change in changeList)
        log += change + ";   ";
      Logger.Debug(log);
    }
    
    public virtual System.Collections.Generic.List<string> GetChangedPropertiesList()
    {
      var result = new System.Collections.Generic.List<string>();

      var objType = _obj.GetType().GetFinalType();
      var objMetadata = objType.GetEntityMetadata();

      foreach (var propertyMetadata in objMetadata.Properties)
      {
        // Коллекции обрабатываем отдельно
        if (propertyMetadata is Sungero.Metadata.CollectionPropertyMetadata colMeta)
        {
          var colChanges = GetCollectionChanges(_obj, colMeta);
          if (colChanges.Count > 0)
            result.AddRange(colChanges);
          continue;
        }

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
      
      if (propertyMetadata.Name == "LocationState" || propertyMetadata.Name == "Modified")
        return string.Empty;

      var stateProperties = entity.State.Properties;
      var statePropInfo = SafeGetPropertyByName(stateProperties, propertyMetadata.Name);
      if (statePropInfo == null)
        return string.Empty;

      var propertyState = statePropInfo.GetValue(stateProperties);
      if (propertyState == null)
        return string.Empty;

      var newValue = propertyMetadata.GetValue(entity);

      object originalValue = null;
      try
      {
        var originalProp = propertyState.GetType().GetProperty("OriginalValue",
                                                               BindingFlags.Public | BindingFlags.Instance);
        if (originalProp == null) return string.Empty;
        originalValue = originalProp.GetValue(propertyState);
      }
      catch (Exception tie) when (tie.InnerException is NotSupportedException)
      {
        return string.Empty;
      }


      if (AreValuesEqual(newValue, originalValue))
        return string.Empty;

      return string.Format("{0} : {1} -> {2}",
                           propertyMetadata.Name,
                           FormatValue(newValue),
                           FormatValue(originalValue));
    }

    /// <summary>
    /// Коллекции: Added/Removed и (опционально) изменения внутри элементов.
    /// </summary>
    private static System.Collections.Generic.List<string> GetCollectionChanges(
      Sungero.Domain.Shared.IEntity entity,
      Sungero.Metadata.CollectionPropertyMetadata colMeta)
    {
      var logs = new System.Collections.Generic.List<string>();

      // Текущее значение коллекции
      var collectionObj = colMeta.GetValue(entity);
      if (collectionObj == null)
        return logs;

      // Состояние коллекции в entity.State.Properties.<Name>
      var stateProperties = entity.State.Properties;
      var statePropInfo = SafeGetPropertyByName(stateProperties, colMeta.Name);
      if (statePropInfo == null)
        return logs;

      var collectionState = statePropInfo.GetValue(stateProperties);
      if (collectionState == null)
        return logs;

      // 1) Added / Removed — пробуем достать через рефлексию
      IEnumerable<object> addedEnum = TryGetEnumerableProperty(collectionState, "Added");
      IEnumerable<object> removedEnum = TryGetEnumerableProperty(collectionState, "Removed");

      // Добавленные
      if (addedEnum != null)
      {
        foreach (var item in addedEnum)
          logs.Add(string.Format("{0} [+] {1}", colMeta.Name, FormatEntityItem(item)));
      }

      // Удалённые
      if (removedEnum != null)
      {
        foreach (var item in removedEnum)
          logs.Add(string.Format("{0} [-] {1}", colMeta.Name, FormatEntityItem(item)));
      }

      // 2) Изменённые элементы коллекции (без добавленных/удалённых)
      // Текущее содержимое
      var currentEnum = collectionObj as IEnumerable<object>;
      if (currentEnum != null)
      {
        foreach (var item in currentEnum)
        {
          // У элемента должно быть состояние с признаком изменения
          var itemEntity = item as Sungero.Domain.Shared.IEntity;
          if (itemEntity == null)
            continue;

          // Пропустим явно "только что добавленные" — их уже залогировали как [+]
          if (IsInEnumerable(addedEnum, itemEntity))
            continue;

          // Если элемент помечен как изменённый — снимем небольшой дифф (по скалярным полям)
          if (itemEntity.State.IsChanged)
          {
            var itemDiff = GetChildEntityScalarDiff(itemEntity, colMeta);
            if (itemDiff.Count > 0)
            {
              logs.Add(string.Format("{0} [~] {1}", colMeta.Name, FormatEntityItem(itemEntity)));
              logs.AddRange(itemDiff.Select(d => "  • " + d));
            }
          }
        }
      }

      return logs;
    }

    /// <summary>
    /// Дифф по скалярным свойствам элемента коллекции (без вложенных коллекций).
    /// </summary>
    private static System.Collections.Generic.List<string> GetChildEntityScalarDiff(
      Sungero.Domain.Shared.IEntity child,
      Sungero.Metadata.CollectionPropertyMetadata parentColMeta)
    {
      var diffs = new System.Collections.Generic.List<string>();

      var type = child.GetType().GetFinalType();
      var md = type.GetEntityMetadata();
      foreach (var prop in md.Properties)
      {
        if (prop is Sungero.Metadata.CollectionPropertyMetadata) // не углубляемся вглубь
          continue;

        // У некоторых служебных полей нет трекинга — пропускаем молча
        var stateProps = child.State.Properties;
        var spi = SafeGetPropertyByName(stateProps, prop.Name);
        if (spi == null) continue;

        var pstate = spi.GetValue(stateProps);
        if (pstate == null) continue;

        var newValue = prop.GetValue(child);
        object originalValue = null;
        try
        {
          var op = pstate.GetType().GetProperty("OriginalValue");
          if (op == null)
            continue;
          originalValue = op.GetValue(pstate);
        }
        catch (Exception x)
        {
          continue;
        }

        if (!AreValuesEqual(newValue, originalValue))
        {
          diffs.Add(string.Format("{0} : {1} -> {2}",
                                  prop.Name, FormatValue(newValue), FormatValue(originalValue)));
        }
      }

      return diffs;
    }

    private static System.Collections.Generic.IEnumerable<object> TryGetEnumerableProperty(object obj, string name)
    {
      var pi = obj.GetType().GetProperty(name,
                                         BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        ?? obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
      if (pi == null) return null;

      // Приведём к IEnumerable<object> без использования необобщённого типа
      var val = pi.GetValue(obj);
      var list = new System.Collections.Generic.List<object>();
      // Идём через рефлексию по паттерну foreach (var x in (IEnumerable)val) — без упоминания необобщённого интерфейса
      var enumerator = val?.GetType().GetMethod("GetEnumerator")?.Invoke(val, null);
      if (enumerator != null)
      {
        var moveNext = enumerator.GetType().GetMethod("MoveNext");
        var currentProp = enumerator.GetType().GetProperty("Current");
        while ((bool)moveNext.Invoke(enumerator, null))
          list.Add(currentProp.GetValue(enumerator));
      }
      return list;
    }

    private static bool IsInEnumerable(IEnumerable<object> seq, Sungero.Domain.Shared.IEntity target)
    {
      if (seq == null) return false;
      foreach (var x in seq)
      {
        var ent = x as Sungero.Domain.Shared.IEntity;
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

    private static string FormatValue(object v)
    {
      if (v == null) return "null";

      var ent = v as Sungero.Domain.Shared.IEntity;
      if (ent != null)
        return string.Format("{0} (Id={1})", ent.ToString(), ent.Id);

      if (v is System.DateTime dt)
        return dt.ToString("yyyy-MM-dd HH:mm:ss");

      if (v is bool bv)
        return bv ? "true" : "false";

      return v.ToString();
    }

    private static string FormatEntityItem(object item)
    {
      var ent = item as Sungero.Domain.Shared.IEntity;
      if (ent != null)
        return string.Format("{0} (Id={1})", ent.ToString(), ent.Id);
      return item != null ? item.ToString() : "null";
    }
    
    private static System.Reflection.PropertyInfo SafeGetPropertyByName(object obj, string name)
    {
      var t = obj.GetType();
      while (t != null)
      {
        var pi = t.GetProperty(name,
                               System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        if (pi != null) return pi;
        t = t.BaseType;
      }
      return null;
    }

    #endregion
  }
}