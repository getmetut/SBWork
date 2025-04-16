using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Structures.Module;

namespace sberdev.SberContracts.Shared
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Возвращает список завершенных рабочих периодов до текущей даты
    /// </summary>
    [Public]
    public List<IDateRange> GenerateCompletedDateRanges(DateTime currentDate, int numberOfSeries, string interval)
    {
      var dateRanges = new List<DateRange>();
      
      for (int i = 0; i < numberOfSeries; i++)
      {
        DateTime startDate;
        DateTime endDate;
        
        switch (interval.ToLower())
        {
          case "weeks":
            // Находим последнее завершенное воскресенье
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
              endDate = currentDate.Date.AddDays(-7 - (i * 7));  // Если сегодня воскресенье, берем предыдущее воскресенье
            else
              endDate = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek - (i * 7));  // Иначе берем ближайшее предыдущее воскресенье
            
            // Начало недели - понедельник (за 6 дней до воскресенья)
            startDate = endDate.AddDays(-6);
            break;
            
          case "months":
            // Находим последний завершенный месяц
            // Вычисляем первый день текущего месяца
            var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            
            // Затем вычисляем последний день предыдущего месяца с учетом смещения i
            endDate = firstDayOfCurrentMonth.AddMonths(-(i + 1)).AddDays(-1);
            
            // Начало месяца
            startDate = new DateTime(endDate.Year, endDate.Month, 1);
            break;
            
          case "quarters":
            // Вычисляем текущий квартал
            int currentQuarter = (currentDate.Month - 1) / 3 + 1;
            
            // Вычисляем, сколько кварталов назад нам нужно
            int quartersBack = i + 1;  // +1 потому что мы всегда берем предыдущий квартал
            
            // Вычисляем целевой год и квартал
            int targetYear = currentDate.Year;
            int targetQuarter = currentQuarter - quartersBack;
            
            // Корректируем год, если нужно
            while (targetQuarter <= 0)
            {
              targetQuarter += 4;
              targetYear--;
            }
            
            // Вычисляем последний месяц квартала
            int endMonth = targetQuarter * 3;
            // Вычисляем первый месяц квартала
            int startMonth = endMonth - 2;
            
            // Конец квартала
            endDate = new DateTime(targetYear, endMonth, DateTime.DaysInMonth(targetYear, endMonth));
            // Начало квартала
            startDate = new DateTime(targetYear, startMonth, 1);
            break;
            
          default:
            throw new ArgumentException("Недопустимый интервал. Поддерживаются: 'weeks', 'months', 'quarters'.");
        }
        
        dateRanges.Add(new DateRange
                       {
                         StartDate = startDate,
                         EndDate = endDate
                       });
      }
      
      // Переворачиваем список, чтобы самый старый период был первым
      dateRanges.Reverse();
      
      return dateRanges.Cast<IDateRange>().ToList();
    }
    
    /// <summary>
    /// Функция создает список с информацией о сериях в графике ControlFlowChart виджета
    /// </summary>
    /// <returns></returns>
    [Public]
    public List<sberdev.SBContracts.Structures.Module.IControlFlowSeriesInfo> CreateControlFlowSeriesInfosList()
    {
      List<ControlFlowSeriesInfo> list = new List<ControlFlowSeriesInfo>();
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "started",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription1,
                 R = 100, G = 149, B = 237
               });
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "completed",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription2,
                 R = 46, G = 159, B = 12
               });
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "inprocess",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription3,
                 R = 255, G = 165, B = 0
               });
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "expired",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription4,
                 R = 217, G = 63, B = 60
               });
      return list.Cast<IControlFlowSeriesInfo>().ToList();
    }
    
    /// <summary>
    /// Возвращает русское название агрегации
    /// </summary>
    [Public]
    public string GetRusAggregationName(Nullable<Enumeration> aggr)
    {
      if (aggr == null)
        return null;
      string result = null;
      switch (aggr.Value.Value)
      {
        case "Devices":
          result = "Устройства";
          break;
        case "Licenses":
          result = "Лицензии";
          break;
        case "Services":
          result = "Сервисы";
          break;
      }
      return result;
    }
    /// <summary>
    /// Функция убиарет из строки символы, которые нельзя использовать в названии файла
    /// </summary>
    [Public]
    public string NormalizeFileName(string name)
    {
      char[] exept = {'/','\\',':','*','?','"','<','>','|'};
      char[] chars = name.ToCharArray();
      var list = chars.Where(c => exept.Contains(c) == false);
      string newName = "";
      foreach (var ch in list)
        newName = newName + ch.ToString();
      return newName;
    }

  }
}