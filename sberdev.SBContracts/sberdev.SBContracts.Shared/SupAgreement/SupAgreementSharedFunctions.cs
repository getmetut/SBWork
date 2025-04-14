using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts.Shared
{
  partial class SupAgreementFunctions
  {
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      
      bool specPAOFlag = _obj.DocumentKind != null && _obj.DocumentKind.Name == "Спецификация ПАО";
      
      _obj.State.Properties.SDSFSberDev.IsVisible = specPAOFlag;
      _obj.State.Properties.SDSFSberDev.IsRequired = specPAOFlag;
      _obj.State.Properties.SRSberDev.IsVisible = specPAOFlag;
      _obj.State.Properties.SRSberDev.IsRequired = specPAOFlag;
      _obj.State.Properties.GoogleDocsLinkSberDev.IsVisible = specPAOFlag;
      _obj.State.Properties.GoogleDocsLinkSberDev.IsRequired = specPAOFlag;
      _obj.State.Properties.SubjectSpecificationSberDev.IsVisible = specPAOFlag;
      
      bool orderXXFlag = _obj.DocumentKind != null && _obj.DocumentKind.Name == "Заказ Xiongxin";
      
      _obj.State.Properties.DelConditionEnSberDev.IsVisible = orderXXFlag;
      _obj.State.Properties.DelConditionEnSberDev.IsRequired = orderXXFlag;
      _obj.State.Properties.OrderXXTableSberDev.IsVisible = orderXXFlag;
      _obj.State.Properties.OrderXXTableSberDev.IsRequired = orderXXFlag;
      _obj.State.Properties.DelConditionChSberDev.IsVisible = orderXXFlag;
      _obj.State.Properties.DeliveryDateSberDev.IsVisible = orderXXFlag;
      _obj.State.Properties.DeliveryDateSberDev.IsRequired = orderXXFlag;
      _obj.State.Properties.AgentSaluteSberDev.IsVisible = orderXXFlag;
      _obj.State.Properties.AgentSaluteSberDev.IsRequired = orderXXFlag;
    }
    
    /// <summary>
    /// Переносит информацию по доставке из таргета в объект
    /// </summary>
    public void FillDeliveryInfo(SBContracts.IContractualDocument target)
    {
      if (target != null && (target.DeliveryMethod?.Id == 3 || target.DeliveryMethod?.Id == 2))
      {
        _obj.AdressSberDev = target.AdressSberDev;
        _obj.EmailSberDev = target.EmailSberDev;
        _obj.PhoneNumberSberDev = target.PhoneNumberSberDev;
      }
    }
  }
}