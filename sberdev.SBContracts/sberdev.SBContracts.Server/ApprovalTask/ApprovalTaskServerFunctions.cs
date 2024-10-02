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
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteFunction(Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;

      if (ourStage.OneTime == true && ourStage.AllowSendToRework == false)
      {
        var isDone = _obj.DoneStage.Any(r => r.Stage == ourStage);

        if (isDone)
        {
          e.Block.Performers.Clear();
        }
        else
        {
          var newStage = _obj.DoneStage.AddNew();
          newStage.Stage = ourStage;
        }

        _obj.Save();
      }
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// </summary>
    public void OneTimeCompleteFunction(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      var stage = _obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber);

      if (stage == null) return;

      var ourStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
      
      if (ourStage == null) return;

      if (ourStage.OneTime == true && ourStage.AllowSendToRework == false)
      {
        var isDone = _obj.DoneStage.Any(r => r.Stage == ourStage);

        if (isDone)
        {
          e.Block.Performers.Clear();
        }
        else
        {
          var newStage = _obj.DoneStage.AddNew();
          newStage.Stage = ourStage;
        }

        _obj.Save();
      }
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