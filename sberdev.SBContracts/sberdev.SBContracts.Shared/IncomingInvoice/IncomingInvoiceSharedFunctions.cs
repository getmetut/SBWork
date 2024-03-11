using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts.Shared
{
  partial class IncomingInvoiceFunctions
  {
    
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      if (_obj.NoNeedLeadingDocs.HasValue)
      {
        _obj.State.Properties.AccDocSberDev.IsRequired = _obj.NoNeedLeadingDocs.Value ? false : (_obj.PayType == PayType.Postpay);
        _obj.State.Properties.LeadingDocument.IsRequired = !_obj.NoNeedLeadingDocs.Value;
      }
      else
      {
        _obj.State.Properties.AccDocSberDev.IsRequired = false;
        _obj.State.Properties.LeadingDocument.IsRequired = false;
      }
      if (!PublicFunctions.Module.IsSystemUser())
      {
        _obj.State.Properties.TotalAmount.IsRequired = true;
        _obj.State.Properties.Currency.IsRequired = true;
        _obj.State.Properties.TotalAmount.IsEnabled = true;
        _obj.State.Properties.Currency.IsEnabled = true;
      }
      _obj.State.Properties.EstPaymentDateSberDev.IsRequired = false;
      _obj.State.Properties.PayTypeBaseSberDev.IsRequired = false;
    }
  }
}