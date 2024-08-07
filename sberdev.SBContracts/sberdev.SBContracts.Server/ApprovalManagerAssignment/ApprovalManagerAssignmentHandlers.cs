using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalManagerAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalManagerAssignmentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
  //    _obj.DocumentIDSberDev = attach?.Id.ToString();
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        e.Result = sberdev.SberContracts.Resources.NotApprove;
      }
    }
  }

}