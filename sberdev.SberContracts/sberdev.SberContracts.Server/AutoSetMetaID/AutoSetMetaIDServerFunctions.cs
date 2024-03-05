using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AutoSetMetaID;

namespace sberdev.SberContracts.Server
{
  partial class AutoSetMetaIDFunctions
  {
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      try
      {
      SBContracts.PublicFunctions.Module.Remote.SetMetadataID(approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault());
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Неуспешное заверщение записи ИД карточки в метаданные", approvalTask);
      }
      return base.Execute(approvalTask);
    }
  }
}