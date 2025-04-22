using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSigningAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalSigningAssignmentServerHandlers
  {

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      var task = sberdev.SBContracts.ApprovalTasks.As(_obj.Task);
      if (task != null)
      {
        _obj.AmountATSDevATSDev = task.AmountATSDev;
        _obj.ContractTypeATSDevATSDev = task.ContractTypeATSDev;
      }
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      PublicFunctions.OfficialDocument.Remote.ClearPublicComment(SBContracts.OfficialDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault()));
    }
  }


}