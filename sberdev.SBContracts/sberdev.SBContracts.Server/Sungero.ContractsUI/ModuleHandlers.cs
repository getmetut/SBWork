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