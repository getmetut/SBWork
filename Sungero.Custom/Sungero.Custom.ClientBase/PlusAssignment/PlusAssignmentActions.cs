using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PlusAssignment;

namespace Sungero.Custom.Client
{
  partial class PlusAssignmentActions
  {
    public virtual void DoWork(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
      {
        Dialogs.ShowMessage("Необходимо добавить комментарий при отправке задания на доработку!");
        e.AddError("Необходимо добавить комментарий при отправке задания на доработку!");
      }
    }

    public virtual bool CanDoWork(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void ExecResult(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
      {
        Dialogs.ShowMessage("Необходимо добавить комментарий при наличии замечаний!");
        e.AddError("Необходимо добавить комментарий при наличии замечаний!");
      }
    }

    public virtual bool CanExecResult(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}