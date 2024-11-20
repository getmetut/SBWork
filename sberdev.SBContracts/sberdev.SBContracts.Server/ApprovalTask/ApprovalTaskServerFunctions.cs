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
    public void SetReadressPerformer(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      var lastAssign = GetLastTaskAssigment(_obj, null);
      var typeedAssign = SBContracts.ApprovalCheckingAssignments.As(lastAssign);
      if (typeedAssign?.ReadressSberDev != null)
      {
        e.Block.Performers.Clear();
        e.Block.Performers.Add(typeedAssign.ReadressSberDev);
      }
      else
        e.Block.Performers.Clear();
    }
    
    [Public]
    public bool IsNecessaryStage(string sid)
    {
      return _obj.ApprovalRule.Stages
        .Where(s => s.Stage != null)
        .Any(s => s.Number == _obj.StageNumber && SBContracts.ApprovalStages.As(s.Stage).SidSberDev == sid);
    }
    
    [Public]
    public SBContracts.IApprovalStage GetStageBySid(string sid)
    {
      return SBContracts.ApprovalStages.As(_obj.ApprovalRule.Stages
        .Where(s => s.Stage != null)
        .FirstOrDefault(s => s.Number == _obj.StageNumber && SBContracts.ApprovalStages.As(s.Stage).SidSberDev == sid));
    }
    
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
    
    public override void UpdateDocumentApprovalState(Sungero.Docflow.IOfficialDocument document, Nullable<Enumeration> state)
    {
      var invoice = SBContracts.IncomingInvoices.As(document);
      if (invoice == null)
        base.UpdateDocumentApprovalState(document, state);
    }
    
    /// <summary>
    /// Функция направля
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