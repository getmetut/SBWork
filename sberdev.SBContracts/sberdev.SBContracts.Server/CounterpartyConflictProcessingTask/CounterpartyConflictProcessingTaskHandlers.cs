using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.CounterpartyConflictProcessingTask;

namespace sberdev.SBContracts
{
  partial class CounterpartyConflictProcessingTaskServerHandlers
  {

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      var member = Roles.GetAll().Where(r => r.Sid == SberContracts.PublicConstants.Module.CpSyncConflictRoleGuid).FirstOrDefault()?.
        RecipientLinks.FirstOrDefault().Member;
      var emp = Sungero.Company.Employees.As(member);
      if (emp != null)
        _obj.Assignee = emp;
      base.Saving(e);
    }
  }

}