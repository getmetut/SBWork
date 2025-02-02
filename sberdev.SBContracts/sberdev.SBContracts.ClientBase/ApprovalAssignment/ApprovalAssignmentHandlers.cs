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
      var task = SBContracts.ApprovalTasks.As(_obj.Task);

      var performer = Users.As(_obj.Performer);
      bool isTreasuryStage = PublicFunctions.ApprovalTask.IsNecessaryStage(task, PublicConstants.Docflow.ApprovalTask.TreasuryStage);

      _obj.State.Properties.NonContractInvoiceCounterSberDev.IsVisible = isTreasuryStage;
      _obj.State.Properties.NonContractInvoiceCounterMoreSberDev.IsVisible = isTreasuryStage;
      _obj.State.Properties.InvApprByTreasSberDev.IsVisible = isTreasuryStage;

      if (isTreasuryStage)
      {
        _obj.State.Properties.NonContractInvoiceCounterSberDev.HighlightColor =
          _obj.NonContractInvoiceCounterSberDev > 3 ? Colors.Common.Red : Colors.Empty;
      }

      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var accounting = SBContracts.AccountingDocumentBases.As(attach);
      bool hasAccounting = accounting != null;

      _obj.State.Properties.InternalApprovalStateSberDev.IsVisible = hasAccounting;
      _obj.State.Properties.ExternalApprovalStateSberDev.IsVisible = hasAccounting;

      bool shouldHideAmountChangeAction = !approvalStage.AmountChangesSberDev.GetValueOrDefault() ||
        !performer.IncludedIn(sberdev.SberContracts.PublicConstants.Module.KZTypeGuid);

      if (shouldHideAmountChangeAction)
        e.HideAction(_obj.Info.Actions.AmountChangesberdev);

      base.Showing(e);

    }

  }
}