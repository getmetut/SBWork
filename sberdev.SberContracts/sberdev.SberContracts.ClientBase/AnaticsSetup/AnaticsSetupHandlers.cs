using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AnaticsSetup;

namespace sberdev.SberContracts
{
  partial class AnaticsSetupClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      if (_obj.DocumentType != SberContracts.AnaticsSetup.DocumentType.Contract)
      { _obj.State.Properties.ContractIsEnabled.IsVisible = true; 
        _obj.State.Properties.ContractIsRequired.IsVisible = true; 
        _obj.State.Properties.ContractIsVisible.IsVisible = true;
        _obj.State.Properties.ActExistsEnabled.IsVisible = true;
        _obj.State.Properties.ActExistsRequired.IsVisible = true;
        _obj.State.Properties.ActExistsVisible.IsVisible = true;
        _obj.State.Properties.DeviceExistsEnabled.IsVisible = true;
        _obj.State.Properties.DeviceExistsRequired.IsVisible = true;
        _obj.State.Properties.DeviceExistsVisible.IsVisible = true;
        _obj.State.Properties.FactOfPaymentEnabled.IsVisible = true;
        _obj.State.Properties.FactOfPaymentRequired.IsVisible = true;
        _obj.State.Properties.FactOfPaymentVisible.IsVisible = true;
        _obj.State.Properties.PricesAgreedEnabled.IsVisible = true;
        _obj.State.Properties.PricesAgreedRequired.IsVisible = true;
        _obj.State.Properties.PricesAgreedVisible.IsVisible = true;
        _obj.State.Properties.FrameworkEnabled.IsVisible = true;
        _obj.State.Properties.FrameworkRequired.IsVisible = true;
        _obj.State.Properties.FrameworkVisible.IsVisible = true;
        _obj.State.Properties.DocumentGroup.IsVisible = false;
     }
      else
      {
        _obj.State.Properties.ContractIsEnabled.IsVisible = false; 
        _obj.State.Properties.ContractIsRequired.IsVisible = false; 
        _obj.State.Properties.ContractIsVisible.IsVisible = false;
        _obj.State.Properties.ActExistsEnabled.IsVisible = false;
        _obj.State.Properties.ActExistsRequired.IsVisible = false;
        _obj.State.Properties.ActExistsVisible.IsVisible = false;
        _obj.State.Properties.DeviceExistsEnabled.IsVisible = false;
        _obj.State.Properties.DeviceExistsRequired.IsVisible = false;
        _obj.State.Properties.DeviceExistsVisible.IsVisible = false;
        _obj.State.Properties.FactOfPaymentEnabled.IsVisible = false;
        _obj.State.Properties.FactOfPaymentRequired.IsVisible = false;
        _obj.State.Properties.FactOfPaymentVisible.IsVisible = false;
        _obj.State.Properties.PricesAgreedEnabled.IsVisible = false;
        _obj.State.Properties.PricesAgreedRequired.IsVisible = false;
        _obj.State.Properties.PricesAgreedVisible.IsVisible = false;
        _obj.State.Properties.FrameworkEnabled.IsVisible = false;
        _obj.State.Properties.FrameworkRequired.IsVisible = false;
        _obj.State.Properties.FrameworkVisible.IsVisible = false;
      }
    }

    public virtual void DocumentTypeValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
          if (e.NewValue != null)
      {
        if (e.NewValue == SberContracts.AnaticsSetup.DocumentType.Contract)
          _obj.State.Properties.DocumentGroup.IsVisible = true;
        else
          _obj.State.Properties.DocumentGroup.IsVisible = false;
      }  
    }
  }

}