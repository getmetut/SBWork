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
      {
        base.CompleteAssignment3(assignment, e);
      }
      
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
        {
          var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
          
          var contract = SBContracts.Contracts.As(document);
          if (contract!= null)
          {
            sberdev.SberContracts.IMVZ mvz = null;
            if (contract.MVZBaseSberDev != null)
            { mvz = sberdev.SberContracts.MVZs.GetAll().Where(l => l.Id == contract.MVZBaseSberDev.Id ).First();
              
            }
            else
            { mvz = sberdev.SberContracts.MVZs.GetAll().Where(l => l.Id == contract.MVPBaseSberDev.Id ).First();
              
            }
            if (mvz != null)
            {
              var operation = new Enumeration("AddApprover");
              blok.Forward(mvz.BudgetOwner, ForwardingLocation.Next, Calendar.Today.AddWorkingDays(2));
              blok.History.Write(operation, operation, Sungero.Company.PublicFunctions.Employee.GetShortName(mvz.BudgetOwner, false));
              
              var task = ApprovalTasks.As(_obj);
              var approvalAsg = ApprovalAssignments.As(_obj);
              if (task != null && approvalAsg != null)
              {
                var approver = task.AddApproversExpanded.AddNew();
                approver.Approver = mvz.BudgetOwner;
                task.Save();
                
              }
              
              
            }}
        }
      }
      
    }

    public override void Script26Execute()
    {
      base.Script26Execute();
      sberdev.SberContracts.PublicFunctions.Module.Remote.LinkDocs(_obj);
    }

    public override void EndBlock3(Sungero.Docflow.Server.ApprovalManagerAssignmentEndBlockEventArguments e)
    {
      base.EndBlock3(e);
    }

    public override void CompleteAssignment31(Sungero.Docflow.IApprovalCheckingAssignment assignment, Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.ApprovalCheckingAssignments.As(assignment);
      if ( blok.NeedAbort.GetValueOrDefault())
      {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
      }
      else
      {
        base.CompleteAssignment31(assignment, e);
        var resJob = assignment.Result;
        var nedRes = Sungero.Docflow.ApprovalCheckingAssignment.Result.ForRework;
        if(resJob != nedRes)
          Functions.ApprovalTask.OneTimeCompleteFunction(_obj, e);
      }
    }

    public override void CompleteAssignment30(Sungero.Docflow.IApprovalSimpleAssignment assignment, Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.ApprovalSimpleAssignments.As(assignment);
      if ( blok.NeedAbort.GetValueOrDefault())
      {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
      }
      else
      {
        base.CompleteAssignment30(assignment, e);
        if (assignment.Result != Sungero.Docflow.FreeApprovalAssignment.Result.ForRework)
          Functions.ApprovalTask.OneTimeCompleteFunction(_obj, e);
        
      }
    }

    public override void StartBlock31(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      base.StartBlock31(e);
      Functions.ApprovalTask.OneTimeCompleteFunction(_obj, e);
    }

    public override void StartBlock30(Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      base.StartBlock30(e);
      Functions.ApprovalTask.OneTimeCompleteFunction(_obj, e);
    }

  }
}
