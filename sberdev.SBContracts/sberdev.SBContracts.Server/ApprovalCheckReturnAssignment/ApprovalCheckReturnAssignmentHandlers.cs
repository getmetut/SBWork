using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckReturnAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalCheckReturnAssignmentServerHandlers
  {

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      PublicFunctions.ApprovalTask.ClearPublicComment(_obj);
    }
  }


}