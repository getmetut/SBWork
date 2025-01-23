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
    
    public virtual void FinishOutcomingExchTasksSberDev()
    {
      Logger.Debug("Exchange. FinishOutcomingExchTasksSberDev. Фоновый процесс запущен. ====================================================");
      var tasks = SBContracts.ExchangeDocumentProcessingTasks.GetAll().Where(t => t.Status == ExchangeDocumentProcessingTask.Status.InProcess
                                                                             && t.IsNeedComeback == false && t.IsIncoming == false).ToList();
      foreach(var task in tasks)
      {
        Logger.Debug($"Exchange. FinishOutcomingExchTasksSberDev. Задача {task.Id} прекращена");
        task.Abort();
      }
      Logger.Debug("Exchange. FinishOutcomingExchTasksSberDev. Фоновый процесс завершен. ====================================================");
    }
    
    public virtual void ComeBackBodies()
    {
      Logger.Debug("Exchange. ComeBackBodies. Фоновый процесс запущен. ====================================================");
      var tasks = SBContracts.ExchangeDocumentProcessingTasks.GetAll().Where(t => t.IsNeedComeback == true).ToList();
      if (!tasks.Any())
      {
        Logger.Debug("Exchange. ComeBackBodies. Нет задач на обработку. ====================================================");
        return;
      }
      
      foreach (var task in tasks)
      {
        Logger.Debug($"Exchange. ComeBackBodies. Процесс по задаче {task.Id}");

        var attachs = task.Attachments?.Select(a => Sungero.Content.ElectronicDocuments.As(a)).Where(a => a != null).ToList();
        if (attachs == null || !attachs.Any())
        {
          Logger.Error($"Exchange. ComeBackBodies. Задача {task.Id} не содержит вложений.");
          task.IsNeedComeback = false;
          task.Save();
          continue;
        }
        
        /*  if (task.IsIncoming.HasValue && !task.IsIncoming.Value)
        {
          Logger.Error($"Exchange. ComeBackBodies. Задача {task.Id} для исходящих документов.");
          task.IsNeedComeback = false;
          task.Save();
          continue;
        }*/

        if (task.NumberOfAttempsComeback > 0)
        {
          var idsDocs = task.NeedComebackAgainAttachments?
                .Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s)) // Фильтруем пустые строки
                .Select(int.Parse)
                .ToList();

          attachs = attachs.Where(a => idsDocs.Contains((int)a.Id)).ToList();
        }
        
        task.NeedComebackAgainAttachments = null;

        foreach (var attach in attachs)
        {
          var incomingDoc = SBContracts.OfficialDocuments.As(attach);
          if (incomingDoc == null)
          {
            Logger.Error($"Exchange. ComeBackBodies. Вложение {attach.Id} не является официальным документом.");
            continue;
          }

          string idStr = incomingDoc.ParentDocIDSberDev;
          if (string.IsNullOrEmpty(idStr))
          {
            try
            {
              idStr = PublicFunctions.Module.Remote.GetMetadataID(incomingDoc);
            }
            catch (Exception ex)
            {
              Logger.Error($"Exchange. ComeBackBodies. Exchange. Ошибка извлечения метаданных у вложения {attach.Id}: {ex.Message}");
              continue;
            }
          }

          if (string.IsNullOrEmpty(idStr))
          {
            Logger.Debug($"Exchange. ComeBackBodies. Не найден ИД родной карточки для вложения {attach.Id}");
            continue;
          }
          
          PublicFunctions.Module.Remote.UnblockCardByDatabase(incomingDoc);
          incomingDoc.ParentDocIDSberDev = idStr;
          incomingDoc.Save();
          
          if (idStr == incomingDoc.Id.ToString())
          {
            Logger.Debug($"Exchange. ComeBackBodies. ИД родной карточки совпадает с ИД вложения {attach.Id}");
            continue;
          }

          var doc = Sungero.Content.ElectronicDocuments.GetAll(d => d.Id.ToString() == idStr).FirstOrDefault();
          if (doc == null)
          {
            Logger.Error($"Exchange. ComeBackBodies. Не существует родного документа с ИД {idStr} для документа {attach.Id}");
            continue;
          }

          var signInfos = Signatures.Get(incomingDoc);
          if (signInfos.Count() < 2)
          {
            Logger.Debug($"Exchange. ComeBackBodies. В документе {attach.Id} меньше двух подписей.");
            task.NumberOfAttempsComeback++;
            task.NeedComebackAgainAttachments += $",{incomingDoc.Id}";
            continue;
          }
          
          using (var strmCommon = incomingDoc.LastVersion.Body.Read())
            using (var strmPublic = incomingDoc.LastVersion.PublicBody.Read())
          {
            if (strmCommon.Length > 0)
            {
              SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(doc);
              doc.CreateVersionFrom(strmCommon, incomingDoc.BodyExtSberDev);
              if (strmPublic.Length > 0)
              {
                doc.LastVersion.PublicBody.Write(strmPublic);
                doc.LastVersion.AssociatedApplication = incomingDoc.LastVersion.AssociatedApplication;
              }
              doc.Save();

              foreach (var signInfo in signInfos)
              {
                try
                {
                  if (signInfo.IsExternal == true)
                  {
                    // Попробуем импортировать подпись
                    Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signInfo.GetDataSignature(), doc.LastVersion);
                    Logger.Debug($"Подпись успешно импортирована для документа {doc.Id}");
                  }
                }
                catch (InvalidOperationException ex)
                {
                  // Логируем исключение и продолжаем обработку
                  Logger.Error($"Ошибка при импорте подписи для документа {doc.Id}: {ex.Message}");
                }
                catch (Exception ex)
                {
                  // Логируем любые другие возможные исключения
                  Logger.Error($"Непредвиденная ошибка при обработке подписи для документа {doc.Id}: {ex.Message}");
                }
              }

              Logger.Debug($"Exchange. ComeBackBodies. Тело и подписи вложения {attach.Id} перенесены.");
              
              var clerk = Sungero.ExchangeCore.BusinessUnitBoxes.GetAll().FirstOrDefault(b => b.BusinessUnit == SBContracts.OfficialDocuments.As(doc).BusinessUnit)?.Responsible;
              var recipients = clerk != null ? new[] { doc.Author, clerk } : new[] { doc.Author };
              var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices($"Документ от {task.Counterparty?.Name} вернулся в исходную карточку", recipients);
              notice.ActiveText = "Документ был автоматически возвращен. Необходимо завершить задание на контроль возврата.";
              notice.Attachments.Add(doc);
              notice.Attachments.Add(task);
              notice.Deadline = Calendar.UserNow.AddDays(3);
              notice.Importance = Sungero.Workflow.SimpleTask.Importance.High;
              notice.Save();
              notice.Start();

              Logger.Debug($"Exchange. ComeBackBodies. Уведомление по документу {attach.Id} отправлено.");
            }
            else
            {
              Logger.Debug($"Exchange. ComeBackBodies. Тело документа {attach.Id} пусто, процесс переноса не выполнен.");
              task.NumberOfAttempsComeback++;
              task.NeedComebackAgainAttachments += $",{incomingDoc.Id}";
            }
          }
        }
        
        if (task.NeedComebackAgainAttachments == null)
          task.IsNeedComeback = false;

        if (task.NumberOfAttempsComeback > 259200)
        {
          task.IsNeedComeback = false;
          task.NeedComebackAgainAttachments = null;
          Logger.Debug($"Exchange. ComeBackBodies. Задача {task.Id} больше не обрабатывается, превышено количество попыток.");
        }

        task.Save();
      }

      Logger.Debug("Exchange. ComeBackBodies. Фоновый процесс завершен. ====================================================");
    }
  }
}
