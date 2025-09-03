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

    public virtual void ControlMarkersFlags2SberDevValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.Company.ShowFocusMarkers(_obj, true);
    }

    public virtual void ControlMarkersFlags1SberDevValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.Company.ShowFocusMarkers(_obj, false);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.Company.UpdInterface(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      Functions.Company.ShowFocusMarkers(_obj, false);
    }
  }
}