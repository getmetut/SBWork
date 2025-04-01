using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Contract;

namespace sberdev.SBContracts.Shared
{
  partial class ContractFunctions
  {
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool isRepeatRegister)
    {
      base.ChangeDocumentPropertiesAccess(isEnabled, isRepeatRegister);
      
      if ( Functions.Contract.Remote.CurrentUserInKZ()  && _obj.InternalApprovalState != sberdev.SBContracts.Contract.InternalApprovalState.Signed)
      {_obj.State.Properties.TotalAmount.IsEnabled = true ;
        _obj.State.Properties.Currency.IsEnabled = true ;}
    }
    
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.DaysToFinishWorks.IsRequired = false;//
      if (_obj.IsAutomaticRenewal.GetValueOrDefault() == true)
      {
        _obj.State.Properties.DaysToFinishWorks.IsEnabled = false;
      }
      _obj.State.Properties.ValidTill.IsRequired = false;
    }
    
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      if (_obj.DocumentKind != null)
      {
        string kind = _obj.DocumentKind.Name;
        bool isAcc = kind == "Счет-договор" || kind == "Договор-оферта";
        _obj.State.Properties.PayTypeBaseSungero.IsVisible = isAcc;
        _obj.State.Properties.PayTypeBaseSungero.IsRequired = isAcc;
      }
    }
  }
}
