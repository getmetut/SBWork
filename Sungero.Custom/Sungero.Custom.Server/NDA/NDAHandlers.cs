using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDA;

namespace Sungero.Custom
{
  partial class NDAServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.NFP = false;
      _obj.KAForm = false;
      _obj.Typing = false;
    }
  }

}