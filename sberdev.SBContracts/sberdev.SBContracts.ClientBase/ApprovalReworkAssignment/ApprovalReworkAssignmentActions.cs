using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalReworkAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalReworkAssignmentActions
  {
    public virtual void Reapprov(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var task = sberdev.SBContracts.ApprovalTasks.Get(_obj.Task.Id);
        task.DoneStage.Clear();
      task.Save();
      foreach (var str in _obj.Approvers)
      {
        str.Approved = sberdev.SBContracts.ApprovalReworkAssignmentApprovers.Approved.NotApproved;
        str.Action = sberdev.SBContracts.ApprovalReworkAssignmentApprovers.Action.SendForApproval;
      }
      e.CloseFormAfterAction = true;
      _obj.Complete( Result.ForReapproving);
    }

    public virtual bool CanReapprov(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}