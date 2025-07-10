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
    public override void StartBlock6(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.StartBlock6(e);
      Functions.ApprovalTask.RemoveOneTimeCompletePerformers(_obj, e);
      Functions.ApprovalTask.CancelApproveSkip(_obj, e);
      Functions.ApprovalTask.ChangeNameCheckCPStage(_obj, e);
    }

    public override void StartAssignment6(Sungero.Docflow.IApprovalAssignment assignment, Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.StartAssignment6(assignment, e);
      Functions.ApprovalTask.SetSubstitutePerformer(_obj, assignment);
      Functions.ApprovalTask.SetChangeAccessRightsOnDocs(_obj, assignment);
      Functions.ApprovalTask.RemoveCPCheckingPerformer(_obj, assignment);
      Functions.ApprovalTask.MarkCPAsChecking(_obj, assignment);
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
          Functions.ApprovalTask.AddMVZApprovers(_obj, blok);
        
        if (assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.ForRevision && assignment.CompletedBy != null)
          Functions.ApprovalTask.SaveSubstitutePerformer(_obj, assignment);
        
        if (assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.Approved)
          if (Functions.ApprovalTask.MarkSubstitutePerformerAsProcessed(_obj, assignment))
            Functions.ApprovalTask.SaveOneTimeCompletePerformer(_obj, assignment);
      }
    }

    public override void EndBlock6(Sungero.Docflow.Server.ApprovalAssignmentEndBlockEventArguments e)
    {
      base.EndBlock6(e);
      Functions.ApprovalTask.CleanupProcessedSubstitutePerformers(_obj);
    }

    public override void StartBlock31(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      base.StartBlock31(e);
      Functions.ApprovalTask.RemoveSupStagePerformer(_obj, e);
      Functions.ApprovalTask.RemoveOneTimeCompletePerformers(_obj, e);
    }

    public override void StartAssignment31(Sungero.Docflow.IApprovalCheckingAssignment assignment, Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      base.StartAssignment31(assignment, e);
      Functions.ApprovalTask.SetSubstitutePerformer(_obj, assignment);
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
        
        if (assignment.Result == Sungero.Docflow.ApprovalCheckingAssignment.Result.ForRework && assignment.CompletedBy != null)
          Functions.ApprovalTask.SaveSubstitutePerformer(_obj, assignment);
        
        if (assignment.Result == Sungero.Docflow.ApprovalCheckingAssignment.Result.Accept)
          if (Functions.ApprovalTask.MarkSubstitutePerformerAsProcessed(_obj, assignment))
            Functions.ApprovalTask.SaveOneTimeCompletePerformer(_obj, assignment);
      }
    }

    public override void EndBlock31(Sungero.Docflow.Server.ApprovalCheckingAssignmentEndBlockEventArguments e)
    {
      base.EndBlock31(e);
      Functions.ApprovalTask.CleanupProcessedSubstitutePerformers(_obj);
    }


    public override void StartBlock30(Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      base.StartBlock30(e);
      Functions.ApprovalTask.RemoveOneTimeCompletePerformers(_obj, e);
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
        Functions.ApprovalTask.SaveOneTimeCompletePerformer(_obj, assignment);
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
    
    public override void Script26Execute()
    {
      base.Script26Execute();
      sberdev.SberContracts.PublicFunctions.Module.Remote.LinkDocs(_obj);
      Functions.ApprovalTask.FillDocumentTypeOnTaskStart(_obj);
    }

    public override bool Decision32Result()
    {
      if (base.Decision32Result())
      {
        _obj.ExecutionInDaysSungero = SberContracts.PublicFunctions.Module.GetExecutionTaskTime(_obj);
        if (_obj.DoneStage.Count() > 0)
          _obj.DoneStage.Clear();
        _obj.Save();
      }
      return base.Decision32Result();
    }

  }
}
