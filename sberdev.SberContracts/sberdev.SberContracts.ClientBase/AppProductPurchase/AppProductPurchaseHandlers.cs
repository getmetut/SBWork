using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts
{
  partial class AppProductPurchaseClientHandlers
  {

    public virtual void CpNumberValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      
    }

    public virtual void DepositValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue > 100)
        e.AddError(sberdev.SberContracts.AppProductPurchases.Resources.DepsitError);
    }

  }
}