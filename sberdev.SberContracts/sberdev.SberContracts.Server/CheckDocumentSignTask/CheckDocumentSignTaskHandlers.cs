using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CheckDocumentSignTask;

namespace sberdev.SberContracts
{
  partial class CheckDocumentSignTaskServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.ReminderCount = 0;
    }
  }



}