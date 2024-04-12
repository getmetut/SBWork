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
        var doc = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var incInv = SBContracts.IncomingInvoices.As(doc);
        try
        {
          if (incInv != null)
          {
            SBContracts.PublicFunctions.Module.Remote.UnblockEntityByDatabase(incInv);
            incInv.LifeCycleState = SBContracts.IncomingInvoice.LifeCycleState.Postponed;
            incInv.InternalApprovalState = SBContracts.IncomingInvoice.InternalApprovalState.OnRework;
            incInv.Save();
          }
        }
        catch (Exception exp)
        {
          Logger.Error("Этап сценария. Доработка задания Оплата счета. Ошибка: " + exp.ToString() + ". Состояние не установлено");
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

  }

}