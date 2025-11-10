using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingTask;
using Sungero.Docflow;

namespace sberdev.SBContracts
{
  partial class ExchangeDocumentProcessingTaskServerHandlers
  {

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      SberDev.ExchDocDistrib.PublicFunctions.Module.DistributeFormalizedAccDocuments(_obj);
      SberDev.ExchDocDistrib.PublicFunctions.Module.DistributeDocuments(_obj);
      base.Saving(e);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsNeedComeback = true;
      _obj.NeedComebackAgainAttachments = "";
      _obj.NumberOfAttempsComeback = 0;
      _obj.OnRedistributionSberDev = false;
      base.Created(e);
    }
  }
}