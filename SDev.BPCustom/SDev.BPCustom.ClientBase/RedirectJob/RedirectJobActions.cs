using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.RedirectJob;

namespace SDev.BPCustom.Client
{
  partial class RedirectJobActions
  {
    public virtual void Redirect(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var dial = Dialogs.CreateInputDialog("Введите данные");
      var newEmpl = dial.AddSelect("Кому переадресовать", true, Sungero.Company.Employees.Null);
      if (dial.Show() != DialogButtons.Ok)
      {
        e.Cancel();
        return
      }
      
      _obj.NewEmployee = newEmpl.Value;
    }

    public virtual bool CanRedirect(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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