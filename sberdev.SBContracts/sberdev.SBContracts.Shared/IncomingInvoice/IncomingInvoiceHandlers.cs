using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts
{

  partial class IncomingInvoiceSharedHandlers
  {

    public virtual void UCNSberDevChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue != null)
      {
        var ucn = Functions.IncomingInvoice.CheckUCNProperty(_obj, e.NewValue);
        if (ucn != null)
          _obj.UCNSberDev = ucn;
      }
    }

    public virtual void NoNeedLeadingDocsChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      //для срабатывания события обновления
    }

    public override void FrameworkBaseSberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      //удаление базового обработчика
    }

    public override void AccDocSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseAccDocSberDevChangedEventArgs e)
    {
      base.AccDocSberDevChanged(e);
      
      if (e.NewValue != null && _obj.PayTypeBaseSberDev != SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay)
        _obj.PayTypeBaseSberDev = SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay;
    }

    public override void AccArtBaseSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseAccArtBaseSberDevChangedEventArgs e)
    {
      base.AccArtBaseSberDevChanged(e);
      if ( e.NewValue != null)
      {
        _obj.BudItemBaseSberDev = _obj.AccArtBaseSberDev.BudgetItem;
      }
    }

    public override void PaymentDueDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.PaymentDueDateChanged(e);
      if (e.NewValue.HasValue && e.NewValue != e.OldValue)
      {
        var date = e.NewValue.Value;
        if (date.DayOfWeek != DayOfWeek.Tuesday && date.DayOfWeek != DayOfWeek.Thursday)
          _obj.PaymentDueDate = (date.DayOfWeek > DayOfWeek.Thursday || date.DayOfWeek == DayOfWeek.Sunday) ?
            Calendar.EndOfWeek(date).AddDays(2) : date.AddDays(1);
      }
    }

    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable) && (_obj.MVPBaseSberDev == null))
      {
        var mvp = SberContracts.MVZs.GetAll( l => (l.BudgetOwner.Department == e.NewValue) && (l.ContrType == SberContracts.MVZ.ContrType.Profitable )).FirstOrDefault();
        if (mvp != null)
        {
          _obj.MVPBaseSberDev = mvp;
        }
      }
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable) && (_obj.MVZBaseSberDev == null))
      {
        var mvp = SberContracts.MVZs.GetAll( l => (l.BudgetOwner.Department == e.NewValue) && (l.ContrType == SberContracts.MVZ.ContrType.Expendable )).FirstOrDefault();
        if (mvp != null)
        {
          _obj.MVZBaseSberDev = mvp;
        }
      }
    }

  }
}