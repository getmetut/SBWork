using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts
{
  partial class OfficialDocumentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      if (sberdev.SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        var DelMeth = Sungero.Docflow.MailDeliveryMethods.GetAll(d => d.Name == "Не определено").FirstOrDefault();
        if (DelMeth != null)
          _obj.DeliveryMethod = DelMeth;        
      }
      else
      {
        var responsible = Sungero.Company.Employees.GetAll(r => r.Login == Users.Current.Login).FirstOrDefault();
        if (responsible != null)
          _obj.ResponsibleATSDev = responsible;
      }
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (SBContracts.PublicFunctions.Module.IsSystemUser())
        _obj.State.Properties.DeliveryMethod.IsRequired = false;
    }
  }


}