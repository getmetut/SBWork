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

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsNeedComeback = true;
      _obj.NeedComebackAgainAttachments = "";
      _obj.NumberOfAttempsComeback = 0;
      _obj.ReadressedSberDev = false;
      base.Created(e);
    }
  }


}