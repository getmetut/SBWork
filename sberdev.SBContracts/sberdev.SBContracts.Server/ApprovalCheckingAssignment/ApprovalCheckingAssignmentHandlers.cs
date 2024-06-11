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
        if (inv != null)
        {
          _obj.OurCompany = inv.BusinessUnit;
          if (_obj.PaymentDueDateSberDev != inv.PaymentDueDate)
          {
            _obj.PaymentDueDateSberDev = inv.PaymentDueDate;
            if (_obj.PaymentDueDateSberDev < _obj.Deadline)
              _obj.State.Properties.PaymentDueDateSberDev.HighlightColor = Sungero.Core.Colors.Common.Red;
            else
              _obj.State.Properties.PaymentDueDateSberDev.HighlightColor = Sungero.Core.Colors.Common.White;
          }
        }
      }
      else
        _obj.State.Properties.PaymentDueDateSberDev.IsVisible = false;
      
      var accounting = SBContracts.AccountingDocumentBases.As(attach);
      if (accounting != null)
      {
        _obj.InternalApprovalStateSberDev = accounting.LeadingDocument?.InternalApprovalState;
        _obj.ExternalApprovalStateSberDev = accounting.LeadingDocument?.ExternalApprovalState;
      }
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        e.Result = sberdev.SberContracts.Resources.NotApprove;
      }
      
      var incInv = IncomingInvoices.As(_obj.DocumentGroup.OfficialDocuments?.First());
      
      if (incInv != null)
      {
        switch (_obj.StageSubject)
        {
          case "Оплата счета":
            SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(incInv);
            incInv.PaymentDateSberDev = Calendar.Now;
            incInv.Save();
            break;
            
          case "Проверка договорных документов Делопроизводителем":
            var contractual = incInv.LeadingDocument;
            if (contractual != null)
            {
              SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(contractual);
              contractual.InternalApprovalState = SBContracts.OfficialDocument.InternalApprovalState.Signed;
              contractual.ExternalApprovalState = SBContracts.OfficialDocument.ExternalApprovalState.Signed;
              contractual.Save();
            }
            var accounting = incInv.AccDocSberDev;
            if (accounting != null)
            {
              SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(accounting);
              accounting.InternalApprovalState = SBContracts.OfficialDocument.InternalApprovalState.Signed;
              accounting.ExternalApprovalState = SBContracts.OfficialDocument.ExternalApprovalState.Signed;
              accounting.Save();
            }
            break;
        }
      }
    }
  }

}