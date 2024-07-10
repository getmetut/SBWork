using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;

namespace sberdev.SBContracts
{
  partial class ExchangeDocumentProcessingAssignmentServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      Functions.ExchangeDocumentProcessingAssignment.DistributeFormalizedDocument(_obj);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      string newNames = PublicFunctions.ExchangeDocumentProcessingAssignment.GetDocsNames(_obj);
      if (_obj.TaskDocsNamesSberDev != newNames)
        _obj.TaskDocsNamesSberDev = newNames;
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