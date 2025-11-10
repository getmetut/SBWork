using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SberDev.ExchDocDistrib.LogEntry;

namespace SberDev.ExchDocDistrib
{

  partial class LogEntryServerHandlers
  {  

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      Functions.LogEntry.FillName(_obj);
    }
  }

}