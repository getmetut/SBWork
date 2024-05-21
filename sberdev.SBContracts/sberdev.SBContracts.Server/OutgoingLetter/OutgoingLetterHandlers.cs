using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OutgoingLetter;

namespace sberdev.SBContracts
{
  partial class OutgoingLetterServerHandlers
  {

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