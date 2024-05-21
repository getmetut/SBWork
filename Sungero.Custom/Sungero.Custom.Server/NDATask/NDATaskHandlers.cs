using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDATask;

namespace Sungero.Custom
{
  partial class NDATaskServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.State.Properties.Email.IsVisible = false;
      _obj.State.Properties.Email.IsRequired = false;
      _obj.State.Properties.Adress.IsVisible = false;
      _obj.State.Properties.Adress.IsRequired = false;
    }
  }

}