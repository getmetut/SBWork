using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Counterparty;

namespace sberdev.SBContracts
{
  partial class CounterpartySharedHandlers
  {

    public virtual void FocusCheckedDateSberDevChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        var operation = new Enumeration("Checking");
        _obj.History.Write(operation, operation, "Контрагент был проверен");
      }
    }

  }
}