using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MarcetingDoc;

namespace Sungero.Custom
{
  partial class MarcetingDocClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      Functions.MarcetingDoc.UpdateControlCard(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.MarcetingDoc.UpdateControlCard(_obj);
    }

  }
}