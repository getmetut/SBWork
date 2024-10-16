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
        baseRoles.Add(sberdev.SberContracts.BudgetOwnerRole.Type.Signatory);
      }
      return baseRoles;
    }
    
    public override void SetPropertiesVisibility()
    {
      
      base.SetPropertiesVisibility();
      
      var type = _obj.StageType;
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
      
      properties.SidSberDev.IsEnabled = false;
      bool isSign = type == StageType.Sign;
      bool isConfirmSignSberDev = _obj.ConfirmSignSberDev.HasValue && _obj.ConfirmSignSberDev.Value;
      if (isSign)
      {
        properties.AssigneeSberDev.IsVisible = isConfirmSignSberDev;
        properties.IsConfirmSigning.IsVisible = isConfirmSignSberDev ? false : properties.IsConfirmSigning.IsVisible;
        properties.AssigneeType.IsVisible = isConfirmSignSberDev ? false : properties.AssigneeType.IsVisible;
        properties.ApprovalRole.IsVisible = isConfirmSignSberDev ? false : properties.ApprovalRole.IsVisible;
      }
      properties.ConfirmSignSberDev.IsVisible = isSign;
      
    }
    
    public override void SetPropertiesAvailability()
    {
      base.SetPropertiesAvailability();
      _obj.State.Properties.AssigneeSberDev.IsEnabled = _obj.ConfirmSignSberDev.HasValue && _obj.ConfirmSignSberDev.Value;
    }
    
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.AssigneeSberDev.IsRequired = _obj.ConfirmSignSberDev.HasValue && _obj.ConfirmSignSberDev.Value;
    }
  }
}