using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PurchaseProtocol;

namespace Sungero.Custom
{
  partial class PurchaseProtocolDocumentFootingPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DocumentFootingFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(d =>((sberdev.SberContracts.AppProductPurchases.Is(d)) || (sberdev.SberContracts.AppNonProdPurchases.Is(d))));
      return query;
    }
  }

}