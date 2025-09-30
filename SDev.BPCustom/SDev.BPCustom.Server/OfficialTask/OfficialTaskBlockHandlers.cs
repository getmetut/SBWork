using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.OfficialTask;

namespace SDev.BPCustom.Server.OfficialTaskBlocks
{
  partial class ScriptTaskApprovalHandlers
  {

    public virtual void ScriptTaskApprovalExecute()
    {
      var doc = _obj.BaseAttachments.OfficialDocuments.FirstOrDefault();   
      if (doc != null)
      {
        var availableApprovalRules = Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetApprovalRules(doc).ToList();
        if (availableApprovalRules.Any())
        {
          var task = Sungero.Docflow.PublicFunctions.Module.Remote.CreateApprovalTask(doc);
          task.Start();
        }
      }
    }
  }

}