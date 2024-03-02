using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts
{
  partial class ContractualDocumentDepartmentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DepartmentFiltering(query, e);
      if (_obj.BusinessUnit != null)
      {
        return query.Where(d => Equals( d.BusinessUnit , _obj.BusinessUnit));
      }
      else
      {
        return query;
      }
    }
  }

  partial class ContractualDocumentMVPBaseSberDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVPBaseSberDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.ContrType == SberContracts.MVZ.ContrType.Profitable && q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class ContractualDocumentMVZBaseSberDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVZBaseSberDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.ContrType == SberContracts.MVZ.ContrType.Expendable && q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class ContractualDocumentCalculationBaseSberDevProductCalcPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CalculationBaseSberDevProductCalcFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals(d.BusinessUnit, _root.BusinessUnit)).Where(d => d.Name != "General");
    }
  }

  partial class ContractualDocumentAccArtPrBaseSberDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccArtPrBaseSberDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      return query.Where(q => q.ContrType == SberContracts.AccountingArticles.ContrType.Profitable);
    }
  }

  partial class ContractualDocumentAccArtPrBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccArtPrBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals( d.ContrType , SberContracts.AccountingArticles.ContrType.Profitable));
    }
  }

  partial class ContractualDocumentProdCollectionPrBaseSberDevProductSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionPrBaseSberDevProductSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class ContractualDocumentProdCollectionPrBaseSberDevProductPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionPrBaseSberDevProductFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var exep = _root.MVPBaseSberDev.ProductsCollection;
      List<SberContracts.IProductsAndDevices> exepQuery = new List<SberContracts.IProductsAndDevices>();
      foreach (var prod in exep)
        exepQuery.Add(prod.ExeptionProducts);
      List<T> filtredQuery = new List<T>();
      foreach (var q in query)
        if (exepQuery.Exists(exept => exept.Id == q.Id) == false)
          filtredQuery.Add(q);
      return filtredQuery.AsQueryable().Where(m => Equals(m.BusinessUnit, _root.BusinessUnit));
    }
  }

  partial class ContractualDocumentProdCollectionExBaseSberDevProductSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionExBaseSberDevProductSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class ContractualDocumentProdCollectionExBaseSberDevProductPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionExBaseSberDevProductFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var exep = _root.MVZBaseSberDev.ProductsCollection;
      List<SberContracts.IProductsAndDevices> exepQuery = new List<SberContracts.IProductsAndDevices>();
      foreach (var prod in exep)
        exepQuery.Add(prod.ExeptionProducts);
      List<T> filtredQuery = new List<T>();
      foreach (var q in query)
        if (exepQuery.Exists(exept => exept.Id == q.Id) == false)
          filtredQuery.Add(q);
      return filtredQuery.AsQueryable().Where(m => Equals(m.BusinessUnit, _root.BusinessUnit));
    }
  }

  partial class ContractualDocumentMVPBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVPBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals(d.ContrType, SberContracts.MVZ.ContrType.Profitable) && Equals(d.BusinessUnit, _obj.BusinessUnit));
    }
  }

  partial class ContractualDocumentMVZBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVZBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals(d.ContrType, SberContracts.MVZ.ContrType.Expendable) && Equals(d.BusinessUnit, _obj.BusinessUnit));
    }
  }

  partial class ContractualDocumentAccArtExBaseSberDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccArtExBaseSberDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      return query.Where(q => q.ContrType == SberContracts.AccountingArticles.ContrType.Expendable);
    }
  }

  partial class ContractualDocumentAccArtExBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccArtExBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals( d.ContrType , SberContracts.AccountingArticles.ContrType.Expendable));
    }
  }

  partial class ContractualDocumentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.OriginalBaseSberDev = false;
      _obj.FrameworkBaseSberDev = false;
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
      base.Created(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      Functions.ContractualDocument.CancelRequiredPropeties(_obj);
      var error = Functions.ContractualDocument.BanToSaveForStabs(_obj);
      if (error != "")
        e.AddError(error);
      
      if (_obj.CalculationFlagBaseSberDev != null)
      {
        if (_obj.CalculationAmountBaseSberDev == _obj.TotalAmount)
        {
          base.BeforeSave(e);
          Functions.ContractualDocument.BeforeSaveFunction(_obj);
        }
        else
          e.AddError(sberdev.SBContracts.Contracts.Resources.CalculationCondition);
      }
      else
      {
        base.BeforeSave(e);
        Functions.ContractualDocument.BeforeSaveFunction(_obj);
      }
      
      //============================================ ATS --------------- Заполнение списка продуктов и калькуляции
        string spisokProd = "";
        string spisokCalc = "";
        if (_obj.ProdCollectionExBaseSberDev.Count > 0)
        {
          foreach (var elem in _obj.ProdCollectionExBaseSberDev)
          {
            spisokProd += elem.Product.Name + "; ";
          }
        }
        if (_obj.ProdCollectionPrBaseSberDev.Count > 0)
        {
          foreach (var elem2 in _obj.ProdCollectionPrBaseSberDev)
          {
            spisokProd += elem2.Product.Name + "; ";
          }
        }
        if (_obj.CalculationBaseSberDev.Count > 0)
        {
          foreach (var elem3 in _obj.CalculationBaseSberDev)
          {
            spisokCalc += elem3.ProductCalc.Name + " " + elem3.AbsoluteCalc.ToString() + " (" + elem3.PercentCalc.ToString() + "); ";
          }
        }
        
        spisokProd = spisokProd.Substring(0, Math.Min(spisokProd.Length, 999));
        spisokCalc = spisokCalc.Substring(0, Math.Min(spisokCalc.Length, 999));
          
        if (_obj.ProductListSDev != spisokProd)
          _obj.ProductListSDev = spisokProd;
        
        if (_obj.CalcListSDev != spisokCalc)
          _obj.CalcListSDev = spisokCalc;
      //=============================================== ATS --------------- Заполнение списка продуктов и калькуляции
    }
  }

}