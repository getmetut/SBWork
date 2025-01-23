using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts.Client
{
  partial class ContractualDocumentFunctions
  {
    [Public]
    public string ShowOldProductsCalcWarning()
    {
      List<SberContracts.IProductsAndDevices> old = new List<SberContracts.IProductsAndDevices>();
      foreach (var prod in _obj.CalculationBaseSberDev)
        if (prod.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Closed)
          old.Add(prod.ProductCalc);
      if (old.Any())
      {
        string err = "";
        foreach (var prod in old)
          err += prod.Name + ", ";
        return "Необходимо заменить устаревшие продукты в калькуляции: " + err.Substring(0, err.Length - 2);
      }
      else
        return "";
    }

    /// <summary>
    /// Показать инструкцию к полю "Номер ЗК"
    /// </summary>       
    public void ShowPurchComNumberInstruction()
    {
      Dialogs.ShowMessage(sberdev.SBContracts.ContractualDocuments.Resources.PurchComNumberInstruction);
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
        PublicFunctions.ContractualDocument.Remote.ReplaceProducts(_obj);
      }
      else
        Dialogs.ShowMessage(sberdev.SBContracts.Resources.NotSelectedDistribution, MessageType.Error);
    }

    public void FillFromCashe()
    {
      SberContracts.PublicFunctions.Module.Remote.FillFromCasheSrv(_obj , Users.Current );
    }
    
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
  }
}