using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Widgets;

namespace sberdev.SberContracts.Server
{
  partial class ApprovalAnalyticsWidgetWidgetHandlers
  {

    public virtual void GetApprovalAnalyticsWidgetAssignAvgApprTimeChartValue(Sungero.Domain.GetWidgetBarChartValueEventArgs e)
    {
      
    }

    public virtual void GetApprovalAnalyticsWidgetTaskDeadlineChartValue(Sungero.Domain.GetWidgetPlotChartValueEventArgs e)
    {
      try
      {
        e.Chart.Axis.Y.Title = Resources.DeadlineChartYTitle;
        e.Chart.Axis.Y.MinValue = 0; // Фиксируем начало оси Y
        e.Chart.Axis.X.Title = sberdev.SberContracts.Resources.DeadlineChartXTitle;
        e.Chart.Axis.X.AxisType = AxisType.DateTime;

        var dateRanges = PublicFunctions.Module.GenerateCompletedDateRanges(
          Calendar.Today,
          6,
          _parameters.AnalysisPeriod.Value
         ).Where(dr => dr.EndDate <= Calendar.Today).ToList();

        if (!dateRanges.Any())
        {
          Logger.Error("Нет доступных диапазонов дат для построения графика");
          return;
        }

        Dictionary<string, WidgetPlotChartSeries> series = new Dictionary<string, WidgetPlotChartSeries>();
        
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialAvg] = e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialAvg, Colors.FromRgb(100, 149, 237));
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialMin] = e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialMin, Colors.FromRgb(46, 159, 12));
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialTarget] = e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialTarget, Colors.FromRgb(255, 165, 0));
        series[sberdev.SberContracts.Resources.TaskDeadlineSerialMax] = e.Chart.AddNewSeries(sberdev.SberContracts.Resources.TaskDeadlineSerialMax, Colors.FromRgb(217, 63, 60));
        
        foreach (var range in dateRanges)
        {
          if (range.StartDate > range.EndDate)
          {
            Logger.Error($"Некорректный диапазон дат: {range.StartDate:d} - {range.EndDate:d}");
            continue;
          }
          
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialAvg]
            .AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, sberdev.SberContracts.Resources.TaskDeadlineSerialAvg));
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialMin]
            .AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, sberdev.SberContracts.Resources.TaskDeadlineSerialMin));
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialTarget]
            .AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, sberdev.SberContracts.Resources.TaskDeadlineSerialTarget));
          series[sberdev.SberContracts.Resources.TaskDeadlineSerialMax]
            .AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, sberdev.SberContracts.Resources.TaskDeadlineSerialMax));
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка при построении графика", ex);
      }
    }

    public virtual void GetApprovalAnalyticsWidgetControlFlowChartValue(Sungero.Domain.GetWidgetBarChartValueEventArgs e)
    {
      e.Chart.AxisTitle = sberdev.SberContracts.Resources.ControlFlowChartAxis;
      var dateRangesList = PublicFunctions.Module.GenerateCompletedDateRanges(Calendar.Now, 6, _parameters.AnalysisPeriod.Value);
      var valuesInfos = PublicFunctions.Module.CreateControlFlowSeriesInfosList();
      List<WidgetBarChartSeries> seriesList = new List<WidgetBarChartSeries>();
      for (int i = 0; i < 6; i++)
      {
        var serial = e.Chart.AddNewSeries(i.ToString(), $"{dateRangesList[i].StartDate.ToShortDateString()} - {dateRangesList[i].EndDate.ToShortDateString()}");
        var values = PublicFunctions.Module.CalculateControlFlowValues(dateRangesList[i]);
        seriesList.Add(serial);
        for(int j = 0; j < 4; j++)
          serial.AddValue(valuesInfos[j].ValueId, valuesInfos[j].Label, values[valuesInfos[j].ValueId],
                          Colors.FromRgb((byte)valuesInfos[j].R, (byte)valuesInfos[j].G, (byte)valuesInfos[j].B));
      }
    }

    public virtual IQueryable<Sungero.Docflow.IApprovalTask> ApprovalAnalyticsWidgetControlFlowChartFiltering(IQueryable<Sungero.Docflow.IApprovalTask> query, Sungero.Domain.WidgetBarChartFilteringEventArgs e)
    {
      //  var filtredQuery = query.
      return query;
    }
  }


}