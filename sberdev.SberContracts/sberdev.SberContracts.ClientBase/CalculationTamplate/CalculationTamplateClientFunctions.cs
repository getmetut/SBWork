using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CalculationTamplate;

namespace sberdev.SberContracts.Client
{
  partial class CalculationTamplateFunctions
  {

    /// <summary>
    /// Очистить таблицу
    /// </summary>       
    public void ClearTable()
    {
      _obj.ProdCollection.Clear();
    }

  }
}