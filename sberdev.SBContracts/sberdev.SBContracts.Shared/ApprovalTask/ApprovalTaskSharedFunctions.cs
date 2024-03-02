using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Shared
{
  partial class ApprovalTaskFunctions
  {
    
  public override void SetVisibleProperties(Sungero.Docflow.Structures.ApprovalTask.RefreshParameters refreshParameters)
  {
    base.SetVisibleProperties(refreshParameters);
    
    if (_obj.ApprovalRule != null && Equals(_obj.ApprovalRule.Name, "Согласование входящих счетов")
         && _obj.DocumentGroup.OfficialDocuments.Any()
         && SBContracts.IncomingInvoices.As(_obj.DocumentGroup.OfficialDocuments.First()).NoNeedLeadingDocs == false)
        _obj.State.Properties.IsNeedManuallyCheckSberDev.IsVisible = true;
      else
        _obj.State.Properties.IsNeedManuallyCheckSberDev.IsVisible = false;
  }
  }
}
