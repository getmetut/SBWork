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
        // Создание диалогового окна для запроса значений параметров
        // beginDate, endDate
        var dialog = Dialogs.CreateInputDialog("Настройки отчета");
        var startDate = dialog.AddDate("От", true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate("До", true, Calendar.UserNow);
        var recip = dialog.AddSelectMany("Пользователи", true, Sungero.Company.Employees.Null);
        if (dialog.Show() == DialogButtons.Ok)
        {
          // Передача введенных значений в параметры beginDate, endDate
          CompleteAssignsReport.StartDate = startDate.Value.Value;
          CompleteAssignsReport.EndDate = endDate.Value.Value;
        }
        else
          e.Cancel = true;
      }
    }

  }
}