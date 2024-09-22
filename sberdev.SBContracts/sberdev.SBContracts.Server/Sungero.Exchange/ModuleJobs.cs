using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;

namespace sberdev.SBContracts.Module.Exchange.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// 
    /// </summary>
    public virtual void ComeBackBodies()
    {
      Logger.Debug("Exchange. ComeBackBodies. Фоновый процесс запущен");
      var tasks = SBContracts.ExchangeDocumentProcessingTasks.GetAll().Where(t => t.IsNeedComeback == true);
      if (tasks.Any())
      {
        int failedDocs = 0, complitedDocs = 0, dontNeedDocs = 0;
        string needComebackAgainAttachments = "";
        int numberOfTasks = tasks.Count();
        foreach (var task in tasks)
        {
          Logger.Debug("Exchange. ComeBackBodies. Начало процесса по задаче " + task.Id + "; Количество попыток выполнения процесса по этой задаче: " +
                       task.NumberOfAttempsComeback + "; Документы требующие повторного возврата: " + task.NeedComebackAgainAttachments);
          var attachs = task.Attachments.Select(a => Sungero.Content.ElectronicDocuments.As(a));
          
          
          if (task.NumberOfAttempsComeback > 0)
          {
            var idsDocs = task.NeedComebackAgainAttachments.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).Select(id => Int32.Parse(id));
            attachs = attachs.Where(a => idsDocs.Contains(Int32.Parse(a.Id.ToString()))).ToList();
          }
          
          
          foreach (var attach in attachs)
          {
            bool flag = true;
            var incomingDoc = SBContracts.OfficialDocuments.As(attach);
            string idStr = null;
            try
            {
              idStr = PublicFunctions.Module.Remote.GetMetadataID(incomingDoc);
            }
            catch (Exception ex)
            {
              Logger.Debug("Exchange. ComeBackBodies. Результат: Неуспешно. Произошла ошибка при извлечении метаданных (" + ex.ToString() + "). Карточка входящего документа: "
                           + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
              flag = false;
              dontNeedDocs++;
              continue;
            }
            if (idStr != null)
            {
              Logger.Debug("Exchange. ComeBackBodies. Копирование тела и подписей карточки." +
                           " Карточка входящего документа: " + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
              if (Sungero.Content.ElectronicDocuments.GetAll().Where(d => Equals(d.Id.ToString(), idStr)).Any())
              {
                var signInfos = Signatures.Get(incomingDoc);
                bool existExSign = task.IsIncoming.HasValue ? task.IsIncoming.Value : false;
                
                //Проверяем есть ли подпись от ка
            /*    foreach (var signInfo in signInfos)
                {
                  try
                  {
                    var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(signInfo.GetDataSignature());
                    string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyTIN(certificateInfo.SubjectInfo);
                    existExSign = task.Counterparty.TIN == tin;
                  }
                  catch (Exception ex)
                  {
                    Logger.ErrorFormat("Exchange. ComeBackBodies. Невозможно получить информацию о подписи. Текст ошибки: "
                                       + ex.Message);
                    continue;
                  }
                  
                  if (existExSign)
                    break;
                }*/
                
                //Начинаем перенос тел и подписей
                var doc = Sungero.Content.ElectronicDocuments.GetAll(d => d.Id == Int64.Parse(idStr)).First();
                if (existExSign)
                {
                  Stream strmCommon = incomingDoc.LastVersion.Body.Read();
                  Stream strmPublic = incomingDoc.LastVersion.PublicBody.Read();
                  
                  if (strmCommon.Length > 0)
                  {
                    doc.CreateVersionFrom(strmCommon, SBContracts.ExchangeDocuments.As(incomingDoc).BodyExtSberDev);
                    if (strmPublic.Length > 0)
                      doc.LastVersion.PublicBody.Write(strmPublic);
                    doc.Save();
                    
                    Logger.Debug("Exchange. ComeBackBodies. Тело скопировано."
                                 + " Карточка входящего документа: " + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
                    foreach(var signInfo in signInfos)
                    {
                      var signaturesBytes = signInfo.GetDataSignature();
                      if (signInfo.IsExternal.HasValue && signInfo.IsExternal.Value)
                        Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, doc.LastVersion);
                    }
                    Logger.Debug("Exchange. ComeBackBodies. Подписи перенесены." +
                                 " Карточка входящего документа: " + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
                  }
                  else
                  {
                    Logger.Debug("Exchange. ComeBackBodies. Тело не скопировано. Причина: Тело входящего документа пусто."
                                 + " Карточка входящего документа: " + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
                    failedDocs++;
                    flag = false;
                    task.NumberOfAttempsComeback++;
                    needComebackAgainAttachments += "," + incomingDoc.Id;
                  }
                  strmCommon.Close();
                  strmPublic.Close();
                }
                else
                {
                  Logger.Debug("Exchange. ComeBackBodies. Тело не скопировано. Причина: Нет подписи от контрагента."
                               + " Карточка входящего документа: " + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
                  failedDocs++;
                  task.NumberOfAttempsComeback++;
                  flag = false;
                  needComebackAgainAttachments += "," + incomingDoc.Id;
                }
              }
              else
              {
                Logger.Debug("Exchange. ComeBackBodies. Результат: Неуспешно. Причина: Не существует родного документа с ИД из метаданных тела.\n"
                             + " Карточка входящего документа: " + attach.Id + "; Карточка родного документа: " + idStr + "; Задача на обработку: " + task.Id);
                flag = false;
                dontNeedDocs++;
              }
            }
            else
            {
              Logger.Debug("Exchange. ComeBackBodies. Результат: Неуспешно. Причина: Не найден ИД родной карточки в метаданных документа"
                           + "; Карточка входящего документа: " + attach.Id + "; Задача на обработку: " + task.Id);
              flag = false;
              dontNeedDocs++;
            }
            if (flag)
            {
              Logger.Debug("Exchange. ComeBackBodies. Результат: Успешно. Карточка входящего документа: " + attach.Id
                           + "; Задача на обработку: " + task.Id);
              complitedDocs++;
              var doc = SBContracts.OfficialDocuments.As(Sungero.Content.ElectronicDocuments.GetAll(d => d.Id == Int64.Parse(idStr)).First());
              var clerk = Sungero.ExchangeCore.BusinessUnitBoxes.GetAll().Where(b => b.BusinessUnit == doc.BusinessUnit).FirstOrDefault()?.Responsible;
              var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(String.Format("Документ от {0} вернулся в исходную карточку", task.Counterparty?.Name), task.Author, clerk);
              notice.ActiveText = String.Format("Документ из задачи был автоматически возвращен в родную карточку. Необходимо завершить задание на контроль возврата");
              notice.Attachments.Add(doc);
              notice.Attachments.Add(task);
              notice.Deadline = Calendar.UserNow.AddDays(3);
              notice.Importance = Sungero.Workflow.SimpleTask.Importance.High;
              notice.Save();
              notice.Start();
              Logger.Debug("Exchange. ComeBackBodies. Отправлено уведомление по вх. документу " + incomingDoc.Id);
            }
          }
          
          Logger.Debug("Exchange. ComeBackBodies. Итоги по задаче: Успешно вернулись - " + complitedDocs +
                       "\n; Не требуют возврата или не могут вернуться: " + dontNeedDocs +
                       "\n; Требуют повторного возврата: " + failedDocs
                       + "\n; Задача на обработку: " + task.Id);
          
          if (failedDocs == 0)
            task.IsNeedComeback = false;
          
          if (task.NumberOfAttempsComeback > 336)
          {
            task.IsNeedComeback = false;
            task.NeedComebackAgainAttachments = null;
            Logger.Error("Exchange. ComeBackBodies. Задача " + task.Id + " превысила максимальное количество попыток обработки и больше не будет обрабатываться");
          }
          
          if (needComebackAgainAttachments.Length > 1)
            task.NeedComebackAgainAttachments = needComebackAgainAttachments.Substring(1);
          
          complitedDocs = 0;
          dontNeedDocs = 0;
          failedDocs = 0;
          needComebackAgainAttachments = "";
          task.Save();
        }
        
        Logger.Debug("Exchange. ComeBackBodies. Фоновый процесс завершился, обработано задач: " + numberOfTasks);
      }
      else
        Logger.Debug("Exchange. ComeBackBodies. Фоновый процесс завершился, нет задач на обработку");
    }

  }
}