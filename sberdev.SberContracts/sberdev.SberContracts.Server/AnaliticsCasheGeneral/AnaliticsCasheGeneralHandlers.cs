using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AnaliticsCasheGeneral;

namespace sberdev.SberContracts
{
  partial class AnaliticsCasheGeneralServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Name = Guid.NewGuid().ToString();
      _obj.User = Users.Current;
    }
  }

}