using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSimpleAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalSimpleAssignmentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      _obj.DocumentIDSberDev = attach?.Id.ToString();
    }
  }

}