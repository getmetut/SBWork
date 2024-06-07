using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;

namespace sberdev.SBContracts
{

  partial class AccountingDocumentBaseLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      return query.Where(q => SBContracts.Contracts.Is(q) || SBContracts.SupAgreements.Is(q));
    }
  }



  partial class AccountingDocumentBaseAccArtBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccArtBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(q => q.ContrType == _obj.ContrTypeBaseSberDev);
    }
  }


  partial class AccountingDocumentBaseMVPBaseSberDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVPBaseSberDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.ContrType == SberContracts.MVZ.ContrType.Profitable && q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class AccountingDocumentBaseMVZBaseSberDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVZBaseSberDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.ContrType == SberContracts.MVZ.ContrType.Expendable && q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class AccountingDocumentBaseProdCollectionBaseSberDevProductSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionBaseSberDevProductSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var cashe = SBContracts.PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      return query.Where(q => q.BusinessUnit == cashe.BusinessUnit);
    }
  }

  partial class AccountingDocumentBaseAccDocSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccDocSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparty != null)
        return query.Where(d => Equals(d.Counterparty, _obj.Counterparty));
      else
        return query;
    }
  }

  partial class AccountingDocumentBaseMVZBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVZBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals(d.ContrType, SberContracts.MVZ.ContrType.Expendable) && Equals(d.BusinessUnit, _obj.BusinessUnit));
    }
  }

  partial class AccountingDocumentBaseMVPBaseSberDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVPBaseSberDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals(d.ContrType, SberContracts.MVZ.ContrType.Profitable) && Equals(d.BusinessUnit, _obj.BusinessUnit));
    }
  }

  partial class AccountingDocumentBaseProdCollectionBaseSberDevProductPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionBaseSberDevProductFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var exepMVZ = _root.MVZBaseSberDev != null ? _root.MVZBaseSberDev.ProductsCollection : null;
      var exepMVP = _root.MVPBaseSberDev != null ? _root.MVPBaseSberDev.ProductsCollection : null;
      List<SberContracts.IProductsAndDevices> exepQuery = new List<SberContracts.IProductsAndDevices>();
      if (exepMVZ != null)
        foreach (var prod in exepMVZ)
          exepQuery.Add(prod.ExeptionProducts);
      if (exepMVP != null)
        foreach (var prod in exepMVP)
          exepQuery.Add(prod.ExeptionProducts);
      List<T> filtredQuery = new List<T>();
      foreach (var q in query)
        if (exepQuery.Exists(exept => exept.Id == q.Id) == false)
          filtredQuery.Add(q);
      return filtredQuery.AsQueryable().Where(m => Equals(m.BusinessUnit, _root.BusinessUnit));
    }
  }

  partial class AccountingDocumentBaseCalculationBaseSberDevProductCalcPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CalculationBaseSberDevProductCalcFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => Equals(d.BusinessUnit, _root.BusinessUnit)).Where(d => d.Name != "General");
    }
  }

  partial class AccountingDocumentBaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.FrameworkBaseSberDev = false;
      base.Created(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      Functions.AccountingDocumentBase.ReplaceProducts(_obj);
      Functions.AccountingDocumentBase.CreateOrUpdateAnaliticsCashe(_obj);
      Functions.AccountingDocumentBase.CreateOrUpdateAnaliticsCasheGeneral(_obj);
      Functions.AccountingDocumentBase.CancelRequiredPropeties(_obj);
      _obj.CalcListSDev = PublicFunctions.AccountingDocumentBase.GetCalculationString(_obj);
      var error = Functions.AccountingDocumentBase.BanToSaveForStabs(_obj);
      if (error != "")
        e.AddError(error);
      if (_obj.CalculationBaseSberDev.Count > 0)
      {
        if (_obj.CalculationAmountBaseSberDev == _obj.TotalAmount)
        {
          base.BeforeSave(e);
        }
        else
          e.AddError(sberdev.SBContracts.Contracts.Resources.CalculationCondition);
      }
      else
        base.BeforeSave(e);
    }
  }

}