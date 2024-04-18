using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.PurchaseTask;

namespace SDev.BPCustom.Server.PurchaseTaskBlocks
{
  partial class StantartJobHandlers
  {

    public virtual void StantartJobStartAssignment(SDev.BPCustom.IStantartJob assignment)
    {
      assignment.NOR = _obj.BaseAttachments.Purchases.FirstOrDefault().BusinessUnit;
      assignment.Counterparty = _obj.BaseAttachments.Purchases.FirstOrDefault().Counterparty;
    }
  }

}