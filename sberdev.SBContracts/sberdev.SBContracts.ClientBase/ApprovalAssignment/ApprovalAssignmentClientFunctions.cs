using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalAssignmentFunctions
  {
    [Public]
    public Nullable<DateTime> ShowPrepayMaturityDateDialog()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ApprovalAssignments.Resources.AdvancePaymentRepaymentPeriodDialog);
      var date = dialog.AddDate(sberdev.SBContracts.ApprovalAssignments.Resources.AdvancePaymentRepaymentPeriod, true);
      if (dialog.Show() == DialogButtons.Ok)
        return date.Value;
      else
        return null;
    }
  }
}