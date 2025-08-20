using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts
{
  partial class AppProductPurchaseComparativeCollection7SharedCollectionHandlers
  {

    public virtual void ComparativeCollection7Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection7", _obj);
    }
  }

  partial class AppProductPurchaseComparativeCollection6SharedCollectionHandlers
  {

    public virtual void ComparativeCollection6Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection6", _obj);
    }
  }

  partial class AppProductPurchaseComparativeCollection5SharedCollectionHandlers
  {

    public virtual void ComparativeCollection5Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection5", _obj);
    }
  }

  partial class AppProductPurchaseComparativeCollection4SharedCollectionHandlers
  {

    public virtual void ComparativeCollection4Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection4", _obj);
    }
  }

  partial class AppProductPurchaseComparativeCollection3SharedCollectionHandlers
  {

    public virtual void ComparativeCollection3Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection3", _obj);
    }
  }

  partial class AppProductPurchaseComparativeCollection2SharedCollectionHandlers
  {

    public virtual void ComparativeCollection2Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection2", _obj);
    }
  }

  partial class AppProductPurchaseComparativeCollection1SharedCollectionHandlers
  {

    public virtual void ComparativeCollection1Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ComparativeCollection1", _obj);
    }
  }

  partial class AppProductPurchaseParticipantsCollectionSharedCollectionHandlers
  {

    public virtual void ParticipantsCollectionAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("ParticipantsCollection", _obj);
    }
  }

  partial class AppProductPurchasePurchasesCollectionSharedCollectionHandlers
  {

    public virtual void PurchasesCollectionAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      PublicFunctions.Module.AutoNumberCollectionItem("PurchasesCollection", _obj);
    }

  }

  partial class AppProductPurchaseSharedHandlers
  {

    public virtual void ProductCategoryChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
        _obj.State.Properties.Lot.IsRequired = _obj.ProductCategory != null ? _obj.ProductCategory == AppProductPurchase.ProductCategory.TVown : false;
    }

    public virtual void PurchasesCollectionChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (_obj.PurchasesCollection.Count > 0)
      {
        double amount = 0;
        foreach (var product in _obj.PurchasesCollection)
          if (product.PriceUnit.HasValue && product.Quantity.HasValue)
            amount += product.PriceUnit.Value * product.Quantity.Value;
        _obj.TotalAmount = amount;
      }
    }

    public virtual void SelectedCounterparty7Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty7");
    }

    public virtual void SelectedCounterparty6Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty6");
    }

    public virtual void SelectedCounterparty5Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty5");
    }

    public virtual void SelectedCounterparty4Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty4");
    }

    public virtual void SelectedCounterparty3Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty3");
    }

    public virtual void SelectedCounterparty2Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty2");
    }

    public virtual void SelectedCounterparty1Changed(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.AppProductPurchase.ResetOtherCounterpartySelections(_obj, e, "SelectedCounterparty1");
    }

    public virtual void AgencyContractChanged(sberdev.SberContracts.Shared.AppProductPurchaseAgencyContractChangedEventArgs e)
    {
     
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
     
      _obj.Relations.AddFromOrUpdate("Purchase", e.OldValue, e.NewValue);
    }

    public virtual void DepositChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      _obj.Balance = 100 - e.NewValue;
    }

  }
}