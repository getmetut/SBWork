using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.GuaranteeLetter;

namespace sberdev.SberContracts.Shared
{
  partial class GuaranteeLetterFunctions
  {
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      
      _obj.State.Properties.ContrTypeBaseSberDev.IsEnabled = false;
      if (_obj.TotalAmount > 5000000)
        _obj.State.Properties.AddendumDocument.IsRequired = true;
      else
        _obj.State.Properties.AddendumDocument.IsRequired = false;
      _obj.State.Properties.ExternalApprovalState.IsVisible = true;
      _obj.State.Properties.ExternalApprovalState.IsEnabled = true;
    }
  }
}