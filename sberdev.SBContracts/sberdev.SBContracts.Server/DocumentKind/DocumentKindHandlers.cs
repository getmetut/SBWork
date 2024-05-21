using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.DocumentKind;

namespace sberdev.SBContracts
{
  partial class DocumentKindServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.NoBodyApprovalSberDev = false;
    }
  }

}