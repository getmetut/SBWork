using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AutoSetFocusFlag;

namespace sberdev.SberContracts.Server
{
  partial class AutoSetFocusFlagFunctions
  {
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (document == null)
        return this.GetErrorResult("Не найден документ.");
      try
      {
        var contractual = SBContracts.ContractualDocuments.As(document);
        if (contractual == null)
          return this.GetErrorResult("Документ не является договорным.");
        var cp = SBContracts.Counterparties.As(contractual.Counterparty);
        if (cp == null)
          return this.GetErrorResult("В документе не указан контрагент.");
        SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(cp);
        cp.FocusCheckedSberDev = true;
        cp.FocusCheckedDateSberDev = Calendar.Now;
        cp.Save();
        return this.GetSuccessResult();
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в AutoSetFocusFlagFunction", ex);
        return this.GetRetryResult(string.Empty);
      }
    }
  }
}