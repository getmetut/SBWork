using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SignatureSetting;

namespace sberdev.SBContracts
{

  partial class SignatureSettingServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.ExpendableSberDev = false;
      _obj.ProfitableSberDev = false;
      _obj.ExpendProfitSberDev = false;
    }
  }

}