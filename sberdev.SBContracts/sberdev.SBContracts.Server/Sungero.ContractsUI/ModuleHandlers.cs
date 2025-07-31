using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Company;

namespace sberdev.SBContracts.Module.ContractsUI.Server
{
  partial class FoundDocRegistrySBDEVFolderHandlers
  {

    public virtual IQueryable<Sungero.Docflow.IDocumentKind> FoundDocRegistrySBDEVDocumentKindSBDEVFiltering(IQueryable<Sungero.Docflow.IDocumentKind> query)
    {
      return query.Where(kind => kind.DocumentType.Name.Contains("Учредительный документ"));
    }

    public virtual IQueryable<sberdev.SberContracts.IFoundingDocument> FoundDocRegistrySBDEVDataQuery(IQueryable<sberdev.SberContracts.IFoundingDocument> query)
    {
      // оработка в рамках задачи DRX-669.
      // Замещения Делопроизводителя, Юриста и  Юриста ГФ ПП.
      var ops = Employees.Get(431);
      var lawyer = Employees.Get(391);
      var lawyerGF = Employees.Get(1456);
      
      // Все замещения на Делопроизводителя ПП.
      var subsOps = Sungero.CoreEntities.Substitutions.GetAll()
        .Where(s => s.User.Login != null && s.User.Login == ops.Login)
        .Select(s => s.Substitute.Login)
        .ToList();
      // Все замещения на Юриста ПП.
      var subsLawyer = Sungero.CoreEntities.Substitutions.GetAll()
        .Where(s => s.User.Login != null && s.User.Login == lawyer.Login)
        .Select(s => s.Substitute.Login)
        .ToList();
      // Все замещения на Юриста ГФ ПП.
      var subsLawyerGF = Sungero.CoreEntities.Substitutions.GetAll()
        .Where(s => s.User.Login != null && s.User.Login == lawyerGF.Login)
        .Select(s => s.Substitute.Login)
        .ToList();
      
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
        #region Area group.
        if (_filter.CommonSBDEV)
          query = query.Where(q => !q.IsSecretDoc.Value);
        if (_filter.SecretSBDEV)
          query = query.Where(q => q.IsSecretDoc.Value
                              && (subsOps.Contains(Employees.Current.Login)
                                  || subsLawyer.Contains(Employees.Current.Login)
                                  || subsLawyerGF.Contains(Employees.Current.Login)
                                  || Employees.Current.IncludedIn(Roles.Administrators)));
        #endregion
        #region Document Kind group.
        if (_filter.DocumentKindSBDEV != null)
          query = query.Where(q => q.DocumentKind != null
                              && q.DocumentKind == _filter.DocumentKindSBDEV);
        #endregion
        #region Main section.
        if (_filter.BusinessUnit != null)
          query = query.Where(q => q.BusinessUnit != null
                              && q.BusinessUnit == _filter.BusinessUnit);
        if (_filter.Department != null)
          query = query.Where(q => q.Department != null
                              && q.Department == _filter.Department);
        #endregion
        #region Period section.
        if (_filter.DateRangeFrom != null)
          query = query.Where(q => q.RegistrationDate >= _filter.DateRangeFrom
                              || q.RegistrationDate == null);
        if (_filter.DateRangeTo != null)
          query = query.Where(q => q.RegistrationDate <= _filter.DateRangeTo
                              || q.RegistrationDate == null);
        #endregion
      }
      
      return query.Where(q => q.DocumentKind != null
                         && q.DocumentKind.DocumentType != null
                         && q.DocumentKind.DocumentType.Name.Contains("Учредительный документ"));
    }
  }

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
      string purchaseAdvertise = "Рекламная заявка";
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
                                  || q.DocumentKind.Name.Contains(purchaseAdvertise)
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
          query = query.Where(q => q.RegistrationDate >= _filter.DateRangeFrom
                              || q.RegistrationDate == null);
        if (_filter.DateRangeTo != null)
          query = query.Where(q => q.RegistrationDate <= _filter.DateRangeTo
                              || q.RegistrationDate == null);
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