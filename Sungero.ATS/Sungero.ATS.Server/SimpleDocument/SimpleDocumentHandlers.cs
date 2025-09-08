using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.SimpleDocument;

namespace Sungero.ATS
{
  partial class SimpleDocumentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var isSystem = sberdev.SBContracts.PublicFunctions.Module.IsSystemUser();
      _obj.State.Properties.DeliveryMethod.IsRequired = !isSystem;
      _obj.State.Properties.CounterpartySberDev.IsRequired = !isSystem;
    }
  }

}