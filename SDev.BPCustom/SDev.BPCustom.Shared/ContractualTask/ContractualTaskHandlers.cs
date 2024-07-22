using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.ContractualTask;

namespace SDev.BPCustom
{
  partial class ContractualTaskSharedHandlers
  {

    public virtual void BaseAttachmentsAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      if (e.Attachment != null)
      {
        if (sberdev.SBContracts.Contracts.Is(e.Attachment))
          _obj.Signer = sberdev.SBContracts.Contracts.As(e.Attachment).OurSignatory;
        
        if (sberdev.SBContracts.SupAgreements.Is(e.Attachment))
          _obj.Signer = sberdev.SBContracts.SupAgreements.As(e.Attachment).OurSignatory;
      }
    }

  }
}