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
        name += _obj.TypePOA.ToString();
      
      if (_obj.Mahineread.HasValue)
        name += _obj.Mahineread.Value ? " (МЧД)" : "";
      
      if (_obj.Name != name)
        _obj.Name = name;
    }

  }
}