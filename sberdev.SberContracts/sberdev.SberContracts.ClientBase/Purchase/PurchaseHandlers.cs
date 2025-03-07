using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts
{
  partial class PurchaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      PublicFunctions.Purchase.UpdateCard(_obj);
      base.Refresh(e);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      PublicFunctions.Purchase.UpdateCard(_obj);
      base.Showing(e);
    }

    public virtual void NecessaryConcludeValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (e.NewValue == NecessaryConclude.SupAgreement && _obj.ConcludedContractsKind == ConcludedContractsKind.No)
        e.AddError(sberdev.SberContracts.Purchases.Resources.NecessaryConcludeError);
    }

    public virtual void MethodPurchaseValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (e.NewValue == MethodPurchase.SeveralCP)
        _obj.CostAnalysisCollection.Clear();
    }

    public virtual void ConcludedContractsKindValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (e.NewValue == ConcludedContractsKind.No)
        _obj.LeadingDocument = null;
    }

    public virtual void PurchaseAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      // для срабатывания обновления формы
    }

    public virtual void PrepaymentPercentValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue > 100)
        e.AddError(sberdev.SberContracts.Purchases.Resources.DiscountError);
    }

    public virtual void NegotiationsDiscountValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue > 100)
        e.AddError(sberdev.SberContracts.Purchases.Resources.DiscountError);
    }

  }
}