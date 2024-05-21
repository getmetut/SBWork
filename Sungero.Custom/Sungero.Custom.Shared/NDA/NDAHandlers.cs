using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDA;

namespace Sungero.Custom
{
  partial class NDASharedHandlers
  {

    public virtual void KAFormChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue.Value)
      {
        _obj.NFP = false;
        _obj.Typing = false;
        PublicFunctions.NDA.UpdateForm(_obj);
      }
    }

    public virtual void NFPChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue.Value)
      {
        _obj.KAForm = false;
        _obj.Typing = false;
        PublicFunctions.NDA.UpdateForm(_obj);
      }
    }

    public virtual void TypingChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue.Value)
      {
        _obj.NFP = false;
        _obj.KAForm = false;
        PublicFunctions.NDA.UpdateForm(_obj);
      }
    }

  }
}