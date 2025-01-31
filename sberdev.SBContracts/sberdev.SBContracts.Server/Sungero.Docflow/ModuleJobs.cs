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
      var devSet = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("Настройка рассылки подзадач по контролю возврата");
      var exeptCPList = devSet.Counterparties.Select(c => c.Counterparty).ToList();
      var devSetArr = devSet.Text.Split(',');
      
      int periodAssign, periodTask, periodNotification;
      if (!int.TryParse(devSetArr[0], out periodAssign) || !int.TryParse(devSetArr[1], out periodTask) || !int.TryParse(devSetArr[1], out periodNotification))
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
        
        if (exeptCPList.Contains(accounting?.Counterparty) || exeptCPList.Contains(contractual?.Counterparty))
          continue;
        
        SberContracts.ICheckDocumentSignTask subtask = assign.Subtasks.Select(s => SberContracts.CheckDocumentSignTasks.As(s)).LastOrDefault();
        
        if (subtask == null)
        {
          Functions.Module.SendCheckDocumentSignTask(assign);
          continue;
        }

        if (subtask.Status != SberContracts.CheckDocumentSignTask.Status.InProcess)
        {
          if (subtask.Deadline.Value <= thresholdDateAssign)
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