using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SberDev.ExchDocDistrib.LogEntry;

namespace SberDev.ExchDocDistrib.Server
{
  partial class LogEntryFunctions
  {
    [Public]
    public static void Create(SberDev.ExchDocDistrib.IRule rule,
                               sberdev.SBContracts.IExchangeDocumentProcessingTask task,
                               bool isAccounting, bool isAuto, bool isRedistribution)
    {
      var entry = LogEntries.Create();
      entry.Task = task;
      entry.Date = Calendar.Now;
      entry.Counterparty = rule.Counterparty;
      entry.DistributedTo = rule.Responsible;
      entry.IsAccounting = isAccounting;
      entry.IsAuto = isAuto;
      entry.IsRedistribution = isRedistribution;
      entry.Save();
    }
  }
}