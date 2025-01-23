using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractsApprovalRule;

namespace sberdev.SBContracts.Server
{
  partial class ContractsApprovalRuleFunctions
  {
    // Переопределяем базовую проверку
    public override bool CheckRoutePossibility(
      List<Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep> route,
      List<Sungero.Docflow.Structures.ApprovalRuleBase.ConditionRouteStep> ruleConditions,
      Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep conditionStep)
    {
      // Базовая логика (родительская)
      var possibleStage = base.CheckRoutePossibility(route, ruleConditions, conditionStep);

      // Находим текущее условие и его тип
      var conditionItem = _obj.Conditions.FirstOrDefault(c => c.Number == conditionStep.StepNumber);
      if (conditionItem == null)
        return possibleStage; // Нет условия - ничего не делаем

      var conditionType = conditionItem.Condition.ConditionType;

      // Собираем все шаги маршрута с тем же самым типом условия (кроме текущего шага)
      var sameTypeConditions = route
        .Where(r => r.StepNumber != conditionStep.StepNumber
               && _obj.Conditions.Any(c => c.Number == r.StepNumber &&
                                      c.Condition.ConditionType == conditionType))
        .ToList();

      // Если проверка конфликтов внутри этого типа не прошла, ветка невалидна
      if (!this.CheckConditionConflict(sameTypeConditions, conditionStep))
        return false;

      return possibleStage;
    }

    // Проверяем конфликт условий одного типа
    private bool CheckConditionConflict(
      List<Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep> allConditions,
      Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep currentConditionStep)
    {
      // Текущее условие
      var currentConditionItem = _obj.Conditions.FirstOrDefault(x => x.Number == currentConditionStep.StepNumber);
      var currentContractCondition = SBContracts.ContractConditions.As(currentConditionItem.Condition);

      // Получаем «сравниваемое значение» (PurchaseKind и т. д.) для текущего условия
      var currentValue = this.GetComparableValue(currentContractCondition);

      // Идём по всем предыдущим условиям того же типа
      foreach (var previousStep in allConditions.TakeWhile(x => !Equals(x, currentConditionStep)))
      {
        var prevItem = _obj.Conditions.FirstOrDefault(x => x.Number == previousStep.StepNumber);
        var prevContractCondition = SBContracts.ContractConditions.As(prevItem.Condition);

        // Аналогично получаем сравниваемое значение
        var prevValue = this.GetComparableValue(prevContractCondition);

        // Если значения совпадают, но ветки разные, — конфликт
        if (currentValue != null
            && prevValue != null
            && currentValue.Equals(prevValue)
            && previousStep.Branch != currentConditionStep.Branch)
        {
          return false;
        }
      }
      return true;
    }

    // Универсальный метод, чтобы получить «сравниваемое» значение
    private object GetComparableValue(SBContracts.IContractCondition contractCondition)
    {
      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.ProductUnit)
        return contractCondition.ProductUnitSberDev;

      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.MarketDirect)
        return contractCondition.MarketDirectSberDev;

      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.InitiatorsDepartment)
        return contractCondition.InitiatorsDepartment;

      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.AccountAticles)
        return contractCondition.AccountingArticles;

      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.MVZ)
        return contractCondition.MVZ;

      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.MVP)
        return contractCondition.MVP;

      if (contractCondition.ConditionType == SBContracts.ContractCondition.ConditionType.ContrCategory)
        return contractCondition.ContrCategorysberdev;

      // Если тип нас не интересует, возвращаем null
      return null;
    }


    
    public override List<Sungero.Docflow.IApprovalRuleBase> GetDoubleRules()
    {
      // Получаем базовые дублирующие правила.
      var conflictedRules = base.GetDoubleRules();

      // Фильтрация по новым свойствам.
      conflictedRules = conflictedRules.Where(rule =>
                                              {
                                                var sbRule = SBContracts.ContractsApprovalRules.As(rule);
                                                if (sbRule == null)
                                                  return false;

                                                // Проверяем соответствие новых булевых свойств
                                                return sbRule.ExpendableSberDev == _obj.ExpendableSberDev &&
                                                  sbRule.ProfitableSberDev == _obj.ProfitableSberDev &&
                                                  sbRule.ExpendProfitSberDev == _obj.ExpendProfitSberDev &&
                                                  sbRule.TypicalSberDev == _obj.TypicalSberDev ;
                                              }).ToList();
      return conflictedRules;
    }
    
    public override Sungero.Docflow.IApprovalRuleBase GetOrCreateNextVersion()
    {
      var version = SBContracts.ContractsApprovalRules.As(base.GetOrCreateNextVersion());
      version.TypicalSberDev = _obj.TypicalSberDev;
      version.ExpendableSberDev = _obj.ExpendableSberDev;
      version.ExpendProfitSberDev = _obj.ExpendProfitSberDev;
      version.ProfitableSberDev = _obj.ProfitableSberDev;
      return version;
    }
  }
}