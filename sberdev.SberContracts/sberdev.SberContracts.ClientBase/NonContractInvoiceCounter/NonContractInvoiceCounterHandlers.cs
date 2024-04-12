using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.NonContractInvoiceCounter;

namespace sberdev.SberContracts
{
  partial class NonContractInvoiceCounterClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      if (_obj.Counter > 3)
      {
        _obj.State.Properties.Counter.HighlightColor = Colors.Common.Red;
      }
      else
      {
        _obj.State.Properties.Counter.HighlightColor = Colors.Empty;
      }
    }

  }
}