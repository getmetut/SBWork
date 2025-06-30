using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.ContractsUI.Server
{
  partial class PurchaseRegistryFolderHandlers
  {

    public virtual IQueryable<sberdev.SberContracts.IAbstractsSupAgreement> PurchaseRegistryDataQuery(IQueryable<sberdev.SberContracts.IAbstractsSupAgreement> query)
    {
      string appProdPurchase = "Заявка на производ. закупку";
      string appRnDPurchase = "Заявка на RnD закупку";
      string purchase = "Закупка";
      string purchaseNonProd = "Закупка непроизводственная";
      string purchaseProd = "Закупка производственная";
      string purchaseStudy = "Закупка учебная";
      if (_filter != null)
      {
        #region Document state.
        if (_filter.Draft)
          query = query.Where(q => q.LifeCycleState == SberContracts.AbstractsSupAgreement.LifeCycleState.Draft);     
        if (_filter.Active)
          query = query.Where(q => q.LifeCycleState == SberContracts.AbstractsSupAgreement.LifeCycleState.Active);
        if (_filter.Obsolete)
          query = query.Where(q => q.LifeCycleState == SberContracts.AbstractsSupAgreement.LifeCycleState.Obsolete);
        #endregion
        #region Document type.
        if (_filter.AppNonProdPurchase)
          query = query.Where(q => q.DocumentKind != null
                              && string.Equals(q.DocumentKind.Name, purchaseNonProd));
        if (_filter.AppProductPurchase)
          query = query.Where(q => q.DocumentKind != null
                              && string.Equals(q.DocumentKind.Name, appProdPurchase));
        if (_filter.AppRnDPurchase)
          query = query.Where(q => q.DocumentKind != null
                              && string.Equals(q.DocumentKind.Name, appRnDPurchase));
        if (_filter.Purchase)
          query = query.Where(q => q.DocumentKind != null
                              && !q.DocumentKind.Name.Contains(purchaseNonProd)
                              && (q.DocumentKind.Name.Contains(purchase)
                                  || q.DocumentKind.Name.Contains(purchaseProd)
                                  || q.DocumentKind.Name.Contains(purchaseStudy)
                                 ));
        #endregion
        #region Main section.
        if (_filter.Contractor != null)
          query = query.Where(q => q.Counterparty != null
                              && q.Counterparty == _filter.Contractor);
        if (_filter.BusinessUnit != null)
          query = query.Where(q => q.BusinessUnit != null
                              && q.BusinessUnit == _filter.BusinessUnit);
        if (_filter.Department != null)
          query = query.Where(q => q.Department != null
                              && q.Department == _filter.Department);
        #endregion
        #region Period section.
        if (_filter.DateRangeFrom != null)
          query = query.Where(q => q.RegistrationDate >= _filter.DateRangeFrom);
        if (_filter.DateRangeTo != null)
          query = query.Where(q => q.RegistrationDate <= _filter.DateRangeTo);
        #endregion
      }
      return query.Where(q => q.DocumentKind != null 
                         && (q.DocumentKind.Name.Contains("Закупка непроизводственная")
                            || q.DocumentKind.Name.Contains("Заявка на производ. закупку")
                            || q.DocumentKind.Name.Contains("Заявка на RnD закупку")
                            || q.DocumentKind.Name.Contains("Закупка")
                           ));
    }
  }

  partial class ReestrNDASDevFolderHandlers
  {

    public virtual IQueryable<Sungero.Custom.INDA> ReestrNDASDevDataQuery(IQueryable<Sungero.Custom.INDA> query)
    {
      if (_filter != null)
      {
        if (_filter.CounterpartySDev != null)
          query = query.Where(q => q.Counterparty == _filter.CounterpartySDev);
        
        if (_filter.KindDocSDev != null)
          query = query.Where(q => q.DocumentKind == _filter.KindDocSDev);
        
        if (_filter.AuthorDocSDev != null)
          query = query.Where(q => q.Author.Login == _filter.AuthorDocSDev.Login);
        
        if (_filter.DepartmentSDev != null)
          query = query.Where(q => q.Department == _filter.DepartmentSDev);
        
        if (_filter.DateRangeSDevTo != null)
          query = query.Where(q => q.RegistrationDate <= _filter.DateRangeSDevTo);
        
        if (_filter.DateRangeSDevFrom != null)
          query = query.Where(q => q.RegistrationDate >= _filter.DateRangeSDevFrom);
      }
      return query;
    }
  }

  partial class ContractsListFolderHandlers
  {

    public override IQueryable<Sungero.Contracts.IContractualDocument> ContractsListDataQuery(IQueryable<Sungero.Contracts.IContractualDocument> query)
    {
      /*
      if (!Users.Current.IncludedIn(sberdev.SberContracts.PublicConstants.Module.FACTypeGuid) &&
          !Users.Current.IncludedIn(Roles.Administrators))
      {
        var currentEmployee = Sungero.Company.Employees.As(Users.Current);
        var currentDepartment = currentEmployee?.Department;
        var mvzList = SberContracts.MVZs.GetAll()
          .Where(l => l.BudgetOwner == currentEmployee)
          .ToList();

        query = query.Cast<sberdev.SBContracts.IContractualDocument>()
          .Where(r =>
                 r.Department == currentDepartment ||
                 (r.MVZBaseSberDev != null && mvzList.Contains(r.MVZBaseSberDev)) ||
                 (r.MVPBaseSberDev != null && mvzList.Contains(r.MVPBaseSberDev)))
          .Cast<Sungero.Contracts.IContractualDocument>();
      }*/

      
      return base.ContractsListDataQuery(query);
    }
  }

  partial class ContractsUIHandlers
  {
  }
}