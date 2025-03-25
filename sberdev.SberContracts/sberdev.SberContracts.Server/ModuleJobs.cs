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

    public virtual void CreateApprovalAnalyticsWidgetsCashes()
    {
      var startTime = Calendar.Now;
      
      try
      {
        Logger.Debug("WidgetCacheUpdaterJob: Начало обновления кеша виджетов");
        
        // Удаление устаревших записей кеша
        PublicFunctions.WidgetCache.CleanupOldCacheRecords();
        
        // Параметры для всех типов кешей
        var now = Calendar.Now;
        var documentTypes = new List<string> { "All", "Contractual", "IncInvoce", "Accounting", "AbstractContr", "Another" };
        var analysisPeriods = new List<string> { "Week", "Month", "Quarter" };
        var serialTypes = new List<string> { "average", "maximum", "minimum", "target" };
        
        // Обрабатываем один тип документа за раз, чтобы не перегружать систему
        foreach (var documentType in documentTypes)
        {
          foreach (var analysisPeriod in analysisPeriods)
          {
            try
            {
              // Получаем диапазоны дат для периода анализа
              var dateRanges = PublicFunctions.Module.GenerateCompletedDateRanges(now, 6, analysisPeriod);
              
              if (dateRanges == null || !dateRanges.Any())
                continue;
              
              Logger.Debug($"WidgetCacheUpdaterJob: Обработка documentType={documentType}, analysisPeriod={analysisPeriod}");
              
              // Обновляем кеш для TaskFlowWidget
              foreach (var dateRange in dateRanges)
              {
                // Всегда обновляем кеш (раз в день)
                // Оптимизированный расчет данных для кеша
                var values = Functions.Module.OptimizedCalculateTaskFlowValues(dateRange, documentType);
                
                // Сохраняем в кеш
                PublicFunctions.WidgetCache.SaveCacheData("TaskFlow", documentType, analysisPeriod, dateRange,
                                                                              string.Empty, JsonConvert.SerializeObject(values));
              }
              
              // Обновляем кеш для AssignAvgApprTimeByDepartWidget (только текущий период)
              var currentRange = dateRanges.FirstOrDefault();
              if (currentRange != null)
              {
                var avgTimeValues = Functions.Module.OptimizedCalculateAssignAvgApprTimeValues(currentRange, documentType);
                PublicFunctions.WidgetCache.SaveCacheData("AssignAvgApprTime", documentType, analysisPeriod, currentRange,
                                                                              string.Empty, JsonConvert.SerializeObject(avgTimeValues));
              }
              
              // Обновляем кеш для TaskDeadlineWidget
              foreach (var dateRange in dateRanges.Where(dr => dr.EndDate <= Calendar.Today))
              {
                foreach (var serialType in serialTypes)
                {
                  double value = Functions.Module.OptimizedCalculateTaskDeadlineChartPoint(dateRange, serialType, documentType);
                  PublicFunctions.WidgetCache.SaveCacheData("TaskDeadline", documentType, analysisPeriod, dateRange,
                                                                                serialType, JsonConvert.SerializeObject(value));
                }
              }
              
              // Обновляем кеш для AssignCompletedByDepartWidget (только текущий период)
              if (currentRange != null)
              {
                var completedValues = Functions.Module.OptimizedCalculateAssignCompletedValues(currentRange, documentType);
                PublicFunctions.WidgetCache.SaveCacheData("AssignCompleted", documentType, analysisPeriod, currentRange,
                                                                              string.Empty, JsonConvert.SerializeObject(completedValues));
              }
            }
            catch (Exception ex)
            {
              Logger.Error($"WidgetCacheUpdaterJob: Ошибка при обработке documentType={documentType}, analysisPeriod={analysisPeriod}", ex);
              // Продолжаем с другими параметрами
            }
          }
        }
        
        Logger.Debug($"WidgetCacheUpdaterJob: Обновление кеша виджетов успешно завершено. Время выполнения: {(Calendar.Now - startTime).TotalMinutes:F1} минут");
      }
      catch (Exception ex)
      {
        Logger.Error("WidgetCacheUpdaterJob: Критическая ошибка при обновлении кеша виджетов", ex);
      }
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