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

    public virtual void GetApprovalAnalyticsWidgetControlFlowChartValue(Sungero.Domain.GetWidgetBarChartValueEventArgs e)
    {
      
      var valuesInfos = PublicFunctions.Module.CreateControlFlowSeriesInfosList();
      var dateRangesList = PublicFunctions.Module.GenerateCompletedDateRanges(Calendar.Now, 6, _parameters.AnalysisPeriod.Value);
      List<WidgetBarChartSeries> seriesList = new List<WidgetBarChartSeries>();
      for (int i = 0; i < 6; i++)
      {
        var serial = e.Chart.AddNewSeries(i.ToString(), $"{dateRangesList[i].StartDate} - {dateRangesList[i].EndDate}");
        var values = PublicFunctions.Module.CalculateControlFlowValues(dateRangesList[i]);
        seriesList.Add(serial);
        for(int j = 0; j < 4; j++)
          serial.AddValue(valuesInfos[j].ValueId, valuesInfos[j].Label, values[valuesInfos[j].ValueId],
                          Colors.FromArgb(255, (byte)valuesInfos[j].R, (byte)valuesInfos[j].G, (byte)valuesInfos[j].B));
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