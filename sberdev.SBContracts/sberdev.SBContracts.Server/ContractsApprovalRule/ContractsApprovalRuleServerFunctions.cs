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
                                                  sbRule.ExpendProfitSberDev == _obj.ExpendProfitSberDev;
                                              }).ToList();
      return conflictedRules;
    }
  }
}