using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.WidgetCache;
using Newtonsoft.Json;

namespace sberdev.SberContracts.Server
{
  partial class WidgetCacheFunctions
  {
    /// <summary>
    /// Определить, нужно ли обновлять кэш для указанных параметров.
    /// </summary>
    [Public]
    public static bool NeedUpdateCache(string widgetName,
                                       string documentType,
                                       string analysisPeriod,
                                       SBContracts.Structures.Module.IDateRange dateRange,
                                       string additionalParam)
    {
      // Всегда возвращаем true для обновления кэша (один раз в день утром)
      return true;
    }
    
    /// <summary>
    /// Сохранить данные в кэш.
    /// </summary>
    [Public]
    public static void SaveCacheData(string widgetName,
                                     string documentType,
                                     string analysisPeriod,
                                     SBContracts.Structures.Module.IDateRange dateRange,
                                     string additionalParam,
                                     string data)
    {
      try
      {
        if (dateRange == null || string.IsNullOrEmpty(data))
          return;
        
        Sungero.Core
          .Transactions.Execute(() =>
                                {
                                  // Ищем существующую запись кеша
                                  var cacheRecord = WidgetCaches.GetAll()
                                    .Where(c => c.WidgetName == widgetName &&
                                           c.DocumentType == (documentType ?? "All") &&
                                           c.AnalysisPeriod == analysisPeriod &&
                                           c.StartDate == dateRange.StartDate &&
                                           c.EndDate == dateRange.EndDate &&
                                           c.AdditionalParam == (additionalParam ?? string.Empty))
                                    .FirstOrDefault();
                                  
                                  // Если записи нет, создаем новую
                                  if (cacheRecord == null)
                                  {
                                    cacheRecord = WidgetCaches.Create();
                                    cacheRecord.Name = string.Format("{0}_{1}_{2}_{3:yyyyMMdd}_{4:yyyyMMdd}_{5}",
                                                                     widgetName,
                                                                     documentType ?? "All",
                                                                     analysisPeriod,
                                                                     dateRange.StartDate,
                                                                     dateRange.EndDate,
                                                                     additionalParam ?? string.Empty);
                                    cacheRecord.WidgetName = widgetName;
                                    cacheRecord.DocumentType = documentType ?? "All";
                                    cacheRecord.AnalysisPeriod = analysisPeriod;
                                    cacheRecord.StartDate = dateRange.StartDate;
                                    cacheRecord.EndDate = dateRange.EndDate;
                                    cacheRecord.AdditionalParam = additionalParam ?? string.Empty;
                                  }
                                  
                                  // Обновляем данные
                                  cacheRecord.CachedData = data;
                                  cacheRecord.LastUpdated = Calendar.Now;
                                  
                                  // Устанавливаем срок актуальности кеша на конец текущего дня
                                  var today = Calendar.Today;
                                  cacheRecord.ActualityDate = today.AddDays(1).AddSeconds(-1);
                                  
                                  cacheRecord.Save();
                                }
                               );
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в SaveCacheData", ex);
      }
    }
    
    /// <summary>
    /// Удалить устаревшие записи кеша виджетов
    /// </summary>
    [Public]
    public static void CleanupOldCacheRecords()
    {
      try
      {
        // Хранить историю кеша 30 дней
        var oldDate = Calendar.Now.AddDays(-30);
        
        // Используем пакетное удаление для оптимизации
        Sungero.Core
          .Transactions.Execute(() =>
                                {
                                  // Получаем устаревшие записи кеша (ограничиваем выборку для уменьшения нагрузки)
                                  var oldRecords = WidgetCaches.GetAll()
                                    .Where(c => c.LastUpdated < oldDate)
                                    .OrderBy(c => c.LastUpdated)
                                    .Take(500)
                                    .ToList();
                                  
                                  if (oldRecords.Any())
                                  {
                                    foreach (var record in oldRecords)
                                    {
                                      try
                                      {
                                        SberContracts.WidgetCaches.Delete(record);
                                      }
                                      catch (Exception ex)
                                      {
                                        Logger.Error($"Ошибка при удалении устаревшей записи кеша {record.Id}", ex);
                                      }
                                    }
                                    
                                    Logger.Debug($"WidgetCacheUpdaterJob: Удалено {oldRecords.Count} устаревших записей кеша");
                                  }
                                }
                               );
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в CleanupOldCacheRecords", ex);
      }
    }
    
    /// <summary>
    /// Получить данные из кеша для виджета TaskFlow.
    /// </summary>
    [Public]
    public static System.Collections.Generic.Dictionary<string, int> GetTaskFlowCacheData(SBContracts.Structures.Module.IDateRange dateRange,
                                                                                          string documentType,
                                                                                          string analysisPeriod)
    {
      if (dateRange == null)
        return null;
      
      try
      {
        // Получаем запись кеша
        var cacheRecord = WidgetCaches.GetAll()
          .Where(c => c.WidgetName == "TaskFlow" &&
                 c.DocumentType == (documentType ?? "All") &&
                 c.AnalysisPeriod == analysisPeriod &&
                 c.StartDate == dateRange.StartDate &&
                 c.EndDate == dateRange.EndDate &&
                 c.AdditionalParam == string.Empty &&
                 c.ActualityDate >= Calendar.Now)
          .FirstOrDefault();
        
        if (cacheRecord != null && !string.IsNullOrEmpty(cacheRecord.CachedData))
        {
          return JsonConvert.DeserializeObject<Dictionary<string, int>>(cacheRecord.CachedData);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в GetTaskFlowCacheData", ex);
      }
      
      return null;
    }
    
    /// <summary>
    /// Получить данные из кеша для виджета AssignAvgApprTime.
    /// </summary>
    [Public]
    public static System.Collections.Generic.Dictionary<string, double> GetAssignAvgApprTimeCacheData(SBContracts.Structures.Module.IDateRange dateRange,
                                                                                                      string documentType,
                                                                                                      string analysisPeriod)
    {
      if (dateRange == null)
        return null;
      
      try
      {
        // Получаем запись кеша
        var cacheRecord = WidgetCaches.GetAll()
          .Where(c => c.WidgetName == "AssignAvgApprTime" &&
                 c.DocumentType == (documentType ?? "All") &&
                 c.AnalysisPeriod == analysisPeriod &&
                 c.StartDate == dateRange.StartDate &&
                 c.EndDate == dateRange.EndDate &&
                 c.AdditionalParam == string.Empty &&
                 c.ActualityDate >= Calendar.Now)
          .FirstOrDefault();
        
        if (cacheRecord != null && !string.IsNullOrEmpty(cacheRecord.CachedData))
        {
          return JsonConvert.DeserializeObject<Dictionary<string, double>>(cacheRecord.CachedData);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в GetAssignAvgApprTimeCacheData", ex);
      }
      
      return null;
    }
    
    /// <summary>
    /// Получить данные из кеша для виджета TaskDeadline.
    /// </summary>
    [Public]
    public static double GetTaskDeadlineCacheData(SBContracts.Structures.Module.IDateRange dateRange,
                                                  string serialType,
                                                  string documentType,
                                                  string analysisPeriod)
    {
      if (dateRange == null)
        return 0;
      
      try
      {
        // Получаем запись кеша
        var cacheRecord = WidgetCaches.GetAll()
          .Where(c => c.WidgetName == "TaskDeadline" &&
                 c.DocumentType == (documentType ?? "All") &&
                 c.AnalysisPeriod == analysisPeriod &&
                 c.StartDate == dateRange.StartDate &&
                 c.EndDate == dateRange.EndDate &&
                 c.AdditionalParam == (serialType ?? string.Empty) &&
                 c.ActualityDate >= Calendar.Now)
          .FirstOrDefault();
        
        if (cacheRecord != null && !string.IsNullOrEmpty(cacheRecord.CachedData))
        {
          return JsonConvert.DeserializeObject<double>(cacheRecord.CachedData);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в GetTaskDeadlineCacheData", ex);
      }
      
      return 0;
    }
    
    /// <summary>
    /// Получить данные из кеша для виджета AssignCompleted.
    /// </summary>
    [Public]
    public static System.Collections.Generic.Dictionary<string, SBContracts.Structures.Module.IAssignApprSeriesInfo> GetAssignCompletedCacheData(SBContracts.Structures.Module.IDateRange dateRange,
                                                                                                                                                 string documentType,
                                                                                                                                                 string analysisPeriod)
    {
      if (dateRange == null)
        return null;
      
      try
      {
        // Получаем запись кеша
        var cacheRecord = WidgetCaches.GetAll()
          .Where(c => c.WidgetName == "AssignCompleted" &&
                 c.DocumentType == (documentType ?? "All") &&
                 c.AnalysisPeriod == analysisPeriod &&
                 c.StartDate == dateRange.StartDate &&
                 c.EndDate == dateRange.EndDate &&
                 c.AdditionalParam == string.Empty &&
                 c.ActualityDate >= Calendar.Now)
          .FirstOrDefault();
        
        if (cacheRecord != null && !string.IsNullOrEmpty(cacheRecord.CachedData))
        {
          // Используем вспомогательный класс для десериализации
          // Предполагая, что существует конкретная реализация AssignApprSeriesInfoDto
          var data = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, SBContracts.Structures.Module.AssignApprSeriesInfoDto>>(cacheRecord.CachedData);
          var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(cacheRecord.CachedData);
          var result = new System.Collections.Generic.Dictionary<string, SBContracts.Structures.Module.IAssignApprSeriesInfo>();

          foreach (var property in jsonObject.Properties())
          {
            var completed = Convert.ToInt32(property.Value["Completed"]);
            var expired = Convert.ToInt32(property.Value["Expired"]);
            
            result[property.Name] = SBContracts.Structures.Module.AssignApprSeriesInfo.Create(completed, expired);
          }

          return result;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в GetAssignCompletedCacheData", ex);
      }
      
      return null;
    }
  }
}