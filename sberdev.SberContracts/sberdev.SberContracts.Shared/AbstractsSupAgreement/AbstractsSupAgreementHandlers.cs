using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AbstractsSupAgreement;

namespace sberdev.SberContracts
{
  partial class AbstractsSupAgreementSharedHandlers
  {

    public virtual void AddendumDocumentChanged(sberdev.SberContracts.Shared.AbstractsSupAgreementAddendumDocumentChangedEventArgs e)
    {
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue != null && SBContracts.ContractualDocuments.Is(e.NewValue))
      {
        var contract = SBContracts.ContractualDocuments.As(e.NewValue);
        _obj.Counterparty = contract.Counterparty;
        _obj.BusinessUnit = contract.BusinessUnit;
        
        FillName();
        _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
        
        _obj.BudItemBaseSberDev = contract.BudItemBaseSberDev ;
        _obj.ContrTypeBaseSberDev = contract.ContrTypeBaseSberDev;
        _obj.Currency = contract.Currency;
        _obj.TotalAmount = contract.TotalAmount;
        _obj.DeliveryMethod = contract.DeliveryMethod;
        _obj.FrameworkBaseSberDev = contract.FrameworkBaseSberDev;
        
        if (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable)
        {
          _obj.AccArtPrBaseSberDev = contract.AccArtPrBaseSberDev ;
          _obj.MVPBaseSberDev = contract.MVPBaseSberDev ;
          _obj.ProdCollectionPrBaseSberDev.Clear();
          
          var collection = contract.ProdCollectionPrBaseSberDev;
          foreach (var prod in collection)
          {
            var i = _obj.ProdCollectionPrBaseSberDev.AddNew();
            i.Product = prod.Product;
          }
        }
        if (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable)
        {
          _obj.MVZBaseSberDev = contract.MVZBaseSberDev ;
          _obj.AccArtExBaseSberDev = contract.AccArtExBaseSberDev;
          _obj.ProdCollectionExBaseSberDev.Clear();
          
          var collection2 = contract.ProdCollectionExBaseSberDev;
          foreach (var dir in collection2)
          {
            var i = _obj.ProdCollectionExBaseSberDev.AddNew();
            i.Product = dir.Product;
          }
          
        }
        if (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
        {
          _obj.MVZBaseSberDev = contract.MVZBaseSberDev ;
          _obj.AccArtExBaseSberDev = contract.AccArtExBaseSberDev;
          _obj.ProdCollectionExBaseSberDev.Clear();
          
          var collection2 = contract.ProdCollectionExBaseSberDev;
          foreach (var dir in collection2)
          {
            var i = _obj.ProdCollectionExBaseSberDev.AddNew();
            i.Product = dir.Product;
          }
          
          _obj.AccArtPrBaseSberDev = contract.AccArtPrBaseSberDev ;
          _obj.MVPBaseSberDev = contract.MVPBaseSberDev ;
          _obj.ProdCollectionPrBaseSberDev.Clear();
          var collection = contract.ProdCollectionPrBaseSberDev;
          foreach (var prod in collection)
          {
            var i = _obj.ProdCollectionPrBaseSberDev.AddNew();
            i.Product = prod.Product;
          }
        }
        
        if (contract.CalculationFlagBaseSberDev.HasValue  && contract.TotalAmount.HasValue)
        {
          switch (contract.CalculationFlagBaseSberDev.Value.ToString())
          {
            case "Absolute":
              _obj.CalculationFlagBaseSberDev = SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Absolute;
              foreach(var prop in contract.CalculationBaseSberDev)
              {
                var target = _obj.CalculationBaseSberDev.AddNew();
                target.ProductCalc = prop.ProductCalc;
                target.AbsoluteCalc = prop.AbsoluteCalc;
              }
              break;
            case "Percent":
              _obj.CalculationFlagBaseSberDev = SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Percent;
              foreach(var prop in contract.CalculationBaseSberDev)
              {
                var target = _obj.CalculationBaseSberDev.AddNew();
                target.ProductCalc = prop.ProductCalc;
                target.PercentCalc = prop.PercentCalc;
              }
              break;
          }
        }
      }
    }

  }
}