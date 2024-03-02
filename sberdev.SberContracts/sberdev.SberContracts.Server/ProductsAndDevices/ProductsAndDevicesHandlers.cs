using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.ProductsAndDevices;

namespace sberdev.SberContracts
{
  partial class ProductsAndDevicesServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.NoDistribute = false;
    }
  }

}