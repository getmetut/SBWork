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

    public virtual void GetApprovalAnalyticsWidgetTaskDeadlineChartValue(Sungero.Domain.GetWidgetPlotChartValueEventArgs e)
    {
      try
      {
        e.Chart.Axis.X.Title = Resources.DeadlineChartX;
        e.Chart.Axis.X.AxisType = AxisType.DateTime;
        e.Chart.Axis.Y.MinValue = 0; // Фиксируем начало оси Y

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
        
        series["Average"] = e.Chart.AddNewSeries("Average", Colors.FromRgb(100, 149, 237));
        series["Maximum"] = e.Chart.AddNewSeries("Maximum", Colors.FromRgb(217, 63, 60));
        series["Minimum"] = e.Chart.AddNewSeries("Minimum", Colors.FromRgb(46, 159, 12));
        series["Target"] = e.Chart.AddNewSeries("Target", Colors.FromRgb(255, 165, 0));
        
        foreach (var range in dateRanges)
        {
          if (range.StartDate > range.EndDate)
          {
            Logger.Error($"Некорректный диапазон дат: {range.StartDate:d} - {range.EndDate:d}");
            continue;
          }
          
          series["Average"].AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "Average"));
          series["Maximum"].AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "Maximum"));
          series["Minimum"].AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "Minimum"));
          series["Target"].AddValue(range.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(range, "Target"));
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
      //  e.
      //  var filtredQuery = query.
      return query;
    }
  }


}