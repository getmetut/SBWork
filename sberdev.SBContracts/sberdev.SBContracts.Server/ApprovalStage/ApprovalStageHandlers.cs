using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalStage;

namespace sberdev.SBContracts
{
  partial class ApprovalStageServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.OneTime = false;
      _obj.AmountChangesSberDev = false;
      _obj.ConfirmSignSberDev = false;
    }
  }

}