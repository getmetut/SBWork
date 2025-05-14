using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.UniversalTransferDocument;

namespace Sungero.ATS
{
  partial class UniversalTransferDocumentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      if (sberdev.SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        var DelMeth = Sungero.Docflow.MailDeliveryMethods.GetAll(d => d.Name == "Не определено").FirstOrDefault();
        if (DelMeth != null)
          _obj.DeliveryMethod = DelMeth;
        
        _obj.Currency = Sungero.Commons.Currencies.Get(1);
      }
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (sberdev.SBContracts.PublicFunctions.Module.IsSystemUser())
        _obj.State.Properties.DeliveryMethod.IsRequired = false;
    }
  }

}