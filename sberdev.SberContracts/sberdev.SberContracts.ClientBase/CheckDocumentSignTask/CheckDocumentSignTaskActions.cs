using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CheckDocumentSignTask;

namespace sberdev.SberContracts.Client
{
  partial class CheckDocumentSignTaskActions
  {
    public virtual bool CanGetFirstAllAttach(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void GetFirstAllAttach(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Dialogs.ShowMessage(Functions.CheckDocumentSignTask.Remote.GetFirstAllAttachment(_obj));
    }

    public virtual void GetFirstAttach(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Dialogs.ShowMessage(Functions.CheckDocumentSignTask.Remote.GetFirstAttachment(_obj));
    }

    public virtual bool CanGetFirstAttach(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}