using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts
{
  partial class IncomingInvoiceClientHandlers
  {

    public virtual void UCNSberDevValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue != null)
      {
        var ucn = Functions.IncomingInvoice.CheckUCNProperty(_obj, e.NewValue);
        if (ucn == null)
          e.AddError(sberdev.SBContracts.IncomingInvoices.Resources.CheckUCNErr);
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      PublicFunctions.IncomingInvoice.UpdateCard(_obj);
      base.Refresh(e);
    }

    public override void MVPBaseSberDevValueInput(sberdev.SBContracts.Client.AccountingDocumentBaseMVPBaseSberDevValueInputEventArgs e)
    {
      base.MVPBaseSberDevValueInput(e);
      if ((e.NewValue != null) && (_obj.Department != null) && (e.NewValue.BudgetOwner.Department != null))
      {
        if (e.NewValue.BudgetOwner.Department != _obj.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
    }

    public override void MVZBaseSberDevValueInput(sberdev.SBContracts.Client.AccountingDocumentBaseMVZBaseSberDevValueInputEventArgs e)
    {
      base.MVZBaseSberDevValueInput(e);
      if ((e.NewValue != null) && (_obj.Department != null) && (e.NewValue.BudgetOwner.Department != null))
      {
        if (e.NewValue.BudgetOwner.Department != _obj.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
    }

    public override void DepartmentValueInput(Sungero.Docflow.Client.OfficialDocumentDepartmentValueInputEventArgs e)
    {
      base.DepartmentValueInput(e);
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable) && (_obj.MVPBaseSberDev != null))
      {
        if (e.NewValue != _obj.MVPBaseSberDev.BudgetOwner.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev ==  sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable) && (_obj.MVZBaseSberDev != null))
      {
        if (e.NewValue != _obj.MVZBaseSberDev.BudgetOwner.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
    }
  }
}