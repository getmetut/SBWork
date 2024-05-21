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

    public virtual void PurchComNumberSberDevValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (e.NewValue != null)
      {
        var valArr = e.NewValue.ToArray();
        if (valArr.Length < 4)
        {
          e.AddError(sberdev.SBContracts.ContractualDocuments.Resources.PurchComNumMask);
          return;
        }
        if (valArr.Length == 4)
        {
          foreach (char val in valArr)
            if (!Char.IsDigit(val))
          {
            e.AddError(sberdev.SBContracts.ContractualDocuments.Resources.PurchComNumMask);
            return;
          }
        }
        if (valArr.Length == 5)
          for (byte i = 0; i < 5; i++)
        {
          if (i == 3 && valArr[i] != '.')
            e.AddError(sberdev.SBContracts.ContractualDocuments.Resources.PurchComNumMask);
          if (!Char.IsDigit(valArr[i]) && i != 3)
            e.AddError(sberdev.SBContracts.ContractualDocuments.Resources.PurchComNumMask);
        }
      }
    }

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
      Functions.ContractualDocument.HighlightClosedAnalitics(_obj);
    }

  }
}