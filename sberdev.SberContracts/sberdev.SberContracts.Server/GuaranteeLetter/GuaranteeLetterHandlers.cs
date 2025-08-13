using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core; 
using Sungero.CoreEntities;
using sberdev.SberContracts.GuaranteeLetter;

namespace sberdev.SberContracts
{
  partial class GuaranteeLetterAddendumDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> AddendumDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //query = base.AddendumDocumentFiltering(query, e);
      query = query.Where(d => ((sberdev.SberContracts.Purchases.Is(d)) || (sberdev.SberContracts.AppNonProdPurchases.Is(d))));
      return query;
    }
  }

  partial class GuaranteeLetterServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
    }
  }


}