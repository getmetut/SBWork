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

    [Public]
    public string TranslatePlanDelType(string enumValue)
    {
      switch (enumValue)
      {
        case "Air":
          return "Авиа";
        case "Railway":
          return "Ж/д";
        case "Auto":
          return "Авто";
          // Добавьте остальные значения
        default:
          return enumValue; // Возвращает оригинальное значение, если сопоставление не найдено
      }
    }

    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      var properties = _obj.State.Properties;
      properties.MarketDirectSberDev.IsRequired = false;
      properties.PurchComNumberSberDev.IsRequired = false;
      properties.TotalAmount.IsRequired = false;
      properties.ContrTypeBaseSberDev.IsEnabled = false;
      
      properties.EmailSberDev.IsVisible = true;
      properties.PhoneNumberSberDev.IsVisible = true;
      
      bool isAgentScheme = _obj.FlagAgencyScheme ?? false;
      var agentProps = new List<Sungero.Domain.Shared.IPropertyState>()
      {
        properties.AgencyContract, properties.AgencyFlagPAO,
        properties.AgencyPercent, properties.AgencyPayDate
      };
      
      foreach (var prop in agentProps)
      {
        prop.IsVisible = isAgentScheme;
        prop.IsRequired = isAgentScheme;
      }
      
      var numberMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
      {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5},
        { "six", 6 },
        { "seven", 7 }
      };

      int cpNum = 0;
      if (_obj.CpNumber.HasValue)
        numberMapping.TryGetValue(_obj.CpNumber.Value.Value.ToLower(), out cpNum);
      cpNum *= 2;
      
      var collections = new List<Sungero.Domain.Shared.IPropertyState>()
      {
        properties.ComparativeCollection1, properties.Counterparty1,
        properties.ComparativeCollection2, properties.Counterparty2,
        properties.ComparativeCollection3, properties.Counterparty3,
        properties.ComparativeCollection4, properties.Counterparty4,
        properties.ComparativeCollection5, properties.Counterparty5,
        properties.ComparativeCollection6, properties.Counterparty6,
        properties.ComparativeCollection7, properties.Counterparty7
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