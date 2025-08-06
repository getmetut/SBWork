using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSendingAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalSendingAssignmentActions
  {
    public virtual void ExtendDeadlineSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ApprovalSendingAssignments.Resources.DialogTitleExtDeadline);
      var newDeadline = dialog.AddDate(sberdev.SBContracts.ApprovalSendingAssignments.Resources.DialogPropNewDeadline, true);
      if (dialog.Show() == DialogButtons.Ok)
      {
        PublicFunctions.Module.Remote.ExtendAssignmentDeadline(_obj, newDeadline.Value.Value);
      }
    }

    public virtual bool CanExtendDeadlineSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Status == Status.InProcess;
    }

  }

}