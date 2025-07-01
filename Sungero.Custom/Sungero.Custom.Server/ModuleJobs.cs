using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;

namespace Sungero.Custom.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Фоновый процесс обхода договорных документов с целью простановки Продуктов в список
    /// </summary>
    public virtual void JobResaveContractualDoc()
    {
      var Exc = sberdev.SBContracts.ContractualDocuments.GetAll(s => ((s.ProdCollectionPrBaseSberDev.Count > 0) || (s.ProdCollectionExBaseSberDev.Count > 0))).ToList();
      foreach (var elem in Exc)
      {
        try
        {
          if (elem.Note.EndsWith(" "))
            elem.Note = elem.Note.Substring(0, elem.Note.Length - 1);
          else
            elem.Note += " ";
          elem.Save();
        }
        catch
        {
          //Res += "-";
        }
      }
    }
    
    private string GenAccesRef(long UsID, long PdrID, bool Edit)
    {
      string log = "";
      var us = Sungero.Company.Employees.Get(UsID);
      var Departament = Sungero.Company.Departments.Get(PdrID);
      var tasks = new List<System.Threading.Tasks.Task>();
      var DefAcc = Edit ? DefaultAccessRightsTypes.Change : DefaultAccessRightsTypes.Read;
      if (Departament.RecipientLinks.Count > 0)
      {
        foreach (var elem in Departament.RecipientLinks)
        {
          log += "В обработке сотрудник: " + elem.Id.ToString();
          if (Sungero.Company.Employees.Is(elem.Member))
          {
            var Tasks = PublicFunctions.Module.GetListTask(elem.Member.Id, us, Edit);
            log += "Задач на обрабтку: " + Tasks.Count.ToString();
            if (Tasks.Count > 0)
            {
              foreach (var task in Tasks)
              {
                var RefAcc = Custom.AccesUserToTasks.GetAll(t => ((t.Recipient.Name == us.Name) && (t.Task == task))).FirstOrDefault();
                if (RefAcc == null)
                {
                  RefAcc = Custom.AccesUserToTasks.Create();
                  RefAcc.Recipient = us;
                  RefAcc.Task = task;
                  RefAcc.Control = false;
                  if (Edit)
                    RefAcc.EditAcces = true;
                  else
                    RefAcc.EditAcces = false;
                  
                  RefAcc.Name = us.Name + " => " + task.Subject;
                  RefAcc.Save();
                  log += "Создана запись в справочнике: " + RefAcc.Name.ToString();
                }
              }
            }
          }
        }
      }
      return log;
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void JobAddAccesToUser()
    {
      string logall = Calendar.Now.ToString();
      var requestList = Custom.RequestAccesTaskToUses.GetAll(r => r.Status == Custom.RequestAccesTaskToUs.Status.Active).ToArray();
      try
      {
        if (requestList.Count() > 0)
        {
          foreach (var req in requestList)
          {
            logall += '\n' + "Получена заявка на выдачу прав: " + req.Name.ToString();
            GenAccesRef(req.Employee.Id, req.Department.Id, req.EditAcces.Value);
          }
        }

        //======================================================================================
        logall += '\n' + "Выдача прав по заявкам завершена: " + Calendar.Now.ToString();
        logall += '\n' + "Начало обработки справочника: " + Calendar.Now.ToString();
        var RefAcc = Custom.AccesUserToTasks.GetAll(t => t.Control == false).ToList();
        logall += '\n' + "Записей: " + RefAcc.Count.ToString();
        if (RefAcc.Count > 0)
        {
          foreach (var str in RefAcc)
          {
            var task = str.Task;
            logall += '\n' + "В работе задача: " + task.Id.ToString();
            foreach (var prop in task.State.Properties)
            {
              prop.IsRequired = false;
            }
            
            string log = "";
            var us = str.Recipient;
            log += "---------------------------------------------" + '\n';
            log += "В работе сотрудник: " + str.Recipient.Id.ToString() + '\n';
            var DefAccBool = str.EditAcces.HasValue ? str.EditAcces.Value : false;
            var DefAcc = str.EditAcces.HasValue ? (str.EditAcces.Value ? DefaultAccessRightsTypes.Change :  DefaultAccessRightsTypes.Read) : DefaultAccessRightsTypes.Read;
            log += PublicFunctions.Module.AddAccesToObject(task, DefAccBool, us);
            try
            {
              task.AccessRights.Grant(us, DefAcc);
              task.AccessRights.Save();
              log += "Выданы права на задачу: " + task.DisplayValue.ToString() + '\n';
            }
            catch (Exception e)
            {
              log += "Ошибка при выдаче прав на задачу: " + e.Message.ToString() + '\n';
            }

            if (task.Attachments.Count > 0)
            {
              foreach (var attach in task.Attachments)
              {
                if (!attach.AccessRights.IsGrantedWithoutSubstitution(DefAcc, us))
                {
                  log += PublicFunctions.Module.AddAccesToObject(attach, DefAccBool, us);
                  try
                  {
                    attach.AccessRights.Grant(us, DefAcc);
                    attach.AccessRights.Save();
                    log += "Выданы права на вложение: " + attach.DisplayValue.ToString() + '\n';
                  }
                  catch (Exception e)
                  {
                    log += "Ошибка при выдаче прав на документ: " + e.Message.ToString() + '\n';
                  }
                }
              }
            }
            logall += '\n' + "ЛОГ: " + log.ToString();
            str.Control = true;
            str.HistoryNote = log;
            str.Save();
          }
        }
        //Logger.Debug(logall);
      }
      catch (Exception er)
      {
        //Logger.Debug("ЗАВЕРШЕНИЕ ФП с ошибкой: " + logall + '\n' + er.Message.ToString());
      }
    }

    /// <summary>
    /// Фоновый професс сбора отчета по заданиям "Проконтролируйте возврат"
    /// </summary>
    public virtual void JobControlFeedback()
    {
      var Jobs = Sungero.Workflow.Assignments.GetAll(j => ((j.Created >= Sungero.Core.Calendar.BeginningOfWeek(Sungero.Core.Calendar.AddWorkingDays(Sungero.Core.Calendar.Now,-2)))
                                                           && (j.Subject.Contains("Проконтролируйте возврат")) && (!j.Subject.Contains(">>")))).ToArray();
      
      string htmlContent = "<html><head><meta charset='utf-8'><title>Отчет Контроль возврата</title></head><body>";
      htmlContent += "<h1>Отчет Контроль возврата на " + Sungero.Core.Calendar.Now.ToString() + "</h1>";
      htmlContent += "<table border='1'><tr><th>Тип</th><th>Тема</th><th>Создан</th><th>Срок</th><th>Инициатор</th><th>Исполнитель</th><th>e-mail</th><th>Руководитель</th><th>e-mail</th></tr>";
      
      if (Jobs.Count() > 0)
      {
        string Data = "Тип|Тема|От|Срок|Инициатор|Исполнитель|e-mail|Руководитель|e-mail";
        foreach (var job in Jobs)
        {
          /*Data += '\n' + "Контроль возврата";
          Data += "|" + job.Subject.ToString();
          Data += "|" + job.Created.ToString();
          Data += "|" + (job.Deadline.HasValue ? job.Deadline.ToString() : "Без срока");
          Data += "|" + (job.Author != null ? job.Author.Name.ToString() : "");
          Data += "|" + (job.Performer != null ? job.Performer.Name.ToString() : "");
          var Performer = Sungero.Company.Employees.GetAll(r => r.Login == job.Performer.Login).FirstOrDefault();
          if (Performer != null)
          {
            Data += "|" + (Performer.Email != null ? Performer.Email.ToString() : "");
            Data += "|" + (Performer.Department.Manager != null ? Performer.Department.Manager.ToString() : "Не указан в подразделении");
            Data += "|" + (Performer.Department.Manager != null ? (Performer.Department.Manager.Email != null ? Performer.Department.Manager.Email.ToString() : "Нет") : "Нет");
          }
          Data = Data.Replace(';',':');
        }
        Data = Data.Replace('|',';');
        string TekDat = Sungero.Core.Calendar.Now.ToString();
        TekDat = TekDat.Replace(':', '-').Replace('.', '-').Replace(' ', '_');
        string filePath = @"C:\temp\Report_" + TekDat + ".csv"; */
          
          htmlContent += "<tr>";
          htmlContent += "<td>Контроль возврата</td>";
          htmlContent += "<td>" + job.Subject + "</td>";
          htmlContent += "<td>" + job.Created.ToString() + "</td>";
          htmlContent += "<td>" + (job.Deadline.HasValue ? job.Deadline.ToString() : "Без срока") + "</td>";
          htmlContent += "<td>" + (job.Author != null ? job.Author.Name : "") + "</td>";
          htmlContent += "<td>" + (job.Performer != null ? job.Performer.Name : "") + "</td>";

          var Performer = Sungero.Company.Employees.GetAll(r => r.Login == job.Performer.Login).FirstOrDefault();
          if (Performer != null)
          {
            htmlContent += "<td>" + (Performer.Email != null ? Performer.Email : "") + "</td>";
            htmlContent += "<td>" + (Performer.Department.Manager != null ? Performer.Department.Manager.ToString() : "Не указан") + "</td>";
            htmlContent += "<td>" + (Performer.Department.Manager != null ? (Performer.Department.Manager.Email != null ? Performer.Department.Manager.Email : "Нет") : "Нет") + "</td>";
          }
          else
          {
            htmlContent += "<td></td><td></td><td></td>";
          }

          htmlContent += "</tr>";
        }

        htmlContent += "</table></body></html>";

        string TekDat = Sungero.Core.Calendar.Now.ToString();
        TekDat = TekDat.Replace(':', '-').Replace('.', '-').Replace(' ', '_');
        string filePath = @"C:\temp\Report_" + TekDat + ".html";
        
        
        try
        {
          File.WriteAllText(filePath, Data, new System.Text.UTF8Encoding(true));
          var doc = Sungero.Docflow.SimpleDocuments.CreateFrom(filePath);
          doc.DocumentKind = Sungero.Docflow.DocumentKinds.GetAll(d => d.Name == "Простой документ").FirstOrDefault();
          doc.Subject = "Отчет Контроль возврата на " + Sungero.Core.Calendar.Now.ToString();
          doc.Name = "Отчет Контроль возврата на " + Sungero.Core.Calendar.Now.ToString();
          doc.Save();
          
          var mintask = Custom.NotiffDocContracts.Create();
          var Isp = Logins.GetAll(l => l.LoginName == "delopp").FirstOrDefault();
          if (Isp != null)
            mintask.Employee = Sungero.Company.Employees.GetAll(empl => empl.Login == Isp).FirstOrDefault();
          else
            mintask.Employee = Sungero.Company.Employees.Get(long.Parse("431"));
          
          mintask.BaseAttachment.ElectronicDocuments.Add(doc);
          mintask.Subject = "Отчет Контроль возврата на " + Sungero.Core.Calendar.Now.ToString();
          mintask.Save();
          mintask.Start();
        }
        catch (Exception ex)
        {
          Logger.Debug("Возникла проблема в фоновом процессе: " + ex.Message.ToString());
        }
        
      }
    }

    /// <summary>
    /// Процесс поиска и расстановки уровней подчиненности среди организаций
    /// </summary>
    public virtual void JobHeadOrg()
    {
      var ListOrgTrue = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => c.HeadOrgSDev == true).ToArray();
      if (ListOrgTrue.Count() > 0)
      {
        foreach (var elem in ListOrgTrue)
        {
          if (elem.TIN != null)
          {
            var OtherOrg = sberdev.SBContracts.Companies.GetAll(c => ((c.TIN == elem.TIN) && (c.HeadCompany != elem) && (c.Id != elem.Id))).ToArray();
            if (OtherOrg.Count() > 0)
            {
              foreach (var othorg in OtherOrg)
              {
                othorg.HeadOrgSDev = false;
                othorg.HeadCompany = elem;
                othorg.Save();
              }
            }
          }
        }
      }
      
      var ListCompany = sberdev.SBContracts.Companies.GetAll(c => ((c.HeadCompany == null) && (!c.HeadOrgSDev.HasValue))).ToArray();
      if (ListCompany.Count() > 0)
      {
        foreach (var org in ListCompany)
        {
          if (org.TIN != null)
          {
            var OtherOrg2 = sberdev.SBContracts.Companies.GetAll(c => ((c.TIN == org.TIN) && (c.HeadOrgSDev.HasValue) && (c.Id != org.Id))).ToArray();
            if (OtherOrg2.Count() > 0)
            {
              foreach (var othorg in OtherOrg2)
              {
                if (othorg.HeadOrgSDev.Value)
                {
                  org.HeadCompany = othorg;
                  org.Save();
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Процесс редоставления прав на документы по признаку нахождения МВЗ
    /// </summary>
    public virtual void JobAccesMVZ()
    {
      var MVZs = sberdev.SberContracts.MVZs.GetAll(m => m.Status == sberdev.SberContracts.MVZ.Status.Active).ToList();
      if (MVZs.Count > 0)
      {
        foreach (var mvz in MVZs)
        {
          if (mvz.CollectionEmplAcc.Count > 0)
          {
            foreach (var empl in mvz.CollectionEmplAcc)
            {
              var usr = empl.Employee;
              
              var DocsContractual = sberdev.SBContracts.ContractualDocuments.GetAll(c => ((c.MVZBaseSberDev == mvz) && (!c.AccessRights.CanRead(usr)))).ToList();
              var DocsAccounting = sberdev.SBContracts.AccountingDocumentBases.GetAll(a => ((a.MVZBaseSberDev == mvz) && (!a.AccessRights.CanRead(usr)))).ToList();
              
              if (DocsContractual.Count > 0)
              {
                foreach (var contr in DocsContractual)
                {
                  if (!contr.AccessRights.CanRead(usr))
                  {
                    contr.AccessRights.Grant(usr, DefaultAccessRightsTypes.Read);
                    contr.Save();
                  }
                  
                  var Tasks = Sungero.Docflow.ApprovalTasks.GetAll(t => t.DocumentGroup.OfficialDocuments.FirstOrDefault() != null).Where(t => ((t.DocumentGroup.OfficialDocuments.FirstOrDefault() == contr) && (!t.AccessRights.CanRead(usr)))).ToList();
                  if (Tasks.Count > 0)
                  {
                    foreach (var tsk in Tasks)
                    {
                      if (!tsk.AccessRights.CanRead(usr))
                      {
                        tsk.AccessRights.Grant(usr, DefaultAccessRightsTypes.Read);
                        tsk.Save();
                      }
                    }
                  }
                }
              }
              if (DocsAccounting.Count > 0)
              {
                foreach (var Accoun in DocsAccounting)
                {
                  if (!Accoun.AccessRights.CanRead(usr))
                  {
                    Accoun.AccessRights.Grant(usr, DefaultAccessRightsTypes.Read);
                    Accoun.Save();
                  }
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Фоновый процесс рассылки уведомлений по договорам с истекающим сроком действия
    /// </summary>
    public virtual void ControlSkorContracts()
    {
      DateTime currentDate = Calendar.Today;
      var targetDates = new List<DateTime>
      {
        currentDate.AddDays(14),
        currentDate.AddDays(7),
        currentDate.AddDays(3),
        currentDate.AddDays(1)
      };
      
      foreach (var targetDate in targetDates)
      {
        if (targetDate.DayOfWeek == DayOfWeek.Saturday)
          targetDate.AddDays(-1);
        else if (targetDate.DayOfWeek == DayOfWeek.Sunday)
          targetDate.AddDays(-2);
      }
      var Contractuals = sberdev.SBContracts.Contracts.GetAll(d => d.ValidTill.HasValue).Where(c => ((targetDates.Contains(c.ValidTill.Value)) &&
                                                                                                     (c.LifeCycleState == sberdev.SBContracts.Contract.LifeCycleState.Active)));
      if (Contractuals.Count() > 0)
      {
        foreach (var cons in Contractuals)
        {
          if (cons.Assignee != null)
          {
            if (cons.Assignee.Login != null)
            {
              var Empl = Sungero.Company.Employees.GetAll(r => r.Login == cons.Assignee.Login).FirstOrDefault();
              if (Empl != null)
              {
                var task = FreedomicTasks.Create();
                task.Employee = Empl;
                task.Subject = "Заканчивается срок действия договора: " + cons.Name;
                task.OtherAttachment.ElectronicDocuments.Add(cons);
                task.Start();
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Обработка заданий для занесения их в список заданий в справочнике
    /// </summary>
    public virtual void JobsinReference()
    {
      var ActualList = DatabookJobses.GetAll();
      List<long> ListIds = new List<long>();
      foreach (var elem in ActualList)
      {
        ListIds.Add(long.Parse(elem.IDJob.Value.ToString()));
      }
      var Jobs = Sungero.Workflow.Assignments.GetAll(a => ((a.Status == Sungero.Workflow.Assignment.Status.InProcess) && (!ListIds.Contains(a.Id)))).ToList();
      foreach (var job in Jobs)
      {
        var DJ = DatabookJobses.Create();
        try
        {
          DJ.IDJob = int.Parse(job.Id.ToString());
          DJ.Save();
        }
        catch (Exception e)
        {
          Logger.Debug("Фоновый процесс сбора заданий завершился ошибкой: " + e.Message.ToString());
        }
      }
    }

  }
}