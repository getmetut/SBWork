using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts
{
  partial class CompleteAssignsReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      if (!CompleteAssignsReport.StartDate.HasValue && !CompleteAssignsReport.EndDate.HasValue)
      {
        var dialog = Dialogs.CreateInputDialog("Настройки отчета");
        var startDate = dialog.AddDate("От", true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate("До", true, Calendar.UserNow);
        var recip = dialog.AddSelectMany("Пользователи", true, Sungero.Company.Employees.Null);
        if (dialog.Show() == DialogButtons.Ok)
        {
          CompleteAssignsReport.StartDate = startDate.Value;
          CompleteAssignsReport.EndDate = endDate.Value;
        }
        else
          e.Cancel = true;
      }
    }

  }
}