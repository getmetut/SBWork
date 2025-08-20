using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.ProtocolBaseAssignment;

namespace SDev.BPCustom.Client
{
  partial class ProtocolBaseAssignmentActions
  {
    public virtual void ReDirect(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Assigner == null)
      {
        _obj.State.Properties.Assigner.HighlightColor = Colors.Common.Red;
        e.AddError("Для передачи задания другом уответственному лицу, укажите сотрудника для переадрессации.");
      }
    }

    public virtual bool CanReDirect(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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