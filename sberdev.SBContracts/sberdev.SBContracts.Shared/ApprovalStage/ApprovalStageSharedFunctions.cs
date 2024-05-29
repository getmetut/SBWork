using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalStage;

namespace sberdev.SBContracts.Shared
{
  partial class ApprovalStageFunctions
  {
    public override List<Enumeration?> GetPossibleRoles()
    {
      var baseRoles = base.GetPossibleRoles();
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers ||
          _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr ||
          _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Notice)
      {
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwner);
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerMVP);
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerMVZ);
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerMark);
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerProd);
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerPrGe);
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.AddApproversProd);
      }
      
      return baseRoles;
    }
  }
}