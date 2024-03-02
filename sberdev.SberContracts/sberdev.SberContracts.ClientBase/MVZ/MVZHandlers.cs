using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.MVZ;

namespace sberdev.SberContracts
{
  partial class MVZClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
            if (_obj.MainMVZ != null)
      {_obj.State.Properties.BudgetOwner.IsRequired = false;}
      else
      {_obj.State.Properties.BudgetOwner.IsRequired = true;}
    }
  }

}