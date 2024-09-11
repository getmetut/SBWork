using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.PowerOfAttorney;

namespace Sungero.ATS
{
  partial class PowerOfAttorneyServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FirstOrDoubleSDev = false;
    }
  }

}