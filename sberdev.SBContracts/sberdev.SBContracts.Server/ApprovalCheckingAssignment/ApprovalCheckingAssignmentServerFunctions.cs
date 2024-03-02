using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckingAssignment;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalCheckingAssignmentFunctions
  {
    /// <summary>
    /// Возвращает согласуемый документ
    /// </summary>
    /// <returns></returns>
    [Remote]
    public SBContracts.IOfficialDocument GetAttachment()
    {
      return SBContracts.OfficialDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault()/*Task?.Attachments.FirstOrDefault()*/);
    }
  }
}