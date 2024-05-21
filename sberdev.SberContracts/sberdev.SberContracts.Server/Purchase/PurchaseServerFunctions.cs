using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts.Server
{
  partial class PurchaseFunctions
  {
    [Remote(IsPure = true)]
    public override StateView GetDocumentSummary()
    {
      var documentSummary = StateView.Create();
      var documentBlock = documentSummary.AddBlock();
      documentBlock.AddLabel("Предмет закупки: " + _obj.SubjectPurchase);
      documentBlock.AddLineBreak();
      documentBlock.AddLabel("Цель закупки: " + _obj.TargetPurchase);
      documentBlock.AddLineBreak();
      var budgetOwner = SBContracts.PublicFunctions.ContractualDocument.GetMVZBudgetOwner(_obj);
      if (budgetOwner != null)
        documentBlock.AddLabel("Владелец бюджета: " + budgetOwner.Name);
      else
        documentBlock.AddLabel("Владелец бюджета: Отсутвует");
      documentBlock.AddLineBreak();
      documentBlock.AddLabel("Продукты (валюта: " + _obj.Currency.DisplayValue + "): ");
      documentBlock.AddLineBreak();
      if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
        foreach(var prod in _obj.CalculationBaseSberDev)
      {
        documentBlock.AddLabel(prod.ProductCalc + " - " + prod.AbsoluteCalc.ToString() + " ");
        documentBlock.AddLineBreak();
      }
      if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Percent)
        foreach(var prod in _obj.CalculationBaseSberDev)
      {
        documentBlock.AddLabel(prod.ProductCalc + " - " + prod.InterestCalc.ToString() + " ");
        documentBlock.AddLineBreak();
      }
      if (_obj.AccArtExBaseSberDev != null)
      {
        documentBlock.AddLabel("Статья управленческого учета (рас.): " +  _obj.AccArtExBaseSberDev.Name);
        documentBlock.AddLineBreak();
      }
      if (_obj.AccArtPrBaseSberDev != null)
      {
        documentBlock.AddLabel("Статья управленческого учета (дох.): " +  _obj.AccArtPrBaseSberDev.Name);
        documentBlock.AddLineBreak();
      }
      if (_obj.MarketDirectSberDev != null)
      {
        documentBlock.AddLabel("Направление маркетинга: " +  Hyperlinks.Get(_obj.MarketDirectSberDev));
        documentBlock.AddLineBreak();
      }
      documentBlock.AddLabel("Общая сумма: " + GetTotalAmountDocumentSummary(_obj.TotalAmount) + (_obj.VAT.Value ? " с учетом НДС" : " без учсета НДС"));
      documentBlock.AddLineBreak();
      documentBlock.AddLabel("Поставщик: " + Hyperlinks.Get(_obj.Counterparty));
      documentBlock.AddLineBreak();
      documentBlock.AddLabel("Обоснование выбора поставщика: " + _obj.ChooseCpJustif);
      documentBlock.AddLineBreak();
      documentBlock.AddLabel("Условия оплаты:" + SBContracts.PublicFunctions.Module.GetPaymentType(_obj));
      documentBlock.AddLineBreak();
      return documentSummary;
    }
  }
}