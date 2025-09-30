using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Company;
using Sungero;

namespace SDev.BProc.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать подзадачи по процессу "Согласование официального документа".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="task">Задача базовая.</param>
    /// <returns>Подзадача по процессу "Согласование официального документа".</returns>
    [Remote(PackResultEntityEagerly = true), Public]
    public static sberdev.SBContracts.IApprovalTask CreateAsSubtaskOffDoc(sberdev.SBContracts.IOfficialDocument document, sberdev.SBContracts.IExchangeDocumentProcessingTask basetask)
    {
      var task = sberdev.SBContracts.ApprovalTasks.CreateAsSubtask(basetask.MainTask);
      task.DocumentGroup.All.Add(document);
      
      return task;
    }
  }
}