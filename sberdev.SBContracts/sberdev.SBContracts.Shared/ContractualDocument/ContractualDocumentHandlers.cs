using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts
{
  partial class ContractualDocumentCalculationBaseSberDevSharedHandlers
  {

    public virtual void CalculationBaseSberDevPercentCalcChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.InterestCalc = null;
        return;
      }
      if (_obj.ContractualDocument.TotalAmount.HasValue)
        _obj.InterestCalc = Math.Round(e.NewValue.Value * _obj.ContractualDocument.TotalAmount.Value / (double)100, 2);
    }

    public virtual void CalculationBaseSberDevProductCalcChanged(sberdev.SBContracts.Shared.ContractualDocumentCalculationBaseSberDevProductCalcChangedEventArgs e)
    {
      _obj.AggregationCalc = _obj.ProductCalc != null ? SberContracts.PublicFunctions.Module.GetRusAggregationName(_obj.ProductCalc.Aggregation): null;
    }
  }

  partial class ContractualDocumentSharedHandlers
  {

    public override void CounterpartyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue != null && SBContracts.Companies.Is(e.NewValue))
        {
          _obj.CounterpartyTINSberDev = e.NewValue.TIN;
          _obj.CouterpartyTRRCSberDev = SBContracts.Companies.As(e.NewValue).TRRC;
        }
        else
        {
          _obj.CounterpartyTINSberDev = null;
          _obj.CouterpartyTRRCSberDev = null;
        }
      }
    }

    public virtual void AmountPrepaySberDevChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue > 100)
        return;
    }

    public virtual void PurchComNumberSberDevChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.Length == 4)
      {
        var valArr = e.NewValue.ToArray();
        bool flag = true;
        foreach (char val in valArr)
          if (!Char.IsDigit(val))
        {
          flag = false;
          return;
        }
        Functions.ContractualDocument.Remote.SetPurchComNumber(_obj, e.NewValue);
      }
    }

    public override void ResponsibleEmployeeChanged(Sungero.Contracts.Shared.ContractualDocumentResponsibleEmployeeChangedEventArgs e)
    {
      base.ResponsibleEmployeeChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void ValidTillChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.ValidTillChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void ValidFromChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.ValidFromChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void CurrencyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCurrencyChangedEventArgs e)
    {
      base.CurrencyChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public override void IsStandardChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.IsStandardChanged(e);
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

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
        _obj.FrameworkBaseSberDev = false;
      }
      if (e.NewValue != null)
      {
        var name = e.NewValue.Name;
        if (name == "Договор Xiongxin" || name == "Дополнительное соглашение Xiongxin")
        {
          List<int> ids = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("ИД сущностей для договора Xiongxin").Text.Split(',').Select(s => Int32.Parse(s)).ToList();
          _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
          Functions.ContractualDocument.Remote.SetXiongxinProps(_obj, ids);
        }
      }
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      
      if (e.NewValue != null)
      {
        if (_obj.Department != null && _obj.Department.BusinessUnit != null && !Equals(e.NewValue , _obj.Department.BusinessUnit))
        {
          _obj.Department = null;
          _obj.ResponsibleEmployee = null;
        }
      }
      _obj.CalculationBaseSberDev.Clear();
      _obj.MVZBaseSberDev = null;
      _obj.MVPBaseSberDev = null;
      _obj.ProdCollectionExBaseSberDev.Clear();
      _obj.ProdCollectionPrBaseSberDev.Clear();
    }

    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable) && (_obj.MVPBaseSberDev == null))
      {
        var mvp = SberContracts.MVZs.GetAll( l => (l.BudgetOwner.Department == e.NewValue) && (l.ContrType == SberContracts.MVZ.ContrType.Profitable )).FirstOrDefault();
        if (mvp != null)
        {
          _obj.MVPBaseSberDev = mvp;
        }
      }
      if ((e.NewValue != null) && (_obj.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable) && (_obj.MVZBaseSberDev == null))
      {
        var mvp = SberContracts.MVZs.GetAll( l => (l.BudgetOwner.Department == e.NewValue) && (l.ContrType == SberContracts.MVZ.ContrType.Expendable )).FirstOrDefault();
        if (mvp != null)
        {
          _obj.MVZBaseSberDev = mvp;
        }
      }
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
          //      SberContracts.PublicFunctions.Module.ShowTotalAmountChangedDialog(false);
        }
        if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
        {
          _obj.CalculationAmountBaseSberDev = _obj.CalculationAmountBaseSberDev;
          //      SberContracts.PublicFunctions.Module.ShowTotalAmountChangedDialog(true);
        }
      }
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      if (e.NewValue != e.OldValue)
        _obj.ModifiedSberDev = Calendar.Now;
      
      if (e.NewValue != null)
      {
        
        SberContracts.PublicFunctions.Module.Remote.FillFromDocumentSrv(_obj, e.NewValue);
        
        var contract = SBContracts.ContractualDocuments.As(e.NewValue);
        if (contract.DocumentKind.Name == "Договор Xiongxin")
        {
          _obj.AgentSaluteSberDev = contract.AgentSaluteSberDev;
        }
      }
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

    public virtual void CalculationAmountBaseSberDevChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
        _obj.CalculationResidualAmountBaseSberDev = _obj.TotalAmount - _obj.CalculationBaseSberDev.Sum(s => s.AbsoluteCalc);
      else
        _obj.CalculationResidualAmountBaseSberDev = 100 - _obj.CalculationBaseSberDev.Sum(s => s.PercentCalc);
    }

    public virtual void CalculationFlagBaseSberDevChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      _obj.CalculationBaseSberDev.Clear();
    }

    public virtual void FrameworkBaseSberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      var name = _obj.DocumentKind?.Name;
      if (e.NewValue == true && (name == "Договор Xiongxin" || name == "Дополнительное соглашение Xiongxin"))
      {
        _obj.FrameworkBaseSberDev = false;
        return;
      }
      if (e.NewValue == true)
      {
        _obj.TotalAmount = null;
        _obj.Currency = null;
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationBaseSberDev.Clear();
        _obj.State.Properties.CalculationBaseSberDev.IsEnabled = false;
        _obj.State.Properties.CalculationBaseSberDev.IsRequired = false;
        _obj.State.Properties.CalculationFlagBaseSberDev.IsEnabled = false;
        _obj.State.Properties.CalculationFlagBaseSberDev.IsRequired = false;
      }
    }

    public virtual void AccArtPrBaseSberDevChanged(sberdev.SBContracts.Shared.ContractualDocumentAccArtPrBaseSberDevChangedEventArgs e)
    {
      if ( e.NewValue != null)
      {
        _obj.BudItemBaseSberDev = _obj.AccArtPrBaseSberDev.BudgetItem;
      }
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public virtual void ProdCollectionPrBaseSberDevChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (_obj.ProdCollectionPrBaseSberDev.Any())
      {
        var prod = _obj.ProdCollectionPrBaseSberDev.FirstOrDefault().Product;
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

    public virtual void MVPBaseSberDevChanged(sberdev.SBContracts.Shared.ContractualDocumentMVPBaseSberDevChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationResidualAmountBaseSberDev = 0;
        _obj.CalculationBaseSberDev.Clear();
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public virtual void AccArtExBaseSberDevChanged(sberdev.SBContracts.Shared.ContractualDocumentAccArtExBaseSberDevChangedEventArgs e)
    {
      if ( e.NewValue != null)
      {
        _obj.BudItemBaseSberDev = _obj.AccArtExBaseSberDev.BudgetItem;
      }
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
    }

    public virtual void ProdCollectionExBaseSberDevChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (_obj.ProdCollectionExBaseSberDev.Any())
      {
        var prod = _obj.ProdCollectionExBaseSberDev.FirstOrDefault().Product;
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

    public virtual void MVZBaseSberDevChanged(sberdev.SBContracts.Shared.ContractualDocumentMVZBaseSberDevChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationResidualAmountBaseSberDev = 0;
        _obj.CalculationBaseSberDev.Clear();
        _obj.ModifiedSberDev = Calendar.Now;
        _obj.MarketDirectSberDev = null;
      }
    }

    public virtual void ContrTypeBaseSberDevChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.ModifiedSberDev = Calendar.Now;
      }
      _obj.DocumentGroup = null;
      if (e.NewValue == ContrTypeBaseSberDev.Profitable)
      {
        _obj.MVZBaseSberDev = null;
        _obj.AccArtExBaseSberDev = null;
        _obj.ProdCollectionExBaseSberDev.Clear();
      }
      if (e.NewValue == ContrTypeBaseSberDev.Expendable)
      {
        _obj.MVPBaseSberDev = null;
        _obj.AccArtPrBaseSberDev = null;
        _obj.ProdCollectionPrBaseSberDev.Clear();
      }
    }
  }
}
