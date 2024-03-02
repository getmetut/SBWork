using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts
{
  partial class ContractualDocumentClientHandlers
  {

    public override void DepartmentValueInput(Sungero.Docflow.Client.OfficialDocumentDepartmentValueInputEventArgs e)
    {
      base.DepartmentValueInput(e);
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable) && (_obj.MVPBaseSberDev != null) && (_obj.MVPBaseSberDev.BudgetOwner != null))
      {
        if (e.NewValue != _obj.MVPBaseSberDev.BudgetOwner.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Expendable) && (_obj.MVZBaseSberDev != null) && (_obj.MVZBaseSberDev.BudgetOwner != null))
      {
        if (e.NewValue != _obj.MVZBaseSberDev.BudgetOwner.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
    }

    public virtual void MVPBaseSberDevValueInput(sberdev.SBContracts.Client.ContractualDocumentMVPBaseSberDevValueInputEventArgs e)
    {
      if ((e.NewValue != null) && (_obj.Department != null) && (e.NewValue.BudgetOwner != null))
      {
        if (e.NewValue.BudgetOwner.Department != _obj.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
    }

    public virtual void MVZBaseSberDevValueInput(sberdev.SBContracts.Client.ContractualDocumentMVZBaseSberDevValueInputEventArgs e)
    {
      if ((e.NewValue != null) && (_obj.Department != null) && (e.NewValue.BudgetOwner != null))
      {
        if (e.NewValue.BudgetOwner.Department != _obj.Department)
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.DivisionDoesNotMatch);
        }
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.ContractualDocument.SetPropertiesAccess(_obj);
    }

  }
}