using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalStage;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalStageFunctions
  {
    public override List<IRecipient> GetStageRecipients(Sungero.Docflow.IApprovalTask task, List<IRecipient> additionalApprovers)
    {
      List<IRecipient> recipients = base.GetStageRecipients(task, additionalApprovers);
      
      var role = _obj.ApprovalRoles.Where(x => x.ApprovalRole.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwnerMark)
        .Select(x => SberContracts.BudgetOwnerRoles.As(x.ApprovalRole))
        .Where(x => x != null).SingleOrDefault();
      if (role!= null)
      {
        recipients.AddRange(SberContracts.PublicFunctions.BudgetOwnerRole.GetRolePerformers(role, task));
      }
      
      role = _obj.ApprovalRoles.Where(x => x.ApprovalRole.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwnerPrGe)
        .Select(x => SberContracts.BudgetOwnerRoles.As(x.ApprovalRole))
        .Where(x => x != null).SingleOrDefault();
      if (role!= null)
      {
        recipients.AddRange(SberContracts.PublicFunctions.BudgetOwnerRole.GetRolePerformers(role, task));
      }
      
      return recipients;
    }
  }
}