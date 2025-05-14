using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocument;

namespace sberdev.SBContracts
{
  partial class ExchangeDocumentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (SBContracts.PublicFunctions.Module.IsSystemUser())
        _obj.State.Properties.DeliveryMethod.IsRequired = false;
    }
  }

}