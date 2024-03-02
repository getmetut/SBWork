using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;
using System.Threading;

namespace sberdev.SBContracts.Client
{
  partial class ExchangeDocumentProcessingAssignmentCollectionActions
  {

    public virtual bool CanReAddressSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ReAddressSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ExchangeDocumentProcessingAssignments.Resources.ReAdressEmp);
      dialog.Width = 200;
      var emp = dialog.AddSelect(sberdev.SBContracts.ExchangeDocumentProcessingAssignments.Resources.Emp, true, Sungero.Company.Employees.Null);
      if (dialog.Show() == DialogButtons.Ok)
      {
        foreach(var assign in _objs)
        {
          if (Locks.TryLock(assign))
          {
            assign.Addressee = emp.Value;
            assign.NewDeadline = Calendar.AddWorkingDays(assign.Deadline.Value, 2);
            // Прокинуть новый срок и исполнителя в задачу.
            var task = ExchangeDocumentProcessingTasks.As(assign.Task);
            task.Addressee = assign.Addressee;
            task.Deadline = assign.NewDeadline;
            task.Save();
            assign.Complete(SBContracts.ExchangeDocumentProcessingAssignment.Result.ReAddress);
          }
          Locks.Unlock(assign);
        }
      }
    }
  }

  partial class ExchangeDocumentProcessingAssignmentActions
  {


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
        _obj.Complete(Result.Complete);
        e.CloseFormAfterAction = true;
      }
    }

    public virtual bool CanAbortTask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}