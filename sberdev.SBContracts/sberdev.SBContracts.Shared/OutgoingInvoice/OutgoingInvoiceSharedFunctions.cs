using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OutgoingInvoice;

namespace sberdev.SBContracts.Shared
{
  partial class OutgoingInvoiceFunctions
  {
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      _obj.State.Properties.InvoiceSberDev.IsRequired = false;
    }
  }
}