using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDATask;

namespace Sungero.Custom
{
  partial class NDATaskSharedHandlers
  {

    public virtual void TravelDocChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      _obj.State.Properties.Email.IsVisible = false;
      _obj.State.Properties.Email.IsRequired = false;
      _obj.State.Properties.Adress.IsVisible = false;
      _obj.State.Properties.Adress.IsRequired = false;
      
      if (e.NewValue == NDATask.TravelDoc.Email)
      {
        _obj.State.Properties.Email.IsVisible = true;
        _obj.State.Properties.Email.IsRequired = true;
      }
      if (e.NewValue == NDATask.TravelDoc.Kurier)
      {
        _obj.State.Properties.Adress.IsVisible = true;
        _obj.State.Properties.Adress.IsRequired = true;
      }
    }

  }
}