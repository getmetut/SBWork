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
    }
  }
}