using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts
{

  partial class SupAgreementSharedHandlers
  {

    public override void TotalAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.TotalAmountChanged(e);
      if ((e.NewValue != null) && (_obj.LeadingDocument != null))
      {
        if (Sungero.Custom.PublicFunctions.Module.RequestCorrectLiminInContract(_obj.LeadingDocument.Id, e.NewValue.Value))
          _obj.State.Properties.TotalAmount.HighlightColor = Colors.Common.LightGreen;
        else
          _obj.State.Properties.TotalAmount.HighlightColor = Colors.Common.Red;
      }
      else
        _obj.State.Properties.TotalAmount.HighlightColor = Colors.Empty;
    }
  }
}