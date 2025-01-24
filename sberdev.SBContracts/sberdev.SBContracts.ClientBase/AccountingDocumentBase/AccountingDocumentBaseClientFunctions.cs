using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;

namespace sberdev.SBContracts.Client
{
  partial class AccountingDocumentBaseFunctions
  {
    
    public void FillFromCasheGeneral()
    {
      SberContracts.PublicFunctions.Module.Remote.FillFromCasheGeneralSrv(_obj, Users.Current);
    }
    
    public void FillFromDocument()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.AccountingDocumentBases.Resources.FillFromDocument);
      var doc = dialog.AddSelect("Документ", true, sberdev.SBContracts.OfficialDocuments.Null);
      if (dialog.Show() == DialogButtons.Ok)
        SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, doc.Value);
    }

    public void FillFromCashe()
    {
      SberContracts.PublicFunctions.Module.Remote.FillFromCasheSrv(_obj, Users.Current);
    }

    /// <summary>
    /// Показать диалог выбора типа договора
    /// </summary>
    [Public]
    public string ShowContractChangedDialog()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.AccountingDocumentBases.Resources.ShowContractChangedDialog_Title, sberdev.SBContracts.AccountingDocumentBases.Resources.ShowContractChangedDialog_Text);
      var butEx = dialog.Buttons.AddCustom(SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_Expendable);
      var butPr = dialog.Buttons.AddCustom(SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_Profitable);
      var result = dialog.Show();
      if (result == butEx)
        return "Expendable";
      if (result == butPr)
        return "Profitable";
      else
        return "";
    }
    /// <summary>
    /// Отчищает таблицу калькуляции
    /// </summary>
    public void ClearCalculation()
    {
      _obj.CalculationBaseSberDev.Clear();
    }
    
    public void ShowCalculationInstruction()
    {
      Dialogs.ShowMessage(sberdev.SberContracts.Resources.CalculationInstruction);
    }

    /// <summary>
    /// Фуннкция для кнопки распеределения
    /// </summary>
    public void DistributeTo()
    {
      if (_obj.CalculationDistributeBaseSberDev != null)
      {
        SberContracts.PublicFunctions.Module.Remote.DistributeToCalculation(_obj);
        PublicFunctions.AccountingDocumentBase.Remote.ReplaceProducts(_obj);
      }
      else
        Dialogs.ShowMessage(sberdev.SBContracts.Resources.NotSelectedDistribution, MessageType.Error);
    }
  }
}