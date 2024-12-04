using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalTaskRouteHandlers
  {

    public override void StartBlock5(Sungero.Docflow.Server.ApprovalReworkAssignmentArguments e)
    {
      base.StartBlock5(e);
      
    }

    public override void StartBlock6(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.StartBlock6(e);
      Functions.ApprovalTask.SetSignApproveStagePerfomer(_obj, e);
      Functions.ApprovalTask.OneTimeCompleteClear(_obj, e);
    }

    public override void StartBlock9(Sungero.Docflow.Server.ApprovalSigningAssignmentArguments e)
    {
      base.StartBlock9(e);
      var stage = SBContracts.ApprovalStages.As(e.Block.Stage);
      if (stage == null)
        return;
      if (stage.ConfirmSignSberDev == true)
      {
        e.Block.Performers.Clear();
        e.Block.Performers.Add(stage.AssigneeSberDev);
      }
    }

    public override void CompleteAssignment3(Sungero.Docflow.IApprovalManagerAssignment assignment, Sungero.Docflow.Server.ApprovalManagerAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.ApprovalManagerAssignments.As(assignment);
      if ( blok.NeedAbort.GetValueOrDefault())
      {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
      }
      else
        base.CompleteAssignment3(assignment, e);
    }

    public override void CompleteAssignment6(Sungero.Docflow.IApprovalAssignment assignment, Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.ApprovalAssignments.As(assignment);
      if ( blok.NeedAbort.GetValueOrDefault())
      {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
      }
      else
      {
        base.CompleteAssignment6(assignment, e);
        if (blok.AmountChanges.GetValueOrDefault())
          Functions.ApprovalTask.AddMVZApprovers(_obj, e, blok);
        if (assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.Approved)
          Functions.ApprovalTask.OneTimeCompleteAdd(_obj, e);
      }
    }

    public override void Script26Execute()
    {
      base.Script26Execute();
      sberdev.SberContracts.PublicFunctions.Module.Remote.LinkDocs(_obj);
    }

    public override void CompleteAssignment31(Sungero.Docflow.IApprovalCheckingAssignment assignment, Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.ApprovalCheckingAssignments.As(assignment);
      if (blok.NeedAbort.GetValueOrDefault())
      {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
      }
      else
      {
        base.CompleteAssignment31(assignment, e);
        if (assignment.Result == Sungero.Docflow.ApprovalCheckingAssignment.Result.Accept)
          Functions.ApprovalTask.OneTimeCompleteAdd(_obj, e);
      }
    }

    public override void CompleteAssignment30(Sungero.Docflow.IApprovalSimpleAssignment assignment, Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.ApprovalSimpleAssignments.As(assignment);
      if (blok.NeedAbort.GetValueOrDefault())
      {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
      }
      else
        base.CompleteAssignment30(assignment, e);
      if (assignment.Result == Sungero.Docflow.ApprovalSimpleAssignment.Result.Complete)
        Functions.ApprovalTask.OneTimeCompleteAdd(_obj, e);
    }

    public override void StartBlock31(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      base.StartBlock31(e);
      Functions.ApprovalTask.SetSupStagePerformer(_obj, e);
      Functions.ApprovalTask.OneTimeCompleteClear(_obj, e);
    }

    public override void StartBlock30(Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      base.StartBlock30(e);
      Functions.ApprovalTask.OneTimeCompleteClear(_obj, e);
    }

  }
}
