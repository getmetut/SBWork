using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.TaskToPerformer;

namespace SDev.BPCustom.Server.TaskToPerformerBlocks
{
  partial class LifeCicleDocHandlers
  {

    public virtual void LifeCicleDocExecute()
    {
      var doc = _obj.BaseAttachments.ElectronicDocuments.FirstOrDefault();
      if (doc != null)
      {
        if (sberdev.SberContracts.AppNonProdPurchases.Is(doc))
        {
          var nonpurch = sberdev.SberContracts.AppNonProdPurchases.As(doc);
          if (_block.InternalApprovalState != null)
          {
            if (_block.InternalApprovalState == InternalApprovalState.Aborted)
              nonpurch.InternalApprovalState = sberdev.SberContracts.AppNonProdPurchase.InternalApprovalState.Aborted;
            if (_block.InternalApprovalState == InternalApprovalState.OnApproval)
              nonpurch.InternalApprovalState = sberdev.SberContracts.AppNonProdPurchase.InternalApprovalState.OnApproval;
            if (_block.InternalApprovalState == InternalApprovalState.OnRework)
              nonpurch.InternalApprovalState = sberdev.SberContracts.AppNonProdPurchase.InternalApprovalState.OnRework;
            if (_block.InternalApprovalState == InternalApprovalState.PendingSign)
              nonpurch.InternalApprovalState = sberdev.SberContracts.AppNonProdPurchase.InternalApprovalState.PendingSign;
            if (_block.InternalApprovalState == InternalApprovalState.Signed)
              nonpurch.InternalApprovalState = sberdev.SberContracts.AppNonProdPurchase.InternalApprovalState.Signed;
          }
          if (_block.LifeCycleState != null)
          {
            if (_block.LifeCycleState == LifeCycleState.Active)
              nonpurch.LifeCycleState = sberdev.SberContracts.AppNonProdPurchase.LifeCycleState.Active;
            if (_block.LifeCycleState == LifeCycleState.Assigned)
              nonpurch.LifeCycleState = sberdev.SberContracts.AppNonProdPurchase.LifeCycleState.Assigned;
            if (_block.LifeCycleState == LifeCycleState.Draft)
              nonpurch.LifeCycleState = sberdev.SberContracts.AppNonProdPurchase.LifeCycleState.Draft;
            if (_block.LifeCycleState == LifeCycleState.Obsolete)
              nonpurch.LifeCycleState = sberdev.SberContracts.AppNonProdPurchase.LifeCycleState.Obsolete;
          }
          if (_block.ExecutionState != null)
          {
            if (_block.ExecutionState == ExecutionState.Aborted)
              nonpurch.ExecutionState = sberdev.SberContracts.AppNonProdPurchase.ExecutionState.Aborted;
            if (_block.ExecutionState == ExecutionState.Executed)
              nonpurch.ExecutionState = sberdev.SberContracts.AppNonProdPurchase.ExecutionState.Executed;
            if (_block.ExecutionState == ExecutionState.OnExecution)
              nonpurch.ExecutionState = sberdev.SberContracts.AppNonProdPurchase.ExecutionState.OnExecution;
            if (_block.ExecutionState == ExecutionState.OnReview)
              nonpurch.ExecutionState = sberdev.SberContracts.AppNonProdPurchase.ExecutionState.OnReview;
            if (_block.ExecutionState == ExecutionState.Sending)
              nonpurch.ExecutionState = sberdev.SberContracts.AppNonProdPurchase.ExecutionState.Sending;
            if (_block.ExecutionState == ExecutionState.WithoutExecut)
              nonpurch.ExecutionState = sberdev.SberContracts.AppNonProdPurchase.ExecutionState.WithoutExecut;
          }
          if (nonpurch.State.IsChanged)
            nonpurch.Save();
        }
      }
    }
  }

}