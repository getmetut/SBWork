using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingLetter;

namespace sberdev.SBContracts
{
  partial class IncomingLetterServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (SBContracts.PublicFunctions.Module.IsSystemUser())
        _obj.State.Properties.DeliveryMethod.IsRequired = false;
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.ExBudg = false;
      _obj.Framework = false;
      _obj.ActExists = false;
      _obj.DeviceExists = false;
      _obj.FactOfPayment =false;
      _obj.PricesAgreed = false;
    }
  }

}