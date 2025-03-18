using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ATS.Module.RecordManagementUI.Server
{
  partial class OrderReestrSDevFolderHandlers
  {

    public virtual IQueryable<Sungero.ATS.IOrderBase> OrderReestrSDevDataQuery(IQueryable<Sungero.ATS.IOrderBase> query)
    {
      if (_filter != null)
      {
        if (_filter.DateRangeSDevFrom != null)
          query = query.Where(q => q.RegistrationDate.HasValue).Where(q => q.RegistrationDate >= _filter.DateRangeSDevFrom.Value);
        
        if (_filter.DateRangeSDevTo != null)
          query = query.Where(q => q.RegistrationDate.HasValue).Where(q => q.RegistrationDate <= _filter.DateRangeSDevTo.Value);
        
        if (!((_filter.OrderSDev) && (_filter.DerectiveSDev)))
        {
          if (_filter.OrderSDev)
            query = query.Where(q => Sungero.RecordManagement.Orders.Is(q));
          
          if (_filter.DerectiveSDev)
            query = query.Where(q => Sungero.RecordManagement.CompanyDirectives.Is(q));
        }
                
        if (_filter.DocKindSDev != null)
          query = query.Where(q => q.DocumentKind == _filter.DocKindSDev);
      }
      
      return query;
    }
  }

  partial class RecordManagementUIHandlers
  {
  }
}