using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts
{
  partial class PurchaseSharedHandlers
  {

    public virtual void MethodPurchaseChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == MethodPurchase.SeveralCP)
        _obj.CostAnalysisCollection.Clear();
    }

    public virtual void SpecificationChanged(sberdev.SberContracts.Shared.PurchaseSpecificationChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public virtual void CommercialOfferChanged(sberdev.SberContracts.Shared.PurchaseCommercialOfferChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public virtual void UpgradedCommercialOfferChanged(sberdev.SberContracts.Shared.PurchaseUpgradedCommercialOfferChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public virtual void ConcludedContractsKindChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == ConcludedContractsKind.No)
        _obj.LeadingDocument = null;
    }

  }
}