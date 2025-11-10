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
          PublicFunctions.Module.Remote.UnblockCardByDatabase(assign);
          if (assign.Status == Status.InProcess)
          {
            Functions.ExchangeDocumentProcessingAssignment.Remote.ReadressAssign(assign, emp.Value);
            continue;
          }
          if (assign.Status == Status.Completed)
          {
            Functions.ExchangeDocumentProcessingAssignment.Remote.RestartTask(assign, emp.Value);
            continue;
          }
        }
      }
    }
  }

  partial class ExchangeDocumentProcessingAssignmentActions
  {
    public virtual void Redistribute(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var task = ExchangeDocumentProcessingTasks.As(_obj.Task);
      var responsible = SberDev.ExchDocDistrib.PublicFunctions.Module.GetBoxResponsibleEmployee(task);
      _obj.ResultIsRedistributionSberDev = true;
      Functions.ExchangeDocumentProcessingAssignment.Remote.ReadressAssign(_obj, Sungero.Company.Employees.As(responsible));
    }

    public virtual bool CanRedistribute(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      var task = ExchangeDocumentProcessingTasks.As(_obj.Task);
      var lastAssig = PublicFunctions.ApprovalTask.Remote.GetLastTaskAssigment(task, _obj.Created);
      Sungero.Company.IEmployee lastPerfrormer = null;
      if (lastAssig != null)
        lastPerfrormer = Sungero.Company.Employees.As(lastAssig.Performer);
      var neededPerformers = SberDev.ExchDocDistrib.PublicFunctions.Module.GetBoxResponsibleEmployees(_obj.Box);
      return (neededPerformers.Contains(lastPerfrormer) || task.IsAutoDistributedSberDev == true)
        && _obj.Status == Status.InProcess && task.OnRedistributionSberDev != true;
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