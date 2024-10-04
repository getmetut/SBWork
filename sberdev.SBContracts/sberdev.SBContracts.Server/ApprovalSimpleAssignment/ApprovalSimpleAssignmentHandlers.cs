using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSimpleAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalSimpleAssignmentServerHandlers
  {

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
        var incInv = IncomingInvoices.As(_obj.DocumentGroup.OfficialDocuments.First());
        var stage = SBContracts.ApprovalStages.GetAll().Where(s => s.Subject == _obj.StageSubject).FirstOrDefault();
        
        if  (stage == null || incInv == null)
          return;
        
        if (stage.SidSberDev == Constants.Docflow.ApprovalTask.ContractCheckByClerkStage)
          PublicFunctions.ApprovalSimpleAssignment.Remote.ContractCheckByClerkStageApprScript(_obj, incInv);
        
        if (stage.SidSberDev == Constants.Docflow.ApprovalTask.CheckUCNStage)
        {
          string ucn = PublicFunctions.ApprovalSimpleAssignment.ShowCheckUCNDialog(_obj);
          if (!PublicFunctions.ApprovalSimpleAssignment.Remote.CheckUCNStageApprScript(_obj, incInv, ucn))
            e.AddError(sberdev.SBContracts.ApprovalSimpleAssignments.Resources.CheckUCNErr);
        }
      base.BeforeComplete(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      _obj.DocumentIDSberDev = attach?.Id.ToString();
    }
  }

}