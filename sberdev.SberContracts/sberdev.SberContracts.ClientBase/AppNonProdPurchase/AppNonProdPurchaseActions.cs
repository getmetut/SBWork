using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts.Client
{
  partial class AppNonProdPurchaseActions
  {
    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.State.Properties.Counterparty.IsRequired = false;
      base.Register(e);
    }

    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRegister(e);
    }

  }

}