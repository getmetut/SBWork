using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppRnDPurchase;

namespace sberdev.SberContracts
{
  partial class AppRnDPurchaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
    }
  }

}