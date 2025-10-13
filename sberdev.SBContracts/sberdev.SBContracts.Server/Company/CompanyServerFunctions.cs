using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;
using Sungero.Domain.Shared;
using sberdev.SBContracts;
using Sungero.Content;

namespace sberdev.SBContracts.Server
{
  partial class CompanyFunctions
  {

    /// <summary>
    /// Сформировать архив проверки контрагента
    /// </summary>
    /// <returns></returns>
    [Public, Remote]
    public Sungero.Core.IZip ExportFocusCheckZipRemote()
    {
      var info = this.GetFocusCheckInfo();
      return info != null ? PublicFunctions.Module.ExportFocusCheckZip(_obj, info) : null;
    }

    /// <summary>
    /// Получить все задания на проверку контрагента без логов
    /// </summary>
    /// <returns></returns>
    [Public]
    public System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment> GetCheckingAssignmentsNoLog()
    {
      var result = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
      var stageSid = PublicConstants.Docflow.ApprovalTask.CheckingCPStage;

      // Парсинг Id из строки в HashSet<long>.
      var idsSet = new System.Collections.Generic.HashSet<long>();
      var raw = (_obj.ManualCheckAssignIDsSberDev ?? string.Empty)
        .Split(new[] { ',', ';', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
      foreach (var s in raw)
      {
        long id;
        if (long.TryParse(s, out id) && id > 0)
          idsSet.Add(id);
      }

      // Подзапрос по задачам: нужный тип и контрагент.
      var taskIdsQuery =
        sberdev.SBContracts.ApprovalTasks.GetAll()
        .Where(t => t.DocumentTypeSungero == "Contractual"
               && t.CounterpartySberDev != null
               && t.CounterpartySberDev.Id == _obj.Id)
        .Select(t => t.Id);

      // Базовый запрос по заданиям: нужный этап + принадлежность к выбранным задачам.
      var baseQuery =
        sberdev.SBContracts.ApprovalAssignments.GetAll()
        .Where(a =>
               sberdev.SBContracts.ApprovalStages.As(a.Stage).SidSberDev == stageSid &&
               taskIdsQuery.Contains(a.Task.Id));

      // Опциональный фильтр по списку Id (если задан).
      if (idsSet.Count == 0)
      {
        result = baseQuery.ToList();
      }
      else
      {
        const int batchSize = 500;
        if (idsSet.Count <= batchSize)
        {
          result = baseQuery.Where(a => idsSet.Contains(a.Id)).ToList();
        }
        else
        {
          var all = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>(idsSet.Count);
          var ids = idsSet.ToArray();
          for (int i = 0; i < ids.Length; i += batchSize)
          {
            var slice = ids.Skip(i).Take(batchSize).ToList();
            var part = baseQuery.Where(a => slice.Contains(a.Id)).ToList();
            all.AddRange(part);
          }

          // На случай дублей.
          result = all.GroupBy(a => a.Id).Select(g => g.First()).ToList();
        }
      }

      return result;
    }
    
    /// <summary>
    /// Получить все задания на проверку контрагента (никогда не возвращает null).
    /// </summary>
    [Public]
    public System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment> GetCheckingAssignments()
    {
      var swTotal = System.Diagnostics.Stopwatch.StartNew();
      var result = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();

      try
      {
        var stageSid = PublicConstants.Docflow.ApprovalTask.CheckingCPStage;
        var companyId = _obj != null ? _obj.Id : 0L;
        var companyName = _obj != null ? _obj.Name : string.Empty;

        Sungero.Core.Logger.Debug(
          $"[GetCheckingAssignments] НАЧАЛО. Компания Id={companyId}, Название='{companyName}', Этап Sid='{stageSid}'.");

        // 1) Получаем Id задач по компании.
        var swTasks = System.Diagnostics.Stopwatch.StartNew();
        var taskIds = sberdev.SBContracts.ApprovalTasks.GetAll()
          .Where(t => t.DocumentTypeSungero == "Contractual"
                 && t.CounterpartySberDev != null
                 && t.CounterpartySberDev.Id == companyId)
          .Select(t => t.Id)
          .ToList();
        swTasks.Stop();
        Sungero.Core.Logger.Debug(
          $"[GetCheckingAssignments] Найдено задач: {taskIds.Count} (время {swTasks.ElapsedMilliseconds} мс).");

        if (taskIds.Count == 0)
        {
          Sungero.Core.Logger.Debug("[GetCheckingAssignments] Задачи для компании не найдены. Возвращаю пустой список.");
          return result;
        }

        // 2) Берём задания на нужном этапе.
        var swBase = System.Diagnostics.Stopwatch.StartNew();
        result = sberdev.SBContracts.ApprovalAssignments.GetAll()
          .Where(a => a.Task != null &&
                 taskIds.Contains(a.Task.Id) &&
                 a.Stage != null &&
                 sberdev.SBContracts.ApprovalStages.Is(a.Stage) &&
                 sberdev.SBContracts.ApprovalStages.As(a.Stage).SidSberDev == stageSid)
          .ToList();
        swBase.Stop();

        Sungero.Core.Logger.Debug(
          $"[GetCheckingAssignments] Найдено заданий на целевом этапе: {result.Count} (время {swBase.ElapsedMilliseconds} мс).");
      }
      catch (System.Exception ex)
      {
        Sungero.Core.Logger.Error($"[GetCheckingAssignments] ОШИБКА: {ex}");
        return new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
      }
      finally
      {
        swTotal.Stop();
        Sungero.Core.Logger.Debug($"[GetCheckingAssignments] КОНЕЦ (время {swTotal.ElapsedMilliseconds} мс).");
      }

      return result;
    }



    [Public]
    public Structures.Module.IFocusCheckInfo GetFocusCheckInfo()
    {
      if (_obj.ManualCheckStatusSberDev == null)
        return null;

      var info = Structures.Module.FocusCheckInfo.Create();
      info.Status = _obj.ManualCheckStatusSberDev.Value;
      List<Enumeration> historyOpNames = new List<Enumeration>(){new Sungero.Core.Enumeration("Checking"), new Sungero.Core.Enumeration("ChangeMarkers")};

      // 1) Сначала запрос к истории -> анонимная проекция -> список дат.
      var history = _obj.History.GetAll()
        .Where(h => historyOpNames.Contains(h.Operation.Value) && h.HistoryDate.HasValue)
        .Select(h => String.Format("{0};{1}", h.HistoryDate.Value.ToShortDateString(), h.Comment))
        .ToList();

      info.History = history;

      // 2) Отдельно собираем файлы.
      info.Files = GetCheckingFiles();

      return info;
    }
    /// <summary>
    /// Получить все файлы «проверки контрагента»:
    /// - Документы контрагента.
    /// - Вложения из заданий проверки (сначала пытаемся взять кэшированные задания, иначе — живой поиск).
    /// Никогда не возвращает null.
    /// </summary>
    private System.Collections.Generic.List<Sungero.Content.IElectronicDocument> GetCheckingFiles()
    {
      var swTotal = System.Diagnostics.Stopwatch.StartNew();
      var documents = new System.Collections.Generic.List<Sungero.Content.IElectronicDocument>();

      try
      {
        var companyId = _obj != null ? _obj.Id : 0L;
        var companyName = _obj != null ? _obj.Name : string.Empty;
        Sungero.Core.Logger.Debug($"[GetCheckingFiles] НАЧАЛО. Компания Id={companyId}, Название='{companyName}'.");

        // 1) Документы контрагента (без сравнения по ссылке, только по Id).
        var swCp = System.Diagnostics.Stopwatch.StartNew();
        var cpDocs = Sungero.Docflow.CounterpartyDocuments.GetAll()
          .Where(d => d.Counterparty != null && d.Counterparty.Id == companyId)
          .Select(d => Sungero.Content.ElectronicDocuments.As(d))
          .ToList();
        swCp.Stop();
        Sungero.Core.Logger.Debug($"[GetCheckingFiles] Документов контрагента найдено: {cpDocs.Count} (время {swCp.ElapsedMilliseconds} мс).");
        documents.AddRange(cpDocs.Where(d => d != null));

        // 2) Пытаемся взять ЗАКЭШИРОВАННЫЕ задания из поля ManualCheckAssignIDsSberDev.
        var cachedAssignments = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
        var rawIds = _obj != null ? (_obj.ManualCheckAssignIDsSberDev ?? string.Empty) : string.Empty;

        if (!string.IsNullOrWhiteSpace(rawIds))
        {
          var swCache = System.Diagnostics.Stopwatch.StartNew();
          // Парсинг строки Id.
          var idsSet = new System.Collections.Generic.HashSet<long>();
          var parts = rawIds.Split(new[] { ',', ';', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
          foreach (var s in parts)
          {
            long id;
            if (long.TryParse(s, out id) && id > 0)
              idsSet.Add(id);
          }

          Sungero.Core.Logger.Debug($"[GetCheckingFiles] Кэшированные Id заданий: всего={parts.Length}, корректных уникальных={idsSet.Count}.");

          if (idsSet.Count > 0)
          {
            // Без тяжёлых IN на большие массивы — батчами.
            const int batchSize = 500;
            var ids = idsSet.ToArray();
            var loaded = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>(ids.Length);

            for (int i = 0; i < ids.Length; i += batchSize)
            {
              var slice = ids.Skip(i).Take(batchSize).ToList();
              var part = sberdev.SBContracts.ApprovalAssignments.GetAll()
                .Where(a => slice.Contains(a.Id))
                .ToList();
              loaded.AddRange(part);
            }

            // На случай дублей.
            cachedAssignments = loaded.GroupBy(a => a.Id).Select(g => g.First()).ToList();
          }

          swCache.Stop();
          Sungero.Core.Logger.Debug($"[GetCheckingFiles] Загружено кэшированных заданий: {cachedAssignments.Count} (время {swCache.ElapsedMilliseconds} мс).");
        }
        else
        {
          Sungero.Core.Logger.Debug("[GetCheckingFiles] Поле ManualCheckAssignIDsSberDev пусто — кэшированных заданий нет.");
        }

        // 3) Если кэша нет — идём живым путём.
        System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment> assignments;
        if (cachedAssignments.Count > 0)
        {
          Sungero.Core.Logger.Debug("[GetCheckingFiles] Использую кэшированные задания, вызов GetCheckingAssignments НЕ требуется.");
          assignments = cachedAssignments;
        }
        else
        {
          Sungero.Core.Logger.Debug("[GetCheckingFiles] Кэш пуст — вызываю GetCheckingAssignments().");
          assignments = GetCheckingAssignments() ?? new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
          Sungero.Core.Logger.Debug($"[GetCheckingFiles] Живых заданий получено: {assignments.Count}.");
        }

        // 4) Вложения из заданий. Разбиваем на шаги, логируем проблемные места.
        int totalAssignments = assignments.Count;
        int totalAttachments = 0, addedElectronic = 0, skippedNull = 0, skippedNonSimple = 0;

        for (int idx = 0; idx < totalAssignments; idx++)
        {
          var a = assignments[idx];
          if (a == null)
          {
            skippedNull++;
            Sungero.Core.Logger.Debug($"[GetCheckingFiles] Задание #{idx} = NULL. Пропуск.");
            continue;
          }

          // Материализуем коллекцию вложений заранее.
          System.Collections.Generic.List<Sungero.Domain.Shared.IEntity> rawAtts;
          try
          {
            // a.Attachments отдает пустую коллекцию
            rawAtts = a.AllAttachments.ToList();
          }
          catch (System.Exception ex)
          {
            Sungero.Core.Logger.Error($"[GetCheckingFiles] Ошибка чтения вложений у задания Id={a.Id}: {ex}");
            continue;
          }

          if (rawAtts == null || rawAtts.Count == 0)
            continue;

          totalAttachments += rawAtts.Count;

          foreach (var att in rawAtts)
          {
            if (att == null)
            {
              skippedNull++;
              continue;
            }
            
            // Проверяем тип документа и записываем
            if (Sungero.Docflow.SimpleDocuments.Is(att))
            {
              var eDoc = Sungero.Content.ElectronicDocuments.As(att);
              if (eDoc != null)
              {
                documents.Add(eDoc);
                addedElectronic++;
                continue;
              }
            }

            // Не простой документ — логируем тип.
            skippedNonSimple++;
            try
            {
              var typeName = att.GetType().FullName;
              Sungero.Core.Logger.Debug($"[GetCheckingFiles] Пропущено вложение нестандартного типа: {typeName}, AssignmentId={a.Id}.");
            }
            catch
            {
              Sungero.Core.Logger.Debug($"[GetCheckingFiles] Пропущено вложение непонятного типа (ошибка получения типа), AssignmentId={a.Id}.");
            }
          }
        }

        Sungero.Core.Logger.Debug(
          $"[GetCheckingFiles] Итог по вложениям: заданий обработано={totalAssignments}, всего вложений={totalAttachments}, добавлено электронных={addedElectronic}, пропущено null={skippedNull}, пропущено неэлектронных/неожиданных={skippedNonSimple}.");
      }
      catch (System.Exception ex)
      {
        Sungero.Core.Logger.Error($"[GetCheckingFiles] ОШИБКА: {ex}");
      }
      finally
      {
        swTotal.Stop();
        Sungero.Core.Logger.Debug($"[GetCheckingFiles] КОНЕЦ (время {swTotal.ElapsedMilliseconds} мс).");
      }

      return documents;
    }


    private System.Collections.Generic.List<Sungero.Content.IElectronicDocument> GetFocusFilesNoLogs()
    {
      var documents = new System.Collections.Generic.List<Sungero.Content.IElectronicDocument>();

      // Документы контрагента (CounterpartyDocuments) -> ElectronicDocument
      var cpDocs = Sungero.Docflow.CounterpartyDocuments.GetAll()
        .Where(d => d.Counterparty == Sungero.Parties.Counterparties.As(_obj))
        .Select(d => Sungero.Content.ElectronicDocuments.As(d))
        .ToList();
      documents.AddRange(cpDocs);

      // Вложения из заданий «проверки контрагента»
      var assignments = GetCheckingAssignments();
      foreach (var a in assignments)
      {
        var attachDocs = a.Attachments
          .Where(att => Sungero.Docflow.SimpleDocuments.Is(att))
          .Select(att => Sungero.Content.ElectronicDocuments.As(att))
          .ToList();
        documents.AddRange(attachDocs);
      }

      return documents;
    }

    public override void FillInRequisites(centrvd.KFIntegration.Structures.Parties.Company.CompanyInfo counterpartyInfo, bool isLegalEntity)
    {
      
      base.FillInRequisites(counterpartyInfo, isLegalEntity);
      
      var yes = new Enumeration("Yes");
      var no = new Enumeration("No");
      var req = counterpartyInfo.Req;
      var egrDetails = counterpartyInfo.EgrDetails;
      var license = counterpartyInfo.License;
      var analitycs = counterpartyInfo.Analytics;
      var reqCompanyAffiliates = counterpartyInfo.ReqCompanyAffiliates;
      var egrDetailsCompanyAffiliates = counterpartyInfo.EgrDetailsCompanyAffiliates;
      var analyticsCompanyAffiliates = counterpartyInfo.AnalyticsCompanyAffiliates;
      var petitionersOfArbitration = counterpartyInfo.PetitionersOfArbitration;
      var moratorium = counterpartyInfo.Moratorium;
      
      // Значительная сумма арбитражных дел в качестве ответчика.
      var caseAmountTo =  isLegalEntity ? (analitycs?.Analytics?.S2001 > 50000000 ? yes : no) : no;
      if (_obj.CaseAmountTo != caseAmountTo)
        _obj.CaseAmountTo = caseAmountTo;
      // Значительная сумма исполнительных производств.
      var execCaseAmount = isLegalEntity ? (analitycs?.Analytics?.S1001 > 50000000 ? yes : no) : no;
      if (_obj.ExecCaseAmount != execCaseAmount)
        _obj.ExecCaseAmount = execCaseAmount;
      // Значительное количество связанных организаций найдены в особых реестрах ФНС.
      var specialList = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M5008 == true || x?.Analytics?.M5009 == true || x?.Analytics?.M5010 == true ||
                                         x?.Analytics?.M5003 == true || x?.Analytics?.M5004 == true || x?.Analytics?.M5005 == true).
        Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.SpecialList != specialList)
        _obj.SpecialList = specialList;
      // Значительное количество связанных организаций с особыми исполнительными производствами.
      var specialExecList = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M1003 == true || x?.Analytics?.M1004 == true || x?.Analytics?.M1005 == true ||
                                         x?.Analytics?.M1006 == true || x?.Analytics?.S1007 > 0 || x?.Analytics?.S1008 > 0 ).
        Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.SpecialExecList != specialExecList)
        _obj.SpecialExecList = specialExecList;
      // Значительное количество связанных организаций, зарегистрированных менее 12 месяцев назад.
      var youngRelOrg = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M7004 == true ).Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.YoungRelOrg != youngRelOrg)
        _obj.YoungRelOrg = youngRelOrg;
      // Значительное количество связанных организаций, по которым требуется дополнительная проверка.
      var relOrgAddValid = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M7001 == true).Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.RelOrgAddValid != relOrgAddValid)
        _obj.RelOrgAddValid = relOrgAddValid;
      // Значительное количество связанных организаций, у которых за последние 12 месяцев хотя бы раз менялся директор или учредитель.
      var relOrgSwapHead = analyticsCompanyAffiliates.Where(x =>
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.Heads != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.Heads.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.ManagementCompanies != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.ManagementCompanies.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.FoundersFL != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.FoundersFL.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.FoundersUL != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.FoundersUL.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.FoundersForeign != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.FoundersForeign.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1)))).
        Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.RelOrgSwapHead != relOrgSwapHead)
        _obj.RelOrgSwapHead = relOrgSwapHead;
      // Значительное число не ликвидированных юридических лиц по тому же адресу.
      var noLiquidAddress = analitycs?.Analytics?.Q7006 > 2 ? yes : no;
      if (_obj.NoLiquidAddress != noLiquidAddress)
        _obj.NoLiquidAddress = noLiquidAddress;
      // Маленькая среднесписочная численность работников.
      var avgEmpl = analitycs?.Analytics?.Q7047 < 5 ? yes : no;
      if (_obj.LowAvgHeadSberDev != avgEmpl)
        _obj.LowAvgHeadSberDev = avgEmpl;
    }
    
  }
}