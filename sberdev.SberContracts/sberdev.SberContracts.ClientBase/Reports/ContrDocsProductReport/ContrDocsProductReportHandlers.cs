using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts
{
  partial class ContrDocsProductReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      if (!ContrDocsProductReport.StartDate.HasValue && !ContrDocsProductReport.EndDate.HasValue)
      {
        // Создание диалогового окна для запроса значений параметров
        // beginDate, endDate
        var dialog = Dialogs.CreateInputDialog("Настройки отчета");
        var startDate = dialog.AddDate("От", true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate("До", true, Calendar.UserNow);
     //   var type = dialog.AddSelect(SberContracts.Reports.Resources.ContrDocsProductReport.PaidInvoiceReportDialog_Type, true, SberContracts.Reports.Resources.ContrDocsProductReport.PaidInvoiceReportDialog_Expendable)
      //    .From(new string[]{SberContracts.Reports.Resources.ContrDocsProductReport.PaidInvoiceReportDialog_Expendable, SberContracts.Reports.Resources.ContrDocsProductReport.PaidInvoiceReportDialog_Profitable});
        if (dialog.Show() == DialogButtons.Ok)
        {
          // Передача введенных значений в параметры beginDate, endDate
          ContrDocsProductReport.StartDate = startDate.Value.Value;
          ContrDocsProductReport.EndDate = endDate.Value.Value;
      /*    if (type.Value == SberContracts.Reports.Resources.ContrDocsProductReport.PaidInvoiceReportDialog_Expendable)
            ContrDocsProductReport.Type = "Expendable";
          else
            ContrDocsProductReport.Type = "Profitable";*/
        }
        else
          e.Cancel = true;
      }
    }

  }
}