using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.NaPodpisanie;

namespace SDev.BPCustom.Client
{
  partial class NaPodpisanieActions
  {
    public virtual void DoWork(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.ActiveText == null)
        e.AddError("Заполните поле комментария для отправки задания на доработку!");
    }

    public virtual bool CanDoWork(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (!_obj.BaseAttachments.ContractualDocuments.FirstOrDefault().LastVersionApproved.Value)
        e.AddError("Выполнение задания без подписания документа утверждающей подписью невозможно!");
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}