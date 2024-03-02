using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AbstractsSupAgreement;

namespace sberdev.SberContracts
{
  partial class AbstractsSupAgreementClientHandlers
  {

    public override void LeadingDocumentValueInput(Sungero.Docflow.Client.OfficialDocumentLeadingDocumentValueInputEventArgs e)
    {
      base.LeadingDocumentValueInput(e);
      
      if (e.NewValue != null)
      {
            if (Functions.AbstractsSupAgreement.HaveDuplicates(_obj, _obj.BusinessUnit, _obj.RegistrationNumber, _obj.RegistrationDate, _obj.Counterparty, SBContracts.ContractualDocuments.As(e.NewValue)))
              e.AddWarning(SBContracts.ContractualDocuments.Resources.DuplicatesDetected + SBContracts.ContractualDocuments.Resources.FindDuplicates,
                           _obj.Info.Properties.Counterparty,
                           _obj.Info.Properties.BusinessUnit,
                           _obj.Info.Properties.RegistrationDate,
                           _obj.Info.Properties.RegistrationNumber,
                           _obj.Info.Properties.LeadingDocument);
        }
    }

  }
}