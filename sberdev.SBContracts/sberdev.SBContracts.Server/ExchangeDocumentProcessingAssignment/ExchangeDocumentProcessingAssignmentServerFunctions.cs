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
    /// Функция рестартует задачу на нужного работника
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
    /// Функция переадресовывает задание на нужного работника
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
    /// Переадресовывает формалиованый документ
    /// </summary>
    public void DistributeFormalizedDocument()
    {
      var task = ExchangeDocumentProcessingTasks.As(_obj.Task);
      if (task.ReadressedSberDev == false)
      {
        string[] accountingDiscriminants = {"a523a263-bc00-40f9-810d-f582bae2205d", "f2f5774d-5ca3-4725-b31d-ac618f6b8850",
          "58986e23-2b0a-4082-af37-bd1991bc6f7e", "f50c4d8a-56bc-43ef-bac3-856f57ca70be",
          "74c9ddd4-4bc4-42b6-8bb0-c91d5e21fb8a", "58ad01fb-6805-426b-9152-4de16d83b258", "4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"};
        var attachs = _obj.AllAttachments;
        var recip = Roles.GetAll().Where(r => r.Sid == SberContracts.PublicConstants.Module.AccDocsHandlerRoleGuid).FirstOrDefault().RecipientLinks.FirstOrDefault().Member;
        var emp = Sungero.Company.Employees.As(recip);
        if (task.Addressee == emp || _obj.Addressee == emp)
          return;
        foreach(var attach in attachs)
        {
          string disc = attach.GetEntityMetadata().GetOriginal().NameGuid.ToString().ToLower();
          if (accountingDiscriminants.Contains(disc))
          {
            _obj.Addressee = emp;
            task.ReadressedSberDev = true;
            task.Addressee = _obj.Addressee;
            task.Save();
            _obj.Complete(SBContracts.ExchangeDocumentProcessingAssignment.Result.ReAddress);
            break;
          }
        }
      }
    }
    /// <summary>
    /// Функция возвращает имена документов из вложений без шелухи
    /// </summary>
    [Public]
    public string GetDocsNames()
    {
      string names = "";
      var attachs = _obj.Task?.Attachments;
      if (!attachs.Any())
        return names;
      foreach (var attach in attachs)
      {
        var doc = Sungero.Content.ElectronicDocuments.As(attach);
        if (doc != null)
        {
          var name = doc.Name;
          name = name.Replace("Вх. док. эл. обмена", "ВЭД");
          name = name.Replace(" от " + _obj.Counterparty.Name, "");
          string id = SberContracts.PublicFunctions.Module.GetNumberTag(name, " _ID");
          while (id != "")
          {
            name = name.Replace(" _ID" + id, "");
            id = SberContracts.PublicFunctions.Module.GetNumberTag(name, " _ID");
          }
          names += name + " // ";
        }
      }
      names = names.TrimEnd(new char[]{'/',' '});
      
      if (names.Length > 1000)
        return names.Substring(0, 1000);
      else
        return names;
    }
  }
}