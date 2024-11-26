using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractsApprovalRule;

namespace sberdev.SBContracts
{
  partial class ContractsApprovalRuleServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.ExpendableSberDev = false;
      _obj.ExpendProfitSberDev = false;
      _obj.ProfitableSberDev = false;
    }
  }

}