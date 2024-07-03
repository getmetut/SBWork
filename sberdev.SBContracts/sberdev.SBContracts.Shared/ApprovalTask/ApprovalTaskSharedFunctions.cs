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
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (attach != null && SBContracts.IncomingInvoices.Is(attach)
          && SBContracts.IncomingInvoices.As(attach).NoNeedLeadingDocs == false)
        _obj.State.Properties.IsNeedManuallyCheckSberDev.IsVisible = true;
      else
        _obj.State.Properties.IsNeedManuallyCheckSberDev.IsVisible = false;
    }
  }
}
