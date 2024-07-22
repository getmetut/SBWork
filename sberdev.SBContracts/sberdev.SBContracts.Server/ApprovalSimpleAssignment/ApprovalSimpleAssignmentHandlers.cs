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

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      _obj.DocumentIDSberDev = attach?.Id.ToString();
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      var incInv = IncomingInvoices.As(_obj.DocumentGroup.OfficialDocuments.First());
      if (incInv != null && _obj.StageSubject == "Проверка договорных документов Делопроизводителем")
      {
        var contractual = incInv.LeadingDocument;
        if (contractual != null)
        {
          contractual.InternalApprovalState = SBContracts.OfficialDocument.InternalApprovalState.Signed;
          contractual.ExternalApprovalState = SBContracts.OfficialDocument.ExternalApprovalState.Signed;
          contractual.Save();
        }
        var accounting = incInv.AccDocSberDev;
        if (accounting != null)
        {
          accounting.InternalApprovalState = SBContracts.OfficialDocument.InternalApprovalState.Signed;
          accounting.ExternalApprovalState = SBContracts.OfficialDocument.ExternalApprovalState.Signed;
          accounting.Save();
        }
      }
    }
  }

}