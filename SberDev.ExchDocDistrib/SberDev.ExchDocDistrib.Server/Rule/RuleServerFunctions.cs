using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SberDev.ExchDocDistrib.Rule;

namespace SberDev.ExchDocDistrib.Server
{
  partial class RuleFunctions
  {
    /// <summary>
    /// Отдает сортированный список правил автораспределения.
    /// Сначала идут правила с высоким коэффициентом успешности.
    /// </summary>
    /// <param name="cp">Контрагент.</param>
    /// <returns>Список правил.</returns>
    [Public, Remote]
    public static System.Collections.Generic.List<IRule> GetRules(Sungero.CoreEntities.IUser author, Sungero.Parties.ICounterparty cp)
    {
      if (cp == null)
        return new System.Collections.Generic.List<IRule>();

      var query = Rules.GetAll().Where(r => r.Author == author && r.Counterparty == cp);

      // Безопасная сортировка без деления на ноль
      var ordered = query
        .OrderByDescending(r => r.SuccessRate)
        .ToList();

      return ordered;
    }

    
    /// <summary>
    /// Функция создает или обновляет правило автоматической переадресации
    /// </summary>
    /// <param name="assign"></param>
    /// <param name="result"></param>
    [Public, Remote]
    public static void CreateOrUpdate(sberdev.SBContracts.IExchangeDocumentProcessingAssignment assign, bool result)
    {
      var task = sberdev.SBContracts.ExchangeDocumentProcessingTasks.As(assign.Task);
      var respEmpList = Functions.Module.GetBoxResponsibleEmployees(assign.Box);
      var assignPerformer = Sungero.Company.Employees.As(assign.Performer);
      
      if ((!respEmpList.Contains(assignPerformer) || respEmpList.Contains(assign.Addressee)) && result)
        return;
      
      var rulePerformer = result ? assign.Addressee : assignPerformer;
      var ruleAuthor = Sungero.CoreEntities.Users.As(task.HasAccAttachSberDev == true ? respEmpList[1] : respEmpList[0]);
      var rule = Rules.GetAll().Where(r => r.Author == ruleAuthor
                                      && r.Counterparty == assign.Counterparty
                                      && r.Responsible == rulePerformer).FirstOrDefault();
      
      if (rule == null && result)
      {
        rule = Rules.Create();
        rule.Author = assign.Performer;
        rule.Counterparty = assign.Counterparty;
        rule.Responsible = assign.Addressee;
      }
      
      if (rule == null && !result)
      {
        Logger.ErrorFormat("Перераспределение по несуществующему правилу. ИД задания: {0}", assign.Id.ToString());
        return;
      }    
      
      if (result)
        rule.AttemptsSuccessful++;
      else
      {
        rule.AttemptsFailed++;
        if (rule.AttemptsSuccessful != 0)
          rule.AttemptsSuccessful--;
      }
      
      rule.Save();
      
      PublicFunctions.LogEntry.Create(rule, task, task.HasAccAttachSberDev == true, false, assign.ResultIsRedistributionSberDev == true);
    }
  }
}