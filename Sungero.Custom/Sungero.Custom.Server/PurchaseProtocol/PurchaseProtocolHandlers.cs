using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PurchaseProtocol;

namespace Sungero.Custom
{
  partial class PurchaseProtocolPurchaseMethodPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> PurchaseMethodFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.RouteType != null)
      {
        if (_obj.RouteType == RouteType.Purchase500)
          query = query.Where(q => q.Filter == PurchaseMethod.Filter.Purchase500);
        if (_obj.RouteType == RouteType.PurchaseFramewo)
          query = query.Where(q => q.Filter == PurchaseMethod.Filter.PurchaseFramewo);
        if (_obj.RouteType == RouteType.PurchaseUL)
          query = query.Where(q => q.Filter == PurchaseMethod.Filter.PurchaseUL);
      }
      else
        query = query.Where(q => q.Name == "@#$%^&");
        
      return query;
    }
  }

  partial class PurchaseProtocolDocumentFootingPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DocumentFootingFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(d =>((sberdev.SberContracts.AppProductPurchases.Is(d)) || (sberdev.SberContracts.AppNonProdPurchases.Is(d)) || (sberdev.SberContracts.AppRnDPurchases.Is(d))));
      return query;
    }
  }

}