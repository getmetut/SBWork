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
      if (incInv != null)
        _obj.State.Properties.PaymentDueDateSberDev.IsVisible = true;
      else
        _obj.State.Properties.PaymentDueDateSberDev.IsVisible = false;
      
      var accounting = SBContracts.AccountingDocumentBases.As(attach);
      if (accounting != null)
      {
        _obj.State.Properties.InternalApprovalStateSberDev.IsVisible = true;
        _obj.State.Properties.ExternalApprovalStateSberDev.IsVisible = true;
      }
      else
      {
        _obj.State.Properties.InternalApprovalStateSberDev.IsVisible = false;
        _obj.State.Properties.ExternalApprovalStateSberDev.IsVisible = false;
      }
      base.Showing(e);
    }
  }


}