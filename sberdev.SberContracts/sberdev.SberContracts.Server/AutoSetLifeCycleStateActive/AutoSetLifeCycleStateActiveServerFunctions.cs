using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AutoSetLifeCycleStateActive;

namespace sberdev.SberContracts.Server
{
  partial class AutoSetLifeCycleStateActiveFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (document == null)
        return this.GetErrorResult("Не найден документ.");
      if (document.DocumentKind == null)
        return this.GetErrorResult("Не найден вид документа.");
      
      try
      {
        SBContracts.PublicFunctions.Module.Remote.UnblockEntityByDatabase(document);
        var invoice = SBContracts.IncomingInvoices.As(document);
        if (invoice == null)
          return this.GetErrorResult("Документ не является входящим счетом.");
        invoice.LifeCycleState = SBContracts.IncomingInvoice.LifeCycleState.Active;
        invoice.InternalApprovalState = SBContracts.IncomingInvoice.InternalApprovalState.Agreed;
        invoice.Save();
        return this.GetSuccessResult();
      }
      catch (Exception ex)
      {
        return this.GetRetryResult(string.Empty);
      }
    }

  }
}