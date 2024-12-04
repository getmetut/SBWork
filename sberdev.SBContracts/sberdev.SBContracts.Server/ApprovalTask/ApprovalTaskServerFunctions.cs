using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalTaskFunctions
  {
    public void AddMVZApprovers(Sungero.Docflow.Server.ApprovalAssignmentArguments e, SBContracts.IApprovalAssignment blok)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      var contract = SBContracts.Contracts.As(document);
      if (contract!= null)
      {
        sberdev.SberContracts.IMVZ mvz = null;
        
        if (contract.MVZBaseSberDev != null)
          mvz = sberdev.SberContracts.MVZs.GetAll().Where(l => l.Id == contract.MVZBaseSberDev.Id ).First();
        else
          mvz = sberdev.SberContracts.MVZs.GetAll().Where(l => l.Id == contract.MVPBaseSberDev.Id ).First();
        
        if (mvz != null)
        {
          var operation = new Enumeration("AddApprover");
          blok.Forward(mvz.BudgetOwner, ForwardingLocation.Next, Calendar.Today.AddWorkingDays(2));
          blok.History.Write(operation, operation, Sungero.Company.PublicFunctions.Employee.GetShortName(mvz.BudgetOwner, false));
          
          var task = ApprovalTasks.As(_obj);
          var approvalAsg = ApprovalAssignments.As(_obj);
          if (task != null && approvalAsg != null)
          {
            var approver = task.AddApproversExpanded.AddNew();
            approver.Approver = mvz.BudgetOwner;
            task.Save();
          }
        }
      }
    }
    
    public void SaveComplitedBy(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      /*if (e.Block.Res
      var lastAssign = SBContracts.ApprovalAssignments.As(GetLastTaskAssigment(_obj, null));
      if (lastAssign.CompletedBy != null)
        e.Block.*/
    }
    
    public void SetSignApproveStagePerfomer(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      var stage = GetStage();
      if (stage == null)
        return;
      if (IsNecessaryStage(PublicConstants.Docflow.ApprovalTask.SignApproveStage))
      {
        var approvers = Sungero.Docflow.PublicFunctions.ApprovalStage.Remote.GetStagePerformers(_obj, GetStage());
        foreach (var approver in approvers)
          e.Block.Performers.Add(approver);
      }
    }
    
    public void SetSupStagePerformer(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      if (IsNecessaryStage(PublicConstants.Docflow.ApprovalTask.SupplementalStage))
      {
        e.Block.Performers.Clear();
        var lastAssign = GetLastTaskAssigment(_obj, null);
        var typedAssign = SBContracts.ApprovalAssignments.As(lastAssign);
        if (typedAssign?.NeedFinanceSberDev == false)
          e.Block.Performers.Clear();
      }
    }
    
    /// <summary>
    /// Определить тот ли ето этап что нужен
    /// </summary>
    /// <param name="sid">Sid</param>
    /// <returns></returns>
    [Public]
    public bool IsNecessaryStage(string sid)
    {
      return _obj.ApprovalRule.Stages
        .Where(s => s.Stage != null)
        .Any(s => s.Number == _obj.StageNumber && SBContracts.ApprovalStages.As(s.Stage).SidSberDev == sid);
    }
    
    /// <summary>
    /// Определить текущий этап
    /// </summary>
    /// <returns></returns>
    [Public]
    public SBContracts.IApprovalStage GetStage()
    {
      return SBContracts.ApprovalStages.As(_obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber));
    }
    
    #region Механика "Выполнять один раз"
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteAdd(Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;

      if (ourStage.OneTime == true)
      {
        var newStage = _obj.DoneStage.AddNew();
        newStage.Stage = ourStage;
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteClear(Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;

      if (ourStage.OneTime == true && _obj.DoneStage.Any(r => r.Stage == ourStage))
        e.Block.Performers.Clear();
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteAdd(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;

      if (ourStage.OneTime == true)
      {
        var newStage = _obj.DoneStage.AddNew();
        newStage.Stage = ourStage;
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteClear(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;
      
      if (ourStage.OneTime == true && _obj.DoneStage.Any(r => r.Stage == ourStage))
        e.Block.Performers.Clear();
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteAdd(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;

      if (ourStage.OneTime == true)
      {
        var newStage = _obj.DoneStage.AddNew();
        newStage.Stage = ourStage;
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteClear(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;
      
      if (ourStage.OneTime == true && _obj.DoneStage.Any(r => r.Stage == ourStage))
        e.Block.Performers.Clear();
    }
    #endregion
    
    public override void UpdateDocumentApprovalState(Sungero.Docflow.IOfficialDocument document, Nullable<Enumeration> state)
    {
      var invoice = SBContracts.IncomingInvoices.As(document);
      if (invoice == null)
        base.UpdateDocumentApprovalState(document, state);
    }
    
    /// <summary>
    /// Функция создания и отправки задачи по настройке Диадока
    /// </summary>
    /// <param name="doc"></param>
    public void SendDiadocSettingsTask(Sungero.Docflow.IOfficialDocument doc)
    {
      var contr = SBContracts.ContractualDocuments.As(doc);
      SBContracts.ICounterparty cp = null;
      if (contr != null)
        cp = SBContracts.Counterparties.As(contr.Counterparty);
      else
        return;
      if (doc.DeliveryMethod == null)
        return;
      if (cp.DiadocIsSetSberDev.Value ||  doc.DeliveryMethod.Id != 1)
        return;
      if (cp.Nonresident.HasValue && cp.Nonresident.Value)
        return;
      if (cp.CanExchange.HasValue && cp.CanExchange.Value)
        return;
      var task = SberContracts.DiadocSettingsTasks.Create();
      task.Counterparty = cp;
      task.Subject = sberdev.SBContracts.ApprovalTasks.Resources.SendDiadocSettingsTaskSubject + task.Counterparty.Name + sberdev.SBContracts.ApprovalTasks.Resources.SendDiadocSettingsTaskSubject2;
      task.NeedsReview = false;
      task.Start();
    }
    
    /// <summary>
    /// Функция возвращает true если в согласуемом документе указаны закрытые аналитики
    /// </summary>
    [Public]
    public bool OldAnaliticsInDocument(Sungero.Docflow.IOfficialDocument doc)
    {
      var contractual = SBContracts.ContractualDocuments.As(doc);
      var accounting = SBContracts.AccountingDocumentBases.As(doc);
      bool flag = false;
      if (contractual != null)
      {
        if (contractual.MVPBaseSberDev != null && contractual.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (contractual.MVZBaseSberDev != null && contractual.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (contractual.AccArtExBaseSberDev != null && contractual.AccArtExBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed)
          flag = true;
        if (contractual.AccArtPrBaseSberDev != null && contractual.AccArtPrBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed)
          flag = true;
        if (contractual.MarketDirectSberDev != null && contractual.MarketDirectSberDev.Status == SberContracts.MarketingDirection.Status.Closed)
          flag = true;
        foreach (var prod in contractual.ProdCollectionExBaseSberDev)
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
            flag = true;
        foreach (var prod in contractual.ProdCollectionPrBaseSberDev)
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
            flag = true;
      }
      if (accounting != null)
      {
        if (accounting.MVPBaseSberDev != null && accounting.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (accounting.MVZBaseSberDev != null && accounting.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (accounting.AccArtBaseSberDev != null && accounting.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed)
          flag = true;
        foreach (var prod in accounting.ProdCollectionBaseSberDev)
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
            flag = true;
      }
      return flag;
    }
  }
}