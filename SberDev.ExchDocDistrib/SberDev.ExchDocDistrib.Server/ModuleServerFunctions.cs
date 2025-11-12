using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace SberDev.ExchDocDistrib.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Отдает всех ответственных за все ящики .
    /// </summary>
    /// <param name="task"></param>
    [Public]
    public System.Collections.Generic.List<Sungero.CoreEntities.IRecipient> GetAllBoxesResponsibleEmployees()
    {
      List<IRecipient> list = new List<IRecipient>();
      
      var accRespList = sberdev.SBContracts.BusinessUnits.GetAll()
        .Where(b => b.AccDocsHandlerSberDev != null)
        .Select(b => Sungero.CoreEntities.Recipients.As(b.AccDocsHandlerSberDev))
        .ToList();
      list.AddRange(accRespList);
      
      var boxesRespList = Sungero.ExchangeCore.BoxBases.GetAll()
        .Where(b => b.Responsible != null)
        .Select(b => Sungero.CoreEntities.Recipients.As(b.Responsible))
        .ToList();
      list.AddRange(boxesRespList);
      
      return list.Distinct().ToList();
    }
    
    /// <summary>
    /// Распределить документы по самому успешному правилу.
    /// </summary>
    /// <param name="task"></param>
    [Public]
    public void DistributeDocuments(sberdev.SBContracts.IExchangeDocumentProcessingTask task)
    {
      if (task.IsAutoDistributedSberDev != null || task.Assignee == null || task.Counterparty == null || task.HasAccAttachSberDev == null)
        return;
      
      var rule = Functions.Rule.GetRules(Sungero.CoreEntities.Users.As(task.Assignee), task.Counterparty).FirstOrDefault();
      if (rule != null && rule.SuccessRate >= 85)
      {
        task.Assignee = rule.Responsible;
        task.IsAutoDistributedSberDev = true;
        rule.AttemptsSuccessful++;
        rule.Save();
        PublicFunctions.LogEntry.Create(rule, task, task.HasAccAttachSberDev.Value, true, false);
      }
      else
        task.IsAutoDistributedSberDev = false;
    }
    
  }
}