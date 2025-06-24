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

    /// <summary>
    /// Обновление карточки документа
    /// </summary>       
    public void UpdateCard()
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        _obj.State.Properties.DDS.IsRequired = true;
        _obj.State.Properties.ZIP.IsRequired = true;
        _obj.State.Properties.ResponsibleForProduction.IsRequired = true;
        _obj.State.Properties.ResponsibleSourcingManager.IsRequired = true;
        _obj.State.Properties.PaymentMethod.IsRequired = true;
      }
    }
    
    /// <summary>
    /// Устанавливает значение false для всех свойств SelectedCounterparty кроме указанного
    /// </summary>
    /// <param name="arg">Аргумент события изменения значения</param>
    /// <param name="propertyName">Имя измененного свойства</param>
    [Public]
    public void ResetOtherCounterpartySelections(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs arg, string propertyName)
    {
      // Проверяем, что новое значение - true
      if (arg.NewValue != true)
        return;
      
      // Проверяем, что имя свойства не пустое и начинается с SelectedCounterparty
      if (string.IsNullOrEmpty(propertyName) || !propertyName.StartsWith("SelectedCounterparty"))
        return;
      
      try
      {
        Logger.DebugFormat("Обрабатываем изменение свойства: {0}", propertyName);
        
        // Получаем все свойства контрагентов кроме указанного
        var counterpartyProps = new List<string>();
        for (int i = 1; i <= 7; i++)
        {
          var counterpartyPropName = $"SelectedCounterparty{i}";
          if (counterpartyPropName != propertyName)
            counterpartyProps.Add(counterpartyPropName);
        }
        
        // Устанавливаем значение false для всех остальных свойств
        foreach (var propName in counterpartyProps)
        {
          var propInfo = _obj.GetType().GetProperty(propName);
          if (propInfo != null)
          {
            var currentValue = (bool?)propInfo.GetValue(_obj, null);
            if (currentValue == true)
            {
              Logger.DebugFormat("Сбрасываем свойство: {0}", propName);
              propInfo.SetValue(_obj, false, null);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Ошибка при сбросе выбора контрагентов: {0}", ex.ToString());
      }
    }

    [Public]
    public string TranslatePaymentMethod(string enumValue)
    {
      switch (enumValue)
      {
        case "Agent":
          return "Агентская схема";
        case "VTB":
          return "Счет в ВТБ";
        case "Chinese":
          return "Китайское юр. лицо";
        default:
          return enumValue; // Возвращает оригинальное значение, если сопоставление не найдено
      }
    }
    
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
      properties.ContrTypeBaseSberDev.IsEnabled = false;
      
      properties.EmailSberDev.IsVisible = true;
      properties.PhoneNumberSberDev.IsVisible = true;
      properties.EmailSberDev.IsEnabled = true;
      properties.PhoneNumberSberDev.IsEnabled = true;
      
      bool isAgentScheme = _obj.PaymentMethod == PaymentMethod.Agent;
      var agentProps = new List<Sungero.Domain.Shared.IPropertyState>()
      {
        properties.AgencyFlagPAO, properties.AgencyPercent, properties.AgencyPayDate
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
      cpNum *= 3;
      
      var collections = new List<Sungero.Domain.Shared.IPropertyState>()
      {
        properties.ComparativeCollection1, properties.Counterparty1, properties.SelectedCounterparty1,
        properties.ComparativeCollection2, properties.Counterparty2, properties.SelectedCounterparty2,
        properties.ComparativeCollection3, properties.Counterparty3, properties.SelectedCounterparty3,
        properties.ComparativeCollection4, properties.Counterparty4, properties.SelectedCounterparty4,
        properties.ComparativeCollection5, properties.Counterparty5, properties.SelectedCounterparty5,
        properties.ComparativeCollection6, properties.Counterparty6, properties.SelectedCounterparty6,
        properties.ComparativeCollection7, properties.Counterparty7, properties.SelectedCounterparty7
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