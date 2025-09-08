using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.SimpleDocument;
using sberdev.SBContracts.PublicFunctions;

namespace Sungero.ATS.Shared
{
  partial class SimpleDocumentFunctions
  {
    
    public void SetPropertiesAccess()
    {
      List<sberdev.SBContracts.IDocumentKind> kinds = new List<sberdev.SBContracts.IDocumentKind>()
      {
        DocumentKind.Remote.GetDocumentKind(sberdev.SberContracts.PublicConstants.Module.ActMHGuid),
        DocumentKind.Remote.GetDocumentKind(sberdev.SberContracts.PublicConstants.Module.APPServiceGuid)
      };
      bool kindFlag = OfficialDocument.EqualsDocumentKinds(_obj, kinds);
      _obj.State.Properties.CounterpartySberDev.IsVisible = kindFlag;
      _obj.State.Properties.CounterpartySberDev.IsRequired = kindFlag;
    }

  }
}