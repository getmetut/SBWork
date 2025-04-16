using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;

namespace sberdev.SBContracts
{
  partial class AccountingDocumentBaseClientHandlers
  {

    public override void DeliveryMethodValueInput(Sungero.Docflow.Client.OfficialDocumentDeliveryMethodValueInputEventArgs e)
    {
      base.DeliveryMethodValueInput(e);
      // для вызова обновления формы
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      if (_obj.LeadingDocument != null)
      {
        _obj.State.Properties.LeadingDocument.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.LeadingDocument, false);
      }
      if (_obj.AccDocSberDev != null)
      {
        _obj.State.Properties.AccDocSberDev.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.AccDocSberDev, false);
      }
      if (_obj.InvoiceSberDev != null)
      {
        _obj.State.Properties.InvoiceSberDev.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.InvoiceSberDev, false);
      }
    }

    public virtual void ContrTypeBaseSberDevValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (e.NewValue == ContrTypeBaseSberDev.Expendable)
        _obj.MVPBaseSberDev = null;
      if (e.NewValue == ContrTypeBaseSberDev.Profitable)
        _obj.MVZBaseSberDev = null;
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.AccountingDocumentBase.SetPropertiesAccess(_obj);
      Functions.AccountingDocumentBase.HighlightClosedAnalitics(_obj);
      Functions.AccountingDocumentBase.UpdateCard(_obj);
    }

  }
}