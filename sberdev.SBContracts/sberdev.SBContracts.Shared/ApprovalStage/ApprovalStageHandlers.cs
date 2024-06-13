using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalStage;

namespace sberdev.SBContracts
{
  partial class ApprovalStageSharedHandlers
  {

    public override void ApprovalRoleChanged(Sungero.Docflow.Shared.ApprovalStageApprovalRoleChangedEventArgs e)
    {
      base.ApprovalRoleChanged(e);
      var isUnit = _obj.ApprovalRole != null ? _obj.ApprovalRole.Type == sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerUnit : false;
      if (_obj.ProductUnitSberDev != null && !isUnit)
        _obj.ProductUnitSberDev = null;
    }

    public override void ApprovalRolesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.ApprovalRolesChanged(e);
      bool isUnits = false;
      foreach (var role in _obj.ApprovalRoles)
      {
        if (role.ApprovalRole != null && role.ApprovalRole.Type == sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerUnit)
          isUnits = true;
      }
      if (_obj.ProductUnitSberDev != null && !isUnits)
        _obj.ProductUnitSberDev = null;
    }

  }
}