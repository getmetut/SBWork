using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.DocumentKind;

namespace sberdev.SBContracts.Server
{
  partial class DocumentKindFunctions
  {
    [Public, Remote]
    public static sberdev.SBContracts.IDocumentKind GetDocumentKind(System.Guid guid)
    {
      return DocumentKinds.GetAll().Where(d => d.SidSberDev == guid.ToString()).FirstOrDefault();
    }
    
    public override bool Equals(object obj)
    {
      var kind = sberdev.SBContracts.DocumentKinds.As((Sungero.Domain.Shared.IEntity)obj);
      return kind != null ? _obj.SidSberDev == kind.SidSberDev : base.Equals(obj);
    }
  }
}