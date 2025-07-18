using System;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Newtonsoft.Json;
using System.Transactions;

namespace sberdev.SberContracts.Server
{
  public class ModuleJobs
  {
    
    public virtual void FillTaskDocunetType()
    {
      // Получаем только ID задач без заполненного DocumentType
      var query = SBContracts.ApprovalTasks.GetAll()
        .Where(t => t.DocumentTypeSungero == null || t.DocumentTypeSungero == "")
        .Select(t => t.Id);
      
      var allTaskIds = query.ToList();
      
      Logger.Debug($"DocumentTypeFillerJob: Найдено задач без типа: {allTaskIds.Count}");
      
      // Будем заполнять постепенно
      int processedCount = 0;
      int updatedCount = 0;
      int errorCount = 0;
      
      // Уменьшаем размер пакета
      int batchSize = 50;
      
      for (int i = 0; i < allTaskIds.Count; i += batchSize)
      {
        var batchIds = allTaskIds.Skip(i).Take(batchSize).ToList();
        
        foreach (var taskId in batchIds)
        {
          try
          {
            bool processed = ProcessSingleTask(taskId, ref updatedCount);
            processedCount++;
            if (processedCount % 50 == 0)
              Logger.Debug($"DocumentTypeFillerJob: Обработано задач: {processedCount}, обновлено: {updatedCount}, ошибок: {errorCount}");
          }
          catch (Exception ex)
          {
            errorCount++;
            Logger.Error($"DocumentTypeFillerJob: Ошибка при обработке задачи {taskId}", ex);
          }
        }
        
        // Явно вызываем сборку мусора между пакетами
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
      
      Logger.Debug($"DocumentTypeFillerJob: Обработка завершена. Всего: {processedCount}, обновлено: {updatedCount}, ошибок: {errorCount}");
    }

    private bool ProcessSingleTask(long taskId, ref int updatedCount)
    {
      try
      {
        var task = SBContracts.ApprovalTasks.GetAll()
          .Where(t => t.Id == taskId)
          .FirstOrDefault();
        
        if (task == null)
          return false;
        
        // Проверяем наличие документов
        if (task.DocumentGroup == null || !task.DocumentGroup.OfficialDocuments.Any())
          return false;
        
        if (string.IsNullOrEmpty(task.DocumentTypeSungero))
        {
          var documentType = DetermineDocumentType(task);
          
          if (!string.IsNullOrEmpty(documentType))
          {
            // Вместо task.Save() используем прямой SQL-запрос
            var sql = $"UPDATE Sungero_WF_Task SET DocumentTypeSu_SBContr_sberdev = '{documentType}' WHERE Id = {taskId}";
            Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(sql);
            
            updatedCount++;
            return true;
          }
        }
        
        return false;
      }
      catch (Exception ex)
      {
        Logger.Error($"DocumentTypeFillerJob: Ошибка при обработке задачи {taskId}: {ex.Message}", ex);
        return false;
      }
    }

    private string DetermineDocumentType(SBContracts.IApprovalTask task)
    {
      try
      {
        if (task == null || task.DocumentGroup == null)
          return string.Empty;
        
        var documents = task.DocumentGroup.OfficialDocuments.ToList();
        if (!documents.Any())
          return string.Empty;
        
        var document = documents.FirstOrDefault();
        if (document == null)
          return string.Empty;
        
        foreach (var docType in new[] { "Contractual", "IncInvoce", "Accounting", "AbstractContr", "Another" })
        {
          try
          {
            if (PublicFunctions.Module.SafeMatchesDocumentType(document, docType))
              return docType;
          }
          catch (Exception ex)
          {
            Logger.Error($"DocumentTypeFillerJob: Ошибка при проверке типа {docType} для задачи {task.Id}", ex);
          }
        }
        
        return "Another";
      }
      catch (Exception ex)
      {
        Logger.Error($"DocumentTypeFillerJob: Ошибка определения типа документа для задачи {task.Id}", ex);
        return string.Empty;
      }
    }

    /// <summary>
    /// Фоновый процесс заполнение времени выполнения в исторических задач
    /// </summary>
    public virtual void FillTaskExecutionTimeData()
    {
      // Логируем начало миграции
      Logger.Debug("FillTaskExecutionTimeDataJob: Начало заполнения времени выполнения для задач");
      DateTime startTime = Calendar.Now;
      
      try
      {
        // Получаем все завершенные задачи без заполненного ExecutionInDaysSungeroSungero
        var tasks = SBContracts.ApprovalTasks.GetAll()
          .Where(t => t.Status == SBContracts.ApprovalTask.Status.Completed &&
                 t.Started != null &&
                 t.ExecutionInDaysSungero == null)
          .OrderByDescending(t => t.Started)
          .ToList();
        
        // Обрабатываем задачи пакетами для снижения нагрузки
        int batchSize = 1000;
        int processed = 0;
        int success = 0;
        int errors = 0;
        
        for (int i = 0; i < tasks.Count(); i += batchSize)
        {
          // Используем транзакцию для каждого пакета
          Sungero.Core
            .Transactions.Execute(() =>
                                  {
                                    var batch = tasks.Skip(i).Take(batchSize).ToList();
                                    
                                    // Предварительно загружаем все связанные задания для уменьшения числа запросов
                                    var taskIds = batch.Select(t => t.Id).ToList();
                                    var assignments = SBContracts.ApprovalAssignments.GetAll()
                                      .Where(a => taskIds.Contains(a.Task.Id) &&
                                             a.Status == Sungero.Workflow.Assignment.Status.Completed &&
                                             a.Completed != null)
                                      .ToList();
                                    
                                    // Группируем задания по задачам
                                    var taskAssignments = assignments
                                      .GroupBy(a => a.Task.Id)
                                      .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Completed).ToList());
                                    
                                    foreach (var task in batch)
                                    {
                                      try
                                      {
                                        // Если для задачи нет заданий в предзагруженном словаре, пытаемся получить напрямую
                                        List<SBContracts.IApprovalAssignment> taskAssList;
                                        if (!taskAssignments.TryGetValue(task.Id, out taskAssList) || !taskAssList.Any())
                                        {
                                          // Пропускаем задачи без заданий
                                          continue;
                                        }
                                        
                                        var lastAssignment = taskAssList.FirstOrDefault();
                                        
                                        if (lastAssignment?.Completed != null && task.Started <= lastAssignment.Completed)
                                        {
                                          int days = SBContracts.PublicFunctions.Module.CalculateBusinessDays(task.Started, lastAssignment.Completed);
                                          // Вместо task.Save() используем прямой SQL-запрос так как ничего не сохраняется
                                          var sql = $"UPDATE Sungero_WF_Task SET ExecutionInDay_SBContr_sberdev = '{days}' WHERE Id = {task.Id}";
                                          Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(sql);
                                          success++;
                                        }
                                      }
                                      catch (Exception ex)
                                      {
                                        errors++;
                                        // Ограничиваем логирование для избежания переполнения лога
                                        if (errors < 100 || errors % 100 == 0)
                                          Logger.Error($"FillTaskExecutionTimeDataJob: Ошибка обработки задачи {task.Id}", ex);
                                      }
                                    }
                                    
                                    processed += batch.Count();
                                  }
                                 );
          
          // Логируем прогресс каждую 1000 задач
          if (processed % 1000 == 0 || processed == tasks.Count)
          {
            var timeSpent = Calendar.Now - startTime;
            Logger.Debug($"FillTaskExecutionTimeDataJob: Обработано {processed} из {tasks.Count} задач. " +
                         $"Успешно: {success}, ошибок: {errors}. " +
                         $"Время выполнения: {timeSpent.TotalMinutes:F1} минут");
          }
        }
        
        // Итоговое сообщение
        var totalTime = Calendar.Now - startTime;
        Logger.Debug($"FillTaskExecutionTimeDataJob: Завершено заполнение времени выполнения. " +
                     $"Всего обработано: {processed}, успешно: {success}, ошибок: {errors}. " +
                     $"Общее время: {totalTime.TotalMinutes:F1} минут");
      }
      catch (Exception ex)
      {
        Logger.Error("Критическая ошибка в FillTaskExecutionTimeDataJob", ex);
      }
    }
    
    /// <summary>
    /// Фоновый процесс обновления кэша аналитических виджетов.
    /// </summary>
    public virtual void CreateApprovalAnalyticsWidgetsCashes()
    {
      var startTime = Calendar.Now;
      
      try
      {
        // Сокращаем логирование - оставляем только одно сообщение о начале
        Logger.Debug("WidgetCacheUpdaterJob: Начало обновления кэша виджетов");
        
        // Удаление устаревших записей кеша
        PublicFunctions.WidgetCache.CleanupOldCacheRecords();
        
        // Параметры для всех типов кешей
        var now = Calendar.Now;
        var documentTypes = new List<string> { "All", "Contractual", "IncInvoce", "Accounting", "AbstractContr", "Another" };
        var analysisPeriods = new List<string> { "weeks", "months", "quarters" };
        var serialTypes = new List<string> { "average", "maximum", "minimum", "target" };
        
        int successCount = 0;
        int errorCount = 0;
        
        // Обрабатываем один тип документа за раз
        foreach (var documentType in documentTypes)
        {
          foreach (var analysisPeriod in analysisPeriods)
          {
            try
            {
              // Получаем диапазоны дат для периода анализа
              var dateRanges = PublicFunctions.Module.GenerateCompletedDateRanges(now, 6, analysisPeriod);
              
              if (dateRanges == null || !dateRanges.Any())
              {
                Logger.Error($"WidgetCacheUpdaterJob: Не удалось сгенерировать диапазоны дат для analysisPeriod={analysisPeriod}");
                continue;
              }
              
              // Обновляем кэш для TaskFlowWidget
              UpdateTaskFlowWidgetCache(dateRanges, documentType, analysisPeriod, ref successCount, ref errorCount);
              
              // Обновляем кэш для AssignAvgApprTimeByDepartWidget (только текущий период)
              var currentRange = dateRanges.LastOrDefault();
              if (currentRange != null)
              {
                UpdateAvgApprTimeWidgetCache(currentRange, documentType, analysisPeriod, ref successCount, ref errorCount);
                UpdateAssignCompletedWidgetCache(currentRange, documentType, analysisPeriod, ref successCount, ref errorCount);
              }
              
              // Обновляем кэш для TaskDeadlineWidget
              UpdateTaskDeadlineWidgetCache(dateRanges, documentType, analysisPeriod, serialTypes, ref successCount, ref errorCount);
            }
            catch (Exception ex)
            {
              Logger.Error($"WidgetCacheUpdaterJob: Ошибка при обработке documentType={documentType}, analysisPeriod={analysisPeriod}", ex);
              errorCount++;
            }
          }
        }
        
        // Итоговое сообщение с общей статистикой
        Logger.Debug($"WidgetCacheUpdaterJob: Обновление кэша виджетов завершено. Успешно: {successCount}, ошибок: {errorCount}. Время выполнения: {(Calendar.Now - startTime).TotalMinutes:F1} минут");
      }
      catch (Exception ex)
      {
        Logger.Error("WidgetCacheUpdaterJob: Критическая ошибка при обновлении кэша виджетов", ex);
      }
    }

    /// <summary>
    /// Обновляет кэш для TaskFlowWidget
    /// </summary>
    private void UpdateTaskFlowWidgetCache(IEnumerable<SBContracts.Structures.Module.IDateRange> dateRanges,
                                           string documentType,
                                           string analysisPeriod,
                                           ref int successCount,
                                           ref int errorCount)
    {
      foreach (var dateRange in dateRanges)
      {
        try
        {
          // Проверяем корректность диапазона дат
          if (dateRange.StartDate > dateRange.EndDate)
            continue;
          
          // Оптимизированный расчет данных для кэша
          var values = Functions.Module.CalculateTaskFlowValues(dateRange, documentType);
          
          // Сохраняем в кэш
          PublicFunctions.WidgetCache.SaveCacheData("TaskFlow", documentType, analysisPeriod, dateRange,
                                                    string.Empty, JsonConvert.SerializeObject(values));
          successCount++;
        }
        catch (Exception ex)
        {
          Logger.Error($"WidgetCacheUpdaterJob: Ошибка при обновлении кэша TaskFlow для {documentType}/{analysisPeriod}", ex);
          errorCount++;
        }
      }
    }

    /// <summary>
    /// Обновляет кэш для AssignAvgApprTimeByDepartWidget
    /// </summary>
    private void UpdateAvgApprTimeWidgetCache(SBContracts.Structures.Module.IDateRange dateRange,
                                              string documentType,
                                              string analysisPeriod,
                                              ref int successCount,
                                              ref int errorCount)
    {
      try
      {
        // Проверяем корректность диапазона дат
        if (dateRange.StartDate > dateRange.EndDate)
          return;
        
        var avgTimeValues = Functions.Module.CalculateAssignAvgApprTimeByDepartValues(dateRange, documentType);
        PublicFunctions.WidgetCache.SaveCacheData("AssignAvgApprTime", documentType, analysisPeriod, dateRange,
                                                  string.Empty, JsonConvert.SerializeObject(avgTimeValues));
        successCount++;
      }
      catch (Exception ex)
      {
        Logger.Error($"WidgetCacheUpdaterJob: Ошибка при обновлении кэша AssignAvgApprTime для {documentType}/{analysisPeriod}", ex);
        errorCount++;
      }
    }

    /// <summary>
    /// Обновляет кэш для AssignCompletedByDepartWidget
    /// </summary>
    private void UpdateAssignCompletedWidgetCache(SBContracts.Structures.Module.IDateRange dateRange,
                                                  string documentType,
                                                  string analysisPeriod,
                                                  ref int successCount,
                                                  ref int errorCount)
    {
      try
      {
        // Проверяем корректность диапазона дат
        if (dateRange.StartDate > dateRange.EndDate)
          return;
        
        var completedValues = Functions.Module.CalculateAssignCompletedByDepartValues(dateRange, documentType);
        PublicFunctions.WidgetCache.SaveCacheData("AssignCompleted", documentType, analysisPeriod, dateRange,
                                                  string.Empty, JsonConvert.SerializeObject(completedValues));
        successCount++;
      }
      catch (Exception ex)
      {
        Logger.Error($"WidgetCacheUpdaterJob: Ошибка при обновлении кэша AssignCompleted для {documentType}/{analysisPeriod}", ex);
        errorCount++;
      }
    }

    /// <summary>
    /// Обновляет кэш для TaskDeadlineWidget
    /// </summary>
    private void UpdateTaskDeadlineWidgetCache(IEnumerable<SBContracts.Structures.Module.IDateRange> dateRanges,
                                               string documentType,
                                               string analysisPeriod,
                                               IEnumerable<string> serialTypes,
                                               ref int successCount,
                                               ref int errorCount)
    {
      foreach (var dateRange in dateRanges.Where(dr => dr.EndDate <= Calendar.Today))
      {
        // Проверяем корректность диапазона дат сразу для всего цикла
        if (dateRange.StartDate > dateRange.EndDate)
          continue;
        
        foreach (var serialType in serialTypes)
        {
          try
          {
            double value = Functions.Module.CalculateTaskDeadlineChartPoints(dateRange, serialType, documentType);
            PublicFunctions.WidgetCache.SaveCacheData("TaskDeadline", documentType, analysisPeriod, dateRange,
                                                      serialType, JsonConvert.SerializeObject(value));
            successCount++;
          }
          catch (Exception ex)
          {
            Logger.Error($"WidgetCacheUpdaterJob: Ошибка при обновлении кэша TaskDeadline для {documentType}/{analysisPeriod}/{serialType}", ex);
            errorCount++;
          }
        }
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