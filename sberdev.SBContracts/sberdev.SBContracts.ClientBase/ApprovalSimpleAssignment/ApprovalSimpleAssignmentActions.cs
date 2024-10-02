using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSimpleAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalSimpleAssignmentActions
  {
    public override void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var incInv = IncomingInvoices.As(_obj.DocumentGroup.OfficialDocuments.First());
      var stage = SBContracts.ApprovalStages.GetAll().Where(s => s.Subject == _obj.StageSubject).FirstOrDefault();
      
      if  (stage == null || incInv == null)
        return;
      
      if (stage.SidSberDev == Constants.Docflow.ApprovalTask.ContractCheckByClerkStage)
        PublicFunctions.ApprovalSimpleAssignment.Remote.ContractCheckByClerkStageApprScript(_obj, incInv);
      
      if (stage.SidSberDev == Constants.Docflow.ApprovalTask.CheckUCNStage)
        if (!PublicFunctions.ApprovalSimpleAssignment.Remote.CheckUCNStageApprScript(_obj, incInv))
          e.AddError(sberdev.SBContracts.ApprovalSimpleAssignments.Resources.CheckUCNErr);
      
      base.Complete(e);
    }

    public override bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanComplete(e);
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
        _obj.Complete(Result.Complete);
        e.CloseFormAfterAction = true;
      }
    }

    public virtual bool CanAbortTask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Task.Status == Sungero.Workflow.Task.Status.InProcess;
    }

  }

}