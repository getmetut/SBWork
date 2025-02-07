using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;

namespace sberdev.SBContracts
{
  partial class AccountingDocumentBaseCalculationBaseSberDevSharedHandlers
  {

    public virtual void CalculationBaseSberDevPercentCalcChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.InterestCalc = null;
        return;
      }
      if (_obj.AccountingDocumentBase.TotalAmount.HasValue)
        _obj.InterestCalc = Math.Round(e.NewValue.Value * _obj.AccountingDocumentBase.TotalAmount.Value / (double)100, 2);
    }

    public virtual void CalculationBaseSberDevProductCalcChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseCalculationBaseSberDevProductCalcChangedEventArgs e)
    {
      _obj.AggregationCalc = _obj.ProductCalc != null ? SberContracts.PublicFunctions.Module.GetRusAggregationName(_obj.ProductCalc.Aggregation): null;
    }
  }

  partial class AccountingDocumentBaseSharedHandlers
  {

    public override void CounterpartyChanged(Sungero.Docflow.Shared.AccountingDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue != null && SBContracts.Companies.Is(e.NewValue))
        {
          if ((SBContracts.Companies.As(e.NewValue).HeadOrgSDev != true) && (!SBContracts.PublicFunctions.Module.IsSystemUser()))
          {
            string TINNew = SBContracts.Companies.As(e.NewValue).TIN;
            var ListOrgTrue = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => c.HeadOrgSDev == true).ToArray();
            if (ListOrgTrue.Count() > 0)
            {
              foreach (var elem in ListOrgTrue)
              {
                if (elem.TIN != null)
                {
                  var OtherOrg = sberdev.SBContracts.Companies.GetAll(c => ((c.TIN == TINNew) && (c.HeadCompany != elem) && (c.Id != e.NewValue.Id))).ToArray();
                  if (OtherOrg.Count() > 0)
                  {
                    foreach (var othorg in OtherOrg)
                    {
                      _obj.Counterparty = othorg;
                    }
                  }
                }
              }
            }
            else
            {
              _obj.CounterpartyTINSberDev = e.NewValue.TIN;
              _obj.CouterpartyTRRCSberDev = SBContracts.Companies.As(e.NewValue).TRRC;
            }
          }
          else
          {
            _obj.CounterpartyTINSberDev = e.NewValue.TIN;
            _obj.CouterpartyTRRCSberDev = SBContracts.Companies.As(e.NewValue).TRRC;
          }
        }
        else
        {
          _obj.CounterpartyTINSberDev = null;
          _obj.CouterpartyTRRCSberDev = null;
        }
      }
    }

    public virtual void AccDocSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseAccDocSberDevChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
      
      if (e.NewValue != null)
      {
        var isInvoice = SBContracts.IncomingInvoices.Is(_obj) || SBContracts.OutgoingInvoices.Is(_obj);
        if (isInvoice)
        {
          if (_obj.PayTypeBaseSberDev == SBContracts.AccountingDocumentBase.PayTypeBaseSberDev.Postpay)
            SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue);
        }
        
        _obj.State.Properties.AccDocSberDev.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.AccDocSberDev, false);
      }
      else
      {
        _obj.State.Properties.AccDocSberDev.HighlightColor = Colors.Common.White;
      }
    }

    public virtual void PayTypeBaseSberDevChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (_obj.PayTypeBaseSberDev == PayTypeBaseSberDev.Postpay)
        _obj.AccDocSberDev = _obj.AccDocSberDev;
      if (_obj.PayTypeBaseSberDev == PayTypeBaseSberDev.Prepayment)
        _obj.LeadingDocument = _obj.LeadingDocument;
    }

    public virtual void InvoiceSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseInvoiceSberDevChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
      
      if (e.NewValue != null)
      {
        _obj.PayTypeBaseSberDev = PayTypeBaseSberDev.Prepayment;
        
        var isInvoice = SBContracts.IncomingInvoices.Is(_obj) || SBContracts.OutgoingInvoices.Is(_obj);
        if (!isInvoice)
        {
          if (_obj.PayTypeBaseSberDev == SBContracts.AccountingDocumentBase.PayTypeBaseSberDev.Prepayment)
            SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue);
        }
        _obj.State.Properties.InvoiceSberDev.HighlightColor = _obj.InvoiceSberDev.InternalApprovalState == SBContracts.OfficialDocument.InternalApprovalState.Signed ?
          Colors.Common.White : Colors.Common.Red;
      }
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void SubjectChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.SubjectChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void NumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.NumberChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void DateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.DateChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void CurrencyChanged(Sungero.Docflow.Shared.AccountingDocumentBaseCurrencyChangedEventArgs e)
    {
      base.CurrencyChanged(e);
      
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void ResponsibleEmployeeChanged(Sungero.Docflow.Shared.AccountingDocumentBaseResponsibleEmployeeChangedEventArgs e)
    {
      base.ResponsibleEmployeeChanged(e);
      
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public virtual void MarketingSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseMarketingSberDevChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.MarketingIDSberDev = e.NewValue.Id.ToString();
      else
        _obj.MarketingIDSberDev = null;
    }

    public virtual void FrameworkBaseSberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        _obj.TotalAmount = null;
        _obj.Currency = null;
        _obj.State.Properties.TotalAmount.IsRequired = false;
        _obj.State.Properties.Currency.IsRequired = false;
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationBaseSberDev.Clear();
        _obj.State.Properties.CalculationBaseSberDev.IsEnabled = false;
        _obj.State.Properties.CalculationBaseSberDev.IsRequired = false;
        _obj.State.Properties.CalculationFlagBaseSberDev.IsEnabled = false;
        _obj.State.Properties.CalculationFlagBaseSberDev.IsRequired = false;
      }
    }

    public virtual void ContrTypeBaseSberDevChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      if ( e.NewValue == ContrTypeBaseSberDev.Profitable)
      {
        _obj.DocumentGroup = null;
        _obj.MVZBaseSberDev = null;
      }
      if ( e.NewValue == ContrTypeBaseSberDev.Expendable)
      {
        _obj.DocumentGroup = null;
        _obj.MVPBaseSberDev = null;
      }
    }

    public virtual void AccArtBaseSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseAccArtBaseSberDevChangedEventArgs e)
    {
      if ( e.NewValue != null)
      {
        _obj.BudItemBaseSberDev = _obj.AccArtBaseSberDev.BudgetItem;
      }
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public virtual void MVPBaseSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseMVPBaseSberDevChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
        _obj.CalculationBaseSberDev.Clear();
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationResidualAmountBaseSberDev = 0;
      }
    }

    public virtual void MVZBaseSberDevChanged(sberdev.SBContracts.Shared.AccountingDocumentBaseMVZBaseSberDevChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
        _obj.CalculationBaseSberDev.Clear();
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationResidualAmountBaseSberDev = 0;
        _obj.MarketDirectSberDev = null;
      }
    }

    public virtual void CalculationResidualAmountBaseSberDevChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue == 0 && _obj.TotalAmount != _obj.CalculationAmountBaseSberDev && _obj.CalculationBaseSberDev.Count > 0)
        if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Percent)
          _obj.CalculationBaseSberDev.Last().InterestCalc += _obj.TotalAmount - _obj.CalculationAmountBaseSberDev;
        else
          _obj.CalculationBaseSberDev.Last().AbsoluteCalc += _obj.TotalAmount - _obj.CalculationAmountBaseSberDev;
      if (e.NewValue < 0.01 && e.NewValue != 0)
        _obj.CalculationResidualAmountBaseSberDev = 0;
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      
      _obj.CalculationBaseSberDev.Clear();
      _obj.MVZBaseSberDev = null;
      _obj.MVPBaseSberDev = null;
      _obj.ProdCollectionBaseSberDev.Clear();
    }

    public virtual void ProdCollectionBaseSberDevChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (_obj.ProdCollectionBaseSberDev.Any())
      {
        var prod = _obj.ProdCollectionBaseSberDev.FirstOrDefault().Product;
        if (prod != null && prod.Name != "General")
        {
          _obj.CalculationFlagBaseSberDev = null;
          _obj.CalculationDistributeBaseSberDev = null;
          _obj.CalculationResidualAmountBaseSberDev = 0;
          _obj.CalculationBaseSberDev.Clear();
        }
      }
      _obj.ModifiedSberDev = Calendar.Now;
    }

    public virtual void CalculationAmountBaseSberDevChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
        _obj.CalculationResidualAmountBaseSberDev = _obj.TotalAmount - _obj.CalculationBaseSberDev.Sum(s => s.AbsoluteCalc);
      else
        _obj.CalculationResidualAmountBaseSberDev = 100 - _obj.CalculationBaseSberDev.Sum(s => s.PercentCalc);
    }

    public override void TotalAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.TotalAmountChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      
      if ( _obj.CalculationBaseSberDev.Any())
      {
        if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Percent)
        {
          foreach (var prop in _obj.CalculationBaseSberDev)
          {
            if (prop.ProductCalc != null)
              prop.PercentCalc = prop.PercentCalc;
          }
        }
        if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
        {
          _obj.CalculationAmountBaseSberDev = _obj.CalculationAmountBaseSberDev;
        }
      }
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      
      
      if (e.NewValue != null)
      {
        var isInvoice = SBContracts.IncomingInvoices.Is(_obj) || SBContracts.OutgoingInvoices.Is(_obj);
        if (isInvoice)
        {
          if (_obj.PayTypeBaseSberDev == SBContracts.AccountingDocumentBase.PayTypeBaseSberDev.Prepayment)
            SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue);
        }
        else
        {
          if (_obj.PayTypeBaseSberDev == SBContracts.AccountingDocumentBase.PayTypeBaseSberDev.Postpay)
            SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue);
        }
        
        _obj.State.Properties.LeadingDocument.HighlightColor = PublicFunctions.Module.HighlightUnsignedDocument(_obj.LeadingDocument, false);
      }
      else
      {
        _obj.State.Properties.LeadingDocument.HighlightColor = Colors.Common.White;
      }
    }

    public virtual void CalculationFlagBaseSberDevChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      _obj.CalculationBaseSberDev.Clear();
    }

    public virtual void CalculationBaseSberDevChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.ModifiedSberDev = Calendar.Now;
      double? amount = 0;
      if (_obj.CalculationBaseSberDev.Any())
      {
        if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
        {
          foreach (var prop in _obj.CalculationBaseSberDev)
            amount += prop.AbsoluteCalc;
        }
        else
        {
          foreach (var prop in _obj.CalculationBaseSberDev)
            amount += prop.InterestCalc;
        }
      }
      if (amount.HasValue)
        _obj.CalculationAmountBaseSberDev = Math.Round(amount.Value, 2);
    }
  }
}