using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts.Shared
{
  partial class SupAgreementFunctions
  {
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      
      bool specPAOFlag = _obj.DocumentKind != null && _obj.DocumentKind.Name == "Спецификация ПАО";
      
      _obj.State.Properties.SDSFSberDev.IsVisible = specPAOFlag;
      _obj.State.Properties.SDSFSberDev.IsRequired = specPAOFlag;
      _obj.State.Properties.SRSberDev.IsVisible = specPAOFlag;
      _obj.State.Properties.SRSberDev.IsRequired = specPAOFlag;
      _obj.State.Properties.GoogleDocsLinkSberDev.IsVisible = specPAOFlag;
      _obj.State.Properties.GoogleDocsLinkSberDev.IsRequired = specPAOFlag;
      _obj.State.Properties.SubjectSpecificationSberDev.IsVisible = specPAOFlag;
    }
  }
}