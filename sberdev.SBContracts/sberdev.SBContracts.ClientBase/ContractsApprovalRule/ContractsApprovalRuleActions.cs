using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractsApprovalRule;

namespace sberdev.SBContracts.Client
{
  partial class ContractsApprovalRuleActions
  {
    public override void CreateVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateVersion(e);
      var oldVersion = SBContracts.ContractsApprovalRules.As(_obj.ParentRule);
      if (oldVersion == null)
        return;
      _obj.TypicalSberDev = oldVersion.TypicalSberDev;
      _obj.ExpendableSberDev = oldVersion.ExpendableSberDev;
      _obj.ExpendProfitSberDev = oldVersion.ExpendProfitSberDev;
      _obj.ProfitableSberDev = oldVersion.ProfitableSberDev;
    }

    public override bool CanCreateVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateVersion(e);
    }

  }

}