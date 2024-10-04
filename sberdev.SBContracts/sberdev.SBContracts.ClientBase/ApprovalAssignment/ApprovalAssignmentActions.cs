using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;
using Sungero.Metadata;
using Sungero.Domain.Shared;


namespace sberdev.SBContracts.Client
{
  partial class ApprovalAssignmentCollectionActions
  {

    public virtual bool CanApproveSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ApproveSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      foreach(var assign in _objs)
      {
        if (Locks.TryLock(assign))
        {
          assign.Complete(SBContracts.ApprovalAssignment.Result.Approved);
        }
        Locks.Unlock(assign);
      }
    }

    public virtual bool CanReAddressSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ReAddressSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ExchangeDocumentProcessingAssignments.Resources.ReAdressEmp);
      dialog.Width = 200;
      var emp = dialog.AddSelect(sberdev.SBContracts.ExchangeDocumentProcessingAssignments.Resources.Emp, true, Sungero.Company.Employees.Null);
      if (dialog.Show() == DialogButtons.Ok)
      {
        foreach(var assign in _objs)
        {
          assign.Addressee = emp.Value;
          var task = ApprovalTasks.As(assign.Task);
          SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(assign);
          SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(task);
          task.Addressee = assign.Addressee;
          task.Save();
          assign.Complete(SBContracts.ApprovalAssignment.Result.Forward);
        }
      }
    }
  }

  partial class ApprovalAssignmentActions
  {


    public virtual void UnblockAttachSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var attach = SBContracts.OfficialDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault());
      PublicFunctions.Module.Remote.UnblockCardByDatabase(attach);
      if (attach.HasVersions == true)
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand("delete from Sungero_System_BinDataLocks where EntityId = "
                                                                 + attach.LastVersion.Id.ToString() + " and EntityTypeGuid = '"
                                                                 + attach.LastVersion.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "'");
    }

    public virtual bool CanUnblockAttachSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Company.Employees.Current.IncludedIn(Roles.GetAll(r => r.Sid == SberContracts.PublicConstants.Module.AdminButtonsUserRoleGuid).FirstOrDefault());
    }

    public override void Approved(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var stage = SBContracts.ApprovalStages.As(_obj.Stage);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var invoice = SBContracts.IncomingInvoices.As(attach);
      if (stage != null && invoice != null && stage.SidSberDev == SBContracts.PublicConstants.Docflow.ApprovalTask.AccountantDZKZStage
          && invoice.Counterparty != null && invoice.Counterparty.Nonresident.HasValue && invoice.Counterparty.Nonresident.Value)
      {
        var date = PublicFunctions.ApprovalAssignment.ShowPrepayMaturityDateDialog(_obj);
        if (date != null)
        {
          SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(invoice);
          invoice.PrepayMaturityDateSberDev = date;
          invoice.Save();
        }
        else
          e.AddError(sberdev.SBContracts.ApprovalAssignments.Resources.AdvancePrepaymentDateError);;
      }
      base.Approved(e);
    }

    public override bool CanApproved(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanApproved(e);
    }



    public virtual void AbortTask(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Подтверждение");
      var abortingReason = dialog.AddMultilineString("Причина прекращения", true, _obj.ActiveText);
      if (dialog.Show() == DialogButtons.Ok)
      {
        _obj.AbortingReason = abortingReason.Value;
        _obj.NeedAbort = true;
        _obj.ActiveText = abortingReason.Value;
        _obj.Save();
        _obj.Complete(Result.ForRevision);
        e.CloseFormAfterAction = true;
      }
      
    }

    public virtual bool CanAbortTask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Task.Status == Sungero.Workflow.Task.Status.InProcess;
    }

    public virtual void AmountChangesberdev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.AmountChanges = true;

      _obj.Complete(Result.Approved);
      e.CloseFormAfterAction = true;
      //_obj.State.Controls.Control.Refresh();
      
      
      
    }
    
    public virtual bool CanAmountChangesberdev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }
}

