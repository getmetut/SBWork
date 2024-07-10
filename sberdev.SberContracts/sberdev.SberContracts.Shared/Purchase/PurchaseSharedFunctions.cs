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
    }
  }
}