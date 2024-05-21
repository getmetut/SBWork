using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.StandartBlock;

namespace Sungero.Custom.Client
{
  partial class StandartBlockActions
  {
    public virtual void DoWork(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanDoWork(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var Empl = Sungero.Company.Employees.GetAll().Where(r => r.Login == Users.Current.Login).FirstOrDefault();
      Custom.Functions.Module.CreateJobRef(int.Parse(_obj.Task.Id.ToString()), Empl, Calendar.Now, "Согласовано", _obj.ActiveText.ToString());
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}