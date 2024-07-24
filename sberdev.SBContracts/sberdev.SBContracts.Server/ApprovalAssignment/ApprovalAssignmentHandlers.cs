using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalAssignmentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.InvApprByTreasSberDev = false;
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      _obj.DocumentIDSberDev = attach?.Id.ToString();
      var incInv = SBContracts.IncomingInvoices.As(attach);
      if (incInv != null)
      {
        var counter = SberContracts.NonContractInvoiceCounters.GetAll().Where(c => c.Counterparty == incInv.Counterparty
                                                                              && c.Employee == incInv.Author).FirstOrDefault();
        if (counter != null)
        {
          _obj.NonContractInvoiceCounterSberDev = counter.Counter;
          _obj.NonContractInvoiceCounterMoreSberDev = counter;
        }
        else
          _obj.NonContractInvoiceCounterSberDev = 0;
      }
      
      var accounting = SBContracts.AccountingDocumentBases.As(attach);
      if (accounting != null)
      {
        _obj.InternalApprovalStateSberDev = accounting.LeadingDocument?.InternalApprovalState;
        _obj.ExternalApprovalStateSberDev = accounting.LeadingDocument?.ExternalApprovalState;
      }
      
      if (accounting != null && incInv == null)
      {
        var signs = Signatures.Get(accounting.InvoiceSberDev?.LastVersion);
        bool signFlag = false;
        var treasurer = Users.GetAll().Where(u => u.Name.IndexOf("Казначей") > -1).FirstOrDefault();
        if (signs != null && treasurer != null)
          foreach (var sign in signs)
        {
          if ((sign.Signatory == treasurer || sign.SubstitutedUser == treasurer) && sign.SignatureType != SignatureType.NotEndorsing)
          {
            signFlag = true;
            break;
          }
        }
          if (signFlag)
            _obj.InvApprByTreasSberDev = true;
          else
            _obj.InvApprByTreasSberDev = false;
      }
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        e.Result = sberdev.SberContracts.Resources.NotApprove;
      }
    }
  }

}