using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDA;

namespace Sungero.Custom
{
  partial class NDAClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      PublicFunctions.NDA.UpdateForm(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      PublicFunctions.NDA.UpdateForm(_obj);
    }

  }
}