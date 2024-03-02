using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AnaliticsCashe;

namespace sberdev.SberContracts
{
  partial class AnaliticsCasheServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      _obj.Modified = Calendar.UserNow;
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Name = Guid.NewGuid().ToString();
      _obj.User = Users.Current;
    }
  }
}