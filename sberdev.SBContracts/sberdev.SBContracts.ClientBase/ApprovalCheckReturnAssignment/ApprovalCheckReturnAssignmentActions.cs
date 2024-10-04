using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckReturnAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalCheckReturnAssignmentActions
  {
    public override void Signed(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      base.Signed(e);
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (SBContracts.ContractualDocuments.Is(document))
      {
        PublicFunctions.Module.Remote.UnblockCardByDatabase(document);
        document.LifeCycleState = Sungero.Docflow.OfficialDocument.LifeCycleState.Active;
        document.InternalApprovalState = Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed;
        document.ExternalApprovalState = Sungero.Docflow.OfficialDocument.ExternalApprovalState.Signed;
      }
    }

    public override bool CanSigned(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanSigned(e);
    }

  }

}