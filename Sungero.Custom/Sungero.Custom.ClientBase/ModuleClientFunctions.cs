using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.Text;

namespace Sungero.Custom.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Тестирование доступности на чтение задач по ИД
    /// </summary>
    public virtual void TestCanRead()
    {
      var Dial = Dialogs.CreateInputDialog("Укажите вводные данные для тестирования");
      var Inpt = Dial.AddSelect("Укажите сотрудника",true,Sungero.Company.Employees.Null);
      var Podr = Dial.AddString("Укажите ИД Задачи",true);
      if (Dial.Show() == DialogButtons.Ok)
      {
        var us = Sungero.Company.Employees.Get(Inpt.Value.Id);
        long Idtask = long.Parse(Podr.Value);
        string log = Calendar.Now.ToString() + '\n';
        var Task = Sungero.Workflow.Tasks.Get(Idtask);
        if (Task.AccessRights.CanRead(us))
          log += "Task.AccessRights.CanRead(us): Вернул TRUE" + '\n';
        else
          log += "Task.AccessRights.CanRead(us): Вернул FALSE" + '\n';
        
        if (Task.AccessRights.CanGrant(DefaultAccessRightsTypes.Read))
          log += "Task.AccessRights.CanGrant(Read): Вернул TRUE" + '\n';
        else
          log += "Task.AccessRights.CanGrant(Read): Вернул FALSE" + '\n';
        
        if (Task.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, us))
          log += "Task.AccessRights.IsGranted(Read, us): Вернул TRUE" + '\n';
        else
          log += "Task.AccessRights.IsGranted(Read, us): Вернул FALSE" + '\n';
        
        Dialogs.ShowMessage(log);
      }
    }

    /// <summary>
    /// Функция выдачи прав на задачи и документы по Подразделению
    /// </summary>
    public virtual void AddAccesToUser()
    {
      var Dial = Dialogs.CreateInputDialog("Укажите вводные данные для тестирования");
      var Inpt = Dial.AddSelect("Укажите сотрудника",true,Sungero.Company.Employees.Null);
      var Podr = Dial.AddSelect("Выберите подразделение",true,Sungero.Company.Departments.Null);
      var DogContr = Dial.AddBoolean("Отработать только договорные документы",false);
      if (Dial.Show() == DialogButtons.Ok)
      {
        var us = Sungero.Company.Employees.Get(Inpt.Value.Id);
        //Dialogs.ShowMessage("Выбран пользователь: " + us.Name + '\n' + "Выбрано подразделение: " + Sungero.Company.Departments.Get(Podr.Value.Id).Name);
        string log = Calendar.Now.ToString() + '\n';
        var Departament = Sungero.Company.Departments.Get(Podr.Value.Id);
        if (Departament.RecipientLinks.Count > 0)
        {
          foreach (var elem in Departament.RecipientLinks)
          {
            var Contractualdocs = sberdev.SBContracts.ContractualDocuments.GetAll(t => t.Author.Login == Sungero.Company.Employees.Get(elem.Member.Id).Login).ToArray();
            Contractualdocs = Contractualdocs.Where(t => t.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, us)).ToArray();
            if (Contractualdocs.Count() > 0)
            {              
              foreach (var doc in Contractualdocs)
              {
                log += "В работе Документ: (" + doc.Id.ToString() + ") " + doc.Name + '\n';
                try
                  {
                doc.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
                doc.Save();
                log += "Выданы права на просмотр: (" + doc.Id.ToString() + ")" + '\n';
                }
                catch (Exception t)
                {
                  log += "Проблема: (" + t.Message.ToString() + ")" + '\n';
                }
              }
            }
            if ((Sungero.Company.Employees.GetAll(r => r.Id == elem.Member.Id).FirstOrDefault() != null) && (!DogContr.Value.Value))
            {
              log += "В обработке автор: " + Sungero.Company.Employees.Get(elem.Member.Id).Name + '\n';
              var Tasks = Sungero.Workflow.Tasks.GetAll(t => t.Author.Login == Sungero.Company.Employees.Get(elem.Member.Id).Login).ToArray();
              Tasks = Tasks.Where(t => t.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, us)).ToArray();
              if (Tasks.Count() > 0)
              {
                foreach (var task in Tasks)
                {
                  log += "В работе Задача: (" + task.Id.ToString() + ") " + task.Subject + '\n';
                  try
                  {
                    if (!task.AccessRights.CanRead(us))
                    {
                      task.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
                      task.Save();
                      log += "Выданы права на просмотр: (" + task.Id.ToString() + ")" + '\n';
                    }
                    else
                    {
                      log += "Контроль прав не пройден! Проверяем, Может права уже выданы?" + '\n';
                      if (task.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, us))
                      {
                        log += "Система уверена, что уже выданы. Похер - выдадим принудительно!" + '\n';
                        task.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
                        task.Save();
                      }
                      else
                      {
                        log += "Система понимает, что НЕ выданы. И не выдает, зараза..." + '\n';
                      }
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
                      if (!attach.AccessRights.CanRead(us))
                      {
                        log += "В работе вложение: (" + attach.Id.ToString() + ") " + attach.DisplayValue + '\n';
                        try
                        {
                          
                          attach.AccessRights.Grant(us, DefaultAccessRightsTypes.Read);
                          attach.Save();
                          log += "Выданы права на просмотр: (" + attach.Id.ToString() + ")" + '\n';
                          
                        }
                        catch (Exception e)
                        {
                          log += "Ошибка при выдаче прав на документ: " + e.Message.ToString() + '\n';
                        }
                      }
                      else
                      {
                        log += "Контроль прав не пройден!" + '\n';
                      }
                    }
                  }
                }
              }
            }
          }
        }
        Dialogs.ShowMessage(log);
      }
    }

    /// <summary>
    /// Обновление (пересохранение) всех договорных документов
    /// </summary>
    public virtual void UpdateContractualDoc()
    {
      var Exc = sberdev.SBContracts.ContractualDocuments.GetAll().ToList();
      string Res = "";
      foreach (var elem in Exc)
      {
        try
        {
          if ((sberdev.SBContracts.Contracts.Is(elem)) || (sberdev.SBContracts.SupAgreements.Is(elem)))
          {
            if (elem.ProdCollectionPrBaseSberDev.Count > 0)
            {
              elem.Note += " ";
              elem.Save();
              Res += "+";
            }
            if (elem.ProdCollectionExBaseSberDev.Count > 0)
            {
              elem.Note += " ";
              elem.Save();
              Res += "+";
            }
          }
        }
        catch
        {
          Res += "-";
        }
      }
      Dialogs.ShowMessage(Res);
    }

    /// <summary>
    /// Замена пользователя в справочниках
    /// </summary>
    public virtual void RemoveUserToReferences()
    {
      var dial = Dialogs.CreateInputDialog("Укажите данные для работы скрипта");
      var empl = dial.AddSelect("Заменяемый сотрудник",true,Sungero.Company.Employees.Null);
      var zam = dial.AddSelect("Сотрудник на замену",false,Sungero.Company.Employees.Null);
      if (dial.Show() == DialogButtons.Ok)
      {
        var emplold = Sungero.Company.Employees.Get(empl.Value.Id);
        var emplnew = emplold.Department.Manager;
        if (zam != null)
          emplnew = Sungero.Company.Employees.Get(zam.Value.Id);
        
        string otch = "Начинается работа по замену пользователя: " + emplold.Name;
        otch += '\n' + "На сотрудника: " + emplnew.Name;
        otch += '\n' + "----------------------------------------------" + '\n';
        
        // Ответственный за контрагентов.
        var activeCompanies = Sungero.Parties.Companies.GetAll(c => c.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
        var counterparties = activeCompanies.Where(c => Equals(c.Responsible, emplold)).ToList();
        if (counterparties.Count > 0)
        {
          foreach (var elem in counterparties)
          {
            elem.Responsible = emplnew;
            elem.Save();
          }
        }
        //========================================================================================================
        // Подразделения.
        if (Sungero.Company.Departments.AccessRights.CanRead())
        {
          var employeeDepartments = Sungero.Company.Departments.GetAll()
            .Where(d => d.RecipientLinks.Any(e => Equals(e.Member, emplold)))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          
          if (employeeDepartments.Count > 0)
          {
            foreach (var elem in employeeDepartments)
            {
              foreach (var str in elem.RecipientLinks)
              {
                if (str.Member == emplold)
                  str.Member = emplnew;
              }
              elem.Save();
              otch += '\n' + "Замена в подразделении (Возможны дублирования сотрудника в группах).";
            }
          }
        }
        
        // НОР.
        if (Sungero.Company.Departments.AccessRights.CanRead() &&
            Sungero.Company.BusinessUnits.AccessRights.CanRead())
        {
          var businessUnits = Sungero.Company.Departments.GetAll()
            .Where(d => d.RecipientLinks.Any(e => Equals(e.Member, emplold)))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
            .Select(b => b.BusinessUnit)
            .Where(b => b.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).Distinct();
        }
        
        // Руководитель подразделений.
        if (Sungero.Company.Departments.AccessRights.CanRead())
        {
          var managerOfDepartments = Sungero.Company.Departments.GetAll().Where(d => Equals(d.Manager, emplold))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          if (managerOfDepartments.Count > 0)
          {
            foreach (var elem in managerOfDepartments)
            {
              if (elem.Manager == emplold)
                elem.Manager = emplnew;
              elem.Save();
              otch += '\n' + "Замена в руководителях подразделении.";
            }
          }
        }
        
        // Руководители НОР.
        if (Sungero.Company.BusinessUnits.AccessRights.CanRead())
        {
          var businessUnitsCEO = Sungero.Company.BusinessUnits.GetAll().Where(b => Equals(b.CEO, emplold))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          if (businessUnitsCEO.Count > 0)
          {
            foreach (var elem in businessUnitsCEO)
            {
              if (elem.CEO == emplold)
                elem.CEO = emplnew;
              elem.Save();
              otch += '\n' + "Замена в руководителях НОР.";
            }
          }
        }
        
        // Главный бухгалтер.
        if (Sungero.Company.BusinessUnits.AccessRights.CanRead())
        {
          var businessUnitsCAO = Sungero.Company.BusinessUnits.GetAll().Where(b => Equals(b.CAO, emplold))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          if (businessUnitsCAO.Count > 0)
          {
            foreach (var elem in businessUnitsCAO)
            {
              if (elem.CAO == emplold)
                elem.CAO = emplnew;
              elem.Save();
              otch += '\n' + "Замена в Глав.Бух.";
            }
          }
        }
        
        // Роли.
        if (Roles.AccessRights.CanRead())
        {
          var roles = Roles.GetAll().Where(r => r.RecipientLinks.Any(e => Equals(e.Member, emplold)))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          if (roles.Count > 0)
          {
            foreach (var elem in roles)
            {
              foreach (var str in elem.RecipientLinks)
              {
                if (str.Member == emplold)
                  str.Member = emplnew;
              }
              elem.Save();
              otch += '\n' + "Замена в ролях (Возможны дублирования сотрудника)";
            }
          }
        }
        
        // Замещения.
        if (Sungero.CoreEntities.Substitutions.AccessRights.CanRead())
        {
          var substitutions = Sungero.CoreEntities.Substitutions.GetAll()
            .Where(s => (Equals(s.Substitute.Login, emplold.Login) ||
                         Equals(s.User.Login, emplold.Login)) &&
                   (!s.EndDate.HasValue || s.EndDate >= Calendar.UserToday)).ToList();
          if (substitutions.Count > 0)
          {
            foreach (var elem in substitutions)
            {
              if (elem.Substitute.Login == emplold.Login)
                elem.Substitute = Users.GetAll(r => r.Login == emplnew.Login).FirstOrDefault();
              elem.Save();
              otch += '\n' + "Замена Замещениях.";
            }
          }
        }
        
        // Цифровые сертификаты.
        if (Certificates.AccessRights.CanRead())
        {
          var certificateResponsibility = Company.Reports.Resources.ResponsibilitiesReport.CertificateResponsibility;
          var certificates = Certificates.GetAll()
            .Where(x => Equals(x.Owner, emplold))
            .Where(d => d.Enabled.HasValue && d.Enabled.Value)
            .Where(d => !d.NotAfter.HasValue || d.NotAfter.Value > Calendar.Now).ToList();
          
          if (certificates.Count > 0)
          {
            foreach (var elem in certificates)
            {
              elem.Owner = emplnew;
              elem.Save();
              otch += '\n' + "Замена: Цифровые сертификаты.";
            }
          }
          
        }
        
        // Ответственный за абонентские ящики наших организаций.
        if (Sungero.ExchangeCore.BoxBases.AccessRights.CanRead())
        {
          var boxResponsibility = Company.Reports.Resources.ResponsibilitiesReport.BoxResponsibility;
          var boxes = Sungero.ExchangeCore.BoxBases.GetAll()
            .Where(x => Equals(x.Responsible, emplold))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          
          if (boxes.Count > 0)
          {
            foreach (var elem in boxes)
            {
              elem.Responsible = emplnew;
              elem.Save();
              otch += '\n' + "Замена: Ответственный за абонентские ящики наших организаций";
            }
          }
          
        }
        
        // Ответственный за группы регистрации.
        if (Sungero.Docflow.RegistrationGroups.AccessRights.CanRead())
        {
          var registrationGroups = Sungero.Docflow.RegistrationGroups.GetAll()
            .Where(r => r.ResponsibleEmployee.Equals(emplold))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          
          if (registrationGroups.Count > 0)
          {
            foreach (var elem in registrationGroups)
            {
              elem.ResponsibleEmployee = emplnew;
              elem.Save();
              otch += '\n' + "Замена: Ответственный за группы регистрации.";
            }
          }
        }
        
        // Участник групп регистрации.
        if (Sungero.Docflow.RegistrationGroups.AccessRights.CanRead())
        {
          var registrationGroups = Sungero.Docflow.RegistrationGroups.GetAll()
            .Where(r => r.RecipientLinks.Any(l => l.Member.Equals(emplold)))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          
          if (registrationGroups.Count > 0)
          {
            foreach (var elem in registrationGroups)
            {
              if (elem.RecipientLinks.Count > 0)
              {
                foreach (var str in elem.RecipientLinks)
                {
                  if (str.Member == emplold)
                  {
                    str.Member = emplnew;
                    elem.Save();
                    otch += '\n' + "Замена: Участник групп регистрации.";
                  }
                }
              }
            }
          }
        }
        
        // Этапы согласования.
        if (Sungero.Docflow.ApprovalStages.AccessRights.CanRead())
        {
          var approvalStages = Sungero.Docflow.ApprovalStages.GetAll()
            .Where(stage => stage.Status == Docflow.ApprovalStage.Status.Active)
            .Where(stage => Equals(stage.Assignee, emplold) ||
                   stage.Recipients.Any(r => Equals(r.Recipient, emplold))).ToList();
          
          if (approvalStages.Count > 0)
          {
            foreach (var elem in approvalStages)
            {
              elem.Assignee = emplnew;
              elem.Save();
              otch += '\n' + "Замена: Этапы согласования.";
            }
          }
        }
        
        
        // Право подписи.
        if (Sungero.Docflow.SignatureSettings.AccessRights.CanRead())
        {
          var signatureSettings = Sungero.Docflow.SignatureSettings.GetAll()
            .Where(r => r.Recipient.Equals(emplold))
            .Where(r => r.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
            .Where(r => r.DocumentFlow != null)
            .Where(r => !r.ValidTill.HasValue || r.ValidTill.Value >= Calendar.UserToday).ToList();
          
          if (signatureSettings.Count > 0)
          {
            foreach (var elem in signatureSettings)
            {
              elem.Recipient = emplnew;
              elem.Save();
              otch += '\n' + "Замена: Право подписи.";
            }
          }
        }
        
        // Правила назначения прав.
        if (Sungero.Docflow.AccessRightsRules.AccessRights.CanRead())
        {
          var accessRightsRules = Sungero.Docflow.AccessRightsRules.GetAll()
            .Where(r => r.Members.Any(l => l.Recipient.Equals(emplold)))
            .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
          
          if (accessRightsRules.Count > 0)
          {
            foreach (var elem in accessRightsRules)
            {
              if (elem.Members.Count > 0)
              {
                foreach (var str in elem.Members)
                {
                  if (str.Recipient == emplold)
                  {
                    str.Recipient = emplnew;
                    elem.Save();
                    otch += '\n' + "Замена: Правила назначения прав.";
                  }
                }
              }
            }
          }
        }

        //========================================================================================================
        
        
        Dialogs.ShowMessage(otch);
        
      }
    }

    /// <summary>
    /// Пересохранение Сотрудников
    /// </summary>
    public virtual void ReSaveEmpl()
    {
      var EmplList = Sungero.Company.Employees.GetAll().ToList();
      string log = "";
      foreach (var empl in EmplList)
      {
        try
        {
          var us = empl.Login;
          if (us != null)
          {
            us.LoginName = us.LoginName + "&";
            us.LoginName = us.LoginName.Replace("&","");
            us.Save();
          }
          empl.Save();
        }
        catch (Exception e)
        {
          log += e.Message.ToString() + '\n';
        }
      }
      if (log != "")
        Dialogs.ShowMessage(log);
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void SearchDocDiadoc()
    {
      var Exc = sberdev.SBContracts.ExchangeDocuments.GetAll(d => ((d.Created > Calendar.AddWorkingDays(Calendar.Today, -10)) && (d.LastVersionApproved.Value))).ToList();
      string Res = "";
      foreach (var elem in Exc)
      {
        string cou = elem.Counterparty != null ? elem.Counterparty.Name.ToString() : "";
        string num = elem.RegistrationNumber != null ? elem.RegistrationNumber.ToString() : "";
        string dat = elem.RegistrationDate != null ? elem.RegistrationDate.ToString() : "";
        string ddat = elem.DocumentDate != null ? elem.DocumentDate.ToString() : "";
        Res += cou + ";" + elem.Name.ToString() + ";" + num + ";" + dat + ";" + ddat + '\n';
      }
      string path = @"C:\temp\" + Calendar.Now.ToString().Replace(".","").Replace(":","").ToString() + ".csv";
      
      Encoding encoding = Encoding.GetEncoding("windows-1251");
      byte[] encodedBytes = encoding.GetBytes(Res);
      string ansiString = encoding.GetString(encodedBytes);
      
      File.AppendAllText(path,ansiString);
      var Doc = Custom.FacelessTochets.Create();
      
      Doc.Name = "Diadoc list - " + Calendar.Now.ToString();
      Doc.Author = Users.Current;
      Doc.Save();
      Doc.CreateVersionFrom(path);
      Doc.Save();
      //File.Delete(path);
      Doc.Show();
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ParseCsv()
    {
      var dialog = Dialogs.CreateInputDialog("Выберите csv-файл для обработки");
      var filePath = dialog.AddFileSelect(@"Выберите файл в папке C:\temp\",true);
      var soglashenie = dialog.AddBoolean("Шаблон файла соответствует требованиям");
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        if (soglashenie.Value != true)
          Dialogs.ShowMessage("Шаблон csv должен содержать колонки: ID, ИНН, КПП, Признак 0/1 на автозаполнение без заголовка!");
        else
        {
          try
          {
            string PathFile = filePath.Value.Name;
            string log = "";
            using (var sr = new StreamReader(@"C:\temp\" + PathFile))
            {
              while (!sr.EndOfStream)
              {
                var line = sr.ReadLine();
                var values = line.Split(';'); // Используем разделитель ";"
                
                // Если количество столбцов не соответствует ожидаемому, пропускаем строку
                if (values.Length != 4)
                {
                  log+= "Ошибка: количество столбцов не соответствует ожидаемому.";
                  continue;
                }
                else
                {
                  // Извлекаем значения столбцов
                  var id = values[0];
                  var inn = values[1];
                  var kpp = values[2];
                  var number = values[3];
                  try
                  {
                    var KA = sberdev.SBContracts.Companies.GetAll(k => k.Id == int.Parse(id)).FirstOrDefault();
                    if (KA != null)
                    {
                      KA.TIN = inn;
                      if (kpp != null)
                        KA.TRRC = kpp;
                      
                      KA.Save();
                      log+= "Успешно обновлен: " + KA.Name.ToString() + '\n';
                    }
                    else
                      log+= "Не найден КА с ИД: " + id + '\n';
                  }
                  catch (Exception err)
                  {
                    log+= "Возникла проблема: " + err.Message.ToString() + '\n';
                  }
                }
              }
              Dialogs.ShowMessage("Готово. Logtext:" + '\n' + log);
            }
          }
          catch (FileNotFoundException)
          {
            Dialogs.ShowMessage("Файл не найден.");
          }
          catch (Exception e)
          {
            Dialogs.ShowMessage("Ошибка при разборе файла: " + e.Message);
          }
        }
      }
    }

    /// <summary>
    /// вывод сообщения для пользователя (информационный диалог)
    /// </summary>
    [Public]
    public void MSG(string msg)
    {
      Dialogs.ShowMessage(msg);
    }

    /// <summary>
    /// Функция пересохранения всех записей справочника Контрагентов
    /// </summary>
    public virtual void ReSaveKA()
    {
      var lst = sberdev.SBContracts.Companies.GetAll(k => k.Status == sberdev.SBContracts.Company.Status.Active);
      int oks = 0;
      int def = 0;
      string log = "";
      foreach (var elem in lst)
      {
        try
        {
          elem.Name = elem.Name;
          elem.Save();
          oks += 1;
        }
        catch (Exception e)
        {
          def += 1;
          log += elem.Id.ToString() + ": " + e.Message.ToString() + '\n';
        }
      }
      Dialogs.ShowMessage("Успешно отработано: " + oks.ToString() + '\n' + "Завершились ошиибкой: " + def.ToString() + '\n' + "ЛОГ: " + log);
    }
  }
}