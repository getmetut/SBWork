using System;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;


namespace sberdev.SberContracts.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// 
    /// </summary>
    public virtual void CreateApprovalAnalyticsWidgetsCashes()
    {
      
    }

    /// <summary>
    /// Процесс очищает временные документы
    /// </summary>
    public virtual void CleanTempDocs()
    {
      DirectoryInfo dir = new DirectoryInfo("C:\\TempDocs");
      if (dir.Exists)
      {
        foreach (var file in dir.GetFiles())
        {
          try
          {
            file.Delete();
          }
          catch
          {
            continue;
          }
        }
      }
      else
        dir.Create();
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void DownloadAllContractsAndInvoices()
    {
      var contracts = SBContracts.Contracts.GetAll();
      string contrPath = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("Путь к папке с договорами").Text;
      var pathes = Directory.GetDirectories(contrPath);
      foreach (var contract in contracts)
      {
        if (contract.HasVersions)
        {
          string path = contrPath + "\\" + contract.Id;
          if (pathes.Contains(path))
            continue;
          else
          {

            try
            {
              contract.LastVersion.Export(path + "." + contract.AssociatedApplication.Extension);
              
            }
            catch (Exception e)
            {
              Logger.Error("Произошла ошибка: \"" + e.ToString() + "\", - экспорта файла у договора с ИД: " + contract.Id.ToString());
            }
            if (contract.HasRelations)
            {
              var invoices = contract.Relations.GetRelated().Where(r => SBContracts.IncomingInvoices.As(r) != null);
              if (invoices != null)
                foreach(var invoice in invoices)
                  if (invoice.HasVersions)
              {
                try
                {
                  invoice.LastVersion.Export(path + "(" + invoice.Id.ToString() +"-ivoice)" + "." + invoice.AssociatedApplication.Extension);
                  
                }
                catch (Exception e)
                {
                  Logger.Error("Произошла ошибка: \"" + e.ToString() + "\", - экспорта файла у договора с ИД: " + invoice.Id.ToString());
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Процесс чистистит папку с логами и оставляет файлы только за последнюю неделю
    /// </summary>
    public virtual void DeletingLogs()
    {
      string logspath = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("Путь к папке с логами").Text;
      string[] pathes = Directory.GetDirectories(logspath);
      List<string> files = new List<string>();
      
      files.AddRange(Directory.GetFiles(logspath).ToList());
      
      var filtredFiles = from f in files
        let arr = f.Substring(f.LastIndexOf('\\')).Split('.')
        let dates = arr[arr.Length - 2].Length > 2 ? arr[arr.Length - 2].Split('-').Select(a => Int32.Parse(a)).ToList()
        : arr[arr.Length - 3].Split('-').Select(a => Int32.Parse(a)).ToList()
        select new
      {
        Date = new DateTime(dates[0], dates[1], dates[2]),
        Way = f
      };
      
      if (filtredFiles != null)
        foreach(var file in filtredFiles)
      {
        if (DateTime.Compare(file.Date, Calendar.Now.AddDays(-7)) < 0)
          File.Delete(file.Way);
      }
    }

    /// <summary>
    /// Процесс по раскрытию имен польователей замещающих псевдо-юзеров
    /// </summary>
    public virtual void ChangeFakePersonsNames()
    {
      var depFU = Sungero.Company.Departments.GetAll().Where(d => Equals(d.Name, "Псевдо-пользователи")).First();
      var depEx = Sungero.Company.Departments.GetAll().Where(d => Equals(d.Name, "Исключения для процесса раскрытия полных имен")).First();
      foreach(var link in depFU.RecipientLinks)
      {
        var user = Sungero.CoreEntities.Users.As(link.Member);
        var subs = Sungero.CoreEntities.Substitutions.ActiveUsersWhoSubstitute(user).Except(depEx.RecipientLinks.Select(u => Sungero.CoreEntities.Users.As(u.Member)));
        string names = " (";
        foreach (var sub in subs)
        {
          var subPerson = Sungero.Company.Employees.As(sub).Person;
          names += subPerson.LastName + " " + subPerson.FirstName.Substring(0, 1) + ", ";
        }
        names = names.TrimEnd(new char[] {',',' '}) + ")";
        var person = Sungero.Company.Employees.As(user).Person;
        string lastName = person.LastName;
        int i = lastName.IndexOf("(");
        
        if (i > 0)
        {
          string pp = lastName.Substring(0 , i - 1);
          if (pp.Length + names.Length > 232)
          {
            person.LastName = pp + names.Substring(0, 231 - pp.Length) + ")";
          }
          else
            person.LastName = pp + names;
        }
        else
        {
          if (lastName.Length + names.Length > 232)
            person.LastName = lastName + names.Substring(0, 231 - lastName.Length) + ")";
          else
            person.LastName = lastName + names;
        }
        person.Save();
      }
    }

  }
}