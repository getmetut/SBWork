using System;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;
using System.Collections.Generic;

namespace sberdev.SBContracts.Module.Exchange.Server
{
  partial class ModuleJobs
  {
    public virtual void ComeBackBodies()
    {
      Logger.Debug("Exchange. ComeBackBodies. Фоновый процесс запущен");
      var tasks = SBContracts.ExchangeDocumentProcessingTasks.GetAll().Where(t => t.IsNeedComeback == true).ToList();
      if (!tasks.Any())
      {
        Logger.Debug("Exchange. ComeBackBodies. Нет задач на обработку");
        return;
      }

      foreach (var task in tasks)
      {
        Logger.Debug($"Exchange. ComeBackBodies. Процесс по задаче {task.Id}");

        var attachs = task.Attachments?.Select(a => Sungero.Content.ElectronicDocuments.As(a)).Where(a => a != null).ToList();
        if (attachs == null || !attachs.Any())
        {
          Logger.Error($"Задача {task.Id} не содержит вложений.");
          continue;
        }

        if (task.NumberOfAttempsComeback > 0)
        {
          var idsDocs = task.NeedComebackAgainAttachments?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
          attachs = attachs.Where(a => idsDocs.Contains((int)a.Id)).ToList();
        }

        foreach (var attach in attachs)
        {
          var incomingDoc = SBContracts.OfficialDocuments.As(attach);
          if (incomingDoc == null)
          {
            Logger.Error($"Вложение {attach.Id} не является документом.");
            continue;
          }

          string idStr;
          try
          {
            idStr = PublicFunctions.Module.Remote.GetMetadataID(incomingDoc);
          }
          catch (Exception ex)
          {
            Logger.Error($"Ошибка извлечения метаданных у документа {attach.Id}: {ex.Message}");
            continue;
          }

          if (string.IsNullOrEmpty(idStr))
          {
            Logger.Error($"Не найден ИД родной карточки для документа {attach.Id}");
            continue;
          }

          var doc = Sungero.Content.ElectronicDocuments.GetAll(d => d.Id.ToString() == idStr).FirstOrDefault();
          if (doc == null)
          {
            Logger.Error($"Не существует родного документа с ИД {idStr} для документа {attach.Id}");
            continue;
          }

          var signInfos = Signatures.Get(incomingDoc);
          var existExSign = task.IsIncoming ?? false;

          using (var strmCommon = incomingDoc.LastVersion.Body.Read())
          using (var strmPublic = incomingDoc.LastVersion.PublicBody.Read())
          {
            if (strmCommon.Length > 0)
            {
              doc.CreateVersionFrom(strmCommon, SBContracts.ExchangeDocuments.As(incomingDoc).BodyExtSberDev);
              if (strmPublic.Length > 0)
                doc.LastVersion.PublicBody.Write(strmPublic);
              doc.Save();

              foreach (var signInfo in signInfos)
              {
                if (signInfo.IsExternal == true)
                  Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signInfo.GetDataSignature(), doc.LastVersion);
              }

              Logger.Debug($"Тело и подписи документа {attach.Id} перенесены.");
            }
            else
            {
              Logger.Debug($"Тело документа {attach.Id} пусто, процесс переноса не выполнен.");
              task.NumberOfAttempsComeback++;
              task.NeedComebackAgainAttachments += $",{incomingDoc.Id}";
            }
          }

          var clerk = Sungero.ExchangeCore.BusinessUnitBoxes.GetAll().FirstOrDefault(b => b.BusinessUnit == SBContracts.OfficialDocuments.As(doc).BusinessUnit)?.Responsible;
          var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices($"Документ от {task.Counterparty?.Name} вернулся в исходную карточку", task.Author, clerk);
          notice.ActiveText = "Документ был автоматически возвращен. Необходимо завершить задание на контроль возврата.";
          notice.Attachments.Add(doc);
          notice.Attachments.Add(task);
          notice.Deadline = Calendar.UserNow.AddDays(3);
          notice.Importance = Sungero.Workflow.SimpleTask.Importance.High;
          notice.Save();
          notice.Start();

          Logger.Debug($"Уведомление по документу {attach.Id} отправлено.");
        }

        if (task.NumberOfAttempsComeback > 336)
        {
          task.IsNeedComeback = false;
          task.NeedComebackAgainAttachments = null;
          Logger.Error($"Задача {task.Id} больше не обрабатывается, превышено количество попыток.");
        }

        task.Save();
      }

      Logger.Debug("Фоновый процесс завершен.");
    }
  }
}
