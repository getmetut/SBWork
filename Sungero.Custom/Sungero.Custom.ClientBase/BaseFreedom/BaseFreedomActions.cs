using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.BaseFreedom;

namespace Sungero.Custom.Client
{
  partial class BaseFreedomActions
  {
    public virtual void ExecResult(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanExecResult(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.AssignyEmpl.Count > 0)
      {
        var task = Custom.FreedomicTasks.Get(_obj.Task.Id);
        foreach (var str in _obj.AssignyEmpl)
        {
          var elem = task.AssignyEmpl.AddNew();
          elem.Employee = str.Employee;
          elem.DogDocument = str.DogDocument;          
        }
        task.Save();
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}