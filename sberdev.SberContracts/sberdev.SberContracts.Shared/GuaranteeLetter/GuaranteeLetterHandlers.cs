using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.GuaranteeLetter;

namespace sberdev.SberContracts
{
  partial class GuaranteeLetterSharedHandlers
  {

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      PublicFunctions.GuaranteeLetter.UpdateCard(_obj);
    }

  }
}