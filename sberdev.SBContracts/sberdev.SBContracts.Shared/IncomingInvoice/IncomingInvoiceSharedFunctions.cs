using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts.Shared
{
  partial class IncomingInvoiceFunctions
  {

    /// <summary>
    /// Обновление карточки докуента и открытие реквизитов по логике запроса и условий
    /// </summary>
    [Public]
    public void UpdateCard()
    {
      bool NMVisible = false;
      if (_obj.AccArtBaseSberDev != null)
      {
        if (_obj.AccArtBaseSberDev.NMA != null)
        {
          if (_obj.AccArtBaseSberDev.NMA.Value)
            NMVisible = true;
        }
      }
      _obj.State.Properties.NMASDev.IsVisible = NMVisible;
    }

    /// <summary>
    /// Проверка свойства UCN на верный формат
    /// </summary>
    [Public]
    public string CheckUCNProperty(string input)
    {
      // Проверяем, если входная строка состоит из 18 цифр
      if (input.Length == 18 && long.TryParse(input, out _))
      {
        // Формируем строку по формату "00000000/0000/0000/0/0"
        return $"{input.Substring(0, 8)}/{input.Substring(8, 4)}/{input.Substring(12, 4)}/{input.Substring(16, 1)}/{input.Substring(17, 1)}";
      }
      // Проверяем, если строка уже в нужном формате
      else if (input.Length == 22 &&
               input[8] == '/' && input[13] == '/' &&
               input[18] == '/' && input[20] == '/')
      {
        return input;
      }
      // Если строка не соответствует ни одному из форматов, возвращаем null
      else
      {
        return null;
      }
    }
    
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      
      if (_obj.NoNeedLeadingDocs.HasValue)
      {
        _obj.State.Properties.AccDocSberDev.IsRequired = _obj.NoNeedLeadingDocs.Value ? false : (_obj.PayTypeBaseSberDev == PayTypeBaseSberDev.Postpay);
        _obj.State.Properties.LeadingDocument.IsRequired = !_obj.NoNeedLeadingDocs.Value;
      }
      else
      {
        _obj.State.Properties.AccDocSberDev.IsRequired = false;
        _obj.State.Properties.LeadingDocument.IsRequired = false;
      }
      if (!PublicFunctions.Module.IsSystemUser())
      {
        _obj.State.Properties.TotalAmount.IsRequired = true;
        _obj.State.Properties.Currency.IsRequired = true;
        _obj.State.Properties.TotalAmount.IsEnabled = true;
        _obj.State.Properties.Currency.IsEnabled = true;
      }
      _obj.State.Properties.EstPaymentDateSberDev.IsRequired = false;
      _obj.State.Properties.InvoiceSberDev.IsRequired = false;
    }
  }
}