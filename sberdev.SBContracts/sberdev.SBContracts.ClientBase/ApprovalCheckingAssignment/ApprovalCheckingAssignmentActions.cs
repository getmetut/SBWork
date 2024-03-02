using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckingAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalCheckingAssignmentActions
  {
    public override void ForRework(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.StageSubject == "Оплата счета")
      {
        var doc = _obj.AllAttachments.FirstOrDefault();
        var incInv = SBContracts.IncomingInvoices.As(doc);
        try
        {
          if (incInv != null)
          {
            incInv.LifeCycleState = SBContracts.IncomingInvoice.LifeCycleState.Postponed;
            incInv.InternalApprovalState = SBContracts.IncomingInvoice.InternalApprovalState.OnRework;
            incInv.Save();
          }
        }
        catch (Exception exp)
        {
          Dialogs.ShowMessage(exp.ToString(), MessageType.Error);
        }
      }
      base.ForRework(e);
      
    }

    public override bool CanForRework(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanForRework(e);
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
        _obj.Complete(Result.ForRework);
        e.CloseFormAfterAction = true;
      }
    }

    public virtual bool CanAbortTask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Task.Status == Sungero.Workflow.Task.Status.InProcess;
    }

    public override void Accept(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      base.Accept(e);
      
      //     var asyncAddStageToTask = sberdev.SberContracts.AsyncHandlers.AddStageToTask.Create();
      
      //      var task = sberdev.SBContracts.ApprovalTasks.Get(_obj.Task.Id);
//
      //        var stage = task.ApprovalRule.Stages
      //        //.Where(s => s.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Execution)
      //        .FirstOrDefault(s => s.Number == task.StageNumber);
      //         if (stage != null)
      //         {
      //           var OurStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
//
      //         if (OurStage != null)
      //         {
      //         if (OurStage.OneTime != null)
      //         {
      //           if (OurStage.OneTime == true)
      //           {
//
      //              var x = task.DoneStage.AddNew();
      //                  x.Stage = OurStage;
      //             _obj.Save();
      //           }
      //         }
      //         }
      //         }
      
    }

    public override bool CanAccept(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanAccept(e);
    }

  }

}