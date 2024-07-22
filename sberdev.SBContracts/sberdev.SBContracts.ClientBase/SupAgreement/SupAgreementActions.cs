using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts.Client
{
  partial class SupAgreementActions
  {
    public override void SendDocInTMSDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.SendDocInTMSDev(e);
    }

    public override bool CanSendDocInTMSDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }


}