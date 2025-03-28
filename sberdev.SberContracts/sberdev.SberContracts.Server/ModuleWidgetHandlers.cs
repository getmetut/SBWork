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
        
        for (int i = 0; i < 6; i++)
        {
          var serial = e.Chart.AddNewSeries(i.ToString(), $"{dateRangesList[i].StartDate.ToShortDateString()} - {dateRangesList[i].EndDate.ToShortDateString()}");
          
          // Получаем данные из кеша, если они есть
          var cachedValues = PublicFunctions.WidgetCache.GetTaskFlowCacheData(
            dateRangesList[i],
            _parameters.DocumentTypes.Value,
            _parameters.AnalysisPeriod.Value);
          
          // Если в кеше нет данных, используем оригинальный метод расчета
          var values = cachedValues;// ?? PublicFunctions.Module.CalculateTaskFlowValues(dateRangesList[i], _parameters.DocumentTypes.Value);
          
          for(int j = 0; j < 4; j++)
            serial.AddValue(valuesInfos[j].ValueId, valuesInfos[j].Label, values[valuesInfos[j].ValueId],
                            Sungero.Core.Colors.FromRgb((byte)valuesInfos[j].R, (byte)valuesInfos[j].G, (byte)valuesInfos[j].B));
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
          return;

        // Получаем данные из кеша, если они есть
        var departmentStats = PublicFunctions.WidgetCache.GetAssignAvgApprTimeCacheData(
          dateRange,
          _parameters.DocumentTypes.Value,
          _parameters.AnalysisPeriod.Value);
        
       /* // Если в кеше нет данных, используем оригинальный метод расчета
        if (departmentStats == null)
        {
          departmentStats = PublicFunctions.Module.CalculateAssignAvgApprTimeByDepartValues(
            dateRange,
            _parameters.DocumentTypes.Value);
        }*/
        
        foreach(var stat in departmentStats)
        {
          var serial = e.Chart.AddNewSeries(stat.Key, stat.Key);
          serial.AddValue(stat.Key, "", stat.Value, Sungero.Core.Colors.FromRgb(100, 149, 237));
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
        //  if (avgValue == 0)
        //    avgValue = PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "average", _parameters.DocumentTypes.Value);
          
          double minValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "minimum", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
       //   if (minValue == 0)
       //     minValue = PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "minimum", _parameters.DocumentTypes.Value);
          
          double targetValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "target", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
       //   if (targetValue == 0)
        //    targetValue = PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "target", _parameters.DocumentTypes.Value);
          
          double maxValue = PublicFunctions.WidgetCache.GetTaskDeadlineCacheData(
            range, "maximum", _parameters.DocumentTypes.Value, _parameters.AnalysisPeriod.Value);
        //  if (maxValue == 0)
        //    maxValue = PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "maximum", _parameters.DocumentTypes.Value);
          
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
        
        // Если в кеше нет данных, используем оригинальный метод расчета
    /*    if (departmentStats == null)
        {
          departmentStats = PublicFunctions.Module.CalculateAssignCompletedByDepartValues(
            dateRange,
            _parameters.DocumentTypes.Value);
        }*/
        
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