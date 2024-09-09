using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts.Shared
{
  partial class PurchaseFunctions
  {
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      _obj.State.Properties.ContrTypeBaseSberDev.IsEnabled = false;
      _obj.State.Properties.Subject.IsEnabled = true;
      _obj.State.Properties.FrameworkBaseSberDev.IsEnabled = false;
      
      if (_obj.PrepaymentPercent > 0)
      {
        _obj.State.Properties.PrepaymentJustification.IsVisible = false;
        _obj.State.Properties.PrepaymentJustification.IsRequired = false;
      }
      else
      {
        _obj.State.Properties.PrepaymentJustification.IsVisible = false;
        _obj.State.Properties.PrepaymentJustification.IsRequired = false;
      }
      
      if (_obj.ConcludedContractsKind == ConcludedContractsKind.No)
      {
        _obj.State.Properties.LeadingDocument.IsVisible = false;
        _obj.State.Properties.LeadingDocument.IsRequired = false;
      }
      else
      {
        _obj.State.Properties.LeadingDocument.IsVisible = true;
        _obj.State.Properties.LeadingDocument.IsRequired = true;
      }
      
      if(_obj.MethodPurchase == MethodPurchase.OneCP)
      {
        _obj.State.Properties.CostAnalysisCollection.IsRequired = false;
        _obj.State.Properties.CostAnalysisCollection.IsVisible = false;
      }
      else
      {
        _obj.State.Properties.CostAnalysisCollection.IsRequired = true;
        _obj.State.Properties.CostAnalysisCollection.IsVisible = true;
      }
      
      bool isShortPurchase = _obj.PurchaseAmount.HasValue && _obj.PurchaseAmount < 500000;
      
      _obj.State.Properties.InfoSubjectPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.InfoSubjectPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.SubjectPurchaseGen.IsRequired = !isShortPurchase;
      _obj.State.Properties.SubjectPurchaseGen.IsVisible = !isShortPurchase;
      _obj.State.Properties.MethodPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.MethodPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.ProjectName.IsRequired = !isShortPurchase;
      _obj.State.Properties.ProjectName.IsVisible = !isShortPurchase;
      _obj.State.Properties.RealizeCollection.IsRequired = !isShortPurchase;
      _obj.State.Properties.RealizeCollection.IsVisible = !isShortPurchase;
      _obj.State.Properties.HistoryPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.HistoryPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.TasksPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.TasksPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.IfNoPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.IfNoPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.JustifImpossibInhouse.IsRequired = !isShortPurchase;
      _obj.State.Properties.JustifImpossibInhouse.IsVisible = !isShortPurchase;
      _obj.State.Properties.KindPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.KindPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.CommercialOffer.IsRequired = !isShortPurchase;
      _obj.State.Properties.CommercialOffer.IsVisible = !isShortPurchase;
      _obj.State.Properties.StagesPurchaseCollection.IsRequired = !isShortPurchase;
      _obj.State.Properties.StagesPurchaseCollection.IsVisible = !isShortPurchase;
      _obj.State.Properties.DepartmentPurchase.IsRequired = !isShortPurchase;
      _obj.State.Properties.DepartmentPurchase.IsVisible = !isShortPurchase;
      _obj.State.Properties.ServiceEndDate.IsRequired = !isShortPurchase;
      _obj.State.Properties.ServiceEndDate.IsVisible = !isShortPurchase;
      _obj.State.Properties.ServiceStartDate.IsRequired = !isShortPurchase;
      _obj.State.Properties.ServiceStartDate.IsVisible = !isShortPurchase;
      _obj.State.Properties.Authorized.IsRequired = !isShortPurchase;
      _obj.State.Properties.Authorized.IsVisible = !isShortPurchase;
      _obj.State.Properties.NecessaryConclude.IsRequired = !isShortPurchase;
      _obj.State.Properties.NecessaryConclude.IsVisible = !isShortPurchase;
      _obj.State.Properties.CostAnalysisCollection.IsVisible = !isShortPurchase;
      _obj.State.Properties.CostAnalysisCollection.IsRequired = !isShortPurchase;
      _obj.State.Properties.Specification.IsVisible = !isShortPurchase;
      _obj.State.Properties.Necessary.IsVisible = !isShortPurchase;
      _obj.State.Properties.ScreenBusinessPlan.IsVisible = !isShortPurchase;
      _obj.State.Properties.UpgradedCommercialOffer.IsVisible = !isShortPurchase;
      _obj.State.Properties.NegotiationsDiscount.IsVisible = !isShortPurchase;
      _obj.State.Properties.PrepaymentJustification.IsVisible = !isShortPurchase;
      
    }
  }
}