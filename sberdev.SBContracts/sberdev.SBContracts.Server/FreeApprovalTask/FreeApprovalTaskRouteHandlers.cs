using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SBContracts.FreeApprovalTask;

namespace sberdev.SBContracts.Server
{
  partial class FreeApprovalTaskRouteHandlers
  {

    public override void CompleteAssignment2(Sungero.Docflow.IFreeApprovalAssignment assignment, Sungero.Docflow.Server.FreeApprovalAssignmentArguments e)
    {
      var blok =  sberdev.SBContracts.FreeApprovalAssignments.As(assignment);
     if ( blok.NeedAbort.GetValueOrDefault())
        {
        _obj.AbortingReason = blok.AbortingReason;
        _obj.NeedAbort = true;
        _obj.Save();
        _obj.Abort();
        }
      else
      {
      base.CompleteAssignment2(assignment, e);
      }
    }
  }

}