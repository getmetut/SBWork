using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts
{
  partial class PurchaseClientHandlers
  {

    public virtual void PrepaymentPercentValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue > 100)
        e.AddError(sberdev.SberContracts.Purchases.Resources.DiscountError);
    }

    public virtual void NegotiationsDiscountValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue > 100)
        e.AddError(sberdev.SberContracts.Purchases.Resources.DiscountError);
    }

  }
}