using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Server
{
  partial class ExchangeDocumentProcessingAssignmentFunctions
  {

    /// <summary>
    /// Рестартует задачу на нужного работника
    /// </summary>
    [Public, Remote]
    public void RestartTask(Sungero.Company.IEmployee emp)
    {
      var task = ExchangeDocumentProcessingTasks.As(_obj.Task);
      var newTask = ExchangeDocumentProcessingTasks.Create();
      /* newTask.Subject = task.Subject;
      newTask.Deadline = task.Deadline;
      newTask.Box = task.Box;
      newTask.Counterparty = task.Counterparty;*/
      newTask.CopyPropertiesFrom(task);
      newTask.Addressee = emp;
      newTask.Status = Sungero.Exchange.ReceiptNotificationSendingTask.Status.Draft;
      var needSign = task.NeedSigning.All;
      var dontNeedSign = task.DontNeedSigning.All;
      foreach(var doc in needSign)
        newTask.NeedSigning.All.Add(doc);
      foreach(var doc in dontNeedSign)
        newTask.DontNeedSigning.All.Add(doc);
      newTask.Start();
      task.Abort();
    }
    
    /// <summary>
    /// Переадресовывает задание на нужного работника
    /// </summary>
    /// <param name="emp"></param>
    [Public, Remote]
    public void ReadressAssign(Sungero.Company.IEmployee emp)
    {
      _obj.Addressee = emp;
      _obj.NewDeadline = Calendar.AddWorkingDays(_obj.Deadline.Value, 2);
      // Прокинуть новый срок и исполнителя в задачу.
      var task = ExchangeDocumentProcessingTasks.As(_obj.Task);
      task.Addressee = _obj.Addressee;
      task.Deadline = _obj.NewDeadline;
      task.Save();
      _obj.Complete(SBContracts.ExchangeDocumentProcessingAssignment.Result.ReAddress);
    }
    
    /// <summary>
    /// Имена документов из вложений задания без «шелухи».
    ///</summary>
    [Public]
    public string GetDocsNames()
    {
      var sb = new System.Text.StringBuilder(256);
      var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

      var cpName = _obj.Counterparty != null ? _obj.Counterparty.Name : string.Empty;

      // Локальный санитайзер названий
      System.Func<string, string> sanitize = raw =>
      {
        if (string.IsNullOrWhiteSpace(raw))
          return string.Empty;

        var name = raw.Replace("Вх. док. эл. обмена", "ВЭД");

        if (!string.IsNullOrEmpty(cpName))
          name = name.Replace(" от " + cpName, string.Empty);

        return name.Trim();
      };

      foreach (var att in _obj.AllAttachments)
      {
        var edoc = Sungero.Content.ElectronicDocuments.As(att);
        if (edoc == null)
          continue;

        var clean = sanitize(edoc.Name);
        if (string.IsNullOrEmpty(clean))
          continue;

        if (seen.Add(clean))
        {
          if (sb.Length > 0)
            sb.Append(" // ");
          sb.Append(clean);

          // Ранний выход, чтобы лишний раз не бегать по коллекции
          if (sb.Length > 1000)
            break;
        }
      }

      var result = sb.ToString();
      return result.Length > 1000 ? result.Substring(0, 1000) : result;
    }


  }
}