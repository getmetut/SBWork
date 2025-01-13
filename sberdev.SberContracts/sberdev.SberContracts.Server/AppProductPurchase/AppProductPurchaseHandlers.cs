using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts
{
  partial class AppProductPurchaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FlagNDA = false;
      _obj.FlagVAT = false;
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
    }
  }

}