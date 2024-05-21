using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CalculationTamplate;

namespace sberdev.SberContracts
{
  partial class CalculationTamplateClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.CalculationTamplate.TableAccess(_obj);
    }
  }


}