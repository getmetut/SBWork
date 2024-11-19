using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MatrixPOA;

namespace Sungero.Custom
{
  partial class MatrixPOASharedHandlers
  {

    public virtual void TypePOAChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (e.NewValue == MatrixPOA.TypePOA.Paper)
          _obj.Mahineread = false;
        else
          _obj.Mahineread = true;
      }
      else
        _obj.Mahineread = null;
      
      PublicFunctions.MatrixPOA.UpdateCard(_obj);
    }

    public virtual void MahinereadChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.MatrixPOA.UpdateCard(_obj);
    }

  }
}