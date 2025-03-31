using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Widgets;

namespace sberdev.SberContracts.Server
{
  partial class TaskFlowWidgetWidgetHandlers
  {

    public virtual void GetTaskFlowWidgetTaskFlowChartValue(Sungero.Domain.GetWidgetBarChartValueEventArgs e)
    {
      try
      {
        e.Chart.AxisTitle = sberdev.SberContracts.Resources.ControlFlowChartAxis;
        var dateRangesList = PublicFunctions.Module.GenerateCompletedDateRanges(Sungero.Core.Calendar.Now, 6, _parameters.AnalysisPeriod.Value);
        var valuesInfos = PublicFunctions.Module.CreateControlFlowSeriesInfosList();
        
        if (dateRangesList == null || !dateRangesList.Any() || valuesInfos == null || !valuesInfos.Any())
        {
          Logger.Error("Ошибка в методе GetTaskFlowWidgetTaskFlowChartValue: отсутствуют диапазоны дат или информация о сериях");
          return;
        }
        
        for (int i = 0; i < Math.Min(6, dateRangesList.Count); i++)
        {
          var serial = e.Chart.AddNewSeries(i.ToString(), $"{dateRangesList[i].StartDate.ToShortDateString()} - {dateRangesList[i].EndDate.ToShortDateString()}");
          
          // Получаем данные из кеша, если они есть
          var cachedValues = PublicFunctions.WidgetCache.GetTaskFlowCacheData(
            dateRangesList[i],
            _parameters.DocumentTypes.Value,
            _parameters.AnalysisPeriod.Value);
          
          // Если в кеше нет данных или данные неполные, используем оригинальный метод расчета
          if (cachedValues == null || cachedValues.Count < valuesInfos.Count)
          {
            cachedValues = PublicFunctions.Module.OptimizedCalculateTaskFlowValues(
              dateRangesList[i],
              _parameters.DocumentTypes.Value);
          }
          
          // Проверяем наличие данных и добавляем значения для каждой серии
          if (cachedValues != null && cachedValues.Count > 0)
          {
            for (int j = 0; j < Math.Min(4, valuesInfos.Count); j++)
            {
              string valueId = valuesInfos[j].ValueId;
              if (cachedValues.ContainsKey(valueId))
              {
                serial.AddValue(
                  valueId,
                  valuesInfos[j].Label,
                  cachedValues[valueId],
                  Sungero.Core.Colors.FromRgb((byte)valuesInfos[j].R, (byte)valuesInfos[j].G, (byte)valuesInfos[j].B));
              }
            }
          }
        }
      }
      catch (System.Exception ex)
      {
        Logger.Error("Ошибка в методе GetTaskFlowWidgetTaskFlowChartValue", ex);
      }
    }
  }

  partial class AssignAvgApprTimeByDepartWidgetWidgetHandlers
  {

    public virtual void GetAssignAvgApprTimeByDepartWidgetAssignAvgApprTimeByDepartChartValue(Sungero.Domain.GetWidgetBarChartValueEventArgs e)
    {
      try
      {
        e.Chart.AxisTitle = "Среднее кол-во дней";

        var dateRange = PublicFunctions.Module.GenerateCompletedDateRanges(
          Sungero.Core.Calendar.Today,
          1, // Текущий период
          _parameters.AnalysisPeriod.Value).FirstOrDefault();

        if (dateRange == null)
        {
          Logger.Error("Ошибка в методе GetAssignAvgApprTimeByDepartWidgetAssignAvgApprTimeByDepartChartValue: не удалось сгенерировать диапазон дат");
          return;
        }

        // Получаем данные из кеша, если они есть
        var departmentStats = PublicFunctions.WidgetCache.GetAssignAvgApprTimeCacheData(
          dateRange,
          _parameters.DocumentTypes.Value,
          _parameters.AnalysisPeriod.Value);
        
        // Если данных в кеше нет, рассчитываем
        if (departmentStats == null || !departmentStats.Any())
        {
          departmentStats = PublicFunctions.Module.OptimizedCalculateAssignAvgApprTimeValues(
            dateRange,
            _parameters.DocumentTypes.Value);
        }
        
        // Проверяем наличие данных перед добавлением в диаграмму
        if (departmentStats != null && departmentStats.Any())
        {
          foreach (var stat in departmentStats)
          {
            if (!string.IsNullOrEmpty(stat.Key))
            {
              var serial = e.Chart.AddNewSeries(stat.Key, stat.Key);
              serial.AddValue(stat.Key, "", stat.Value, Sungero.Core.Colors.FromRgb(100, 149, 237));
            }
          }
        }
        else
        {
          Logger.Debug("GetAssignAvgApprTimeByDepartWidgetAssignAvgApprTimeByDepartChartValue: Нет данных для отображения");
        }
      }
      catch (System.Exception ex)
      {
        Logger.Error("Ошибка построения гистограммы", ex);
      }
    }
  }

  partial class TaskDeadlineWidgetWidgetHandlers
  {

    public virtual void GetTaskDeadlineWidgetTaskDeadlineChartValue(Sungero.Domain.GetWidgetPlotChartValueEventArgs e)
    {
      
      try
      {
        e.Chart.Axis.Y.Title = Resources.DeadlineChartYTitle;
        e.Chart.Axis.Y.MinValue = 0; // Фиксируем начало оси Y
        e.Chart.Axis.X.Title = sberdev.SberContracts.Resources.DeadlineChartXTitle;
        e.Chart.Axis.X.AxisType = AxisType.DateTime;
        
        var dateRanges = PublicFunctions.Module.GenerateCompletedDateRanges(
          Sungero.Core.Calendar.Today,
          6,
          _parameters.AnalysisPeriod.Value
         ).Where(dr => dr.EndDate <= Sungero.Core.Calendar.Today).ToList();
        
        if (!dateRanges.Any())
        {
          Logger.Error("Нет доступных диапазонов дат для построения графика");
          return;
        }
        
        System.Collections.Generic.Dictionary<string, Sungero.Domain.Widgets.WidgetPlotChartSeries> series =
          new System.Collections.Generic.Dictionary<string, Sungero.Domain.Widgets.WidgetPlotChartSeries>();
        
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialAvg] =
          e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialAvg, Sungero.Core.Colors.FromRgb(100, 149, 237));
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialMin] =
          e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialMin, Sungero.Core.Colors.FromRgb(46, 159, 12));
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialTarget] =
          e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialTarget, Sungero.Core.Colors.FromRgb(255, 165, 0));
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialMax] =
          e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialMax, Sungero.Core.Colors.FromRgb(217, 63, 60));
        
        foreach (var range in dateRanges)
        {
          if (range.StartDate > range.EndDate)
          {
            Logger.Error($"Некорректный диапазон дат: {range.StartDate:d} - {range.EndDate:d}");
            continue;
          }
          
          // Получаем значения из кеша для каждого типа серии или рассчитываем при отсутствии в кеше
          double avgValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "average", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
          
          double minValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "minimum", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
          
          double targetValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "target", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
          
          double maxValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "maximum", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
          
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialAvg].AddValue(range.EndDate, avgValue);
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialMin].AddValue(range.EndDate, minValue);
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialTarget].AddValue(range.EndDate, targetValue);
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialMax].AddValue(range.EndDate, maxValue);
        }
      }
      catch (System.Exception ex)
      {
        Logger.Error("Ошибка при построении графика", ex);
      }
    }
  }

  partial class AssignCompletedByDepartWidgetWidgetHandlers
  {

    public virtual void GetAssignCompletedByDepartWidgetAssignCompletedByDepartChartValue(Sungero.Domain.GetWidgetBarChartValueEventArgs e)
    {
      
      try
      {
        var dateRange = PublicFunctions.Module.GenerateCompletedDateRanges(
          Sungero.Core.Calendar.Today,
          1, // Текущий период
          _parameters.AnalysisPeriod.Value).FirstOrDefault();
        
        if (dateRange == null)
          return;
        
        // Получаем данные из кеша, если они есть
        var departmentStats = PublicFunctions.WidgetCache.GetAssignCompletedCacheData(
          dateRange,
          _parameters.DocumentTypes.Value,
          _parameters.AnalysisPeriod.Value);
        
        if (departmentStats == null || !departmentStats.Any())
          return;
        
        foreach (var kvp in departmentStats)
        {
          string department = kvp.Key;
          var stats = kvp.Value;
          
          var series = e.Chart.AddNewSeries(department, department);
          
          series.AddValue("В срок", "", stats.Completed, Sungero.Core.Colors.FromRgb(46, 159, 12));
          series.AddValue("Просрочено", "", stats.Expired, Sungero.Core.Colors.FromRgb(217, 63, 60));
        }
        
        e.Chart.AxisTitle = "Количество заданий";
      }
      catch (System.Exception ex)
      {
        Logger.Error("Ошибка в методе GetAssignCompletedByDepartWidgetAssignCompletedByDepartChartValue", ex);
      }
    }
  }


}