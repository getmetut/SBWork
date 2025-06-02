using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts
{
  partial class AppNonProdPurchaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.AppNonProdPurchase.UpdateCard(_obj);
    }

  }
}