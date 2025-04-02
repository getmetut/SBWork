using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.PowerOfAttorney;

namespace Sungero.ATS
{
  partial class PowerOfAttorneyServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (_obj.CategorySDev == null)
        e.AddError("Заполнение категории доверенности - обязательно!");
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FirstOrDoubleSDev = false;
      _obj.LifeCycleState = PowerOfAttorney.LifeCycleState.Draft;
    }
  }

}