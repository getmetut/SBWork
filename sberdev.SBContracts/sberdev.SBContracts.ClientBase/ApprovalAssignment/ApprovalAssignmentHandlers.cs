using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalAssignmentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      var approvalStage = sberdev.SBContracts.ApprovalStages.As(_obj.Stage);
      var performer = Users.As(_obj.Performer);
      if (approvalStage.Name == "Казначей ПП")
      {
        _obj.State.Properties.NonContractInvoiceCounterSberDev.IsVisible = true;
        _obj.State.Properties.NonContractInvoiceCounterMoreSberDev.IsVisible = true;
        _obj.State.Properties.InvApprByTreasSberDev.IsVisible = true;
        if (_obj.NonContractInvoiceCounterSberDev > 3)
        {
          _obj.State.Properties.NonContractInvoiceCounterSberDev.HighlightColor = Colors.Common.Red;
        }
        else
        {
          _obj.State.Properties.NonContractInvoiceCounterSberDev.HighlightColor = Colors.Empty;
        }
      }
      else
      {
        _obj.State.Properties.NonContractInvoiceCounterSberDev.IsVisible = false;
        _obj.State.Properties.NonContractInvoiceCounterMoreSberDev.IsVisible = false;
        _obj.State.Properties.InvApprByTreasSberDev.IsVisible = false;
      }
      
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
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
      
      if ((!approvalStage.AmountChangesberdev.GetValueOrDefault()) || (!performer.IncludedIn( sberdev.SberContracts.PublicConstants.Module.KZTypeGuid)))
      {e.HideAction(_obj.Info.Actions.AmountChangesberdev);}
      base.Showing(e);
    }

  }
}