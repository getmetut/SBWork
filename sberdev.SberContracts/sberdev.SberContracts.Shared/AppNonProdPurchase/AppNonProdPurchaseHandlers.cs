using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using System.Net;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts
{
  partial class AppNonProdPurchaseSharedHandlers
  {

    public override void NoteChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.NoteChanged(e);
      if (Users.Current != _obj.Author)
        _obj.EditPrim = ((e.NewValue != e.OldValue) && (e.NewValue != string.Empty));
      
    }

    public override void CalculationBaseSberDevChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.CalculationBaseSberDevChanged(e);
    }

  }
}