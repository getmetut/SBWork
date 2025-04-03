using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.GuaranteeLetter;

namespace sberdev.SberContracts
{
  partial class GuaranteeLetterClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      PublicFunctions.GuaranteeLetter.UpdateCard(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      PublicFunctions.GuaranteeLetter.UpdateCard(_obj);
    }

  }
}