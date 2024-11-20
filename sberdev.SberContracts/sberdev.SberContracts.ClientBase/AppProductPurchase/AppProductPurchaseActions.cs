using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts.Client
{
  partial class AppProductPurchaseActions
  {
    public override void CreateBodyByPropertiesSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateBodyByPropertiesSberDev(e);
    }

    public override bool CanCreateBodyByPropertiesSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

  }

}