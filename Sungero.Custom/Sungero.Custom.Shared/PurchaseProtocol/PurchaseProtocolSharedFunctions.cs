using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PurchaseProtocol;

namespace Sungero.Custom.Shared
{
  partial class PurchaseProtocolFunctions
  {

    /// <summary>
    /// Обновление формы карточки в зависимости от выбора пользователя
    /// </summary>  
    [Public]
    public void UpdateCard()
    {
      _obj.State.Properties.DocumentFooting.IsRequired = false;
      if (_obj.TotalSumm.HasValue)
      {
        _obj.State.Properties.DocumentFooting.IsRequired = ((_obj.TotalSumm > 500000) || (_obj.RouteType == Custom.PurchaseProtocol.RouteType.PurchaseFramewo));
      }
    }

  }
}