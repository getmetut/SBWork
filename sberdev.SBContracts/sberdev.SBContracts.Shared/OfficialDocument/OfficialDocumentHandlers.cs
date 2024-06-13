using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts
{
  partial class OfficialDocumentVersionsSharedCollectionHandlers
  {

    public override void VersionsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      base.VersionsAdded(e);
      Nullable<Enumeration> statusExt = null;
      Nullable<Enumeration> statusInt = null;
      e.Params.TryGetValue(Constants.Docflow.OfficialDocument.OldExtStatus, out statusExt);
      e.Params.TryGetValue(Constants.Docflow.OfficialDocument.OldIntStatus, out statusExt);
      if (statusExt != null)
        _obj.ExternalApprovalState = statusExt;
      if (statusInt != null)
        _obj.ExternalApprovalState = statusInt;
    }
  }

  
  partial class OfficialDocumentSharedHandlers
  {

    public override void ExternalApprovalStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      bool flag = PublicFunctions.Module.IsSystemUser() || Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"))
        || Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));
      if (!e.Params.Contains(Constants.Docflow.OfficialDocument.OldExtStatus) && flag)
        e.Params.AddOrUpdate(Constants.Docflow.OfficialDocument.OldExtStatus, e.OldValue);
      base.ExternalApprovalStateChanged(e);
    }

    public override void InternalApprovalStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      bool flag = PublicFunctions.Module.IsSystemUser() || Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"))
        || Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));
      if (!e.Params.Contains(Constants.Docflow.OfficialDocument.OldIntStatus) && flag)
        e.Params.AddOrUpdate(Constants.Docflow.OfficialDocument.OldIntStatus, e.OldValue);
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