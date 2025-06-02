using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts.Shared
{
  partial class AppNonProdPurchaseFunctions
  {

    /// <summary>
    /// Обновление отображения полей карточки
    /// </summary>   
    public void UpdateCard()
    {
      base.UpdateCard();
      foreach (var elem in _obj.State.Properties)
      {
        elem.IsRequired = false;
      }
    }

  }
}