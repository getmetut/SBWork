using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Docflow.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// Отправляет уведомления по контролю возврата заданий. Создаёт подзадачи для инициаторов согласования и прекращает их при отсутствии ответа.
    /// </summary>
    public virtual void SendCheckReturnNotificationsSberDev()
    {
      var devSet = SberContracts.PublicFunctions.DevSettings.Remote.GetDevSetting("Настройка рассылки подзадач по контролю возврата");
      var exeptCPList = devSet.Counterparties.Select(c => c.Counterparty).ToList();
      var devSetArr = devSet.Text.Split(',');
      
      int periodAssign, periodTask, periodNotification;
      if (!int.TryParse(devSetArr[0], out periodAssign) || !int.TryParse(devSetArr[1], out periodTask) || !int.TryParse(devSetArr[2], out periodNotification))
        throw new ArgumentException("Укажите корректные значениея в текстовом параметре. Модуль Договоры -> Системные настройки -> Настройка рассылки подзадач по контролю возврата");
      
      var thresholdDateAssign = Calendar.AddWorkingDays(Calendar.Now, -periodAssign);
      var thresholdDateTask = Calendar.AddWorkingDays(Calendar.Now, -periodTask);
      var assigns = SBContracts.ApprovalCheckReturnAssignments.GetAll().Where(a => a.Status == SBContracts.ApprovalCheckReturnAssignment.Status.InProcess
                                                                              && a.Created.Value <= thresholdDateAssign);
      foreach (var assign in assigns)
      {
        var doc = assign.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var accounting = SBContracts.AccountingDocumentBases.As(doc);
        var contractual = SBContracts.ContractualDocuments.As(doc);
        SberContracts.ICheckDocumentSignTask subtask = null;
        
        if (exeptCPList.Contains(accounting?.Counterparty) || exeptCPList.Contains(contractual?.Counterparty))
          continue;
        
        if (assign?.Subtasks != null && assign.Subtasks.Any())
        {
          subtask = assign.Subtasks
            .Select(s => SberContracts.CheckDocumentSignTasks.As(s))
            .Where(s => s != null && s.Created.HasValue) // Фильтрация `null` и проверка `Created`
            .OrderByDescending(s => s.Created)
            .FirstOrDefault();
        }

        
        if (subtask == null)
        {
          Functions.Module.SendCheckDocumentSignTask(assign);
          continue;
        }

        if (subtask.Status != SberContracts.CheckDocumentSignTask.Status.InProcess)
        {
          var assignSubtask = Sungero.Workflow.Assignments
            .GetAll()
            .Where(a => a.Task == subtask)
            .OrderByDescending(a => a.Completed)
            .FirstOrDefault();
            if (assignSubtask != null && assignSubtask.Completed.HasValue
                && assignSubtask.Completed.Value <= thresholdDateAssign)
              Functions.Module.SendCheckDocumentSignTask(assign);
          continue;
        }
        
        if (subtask.Deadline.Value <= Calendar.Now)
        {
          subtask.Abort();
          assign.Save();
          continue;
        }

        int subtaskAgeDays = PublicFunctions.Module.CalculateBusinessDays(subtask.Created.Value, Calendar.Now);
        if (subtaskAgeDays / periodNotification != subtask.ReminderCount && subtask.Status == SberContracts.CheckDocumentSignTask.Status.InProcess)
        {
          Functions.Module.SendCheckDocumentSignNotification(subtask);
          SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(subtask);
          subtask.ReminderCount++;
          subtask.Save();
        }

      }
    }

  }
}