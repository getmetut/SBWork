using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BProc.ExchangeDocumentProcessingTask;

namespace SDev.BProc
{
  partial class ExchangeDocumentProcessingTaskClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      _obj.State.Properties.IncomingInvoiceATSDev.IsVisible = (_obj.IncomingInvoiceATSDev != null);
    }

  }
}