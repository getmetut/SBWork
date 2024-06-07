using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts
{
  partial class CompanyClientHandlers
  {

    public virtual void IPSberDevValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.Company.UpdInterface(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.Company.UpdInterface(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      Functions.Company.UpdInterface(_obj);
      Functions.Company.HideEmptyFocusMarkers(_obj);
    }
  }
}