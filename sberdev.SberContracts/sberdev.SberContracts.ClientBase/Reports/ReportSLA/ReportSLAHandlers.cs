using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts
{
  partial class ReportSLAClientHandlers
  {
    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var dial = Dialogs.CreateInputDialog("Выберите документ для анализа SLA");
      var idDoc = dial.AddSelect("Укажите документ", true, Sungero.Content.ElectronicDocuments.Null);
      if (dial.Show() != DialogButtons.Ok)
        return;
      
      ReportSLA.IDDoc = int.Parse(idDoc.Value.Id.ToString());
    }
  }
}