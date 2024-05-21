using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.MVZ;

namespace sberdev.SberContracts
{
  partial class MVZProductsCollectionExeptionProductsPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProductsCollectionExeptionProductsFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(q => Equals(q.BusinessUnit, _root.BusinessUnit));
    }
  }

  partial class MVZServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.CalculationIsWorking = false;
    }
  }

  partial class MVZMainMVZPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MainMVZFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.ContrType != null)
      {
        if (_obj.ContrType == SberContracts.MVZ.ContrType.Expendable)
        {return query.Where(d => Equals( d.ContrType , SberContracts.MVZ.ContrType.Expendable));}
        else
        {return query.Where(d => Equals( d.ContrType , SberContracts.MVZ.ContrType.Profitable));}
      }
      else
      {return query;}
    }
  }

}