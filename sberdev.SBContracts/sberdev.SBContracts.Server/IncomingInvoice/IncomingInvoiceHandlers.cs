using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts
{
  partial class IncomingInvoicePurchaseATSDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> PurchaseATSDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparty != null)
        query = query.Where(q => q.Counterparty == _obj.Counterparty);

      return query;
    }
  }


  partial class IncomingInvoiceServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Original = false;
      _obj.NoNeedLeadingDocs = false;
      base.Created(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      Functions.IncomingInvoice.SendNotice(_obj);
      Functions.IncomingInvoice.SetCurrency(_obj);
      Functions.IncomingInvoice.SetPaymentDueDate(_obj);
      Functions.IncomingInvoice.SetDeliveryMethod(_obj);
    }
  }

}