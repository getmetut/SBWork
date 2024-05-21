using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.DatabookJobs;

namespace Sungero.Custom
{
  partial class DatabookJobsFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if ((!Users.Current.IncludedIn(Roles.Administrators)) && (Users.Current.Name != "Service User") && (Users.Current.Name != "Administrator"))
      {
        var CurUs = Sungero.Company.Employees.GetAll(u => u.Login == Users.Current.Login).FirstOrDefault();
        if (CurUs != null)
          query = query.Where(r => r.EmployeeJob == CurUs);
      }
      if (_filter != null)
      {
        if (_filter.NOR != null)
          query = query.Where(q => q.BusinessUnit == _filter.NOR);
        
        if (_filter.Company != null)
          query = query.Where(q => q.Company == _filter.Company);
      }
      return query;
    }
  }

}