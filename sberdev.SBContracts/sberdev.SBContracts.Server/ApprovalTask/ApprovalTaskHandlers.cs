using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SBContracts.ApprovalTask;
using System.IO;

namespace sberdev.SBContracts
{

  partial class ApprovalTaskServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      // Определяем документ из группы вложений
      if (_obj.DocumentGroup != null && _obj.DocumentGroup.OfficialDocuments.Any())
      {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (document == null)
          return;
        var accounting = SBContracts.AccountingDocumentBases.As(document);
        var contractual = SBContracts.ContractualDocuments.As(document);
        var cp = Counterparties.As(accounting?.Counterparty ?? contractual?.Counterparty);
        if (cp != null && _obj.CounterpartySberDev != cp)
          _obj.CounterpartySberDev = cp;
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsNeedManuallyCheckSberDev = false;
    }

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      // Чистим завершенные задачи
      _obj.DoneStage.Clear();
      // Механика указания нового ответсвенного
      if (_obj.NewAuthorSDev != null)
        if (_obj.NewAuthorSDev.Login != null)
          _obj.Author = Users.GetAll(r => r.Login == _obj.NewAuthorSDev.Login).FirstOrDefault();

      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      base.BeforeStart(e);
      
      
      // Механика проверки обязательных свойств и заглушек
      var accounting = SBContracts.AccountingDocumentBases.As(document);
      var contractual = SBContracts.ContractualDocuments.As(document);
      string er1 = "";
      string er2 = "";
      if (accounting != null)
      {
        er1 = PublicFunctions.AccountingDocumentBase.BanToSaveForStabs(accounting);
        if (accounting.TotalAmount.HasValue)
          _obj.AmountATSDev = accounting.TotalAmount;
        
        if (accounting.ContrTypeBaseSberDev.HasValue)
        {
          if (accounting.ContrTypeBaseSberDev == SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Profitable;
          if (accounting.ContrTypeBaseSberDev == SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Expendable;
        }
      }
      if (contractual != null)
      {
        er2 = PublicFunctions.ContractualDocument.CheckAnaliticsStabs(contractual);
        if (contractual.TotalAmount.HasValue)
          _obj.AmountATSDev = contractual.TotalAmount;
        
        if (contractual.ContrTypeBaseSberDev.HasValue)
        {
          if (contractual.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Profitable;
          if (contractual.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Expendable;
          if (contractual.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.ProfExpen;
        }
      }
      if (er1 != "")
      {
        e.AddError(er1);
      }
      if (er2 != "")
      {
        e.AddError(er2);
      }
      
      // Механика подтверждения суммы закупки
      var gurantee = SberContracts.GuaranteeLetters.As(document);
      if (gurantee != null)
      {
        if (gurantee.ContrTypeBaseSberDev.HasValue)
        {
          if (gurantee.ContrTypeBaseSberDev == SberContracts.GuaranteeLetter.ContrTypeBaseSberDev.Profitable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Profitable;
          if (gurantee.ContrTypeBaseSberDev == SberContracts.GuaranteeLetter.ContrTypeBaseSberDev.Expendable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Expendable;
          if (gurantee.ContrTypeBaseSberDev == SberContracts.GuaranteeLetter.ContrTypeBaseSberDev.ExpendProfitSberDev)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.ProfExpen;
        }
      }
      if (gurantee != null && gurantee.TotalAmount < 5000000 && gurantee.AddendumDocument == null)
      {
        if (!PublicFunctions.ApprovalTask.ShowConfirmationAmountDialog(_obj))
          e.AddError(sberdev.SBContracts.ApprovalTasks.Resources.NotConfirmedPurchAmount);
      }
      Functions.ApprovalTask.SendDiadocSettingsTask(_obj, document);
      _obj.NeedAbort = false;
      
      // Механика проверки бездоговорных счетов
      var incInv = SBContracts.IncomingInvoices.As(document);
      if (incInv != null)
      {
        if (incInv.TotalAmount.HasValue)
          _obj.AmountATSDev = incInv.TotalAmount;
        
        if (incInv.ContrTypeBaseSberDev.HasValue)
        {
          if (incInv.ContrTypeBaseSberDev == SBContracts.IncomingInvoice.ContrTypeBaseSberDev.Profitable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Profitable;
          if (incInv.ContrTypeBaseSberDev == SBContracts.IncomingInvoice.ContrTypeBaseSberDev.Expendable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Expendable;
        }
      }
      if (incInv != null && incInv.NoNeedLeadingDocs.HasValue && incInv.NoNeedLeadingDocs.Value)
      {
        var counter = SberContracts.NonContractInvoiceCounters.GetAll().Where(c => c.Counterparty == incInv.Counterparty
                                                                              && c.Employee == incInv.Author).FirstOrDefault();
        if (counter == null)
        {
          counter = SberContracts.NonContractInvoiceCounters.Create();
          counter.Employee = incInv.Author;
          counter.Counterparty = incInv.Counterparty;
          counter.Counter = 1;
        }
        else if (!counter.Tasks.Select(t => SBContracts.ApprovalTasks.As(t.Task).DocumentGroup.OfficialDocuments.First()).Where(d => d.Id == incInv.Id).Any())
        {
          var task = counter.Tasks.AddNew();
          task.Task = _obj;
          counter.Counter++;
        }
      }
      
      if (SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        _obj.State.Properties.DeliveryMethod.IsRequired = false;
      }
      
      // Мезаника проверки договора в Заявке на закупку
      var appProdPurch = SberContracts.AppProductPurchases.As(document);
      if (appProdPurch != null)
      {
        if (appProdPurch.TotalAmount.HasValue)
          _obj.AmountATSDev = appProdPurch.TotalAmount;
        
        if (appProdPurch.ContrTypeBaseSberDev.HasValue)
        {
          if (appProdPurch.ContrTypeBaseSberDev == SberContracts.AppProductPurchase.ContrTypeBaseSberDev.Profitable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Profitable;
          if (appProdPurch.ContrTypeBaseSberDev == SberContracts.AppProductPurchase.ContrTypeBaseSberDev.Expendable)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.Expendable;
          if (appProdPurch.ContrTypeBaseSberDev == SberContracts.AppProductPurchase.ContrTypeBaseSberDev.ExpendProfitSberDev)
            _obj.ContractTypeATSDev = ApprovalTask.ContractTypeATSDev.ProfExpen;
        }
        bool flag = SBContracts.PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(appProdPurch.LeadingDocument);
        if (!flag)
          e.AddError(sberdev.SBContracts.ApprovalTasks.Resources.LeadocSignErr);
      }
    }

    public override void BeforeAbort(Sungero.Workflow.Server.BeforeAbortEventArgs e)
    {
      base.BeforeAbort(e);
      
      var contractual = SBContracts.ContractualDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault());
      if (contractual == null)
        return;
      
      var cp = SBContracts.Counterparties.As(contractual.Counterparty);
      if (cp == null)
        return;
      PublicFunctions.Counterparty.SetFocusCheckingValue(cp, false);
    }
  }
}