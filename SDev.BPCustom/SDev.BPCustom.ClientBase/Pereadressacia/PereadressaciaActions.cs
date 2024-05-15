using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.Pereadressacia;

namespace SDev.BPCustom.Client
{
  partial class PereadressaciaActions
  {
    public virtual void Pereadres(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Employee == null)
      {
        _obj.State.Properties.Employee.HighlightColor = Colors.Common.LightYellow;
        e.AddError("чтобы назначить исполнителя - укажите его на карточке задания!");
      }
    }

    public virtual bool CanPereadres(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void DoWork(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanDoWork(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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