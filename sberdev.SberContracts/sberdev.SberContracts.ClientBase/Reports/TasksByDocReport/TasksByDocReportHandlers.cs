using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts
{
  partial class TasksByDocReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      if (!StartedTasksReport.StartDate.HasValue && !StartedTasksReport.EndDate.HasValue)
      {
        // Создание диалогового окна для запроса значений параметров
        // beginDate, endDate
        var dialog = Dialogs.CreateInputDialog(sberdev.SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_Title);
        var startDate = dialog.AddDate(sberdev.SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_StartDate, true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate(sberdev.SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_EndDate, true, Calendar.UserNow);
        if (dialog.Show() == DialogButtons.Ok)
        {
          // Передача введенных значений в параметры beginDate, endDate
          StartedTasksReport.StartDate = startDate.Value.Value;
          StartedTasksReport.EndDate = endDate.Value.Value;
        }
        else
          e.Cancel = true;
      }
    }

  }
}