using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts
{
  partial class OfficialDocumentSharedHandlers
  {

    public override void ExternalApprovalStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != ExternalApprovalState.Signed)
      {
        bool flag = true;
        var indefiniteParam = e.Params.Contains(Constants.Docflow.OfficialDocument.IsNeedChangeApprovalStatus);
        e.Params.TryGetValue(Constants.Docflow.OfficialDocument.IsNeedChangeApprovalStatus, out flag);
        if (!flag)
        {
          _obj.ExternalApprovalState = e.OldValue;
          e.Params.AddOrUpdate(Constants.Docflow.OfficialDocument.IsNeedChangeApprovalStatus, true);
        }
      }
      base.ExternalApprovalStateChanged(e);
    }

    public override void InternalApprovalStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != InternalApprovalState.Signed)
      {
        bool flag = true;
        var indefiniteParam = e.Params.Contains(Constants.Docflow.OfficialDocument.IsNeedChangeApprovalStatus);
        e.Params.TryGetValue(Constants.Docflow.OfficialDocument.IsNeedChangeApprovalStatus, out flag);
        if (!flag)
        {
          _obj.InternalApprovalState = e.OldValue;
          e.Params.AddOrUpdate(Constants.Docflow.OfficialDocument.IsNeedChangeApprovalStatus, true);
        }
      }
      base.InternalApprovalStateChanged(e);
    }

    public override void DeliveryMethodChanged(Sungero.Docflow.Shared.OfficialDocumentDeliveryMethodChangedEventArgs e)
    {
      base.DeliveryMethodChanged(e);
      if (PublicFunctions.Module.IsSystemUser() && e.OldValue != null && e.OldValue != e.NewValue
          && e.NewValue.Sid == Sungero.Docflow.Constants.MailDeliveryMethod.Exchange)
        return;
    }

  }
}