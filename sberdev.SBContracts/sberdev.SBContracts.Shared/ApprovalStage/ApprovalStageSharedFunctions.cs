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
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerUnit);
      }
      
      return baseRoles;
    }
    
    public override void SetPropertiesVisibility()
    {
      base.SetPropertiesVisibility();
      
      var properties = _obj.State.Properties;
      var isUnits = false;
      foreach (var role in _obj.ApprovalRoles)
      {
        if (role.ApprovalRole.Type == sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerUnit)
          isUnits = true;
      }
      var isUnit = _obj.ApprovalRole != null ? _obj.ApprovalRole.Type == sberdev.SberContracts.BudgetOwnerRole.Type.BudgetOwnerUnit : false;
      if (isUnit || isUnits)
      {
        properties.ProductUnitSberDev.IsVisible = true;
        properties.ProductUnitSberDev.IsRequired = true;
      }
      else
      {
        properties.ProductUnitSberDev.IsVisible = false;
        properties.ProductUnitSberDev.IsRequired = false;
      }
    }
  }
}