using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSimpleAssignment;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalSimpleAssignmentFunctions
  {

    /// <summary>
    /// Механика этап согласования Проверить постановку договора на валютный банковский контроль
    /// </summary>       
    [Public, Remote]
    public bool CheckUCNStageApprScript(SBContracts.IIncomingInvoice incInv, string ucn)
    {
        if (ucn != null)
        {
          PublicFunctions.Module.Remote.UnblockCardByDatabase(incInv);
          incInv.UCNSberDev = ucn;
          incInv.Save();
          return true;
        }
        else
          return false;
    }

    /// <summary>
    /// Механика этапа согласования Проверка договорных документов Делопроизводителем
    /// </summary>
    [Public, Remote]
    public void ContractCheckByClerkStageApprScript(SBContracts.IIncomingInvoice incInv)
    {
      var contractual = incInv.LeadingDocument;
      if (contractual != null)
      {
        contractual.InternalApprovalState = SBContracts.OfficialDocument.InternalApprovalState.Signed;
        contractual.ExternalApprovalState = SBContracts.OfficialDocument.ExternalApprovalState.Signed;
        contractual.Save();
      }
      var accounting = incInv.AccDocSberDev;
      if (accounting != null)
      {
        accounting.InternalApprovalState = SBContracts.OfficialDocument.InternalApprovalState.Signed;
        accounting.ExternalApprovalState = SBContracts.OfficialDocument.ExternalApprovalState.Signed;
        accounting.Save();
      }
    }

  }
}