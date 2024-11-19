using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts.Shared
{
  partial class AppProductPurchaseFunctions
  {

    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      _obj.State.Properties.MarketDirectSberDev.IsRequired = false;
      _obj.State.Properties.PurchComNumberSberDev.IsRequired = false;
      _obj.State.Properties.TotalAmount.IsRequired = false;
      _obj.State.Properties.Currency.IsRequired = false;
      _obj.State.Properties.ContrTypeBaseSberDev.IsEnabled = false;
      
      _obj.State.Properties.EmailSberDev.IsVisible = true;
      _obj.State.Properties.PhoneNumberSberDev.IsVisible = true;
      
      int cpNum = 0;
      if (_obj.CpNumber.HasValue)
        cpNum = Int32.Parse(_obj.CpNumber.Value.Value);
      
      var properties = _obj.State.Properties;
      var collections = new List<Sungero.Domain.Shared.IPropertyState>()
      {
        properties.ComparativeCollection1,
        properties.ComparativeCollection2,
        properties.ComparativeCollection3
      };

      // Установим значения для каждой коллекции в зависимости от cpNum
      for (int i = 0; i < collections.Count; i++)
      {
        collections[i].IsRequired = i < cpNum;
        collections[i].IsVisible = i < cpNum;
      }

    }

  }
}