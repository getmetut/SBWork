using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.UniversalTransferDocument;

namespace Sungero.ATS
{
  partial class UniversalTransferDocumentSharedHandlers
  {

    public override void CounterpartyChanged(Sungero.Docflow.Shared.AccountingDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      if (e.NewValue != null)
      {
        var comp = sberdev.SBContracts.Companies.As(e.NewValue);
        if ((comp.HeadOrgSDev != true) && (!sberdev.SBContracts.PublicFunctions.Module.IsSystemUser()))
        {
          var OrgTrue = PublicFunctions.UniversalTransferDocument.Remote.GetHeadCompanies(_obj, comp);
          if ((OrgTrue != null) && (OrgTrue != comp))
          {
            if (_obj.Counterparty != OrgTrue)
              _obj.Counterparty = OrgTrue;
          }
        }
      }
    }

  }
}