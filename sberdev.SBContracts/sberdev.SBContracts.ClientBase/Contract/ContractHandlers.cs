using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Contract;

namespace sberdev.SBContracts
{
  partial class ContractClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      if( (!Users.Current.IncludedIn( sberdev.SberContracts.PublicConstants.Module.GBTypeGuid )) && (!Users.Current.IncludedIn( sberdev.SberContracts.PublicConstants.Module.FDTypeGuid )))
      {
        _obj.State.Properties.ExitCommentBaseSberDev.IsVisible = false;
      }
      
    }

  }
}