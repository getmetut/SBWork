using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.ProtocolPurchTask;

namespace SDev.BPCustom.Server.ProtocolPurchTaskBlocks
{
  partial class ScriptBlockHandlers
  {

    public virtual void ScriptBlockExecute()
    {
      var doc = _obj.BaseAttachment.PurchaseProtocols.FirstOrDefault();
      if (doc != null)
      {
        var ka = doc.Counterparty;
        if (ka != null)
        {
          var nka = sberdev.SBContracts.Counterparties.As(ka);
          nka.FocusCheckingSberDev = true;
          nka.Save();
        }
      }
      
    }
  }

}