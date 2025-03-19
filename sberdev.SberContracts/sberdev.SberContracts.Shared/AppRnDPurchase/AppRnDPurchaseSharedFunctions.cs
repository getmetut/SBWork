using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppRnDPurchase;

namespace sberdev.SberContracts.Shared
{
  partial class AppRnDPurchaseFunctions
  {
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      var properties = _obj.State.Properties;
      properties.Currency.IsRequired = false;
      properties.DeliveryMethod.IsRequired = false;
      properties.Counterparty.IsRequired = false;
      properties.MarketDirectSberDev.IsRequired = false;
      properties.PurchComNumberSberDev.IsRequired = false;
      properties.TotalAmount.IsRequired = false;
      properties.TotalAmount.IsEnabled = false;
      properties.ContrTypeBaseSberDev.IsEnabled = false;
    }
  }
}