using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Contract;

namespace sberdev.SBContracts.Client
{
  partial class ContractActions
  {


    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Save(e);
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public override void ShowRelatedDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ShowRelatedDocuments(e);
    }

    public override bool CanShowRelatedDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowRelatedDocuments(e);
    }

    public override void DeleteEntity(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.DeleteEntity(e);
    }

    public override bool CanDeleteEntity(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanDeleteEntity(e);
    }


    public virtual void SendToTM(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Если по документу ранее были запущены задачи, то вывести соответствующий диалог.
      if (! Sungero.Docflow.PublicFunctions.OfficialDocument.NeedCreateApprovalTask(_obj))
        return;
      
      // Принудительно сохранить документ, чтобы сохранились связи. Иначе они не попадут в задачу.
      _obj.Save();
      
      this.CreateApprovalTask(e);
    }
    
    private void CreateApprovalTask(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var task = sberdev.SberContracts.PublicFunctions.Module.Remote.CreateApprovalTask(_obj);
      if (task.ApprovalRule != null)
      {
        task.Show();
        e.CloseFormAfterAction = true;
      }
      else
      {
        // Если по документу нет регламента, вывести сообщение.
        Dialogs.ShowMessage("Нет регламента для согласования", MessageType.Warning);
        throw new OperationCanceledException();
      }
    }
    public virtual bool CanSendToTM(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}