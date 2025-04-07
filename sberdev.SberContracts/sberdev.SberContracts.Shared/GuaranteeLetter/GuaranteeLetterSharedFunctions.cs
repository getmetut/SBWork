using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.GuaranteeLetter;

namespace sberdev.SberContracts.Shared
{
  partial class GuaranteeLetterFunctions
  {

    /// <summary>
    /// Обновление карточки документа
    /// </summary>  
    [Public]
    public void UpdateCard()
    {
      _obj.State.Properties.AddendumDocument.IsVisible = true;
      _obj.State.Properties.AddendumDocument.IsRequired = true;
      _obj.State.Properties.TotalAmount.IsVisible = true;
      _obj.State.Properties.Currency.IsVisible = true;
      if (_obj.DocumentKind != null)
      {
        var Kind = sberdev.SBContracts.DocumentKinds.As(_obj.DocumentKind);
        if (Kind.ReklamaSDev.HasValue)
        {
          if (Kind.ReklamaSDev.Value)
          {
            _obj.State.Properties.TotalAmount.IsVisible = false;
            _obj.State.Properties.TotalAmount.IsRequired = false;
            _obj.State.Properties.Currency.IsVisible = false;
            _obj.State.Properties.Currency.IsRequired = false;
            _obj.State.Properties.AddendumDocument.IsVisible = false;
            _obj.State.Properties.AddendumDocument.IsRequired = false;
          }
        }
      }
    }
    public override void SetPropertiesAccess()
    {
      base.SetPropertiesAccess();
      
      _obj.State.Properties.ContrTypeBaseSberDev.IsEnabled = false;
      if (_obj.TotalAmount > 5000000)
        _obj.State.Properties.AddendumDocument.IsRequired = true;
      else
        _obj.State.Properties.AddendumDocument.IsRequired = false;
      _obj.State.Properties.ExternalApprovalState.IsVisible = true;
      _obj.State.Properties.ExternalApprovalState.IsEnabled = true;
    }
  }
}