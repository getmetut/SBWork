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
    /// 
    /// </summary>
    public virtual void SendCheckReturnNotificationsSberDev()
    {/*
      var devSet = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("Настройка рассылки подзадач по контролю возврата").Text.Split(',');
      int period1, period2;
      if (!int.TryParse(devSet[0], out period1) || !int.TryParse(devSet[1], out period2))
        throw new ArgumentException("Укажите корректные значениея в текстовом параметре. Модуль Договоры -> Системные настройки -> Настройка рассылки подзадач по контролю возврата");
      var thresholdDate = Calendar.AddWorkingDays(Calendar.Now, -period1);
      var assigns = SBContracts.ApprovalCheckReturnAssignments.GetAll().Where(a => a.Status == SBContracts.ApprovalCheckReturnAssignment.Status.InProcess
                                                                              && a.Created.Value <= thresholdDate
                                                                              && a.IsCheckReturnNotificationSendedSberDev != true);
      foreach (var assign in assigns)
      {
        if (assign.Subtasks.Any())
        {
          var subtask = assign.Subtasks.Last();
      //    SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(subtask);
          thresholdDate = Calendar.AddWorkingDays(Calendar.Now, -period2);
          if (subtask.Created.Value <= thresholdDate)
          {
            subtask.Abort();
            SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(assign);
            assign.IsCheckReturnNotificationSendedSberDev = true;
            assign.Save();
          }
        }
        else
        {
          var subtask = SberContracts.CheckCounterpartyDocumentReturns.CreateAsSubtask(assign);
          subtask.Author = assign.Performer;
          subtask.Subject = "Напомнить о подписании";
          subtask.ActiveText = "Просим напомнить к/а о подписании документа. В случае отсутствия подписанного с двух сторон документа," +
            " через 1 неделю подзадача переадресуется обратно Делопроизводителю";
          var route = subtask.RouteSteps.AddNew();
          route.Performer = assign.Task.Author;
          subtask.Start();
        }
      }*/
    }

  }
}