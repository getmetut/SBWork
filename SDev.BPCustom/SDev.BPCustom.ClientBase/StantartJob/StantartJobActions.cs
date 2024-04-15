using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.StantartJob;

namespace SDev.BPCustom.Client
{
  partial class StantartJobActions
  {
    public virtual void ExecResult(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
        e.AddError("Необходимо написать комментаорий для отправки на доработку!");
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