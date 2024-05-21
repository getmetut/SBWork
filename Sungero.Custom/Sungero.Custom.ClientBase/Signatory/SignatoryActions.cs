using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.Signatory;

namespace Sungero.Custom.Client
{
  partial class SignatoryActions
  {
    public virtual void CancelJob(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanCancelJob(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var Empl = Sungero.Company.Employees.GetAll().Where(r => r.Login == Users.Current.Login).FirstOrDefault();
      Custom.Functions.Module.CreateJobRef(int.Parse(_obj.Task.Id.ToString()), Empl, Calendar.Now, "Подписано", _obj.ActiveText.ToString());
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}