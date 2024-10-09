using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SBContracts.ExchangeDocumentProcessingTask;

namespace sberdev.SBContracts.Server
{
  partial class ExchangeDocumentProcessingTaskRouteHandlers
  {

    public override void StartBlock2(Sungero.Exchange.Server.ExchangeDocumentProcessingAssignmentArguments e)
    {
      base.StartBlock2(e);
   //   if (_obj.IsIncoming != true)
    //    e.Block.Performers.Clear();
    }

  }
}