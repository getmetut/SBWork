using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Server
{
  partial class FolderPurchaseProtocolFolderHandlers
  {

    public virtual IQueryable<Sungero.Custom.IPurchaseProtocol> FolderPurchaseProtocolPreFiltering(IQueryable<Sungero.Custom.IPurchaseProtocol> query)
    {
      if (_filter != null)
      {
        if (_filter.ZK)
          query = query.Where(q => q.RouteType == Sungero.Custom.PurchaseProtocol.RouteType.PurchaseFramewo);
        if (_filter.UL)
          query = query.Where(q => q.RouteType == Sungero.Custom.PurchaseProtocol.RouteType.PurchaseUL);
        if (_filter.Do500)
          query = query.Where(q => q.RouteType == Sungero.Custom.PurchaseProtocol.RouteType.Purchase500);
      }
      return query;
    }
  }


  partial class SberContractsHandlers
  {
  }
}