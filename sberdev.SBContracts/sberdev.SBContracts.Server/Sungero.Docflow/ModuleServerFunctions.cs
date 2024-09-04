using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Docflow.Server
{
  partial class ModuleFunctions
  {    
    public override bool GeneratePublicBodyForExchangeDocument(Sungero.Docflow.IOfficialDocument document, long versionId, Nullable<Enumeration> exchangeState, Nullable<DateTime> startTime)
    {
      SBContracts.OfficialDocuments.As(document).BodyExtSberDev = document.LastVersion.AssociatedApplication.Extension;
      return base.GeneratePublicBodyForExchangeDocument(document, versionId, exchangeState, startTime);
    }
  }
}