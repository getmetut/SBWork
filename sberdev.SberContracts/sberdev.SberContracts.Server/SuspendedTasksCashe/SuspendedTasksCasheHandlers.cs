using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.SuspendedTasksCashe;

namespace sberdev.SberContracts
{
  partial class SuspendedTasksCasheServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.TaskId == null)
        _obj.TaskId = _obj.Task?.Id.ToString();
    }
  }

}