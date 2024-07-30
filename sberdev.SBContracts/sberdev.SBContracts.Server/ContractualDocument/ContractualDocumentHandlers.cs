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
      var exep = _root.MVPBaseSberDev?.ProductsCollection;
      if (exep == null)
        return query.Where(m => Equals(m.BusinessUnit, _root.BusinessUnit));
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
      var exep = _root.MVZBaseSberDev?.ProductsCollection;
      if (exep == null)
        return query.Where(m => Equals(m.BusinessUnit, _root.BusinessUnit));
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
      base.Created(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      Functions.ContractualDocument.CancelRequiredPropeties(_obj);
      var error = Functions.ContractualDocument.BanToSaveForStabs(_obj);
      if (error != "")
        e.AddError(error);
      
      if (_obj.CalculationBaseSberDev.Count > 0)
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
      int year = Calendar.Now.Year % 100; // Получаем последние две цифры года
      char symbol = 'Z';
      int sequenceNumber = int.Parse(_obj.Id.ToString()); // Пример порядкового номера
      string Num1C = $"{year:D2}{symbol:D}{sequenceNumber:D5}";
      _obj.NumOrder1CUTSDev = Num1C;
    }
  }
}