using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;


namespace sberdev.SBContracts.Client
{
  partial class ApprovalAssignmentActions
  {
    public override void Approved(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      base.Approved(e);
    }

    public override bool CanApproved(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanApproved(e);
    }



    public virtual void AbortTask(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Подтверждение");
      var abortingReason = dialog.AddMultilineString("Причина прекращения", true, _obj.ActiveText);
      if (dialog.Show() == DialogButtons.Ok)
      {
        _obj.AbortingReason = abortingReason.Value;
        _obj.NeedAbort = true;
        _obj.ActiveText = abortingReason.Value;
        _obj.Save();
        _obj.Complete(Result.ForRevision);
        e.CloseFormAfterAction = true;
      }
      
    }

    public virtual bool CanAbortTask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Task.Status == Sungero.Workflow.Task.Status.InProcess; 
    }

    public virtual void AmountChangesberdev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.AmountChanges = true;

      _obj.Complete(Result.Approved);
      e.CloseFormAfterAction = true;
     //_obj.State.Controls.Control.Refresh();
                                 
          
          
    }
    
    public virtual bool CanAmountChangesberdev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }
}

