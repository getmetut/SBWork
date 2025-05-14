using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.OrderBase;

namespace Sungero.ATS
{
  partial class OrderBaseServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (sberdev.SBContracts.PublicFunctions.Module.IsSystemUser())
        _obj.State.Properties.DeliveryMethod.IsRequired = false;
    }
  }

}