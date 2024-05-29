using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts
{
  partial class ApprovalTaskClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if (_obj.State.IsInserted && _obj.State.Properties.DeliveryMethod.IsVisible)
      {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (document != null && _obj.DeliveryMethod != document.DeliveryMethod && document.DeliveryMethod != null
            && document.DeliveryMethod.Sid != Sungero.Docflow.Constants.MailDeliveryMethod.Exchange)
        {
          _obj.DeliveryMethod = document.DeliveryMethod;
          _obj.ExchangeService = null;
        }
      }
    }
  }



}