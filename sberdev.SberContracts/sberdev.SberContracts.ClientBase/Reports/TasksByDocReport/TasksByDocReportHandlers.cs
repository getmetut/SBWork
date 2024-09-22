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
      if (!TasksByDocReport.StartDate.HasValue && !TasksByDocReport.EndDate.HasValue)
      {
        // Создание диалогового окна для запроса значений параметров
        // beginDate, endDate
        var dialog = Dialogs.CreateInputDialog(sberdev.SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_Title);
        var startDate = dialog.AddDate(sberdev.SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_StartDate, true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate(sberdev.SberContracts.Reports.Resources.PaidInvoiceReport.PaidInvoiceReportDialog_EndDate, true, Calendar.UserNow);
        var bu = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
        if (dialog.Show() == DialogButtons.Ok)
        {
          // Передача введенных значений в параметры beginDate, endDate
          TasksByDocReport.StartDate = startDate.Value.Value;
          TasksByDocReport.EndDate = endDate.Value.Value;
          TasksByDocReport.BusinessUnit = bu.Value.Id;
        }
        else
          e.Cancel = true;
      }
    }

  }
}