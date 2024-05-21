using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MarcetingDoc;

namespace Sungero.Custom
{
  partial class MarcetingDocDevicesActionSharedHandlers
  {

    public virtual void DevicesActionPromoCurrencyChanged(Sungero.Custom.Shared.MarcetingDocDevicesActionPromoCurrencyChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.PriceNoPrCurrency = e.NewValue;
    }

    public virtual void DevicesActionProductsAndDevicesChanged(Sungero.Custom.Shared.MarcetingDocDevicesActionProductsAndDevicesChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.MarcetingDoc.Channels.AddNew().ProductsAndDevices = e.NewValue;
    }

    public virtual void DevicesActionKolVoDevicesChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.PromoCurrency = _obj.PromoCurrency = Sungero.Commons.Currencies.Get(1);
        _obj.PriceNoPrCurrency = _obj.PriceNoPrCurrency = Sungero.Commons.Currencies.Get(1);
      }
      else
      {
        _obj.PromoCurrency = null;
        _obj.PriceNoPrCurrency = null;
      }
    }
  }

  partial class MarcetingDocChannelsSharedHandlers
  {

    public virtual void ChannelsKomissionChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.KommissCurrency = _obj.KommissCurrency = Sungero.Commons.Currencies.Get(1);
      else
        _obj.KommissCurrency = null;
    }
  }

  partial class MarcetingDocSharedHandlers
  {

    public virtual void ChannelsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      ControlKol();
    }

    public virtual void DevicesActionChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      ControlKol();
    }
    
    public void ControlKol()
    {
      double kv = 0.0;
      if (_obj.DevicesAction.Count > 0)
      {        
        foreach (var elem in _obj.DevicesAction)
        {
          if (elem.KolVoDevices != null)
            kv+= elem.KolVoDevices.Value;
        }
      }
      if (_obj.Channels.Count > 0)
      {
        foreach (var elem in _obj.Channels)
        {
          if (elem.KolVoChannels != null)
            kv-= elem.KolVoChannels.Value;
        }        
      }
      _obj.CtrlKolvo = kv;
    }

    public virtual void ActionNameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.Name = e.NewValue + " ID" + _obj.Id.ToString();
    }

    public virtual void StartDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (e.NewValue < Calendar.Today)
        {
          Sungero.Custom.PublicFunctions.Module.MSG("Дата начала не может быть меньше текущего дня!");
          _obj.StartDate = Calendar.Today;
        }
        
        if (_obj.EndDate != null)
        {
          if (e.NewValue >= _obj.EndDate)
          {
            Sungero.Custom.PublicFunctions.Module.MSG("Дата начала не может быть больше даты окончания!");
            _obj.StartDate = _obj.EndDate;
          }
        }
      }
    }

    public virtual void EndDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (_obj.StartDate != null)
      {
        if (e.NewValue <= _obj.StartDate)
        {
          Sungero.Custom.PublicFunctions.Module.MSG("Дата окончания не может быть меньше даты начала!");
          _obj.EndDate = null;         
        }
      }
    }

    public virtual void MarketingKindChanged(Sungero.Custom.Shared.MarcetingDocMarketingKindChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.State.Properties.MarketingSubKind.IsEnabled = true;
      }
      else
      {
        _obj.State.Properties.MarketingSubKind.IsEnabled = false;
        _obj.MarketingSubKind = null;
      }
    }
  }

}