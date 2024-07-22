using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.Justice;

namespace SDev.BPCustom.Client
{
  partial class JusticeActions
  {
    public virtual void Canceled(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
        e.AddError("Для выполнения задания необходимо записать комментарий!");
      
      if (_obj.ActiveText != null)
        if (_obj.ActiveText.Length < 15)
          e.AddError("Для выполнения задания необходимо записать подробный комментарий!");
    }

    public virtual bool CanCanceled(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Finance(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
        e.AddError("Для выполнения задания необходимо записать комментарий!");
    }

    public virtual bool CanFinance(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void AddDocs(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
        e.AddError("Для выполнения задания необходимо записать комментарий!");
      
      if (_obj.ActiveText.Length < 15)
        e.AddError("Для выполнения задания необходимо записать подробный комментарий!");
    }

    public virtual bool CanAddDocs(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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