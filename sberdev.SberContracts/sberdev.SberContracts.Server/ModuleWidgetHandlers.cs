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
      e.Chart.Axis.X.Title = sberdev.SberContracts.Resources.DeadlineChartX;
      e.Chart.Axis.X.AxisType = AxisType.DateTime;
      
      var dateRangesList = PublicFunctions.Module.GenerateCompletedDateRanges(Calendar.Now, 6, _parameters.AnalysisPeriod.Value);
      
      var serialAverage = e.Chart.AddNewSeries("Average", Colors.FromRgb(100, 149, 237));
      foreach(var dateRange in dateRangesList)
        serialAverage.AddValue(dateRange.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(dateRange, "Average"));
      
      var serialMaximum = e.Chart.AddNewSeries("Maximum", Colors.FromRgb(217, 63, 60));
      foreach(var dateRange in dateRangesList)
        serialMaximum.AddValue(dateRange.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(dateRange, "Maximum"));
      
      var serialMinimum = e.Chart.AddNewSeries("Minimum", Colors.FromRgb(46, 159, 12));
      foreach(var dateRange in dateRangesList)
        serialMinimum.AddValue(dateRange.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(dateRange, "Minimum"));
      
      var seriesTarget = e.Chart.AddNewSeries("Target", Colors.FromRgb(255, 165, 0));
      foreach(var dateRange in dateRangesList)
        seriesTarget.AddValue(dateRange.EndDate, PublicFunctions.Module.CalculateTaskDeadlineChartPoint(dateRange, "Target"));
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