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

    public override void LeadingDocumentValueInput(Sungero.Docflow.Client.OfficialDocumentLeadingDocumentValueInputEventArgs e)
    {
      base.LeadingDocumentValueInput(e);
      if (e.NewValue != null)
      {
        var isInvoice = SBContracts.IncomingInvoices.Is(_obj) || SBContracts.OutgoingInvoices.Is(_obj);
        if (isInvoice)
        {
          if (_obj.PayTypeBaseSberDev == SBContracts.AccountingDocumentBase.PayTypeBaseSberDev.Prepayment)
          {
            string contrTypeChoice = sberdev.SBContracts.PublicFunctions.AccountingDocumentBase
              .ShowContractChangedDialog(_obj);
            SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue, contrTypeChoice);
          }
        }
        else
        {
          Functions.AccountingDocumentBase.FillDeliveryInfo(_obj, SBContracts.ContractualDocuments.As(_obj.LeadingDocument));
          if (_obj.PayTypeBaseSberDev == SBContracts.AccountingDocumentBase.PayTypeBaseSberDev.Postpay)
          {
            string contrTypeChoice = sberdev.SBContracts.PublicFunctions.AccountingDocumentBase
              .ShowContractChangedDialog(_obj);
            SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue, contrTypeChoice);
          }
        }
      }
      else
      {
        _obj.State.Properties.LeadingDocument.HighlightColor = Colors.Common.White;
      }
    }

    public override void DeliveryMethodValueInput(Sungero.Docflow.Client.OfficialDocumentDeliveryMethodValueInputEventArgs e)
    {
      base.DeliveryMethodValueInput(e);
      // для вызова обновления формы
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      try
      {
        if (_obj.LeadingDocument != null)
          _obj.State.Properties.LeadingDocument.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.LeadingDocument, false);
        
        if (_obj.AccDocSberDev != null)
          _obj.State.Properties.AccDocSberDev.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.AccDocSberDev, false);
        
        if (_obj.InvoiceSberDev != null)
          _obj.State.Properties.InvoiceSberDev.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.InvoiceSberDev, false);
      }
      catch (Exception ex)
      {
        // Документ удален/рассинхрон – тихо пропускаем подсветку
        Logger.DebugFormat("HighlightUnsignedDocument skipped. Entity not found. Id={0}. {1}", _obj.Id, ex.Message);
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