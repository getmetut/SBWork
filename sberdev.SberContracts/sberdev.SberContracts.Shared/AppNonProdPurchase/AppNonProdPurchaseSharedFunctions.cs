using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts.Shared
{
  partial class AppNonProdPurchaseFunctions
  {

    /// <summary>
    /// Обновление отображения полей карточки
    /// </summary>
    public void UpdateCard()
    {
      base.UpdateCard();
      foreach (var elem in _obj.State.Properties)
      {
        elem.IsRequired = false;
      }
      
      _obj.State.Properties.PurchaseItemDescription.IsRequired = true;
      _obj.State.Properties.PurchaseJustification.IsRequired = true;
      _obj.State.Properties.ExpectedDeliveryDate.IsRequired = true;
      _obj.State.Properties.EstimatedDeliveryCost.IsRequired = true;
      _obj.State.Properties.BusinessUnit.IsRequired = true;
      
      _obj.State.Properties.CalculationFlagBaseSberDev.IsVisible = true;
      _obj.State.Properties.CalculationDistributeBaseSberDev.IsVisible = true;
      _obj.State.Properties.CalculationBaseSberDev.IsVisible = true;
      _obj.State.Properties.CalculationResidualAmountBaseSberDev.IsVisible = true;
      _obj.State.Properties.CalculationAmountBaseSberDev.IsVisible = true;
      _obj.State.Properties.MVZBaseSberDev.IsVisible = true;
      _obj.State.Properties.MVZBaseSberDev.IsEnabled = true;
      
      _obj.State.Properties.CalculationFlagBaseSberDev.IsEnabled = true;
      _obj.State.Properties.CalculationDistributeBaseSberDev.IsEnabled = true;
      _obj.State.Properties.CalculationBaseSberDev.IsEnabled = true;
      _obj.State.Properties.CalculationResidualAmountBaseSberDev.IsEnabled = true;
      _obj.State.Properties.CalculationAmountBaseSberDev.IsEnabled = true;
      _obj.State.Properties.MVPBaseSberDev.IsEnabled = true;

      var Purchaser = Roles.GetAll(r => r.Sid == Constants.Module.PurchaserBySing).FirstOrDefault();
      if (Purchaser != null)
      {
        _obj.State.Properties.MethodPurchase.IsEnabled = Users.Current.IncludedIn(Purchaser);
        _obj.State.Properties.TotalAmount.IsEnabled = Users.Current.IncludedIn(Purchaser);
        _obj.State.Properties.Economy.IsEnabled = Users.Current.IncludedIn(Purchaser);
        _obj.State.Properties.Counterparty.IsEnabled = Users.Current.IncludedIn(Purchaser);
        _obj.State.Properties.PurchaseOrderNumber.IsEnabled = Users.Current.IncludedIn(Purchaser);
        _obj.State.Properties.ValidFrom.IsEnabled = Users.Current.IncludedIn(Purchaser);
        _obj.State.Properties.AddendumDocument.IsEnabled = Users.Current.IncludedIn(Purchaser); 
      }
      
    }

  }
}