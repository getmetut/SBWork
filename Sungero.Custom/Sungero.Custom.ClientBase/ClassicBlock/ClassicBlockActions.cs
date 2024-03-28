using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.ClassicBlock;

namespace Sungero.Custom.Client
{
  partial class ClassicBlockActions
  {
    public virtual void Ispoln(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Employee == null)
      {
        _obj.State.Properties.Employee.HighlightColor = Colors.Common.LightYellow;
        e.AddError("Чтобы назначить исполнителя - укажите его на карточке задания!");
      }
    }

    public virtual bool CanIspoln(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Canceled(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanCanceled(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void GoDoWork(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanGoDoWork(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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