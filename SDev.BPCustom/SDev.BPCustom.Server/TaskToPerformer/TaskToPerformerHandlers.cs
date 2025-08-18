using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.TaskToPerformer;

namespace SDev.BPCustom
{
  partial class TaskToPerformerServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      var basedoc = _obj.BaseAttachments.ElectronicDocuments.FirstOrDefault();
      if (sberdev.SberContracts.AppNonProdPurchases.Is(sberdoc))
      {
        var AppNonProdPurchas = sberdev.SberContracts.AppNonProdPurchases.As(sberdoc);
        AppNonProdPurchas.TochInWork = _obj.Performer;
        AppNonProdPurchas.LifeCycleState = sberdev.SberContracts.AppNonProdPurchase.LifeCycleState.Assigned;
        AppNonProdPurchas.Save();
      }
    }
  }

}