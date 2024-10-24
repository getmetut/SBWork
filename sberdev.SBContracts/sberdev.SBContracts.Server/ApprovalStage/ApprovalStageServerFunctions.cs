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
      
      var role = _obj.ApprovalRoles.Where(x => x.ApprovalRole.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerMark)
        .Select(x => SberContracts.CustomAppovalRoles.As(x.ApprovalRole))
        .Where(x => x != null).SingleOrDefault();
      if (role!= null)
      {
        recipients.AddRange(SberContracts.PublicFunctions.CustomAppovalRole.GetRolePerformers(role, task));
      }
      
      role = _obj.ApprovalRoles.Where(x => x.ApprovalRole.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerPrGe)
        .Select(x => SberContracts.CustomAppovalRoles.As(x.ApprovalRole))
        .Where(x => x != null).SingleOrDefault();
      if (role!= null)
      {
        recipients.AddRange(SberContracts.PublicFunctions.CustomAppovalRole.GetRolePerformers(role, task));
      }
      
      return recipients;
    }
  }
}