using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Company;

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
    public static sberdev.SBContracts.IApprovalTask CreateApprovalTask(sberdev.SBContracts.IOfficialDocument document, Sungero.Workflow.ITask basetask)
    {
      var task = sberdev.SBContracts.ApprovalTasks.CreateAsSubtask(basetask);
      task.DocumentGroup.All.Add(document);
      
      return task;
    }
  }
}