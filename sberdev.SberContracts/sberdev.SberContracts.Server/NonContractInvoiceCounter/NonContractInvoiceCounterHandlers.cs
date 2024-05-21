using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.NonContractInvoiceCounter;

namespace sberdev.SberContracts
{
  partial class NonContractInvoiceCounterServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      _obj.Name = sberdev.SberContracts.NonContractInvoiceCounters.Resources.Name + _obj.Employee.Name
        + sberdev.SberContracts.NonContractInvoiceCounters.Resources.By + _obj.Counterparty.Name;
    }
  }


}