using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSigningAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalSigningAssignmentCollectionActions
  {

    public virtual bool CanActionSignedSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ActionSignedSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      foreach (var job in _objs.Where(j => j.Status == SBContracts.ApprovalSigningAssignment.Status.InProcess).ToList())
      {
        job.Complete(ApprovalSigningAssignment.Result.Sign);
      }
    }
  }

}