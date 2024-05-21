using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts
{

  partial class SupAgreementSharedHandlers
  {

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      if ( e.NewValue != null)
      {
        SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue);
      }
    }
  }
}