using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts
{
  partial class AppNonProdPurchaseSharedHandlers
  {

    public override void CalculationBaseSberDevChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.CalculationBaseSberDevChanged(e);
      Functions.AppNonProdPurchase.UpdateCard(_obj);
    }

  }
}