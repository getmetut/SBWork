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

      string statusExt = null;
      string statusInt = null;

      if (e.Params.TryGetValue(Constants.Docflow.OfficialDocument.OldExtStatus, out statusExt))
        _obj.ExternalApprovalState = Functions.OfficialDocument.GetExtApprEnum(_obj, statusExt);

      if (e.Params.TryGetValue(Constants.Docflow.OfficialDocument.OldIntStatus, out statusInt))
        _obj.InternalApprovalState= Functions.OfficialDocument.GetIntApprEnum(_obj, statusInt);
    }
  }


  
  partial class OfficialDocumentSharedHandlers
  {

    public override void ExternalApprovalStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      bool flag = PublicFunctions.Module.IsSystemUser() ||
        Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители")) ||
        Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));
      
      if (flag && e.OldValue.HasValue)
        e.Params.AddOrUpdate(Constants.Docflow.OfficialDocument.OldExtStatus, e.OldValue.Value.Value.ToString());

      base.ExternalApprovalStateChanged(e);
    }

    public override void InternalApprovalStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      bool flag = PublicFunctions.Module.IsSystemUser() ||
        Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители")) ||
        Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));

      if (flag && e.OldValue.HasValue)
        e.Params.AddOrUpdate(Constants.Docflow.OfficialDocument.OldIntStatus, e.OldValue.Value.Value.ToString());

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