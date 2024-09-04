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
      var doc = approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      if (doc != null)
      {
        try
        {
          SBContracts.PublicFunctions.Module.Remote.SetMetadataID(doc);
          return this.GetSuccessResult();
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Этап сценария. Неуспешное завершение записи ИД карточки (" + doc.Id.ToString() + ") в метаданные. Причина: " +  ex.ToString(), approvalTask);
          return this.GetSuccessResult();
        }
      }
      else
        return this.GetErrorResult("Не найден документ.");
    }
  }
}