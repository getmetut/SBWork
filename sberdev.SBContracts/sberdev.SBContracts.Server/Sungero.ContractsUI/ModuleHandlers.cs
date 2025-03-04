using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.ContractsUI.Server
{
  partial class ReestrNDASDevFolderHandlers
  {

    public virtual IQueryable<Sungero.Docflow.IDocumentKind> ReestrNDASDevKindDocSDevFiltering(IQueryable<Sungero.Docflow.IDocumentKind> query)
    {
      query = query.Where(q => q.DocumentType.Name == "NDA / Соглашение об ЭДО");
      return query;
    }

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