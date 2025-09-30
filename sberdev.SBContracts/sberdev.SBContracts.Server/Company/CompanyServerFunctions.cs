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

    [Public, Remote]
    public Sungero.Core.IZip ExportFocusCheckZipRemote()
    {
      var info = this.GetFocusCheckInfo();
      return info != null ? PublicFunctions.Module.ExportFocusCheckZip(_obj, info) : null;
    }
    
    [Public]
    public Structures.Module.IFocusCheckInfo GetFocusCheckInfo()
    {
      if (_obj.FocusCheckStatusSberDev == null)
        return null;

      var info = Structures.Module.FocusCheckInfo.Create();
      info.Status = _obj.FocusCheckStatusSberDev.Value;

      // 1) Сначала запрос к истории -> анонимная проекция -> список дат.
      var dates = _obj.History.GetAll()
        .Where(h => h.Operation.Value == new Sungero.Core.Enumeration("Checking") && h.HistoryDate.HasValue)
        .Select(h => h.HistoryDate.Value)
        .ToList();

      info.Checks = dates;

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

    [Public]
    public System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment> GetCheckingAssignments()
    {
      var result = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
      var targetCounterparty = Sungero.Parties.Counterparties.As(_obj);
      if (targetCounterparty == null)
        return result;

      // ---- Шаг 1. Только то, что реально маппится в БД (Id, StageSid, тип задачи)
      var stageSid = PublicConstants.Docflow.ApprovalTask.CheckingCPStage;

      var assignmentIds = sberdev.SBContracts.ApprovalAssignments.GetAll()
        .Where(a =>
               sberdev.SBContracts.ApprovalStages.As(a.Stage).SidSberDev == stageSid &&
               sberdev.SBContracts.ApprovalTasks.Is(a.Task) &&
               sberdev.SBContracts.ApprovalTasks.As(a.Task).DocumentTypeSungero == "Contractual")
        .Select(a => a.Id)
        .ToList();

      if (!assignmentIds.Any())
        return result;

      // ---- Шаг 2. Последовательно подгружаем сущности и работаем с DocumentGroup в памяти
      foreach (var id in assignmentIds)
      {
        sberdev.SBContracts.IApprovalAssignment assignment = null;
        try
        {
          assignment = sberdev.SBContracts.ApprovalAssignments.Get(id);
          if (assignment == null)
            continue;
 
          // Важно: доступ к группам вложений только после материализации
          var mainDoc = ContractualDocuments.As(
            assignment.DocumentGroup?.OfficialDocuments?.FirstOrDefault());

          if (mainDoc != null && mainDoc.Counterparty == targetCounterparty)
            result.Add(assignment);
        }
        catch (System.Exception ex)
        {
          // Не падаем из-за одного «битого» задания, просто логируем
          Logger.DebugFormat("GetCheckingAssignments: skip assignment {0}: {1}", id, ex.Message);
        }
      }

      return result;
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