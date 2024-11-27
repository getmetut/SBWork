using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts
{
  partial class SupAgreementClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if ((_obj.LeadingDocument != null) && (_obj.TotalAmount != null))
        if (Sungero.Custom.PublicFunctions.Module.RequestSummLiminInContract(_obj.LeadingDocument.Id, _obj.TotalAmount.Value) < 0.0)
          e.AddInformation("Данный документ превышает лимит по основному договору на " + Sungero.Custom.PublicFunctions.Module.RequestSummLiminInContract(_obj.LeadingDocument.Id, _obj.TotalAmount.Value));
    }
  }

}