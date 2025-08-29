using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractsApprovalRule;

namespace sberdev.SBContracts
{
  partial class ContractsApprovalRuleCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
      var rule = ContractsApprovalRules.As(e.Entity);
      
      rule.ExpendableSberDev = _source.ExpendableSberDev;
      rule.ExpendProfitSberDev = _source.ExpendProfitSberDev;
      rule.ProfitableSberDev = _source.ProfitableSberDev;
      rule.TypicalSberDev = _source.TypicalSberDev;
    }
  }

  partial class ContractsApprovalRuleServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.ExpendableSberDev = false;
      _obj.ExpendProfitSberDev = false;
      _obj.ProfitableSberDev = false;
      _obj.TypicalSberDev = false;
    }
  }

}