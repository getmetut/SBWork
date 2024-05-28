using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.ProductsAndDevices;

namespace sberdev.SberContracts
{
  partial class ProductsAndDevicesAddApproversApproverPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AddApproversApproverFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(c => Sungero.Company.Employees.Is(c));
    }
  }

  partial class ProductsAndDevicesServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.NoDistribute = false;
    }
  }

}