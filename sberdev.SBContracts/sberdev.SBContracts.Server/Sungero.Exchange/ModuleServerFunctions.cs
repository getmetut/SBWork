using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ExchangeCore;
using Sungero.Parties;

namespace sberdev.SBContracts.Module.Exchange.Server
{
  partial class ModuleFunctions
  {
    protected override Sungero.Exchange.IExchangeDocumentProcessingTask CreateExchangeTask(NpoComputer.DCX.Common.IMessage message, List<Sungero.Exchange.IExchangeDocumentInfo> infos, Sungero.Parties.ICounterparty sender, bool isIncoming, Sungero.ExchangeCore.IBoxBase box, string exchangeTaskActiveTextBoundedDocuments)
    {
      var task = base.CreateExchangeTask(message, infos, sender, isIncoming, box, exchangeTaskActiveTextBoundedDocuments);
      task.Author = Sungero.ExchangeCore.PublicFunctions.BoxBase.Remote.GetExchangeDocumentResponsible(box, sender, infos);
      task.Save();
      return task;
    }
  }
}