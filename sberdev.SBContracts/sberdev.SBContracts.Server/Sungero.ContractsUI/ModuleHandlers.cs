using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.ContractsUI.Server
{
  partial class ContractsListFolderHandlers
  {

    public override IQueryable<Sungero.Contracts.IContractualDocument> ContractsListDataQuery(IQueryable<Sungero.Contracts.IContractualDocument> query)
    {
      if (!Users.Current.IncludedIn( sberdev.SberContracts.PublicConstants.Module.FACTypeGuid) && !Users.Current.IncludedIn( Roles.Administrators))
          {
        var MVZ = SberContracts.MVZs.GetAll().Where(l => l.BudgetOwner == Sungero.Company.Employees.As(Users.Current));
        query = query.Cast<sberdev.SBContracts.IContract>().Where(r => (r.Department == Sungero.Company.Employees.As(Users.Current).Department) || (  r.MVZBaseSberDev != null && MVZ != null && r.MVZBaseSberDev == MVZ ) || (  r.MVPBaseSberDev != null && MVZ != null && r.MVPBaseSberDev == MVZ ))
          .Cast<Sungero.Contracts.IContractualDocument>(); //.GetAll().Where(d => d.Login == Users.Current.Login).First().Department);
          }
      
      return base.ContractsListDataQuery(query);
    }
  }

  partial class ContractsUIHandlers
  {
  }
}