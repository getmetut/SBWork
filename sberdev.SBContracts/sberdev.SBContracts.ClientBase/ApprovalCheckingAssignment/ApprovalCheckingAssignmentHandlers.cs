using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckingAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalCheckingAssignmentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      var incInv = SBContracts.IncomingInvoices.As(attach);
      _obj.State.Properties.PaymentDueDateSberDev.IsVisible = incInv != null;
      
      var accounting = SBContracts.AccountingDocumentBases.As(attach);
      bool accFlag = accounting != null;
      _obj.State.Properties.InternalApprovalStateSberDev.IsVisible = accFlag;
      _obj.State.Properties.ExternalApprovalStateSberDev.IsVisible = accFlag;
      _obj.State.Properties.FDAApprByTreasSberDev.IsVisible = accFlag;
      
      base.Showing(e);
    }
  }


}