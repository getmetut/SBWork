using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts.Client
{
  partial class IncomingInvoiceFunctions
  {
    public void FillFromCashe()
    {
      SberContracts.PublicFunctions.Module.Remote.FillFromCasheSrv(_obj , Users.Current );
    }
  }
}