using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.MVZ;

namespace sberdev.SberContracts
{
  partial class MVZSharedHandlers
  {

    public virtual void MainMVZChanged(sberdev.SberContracts.Shared.MVZMainMVZChangedEventArgs e)
    {
          
      if (e.NewValue != null)
      {_obj.State.Properties.BudgetOwner.IsRequired = false;}
      else
      {_obj.State.Properties.BudgetOwner.IsRequired = true;}
    
    }

  }
}