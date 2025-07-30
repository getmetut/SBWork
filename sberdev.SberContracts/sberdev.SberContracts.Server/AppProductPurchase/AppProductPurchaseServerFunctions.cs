using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts.Server
{
  partial class AppProductPurchaseFunctions
  {
    public override void BeforeSaveFunction()
    {
      // Убран базовый обработчик
    }
  }
}