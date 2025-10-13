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
    /// Получить все задания на проверку контрагента
    /// </summary>
    /// <returns></returns>
    [Public]
    public System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment> GetCheckingAssignmentsNoLog()
    {
      var result = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
      var stageSid = PublicConstants.Docflow.ApprovalTask.CheckingCPStage;

      // Парсинг Id из строки в HashSet<long>.
      var idsSet = new System.Collections.Generic.HashSet<long>();
      var raw = (_obj.FocusCheckAssignIDsSberDev ?? string.Empty)
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
          $"[GetCheckingAssignments] НАЧАЛО. КомпанияId={companyId}, Название='{companyName}', ЭтапSid='{stageSid}'.");

        // 1) Парсим строку с Id.
        var rawIdsString = _obj != null ? (_obj.FocusCheckAssignIDsSberDev ?? string.Empty) : string.Empty;
        var raw = rawIdsString.Split(new[] { ',', ';', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        var idsSet = new System.Collections.Generic.HashSet<long>();
        foreach (var s in raw)
        {
          long id;
          if (long.TryParse(s, out id) && id > 0)
            idsSet.Add(id);
        }
        Sungero.Core.Logger.Debug(
          $"[GetCheckingAssignments] Разобрано Id: всего={raw.Length}, корректных уникальных={idsSet.Count}.");

        // 2) Получаем Id задач по компании.
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

        // 3) Базовая выборка заданий по нужному этапу.
        var swBase = System.Diagnostics.Stopwatch.StartNew();
        var baseAssignments = sberdev.SBContracts.ApprovalAssignments.GetAll()
          .Where(a => a.Task != null &&
                 taskIds.Contains(a.Task.Id) &&
                 a.Stage != null &&
                 sberdev.SBContracts.ApprovalStages.Is(a.Stage) &&
                 sberdev.SBContracts.ApprovalStages.As(a.Stage).SidSberDev == stageSid)
          .Select(a => a.Id)
          .ToList();
        swBase.Stop();
        Sungero.Core.Logger.Debug(
          $"[GetCheckingAssignments] Заданий на нужном этапе: {baseAssignments.Count} (время {swBase.ElapsedMilliseconds} мс).");

        if (baseAssignments.Count == 0)
        {
          Sungero.Core.Logger.Debug("[GetCheckingAssignments] На нужном этапе заданий нет. Возвращаю пустой список.");
          return result;
        }

        // 4) Применяем фильтр по Id, если есть.
        System.Collections.Generic.List<long> filteredIds;
        if (idsSet.Count == 0)
        {
          filteredIds = baseAssignments;
          Sungero.Core.Logger.Debug("[GetCheckingAssignments] Входной фильтр Id не задан, берём все найденные задания.");
        }
        else
        {
          const int batchSize = 500;
          Sungero.Core.Logger.Debug(
            $"[GetCheckingAssignments] Указан фильтр Id ({idsSet.Count}). Применяю пакетами по {batchSize}.");

          if (idsSet.Count <= batchSize)
          {
            filteredIds = baseAssignments.Where(id => idsSet.Contains(id)).ToList();
          }
          else
          {
            var all = new System.Collections.Generic.List<long>(idsSet.Count);
            var ids = idsSet.ToArray();
            for (int i = 0; i < ids.Length; i += batchSize)
            {
              var slice = ids.Skip(i).Take(batchSize).ToList();
              var part = baseAssignments.Where(id => slice.Contains(id)).ToList();
              all.AddRange(part);
            }
            filteredIds = all.Distinct().ToList();
          }

          Sungero.Core.Logger.Debug(
            $"[GetCheckingAssignments] После фильтра осталось {filteredIds.Count} заданий.");
        }

        if (filteredIds.Count == 0)
        {
          Sungero.Core.Logger.Debug("[GetCheckingAssignments] После фильтра ничего не осталось. Возвращаю пустой список.");
          return result;
        }

        // 5) Загружаем сущности по Id.
        var swLoad = System.Diagnostics.Stopwatch.StartNew();
        result = sberdev.SBContracts.ApprovalAssignments.GetAll()
          .Where(a => filteredIds.Contains(a.Id))
          .ToList();
        swLoad.Stop();

        Sungero.Core.Logger.Debug(
          $"[GetCheckingAssignments] Загружено сущностей: {result.Count} (время {swLoad.ElapsedMilliseconds} мс).");
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
      if (_obj.FocusCheckStatusSberDev == null)
        return null;

      var info = Structures.Module.FocusCheckInfo.Create();
      info.Status = _obj.FocusCheckStatusSberDev.Value;
      List<Enumeration> historyOpNames = new List<Enumeration>(){new Sungero.Core.Enumeration("Checking"), new Sungero.Core.Enumeration("ChangeMarkers")};

      // 1) Сначала запрос к истории -> анонимная проекция -> список дат.
      var history = _obj.History.GetAll()
        .Where(h => historyOpNames.Contains(h.Operation.Value) && h.HistoryDate.HasValue)
        .Select(h => String.Format("{0};{1}", h.HistoryDate.Value.ToShortDateString(), h.Comment))
        .ToList();

      info.History = history;

      // 2) Отдельно собираем файлы.
      info.Files = this.GetFocusFiles();

      return info;
    }

    private System.Collections.Generic.List<Sungero.Content.IElectronicDocument> GetFocusFiles()
    {
      var documents = new System.Collections.Generic.List<Sungero.Content.IElectronicDocument>();

      // Документы контрагента (CounterpartyDocuments) -> ElectronicDocument
      var cpDocs = Sungero.Docflow.CounterpartyDocuments.GetAll()
        .Where(d => d.Counterparty == Sungero.Parties.Counterparties.As(_obj))
        .Select(d => Sungero.Content.ElectronicDocuments.As(d))
        .ToList();
      documents.AddRange(cpDocs);

      // Вложения из заданий «проверки контрагента»
      var assignments = this.GetCheckingAssignments();
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