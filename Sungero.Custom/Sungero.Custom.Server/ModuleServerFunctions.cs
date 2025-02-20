using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sungero.Core;
using Sungero.CoreEntities;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Events;

namespace Sungero.Custom.Server
{
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Функция выдачи прав на документы и задачи подразделения для указанного пользователя
    /// </summary>
    [Public]
    public List<Sungero.Workflow.ITask> GetListTask(long memberid, Sungero.Company.IEmployee us, bool OnlyDogContr)
    {
      List<Sungero.Workflow.ITask> TaskList = new List<Sungero.Workflow.ITask>();
      var DefAcc = OnlyDogContr ? DefaultAccessRightsTypes.Change : DefaultAccessRightsTypes.Read;
      var Tasks = Sungero.Workflow.Tasks.GetAll(t => t.Author.Login == Sungero.Company.Employees.Get(memberid).Login).ToArray();
      Tasks = Tasks.Where(t => !t.AccessRights.IsGrantedWithoutSubstitution(DefAcc, us)).ToArray();
      if (Tasks.Count() > 0)
      {
        foreach (var task in Tasks)
        {
          if (Custom.AccesUserToTasks.GetAll(r => r.Task == task).FirstOrDefault() == null)
            TaskList.Add(task);
        }
      }
      return TaskList;
    }
    
    /// <summary>
    /// Функция выдачи прав на документы и задачи подразделения для указанного пользователя
    /// </summary>
    [Public]
    public string AddAccesToDocAndTasks2(long memberid, Sungero.Company.IEmployee us, bool OnlyDogContr)
    {
      string log = "";
      if (Sungero.Company.Employees.GetAll(r => r.Id == memberid).FirstOrDefault() != null)
      {
        var DefAcc = OnlyDogContr ? DefaultAccessRightsTypes.Change : DefaultAccessRightsTypes.Read;
        var Tasks = Sungero.Workflow.Tasks.GetAll(t => t.Author.Login == Sungero.Company.Employees.Get(memberid).Login).ToArray();
        Tasks = Tasks.Where(t => !t.AccessRights.IsGrantedWithoutSubstitution(DefAcc, us)).ToArray();
        if (Tasks.Count() > 0)
        {
          foreach (var task in Tasks)
          {
            log += "В работе Задача: (" + task.Id.ToString() + ") " + task.Subject + '\n';
            var RefAcc = Custom.AccesUserToTasks.GetAll(t => ((t.Recipient.Name == us.Name) && (t.Task == task))).FirstOrDefault();
            if (RefAcc == null)
            {
              RefAcc = Custom.AccesUserToTasks.Create();
              RefAcc.Recipient = us;
              RefAcc.Task = task;
              RefAcc.Control = false;
              RefAcc.Name = us.Name + " => " + task.Subject;
              RefAcc.Save();
            }
            
            try
            {
              if (!task.AccessRights.IsGrantedWithoutSubstitution(DefAcc, us))
              {
                task.AccessRights.Grant(us, DefAcc);
                task.AccessRights.Save();
              }
            }
            catch (Exception f)
            {
              log += "Ошибка при выдаче прав на задачу: " + f.Message.ToString() + '\n';
            }
            if (task.Attachments.Count > 0)
            {
              foreach (var attach in task.Attachments)
              {
                if (!attach.AccessRights.IsGrantedWithoutSubstitution(DefAcc, us))
                {
                  log += "В работе вложение: (" + attach.Id.ToString() + ") " + attach.DisplayValue + '\n';
                  try
                  {
                    attach.AccessRights.Grant(us, DefAcc);
                    attach.AccessRights.Save();
                  }
                  catch (Exception e)
                  {
                    log += "Ошибка при выдаче прав на документ: " + e.Message.ToString() + '\n';
                  }
                }
              }
            }
          }
        }
      }
      if (log.Length < 1000)
        return log;
      else
      {
        Logger.Debug(log);
        return "Лог содержит более 1000 симвлов и не отображается. Вся информация в логах.";
      }
    }
    
    [Public]
    public string AsyncAddAccesToDocAndTasks(long memberid, Sungero.Company.IEmployee us, bool OnlyDogContr)
    {
      System.Threading.Tasks.Task<string> result = AddAccesToDocAndTasks(memberid, us, OnlyDogContr);
      return result.ToString();
    }
    
    /// <summary>
    /// Функция выдачи прав на документы и задачи подразделения для указанного пользователя
    /// </summary>
    private async System.Threading.Tasks.Task<string> AddAccesToDocAndTasks(long memberid, Sungero.Company.IEmployee us, bool OnlyDogContr)
    {
      StringBuilder log = new StringBuilder();
      var Contractualdocs = sberdev.SBContracts.ContractualDocuments.GetAll(t => t.Author.Login == Sungero.Company.Employees.Get(memberid).Login).ToArray();
      Contractualdocs = Contractualdocs.Where(t => !t.AccessRights.IsGrantedWithoutSubstitution(DefaultAccessRightsTypes.Read, us)).ToArray();
      
      if (Contractualdocs.Count() > 0)
      {
        log.AppendLine("В обработке автор: " + Sungero.Company.Employees.Get(memberid).Name);
        foreach (var doc in Contractualdocs)
        {
          log.AppendLine("В работе Документ: (" + doc.Id + ") " + doc.Name);
          try
          {
            doc.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
            await SaveDocumentAsync(doc);
          }
          catch (Exception ex)
          {
            log.AppendLine("Ошибка при выдаче прав на документ: " + ex.Message);
          }
          
          // Проверяем, не превышён ли лимит для логов
          //if (log.Length > 800) // Проверка длины логов
          //{
          //  Logger.Debug(log.ToString());
          //  log.Clear(); // Очищаем логи, чтобы не накапливать слишком много данных
          //}
        }
      }

      if ((Sungero.Company.Employees.GetAll(r => r.Id == memberid).FirstOrDefault() != null) && (!OnlyDogContr))
      {
        var Tasks = Sungero.Workflow.Tasks.GetAll(t => t.Author.Login == Sungero.Company.Employees.Get(memberid).Login).ToArray();
        Tasks = Tasks.Where(t => !t.AccessRights.IsGrantedWithoutSubstitution(DefaultAccessRightsTypes.Read, us)).ToArray();

        if (Tasks.Count() > 0)
        {
          foreach (var task in Tasks)
          {
            log.AppendLine("В работе Задача: (" + task.Id + ") " + task.Subject);
            try
            {
              if (!task.AccessRights.IsGrantedWithoutSubstitution(DefaultAccessRightsTypes.Read, us))
              {
                task.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
                await SaveTaskAsync(task);
              }
            }
            catch (Exception ex)
            {
              log.AppendLine("Ошибка при выдаче прав на задачу: " + ex.Message);
            }

            foreach (var attach in task.Attachments)
            {
              if (!attach.AccessRights.IsGrantedWithoutSubstitution(DefaultAccessRightsTypes.Read, us))
              {
                try
                {
                  if (Sungero.Content.ElectronicDocuments.Is(attach))
                  {
                    attach.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
                    await SaveAttachmentAsync(Sungero.Content.ElectronicDocuments.As(attach));
                  }
                }
                catch (Exception ex)
                {
                  log.AppendLine("Ошибка при выдаче прав на вложение: " + ex.Message);
                }
              }
            }
            
            // Проверка длины логов после каждой задачи
            //if (log.Length > 800)
            //{
            //  Logger.Debug(log.ToString());
            //  log.Clear();
            //}
          }
        }
      }

      //if (log.Length < 1000)
      return log.ToString();
      //else
      //{
      //  Logger.Debug(log.ToString());
      //  return "Лог содержит более 1000 символов и не отображается. Вся информация в логах.";
      //}
    }

    private async Task SaveDocumentAsync(Sungero.Content.IElectronicDocument document)
    {
      // Имитация асинхронного сохранения документа
      await Task.Run(() => document.AccessRights.Save());
    }

    private async Task SaveTaskAsync(Sungero.Workflow.ITask task)
    {
      // Имитация асинхронного сохранения задачи
      await Task.Run(() => task.AccessRights.Save());
    }

    private async Task SaveAttachmentAsync(Sungero.Content.IElectronicDocument attachment)
    {
      // Имитация асинхронного сохранения вложения
      await Task.Run(() => attachment.AccessRights.Save());
    }

    /// <summary>
    /// Функция создания лимита по документу типа договор
    /// </summary>
    [Public]
    public void CreateLiminInContract(long IdDoc)
    {
      if (ControlContracts.GetAll(d => ((d.LeadingDocID == int.Parse(IdDoc.ToString())) && (d.Limit != null))).FirstOrDefault() == null)
      {
        var doc = ControlContracts.Create();
        doc.LeadingDocID = int.Parse(IdDoc.ToString());
        if (doc.Limit != null)
          doc.Save();
      }
    }
    
    /// <summary>
    /// Функция Занесения нового элемента в список контроля лимита по договору
    /// </summary>
    [Public]
    public void CreateSupInLiminInContract(long IdLeadDoc, long IdDoc)
    {
      var LeadDoc = ControlContracts.GetAll(d => ((d.LeadingDocID == int.Parse(IdLeadDoc.ToString())) && (d.Limit != null))).FirstOrDefault();
      if (LeadDoc != null)
      {
        var DDS = LeadDoc.CollectionDocs;
        bool valid = true;
        if (DDS.Count > 0)
        {
          foreach (var str in DDS)
          {
            if (str.IDDoc.Value == int.Parse(IdDoc.ToString()))
              valid = false;
          }
          if (valid)
          {
            var NewStr = LeadDoc.CollectionDocs.AddNew();
            NewStr.IDDoc = int.Parse(IdDoc.ToString());
            LeadDoc.Save();
          }
        }
      }
    }
    
    /// <summary>
    /// Функция запрос лимита по догвоору с утетом Указанной суммы - true если контроля нет или он допустимый и false если он превышен.
    /// </summary>
    [Public]
    public bool RequestCorrectLiminInContract(long LeadingDocID, double EntityDocSumm)
    {
      var LeadDoc = ControlContracts.GetAll(d => ((d.LeadingDocID == int.Parse(LeadingDocID.ToString())) && (d.Limit != null))).FirstOrDefault();
      if (LeadDoc != null)
      {
        if ((LeadDoc.TotalLimit - EntityDocSumm) < 0.0)
          return false;
        else
          return true;
      }
      else
        return true;
    }
    
    /// <summary>
    /// Функция запрос лимита по догвоору с утетом Указанной суммы - Возвращает вещественное число разницы
    /// </summary>
    [Public]
    public double RequestSummLiminInContract(long LeadingDocID, double EntityDocSumm)
    {
      var LeadDoc = ControlContracts.GetAll(d => ((d.LeadingDocID == int.Parse(LeadingDocID.ToString())) && (d.Limit != null))).FirstOrDefault();
      if (LeadDoc != null)
        return LeadDoc.TotalLimit.Value - EntityDocSumm;
      else
        return 0.0;
    }

    /// <summary>
    /// Функция проверки соответствия пользователя на принадлежность группе "Допуск к маркетинговым документам"
    /// </summary>
    [Public]
    public bool DostumMarketing(Sungero.CoreEntities.IUser user)
    {
      var Rol = Roles.GetAll(r => r.Name == "Допуск к маркетинговым документам").FirstOrDefault();
      bool marker = false;
      if (Rol != null)
      {
        var Empl = Sungero.Company.Employees.GetAll(t => t.Login == user.Login).FirstOrDefault();
        if (Empl != null)
        {
          if (Rol.RecipientLinks.Count > 0)
          {
            foreach (var elem in Rol.RecipientLinks)
            {
              if (elem.Member.Id == Empl.Id)
                marker = true;
            }
          }
        }
      }
      return marker;
    }
    
    
    [Public(WebApiRequestType = RequestType.Get)]
    public string GetTestValFromRabbitMQ()
    {
      string originalMessage = string.Empty;
      
      // Создать очередь сообщений для справочника «Организации»
      string exchangeName = "adm.companies.changeElements.drx";
      string queueName = "adm.companies.changeElements.drx";
      string routingKey = "adm.companies.drx";
      
      ConnectionFactory factory = new ConnectionFactory();
      // 'virtualhost=rxhost;hostname=192.168.115.10;username=rxhost;password=rxhost;exchange=directumrx_sberdev;Port=5672'
      factory.UserName =  "rxhost";
      factory.Password =  "rxhost";
      factory.VirtualHost = "rxhost";
      factory.HostName = "localhost"; // hostName;
      factory.Port = 5672;

      // Создание соединения
      IConnection conn = factory.CreateConnection();
      
      // Создание канала
      IModel channel = GetRabbitChannel(conn, exchangeName, queueName, routingKey); // channel = conn.CreateModel();

      // Получение сообщения
      BasicGetResult result = channel.BasicGet(queueName, false);
      if (result == null)
      {
        // В настоящее время нет доступных сообщений.
        originalMessage = "0";
      }
      else
      {
        //byte[] body = result.Body;
        originalMessage = result.MessageCount.ToString(); //  System.Text.Encoding.UTF8.GetString(body);
      }
      
      // Отключение соединения
      channel.Close();
      conn.Close();
      
      return originalMessage;
    }
    
    private IModel GetRabbitChannel(IConnection conn, string exchangeName, string queueName, string routingKey)
    {
      IModel model = conn.CreateModel();
      model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
      model.QueueDeclare(queueName, false, false, false, null);
      model.QueueBind(queueName, exchangeName, routingKey, null);
      return model;
    }
    
  }
}