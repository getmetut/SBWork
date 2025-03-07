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

    public override void AccArtExBaseSberDevChanged(sberdev.SBContracts.Shared.ContractualDocumentAccArtExBaseSberDevChangedEventArgs e)
    {
      PublicFunctions.Purchase.UpdateCard(_obj);
      base.AccArtExBaseSberDevChanged(e);
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      _obj.ModifiedSberDev = Calendar.Now;
     _obj.Relations.AddFromOrUpdate("Purchase", e.OldValue, e.NewValue);
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

  }
}