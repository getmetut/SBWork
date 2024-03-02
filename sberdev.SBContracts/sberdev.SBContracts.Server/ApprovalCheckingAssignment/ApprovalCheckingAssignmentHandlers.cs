using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckingAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalCheckingAssignmentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var attach = Functions.ApprovalCheckingAssignment.GetAttachment(_obj);
      _obj.DocumentIDSberDev = attach?.Id.ToString();
      if (_obj.StageSubject == "Оплата счета")
      {
        _obj.State.Properties.PaymentDueDateSberDev.IsVisible = true;
        var inv = SBContracts.IncomingInvoices.As(attach);
        if (inv != null && _obj.PaymentDueDateSberDev != inv.PaymentDueDate)
        {
          _obj.PaymentDueDateSberDev = inv.PaymentDueDate;
          if (_obj.PaymentDueDateSberDev < _obj.Deadline)
            _obj.State.Properties.PaymentDueDateSberDev.HighlightColor = Sungero.Core.Colors.Common.Red;
          else
            _obj.State.Properties.PaymentDueDateSberDev.HighlightColor = Sungero.Core.Colors.Common.White;
        }
      }
      else
        _obj.State.Properties.PaymentDueDateSberDev.IsVisible = false;
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        e.Result = sberdev.SberContracts.Resources.NotApprove;
      }
      
      var incInv = IncomingInvoices.As(_obj.DocumentGroup.OfficialDocuments?.First());
      if (incInv != null &&  _obj.StageSubject == "Оплата счета")
      {
        incInv.PaymentDateSberDev = Calendar.Now;
        incInv.Save();
      }
    }
  }

}