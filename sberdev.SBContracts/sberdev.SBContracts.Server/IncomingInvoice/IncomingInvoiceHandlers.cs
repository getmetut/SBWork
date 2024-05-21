using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts
{

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
      
      var date = _obj.Date.Value;
      if (_obj.PaymentDueDate == null)
      {
        if (date.DayOfWeek != DayOfWeek.Tuesday && date.DayOfWeek != DayOfWeek.Thursday)
          _obj.PaymentDueDate = (date.DayOfWeek > DayOfWeek.Thursday || date.DayOfWeek == DayOfWeek.Sunday) ?
            Calendar.EndOfWeek(date).AddDays(2) : date.AddDays(1);
        else
          _obj.PaymentDueDate = date;
      }
    }
  }

}