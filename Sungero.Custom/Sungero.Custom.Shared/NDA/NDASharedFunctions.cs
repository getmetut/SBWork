using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDA;

namespace Sungero.Custom.Shared
{
  partial class NDAFunctions
  {
    [Public]
    public void UpdateForm()
    {
      if (_obj.KAForm != null)
      {
        if (_obj.KAForm.Value)
        {
          _obj.State.Properties.Justification.IsVisible = true;
          _obj.State.Properties.Justification.IsRequired = true;
        }
        else
        {
          _obj.State.Properties.Justification.IsVisible = false;
          _obj.State.Properties.Justification.IsRequired = false;
        }
        
        if (_obj.NFP.Value)
        {
          _obj.KAForm = false;
          _obj.Typing = false;
        }
        
        if (_obj.Typing != null)
        {
          if (_obj.Typing.Value)
          {
            _obj.NFP = false;
            _obj.KAForm = false;
          } 
        }
      }
      else
      {
        _obj.State.Properties.Justification.IsVisible = false;
        _obj.State.Properties.Justification.IsRequired = false;
      }
      
    }
  }
}