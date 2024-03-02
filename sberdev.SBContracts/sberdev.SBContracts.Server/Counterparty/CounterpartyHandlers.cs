using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Counterparty;

namespace sberdev.SBContracts
{
  partial class CounterpartyServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.DiadocIsSetSberDev = false;
    }
  }

}