using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts.Client
{
  partial class OfficialDocumentFunctions
  {
    public override Sungero.Docflow.IOfficialDocument ChangeDocumentType(List<Sungero.Domain.Shared.IEntityInfo> types)
    {
      var CDT = base.ChangeDocumentType(types);
      CDT.RegistrationDate = CDT.Created;
      
      return CDT;
    }
  }
}