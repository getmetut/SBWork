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

    public override void DeliveryMethodChanged(Sungero.Docflow.Shared.OfficialDocumentDeliveryMethodChangedEventArgs e)
    {
      base.DeliveryMethodChanged(e);
      if (PublicFunctions.Module.IsSystemUser() && e.OldValue != null && e.OldValue != e.NewValue
          && e.NewValue.Sid == Sungero.Docflow.Constants.MailDeliveryMethod.Exchange)
       return;
    }

  }
}