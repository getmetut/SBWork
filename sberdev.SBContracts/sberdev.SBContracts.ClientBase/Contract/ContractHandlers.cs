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
      /*
      var analiticSetup = SberContracts.AnaticsSetups.GetAll(r => (r.DocumentGroup == _obj.DocumentGroup) && (r.DocumentType == SberContracts.AnaticsSetup.DocumentType.Contract) && (r.DocumentKind == _obj.DocumentKind)).FirstOrDefault();
      if (analiticSetup != null)
        PublicFunctions.ContractualDocument.ApplyAnaliticSetup(_obj, analiticSetup);
      else
        if (_obj.ContrTypeBaseSberDev.HasValue)
          PublicFunctions.ContractualDocument.ChangeAnaliticsAccess(_obj, _obj.ContrTypeBaseSberDev.Value.Value);*/
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