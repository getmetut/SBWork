using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts
{
  partial class AccDocsProductReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      if (!AccDocsProductReport.StartDate.HasValue && !AccDocsProductReport.EndDate.HasValue)
      {
        // Создание диалогового окна для запроса значений параметров
        // beginDate, endDate
        var dialog = Dialogs.CreateInputDialog("Настройки отчета");
        var startDate = dialog.AddDate("От", true, Calendar.Now.AddMonths(-1));
        var endDate = dialog.AddDate("До", true, Calendar.UserNow);
     //   var type = dialog.AddSelect(SberContracts.Reports.Resources.AccDocsProductReport.PaidInvoiceReportDialog_Type, true, SberContracts.Reports.Resources.AccDocsProductReport.PaidInvoiceReportDialog_Expendable)
      //    .From(new string[]{SberContracts.Reports.Resources.AccDocsProductReport.PaidInvoiceReportDialog_Expendable, SberContracts.Reports.Resources.AccDocsProductReport.PaidInvoiceReportDialog_Profitable});
        if (dialog.Show() == DialogButtons.Ok)
        {
          // Передача введенных значений в параметры beginDate, endDate
          AccDocsProductReport.StartDate = startDate.Value.Value;
          AccDocsProductReport.EndDate = endDate.Value.Value;
      /*    if (type.Value == SberContracts.Reports.Resources.AccDocsProductReport.PaidInvoiceReportDialog_Expendable)
            AccDocsProductReport.Type = "Expendable";
          else
            AccDocsProductReport.Type = "Profitable";*/
        }
        else
          e.Cancel = true;
      }
    }

  }
}