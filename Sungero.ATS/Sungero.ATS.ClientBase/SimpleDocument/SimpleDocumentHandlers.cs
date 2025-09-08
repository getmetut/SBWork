using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.SimpleDocument;

namespace Sungero.ATS
{
  partial class SimpleDocumentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.SimpleDocument.SetPropertiesAccess(_obj);
    }

  }
}