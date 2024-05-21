using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.StandartAssignment;

namespace Sungero.Custom.Client
{
  partial class StandartAssignmentActions
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

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}