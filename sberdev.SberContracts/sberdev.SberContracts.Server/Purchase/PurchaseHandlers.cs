using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts
{

  partial class PurchaseServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      //удаление базового обработчика
      if (_obj.StagesPurchaseCollection.Select(p => p.Cost).Sum() != _obj.PurchaseAmount.Value)
        e.AddError(sberdev.SberContracts.Purchases.Resources.AmountError);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
      _obj.VAT = false;
    }
  }

}