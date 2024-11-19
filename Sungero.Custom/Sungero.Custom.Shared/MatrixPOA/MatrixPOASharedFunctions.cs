using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MatrixPOA;

namespace Sungero.Custom.Shared
{
  partial class MatrixPOAFunctions
  {
    /// <summary>
    /// Обновление формы, формиование имени
    /// </summary>
    [Public]
    public void UpdateCard()
    {
      string name = "";
      if (_obj.TypePOA != null)
      {
        if (_obj.TypePOA == Custom.MatrixPOA.TypePOA.Paper)
          name += "На бумажном носителе (Письменная доверенность)";
        if (_obj.TypePOA == Custom.MatrixPOA.TypePOA.M4D)
          name += "Машиночитаемая доверенность (Электронная доверенность)";
        if (_obj.TypePOA == Custom.MatrixPOA.TypePOA.Mix)
          name += "МЧД/На бумажном носителе (Комбинированная)";
      }
      
      if (_obj.Mahineread.HasValue)
      {
        if (_obj.TypePOA == Custom.MatrixPOA.TypePOA.Mix)
          name += _obj.Mahineread.Value ? " (МЧД)" : "";
      }
      
      if (_obj.Name != name)
        _obj.Name = name;
    }

  }
}