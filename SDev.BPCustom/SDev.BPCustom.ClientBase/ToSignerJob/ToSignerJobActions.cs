using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.ToSignerJob;

namespace SDev.BPCustom.Client
{
  partial class ToSignerJobActions
  {
    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Employee == null)
      {
        _obj.State.Properties.Employee.HighlightColor = Colors.Common.Red;
        e.AddError("Укажите подписанта на карточке задания перед выполнением!");
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}