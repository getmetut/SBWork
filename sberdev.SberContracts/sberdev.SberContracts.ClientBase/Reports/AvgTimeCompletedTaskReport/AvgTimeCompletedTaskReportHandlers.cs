using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts
{
  partial class AvgTimeCompletedTaskReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      if (!AvgTimeCompletedTaskReport.StartDate.HasValue && !AvgTimeCompletedTaskReport.EndDate.HasValue)
      {
        var dialog = Dialogs.CreateInputDialog("Настройки отчета");
        var startDate = dialog.AddDate("От", true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate("До", true, Calendar.UserNow);
        var bu = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
        if (dialog.Show() == DialogButtons.Ok)
        {
          AvgTimeCompletedTaskReport.StartDate = startDate.Value;
          AvgTimeCompletedTaskReport.EndDate = endDate.Value;
          AvgTimeCompletedTaskReport.BusinessUnit = bu.Value.Id;
        }
        else
          e.Cancel = true;
      }
    }

  }
}