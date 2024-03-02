using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts.Shared
{
  partial class PurchaseFunctions
  {
  public override void SetPropertiesAccess()
  {
    base.SetPropertiesAccess();
    
    _obj.State.Properties.Counterparty.IsRequired = false;
    _obj.State.Properties.Subject.IsRequired = false;
    _obj.State.Properties.DeliveryMethod.IsRequired = false; 
  }
  }
}