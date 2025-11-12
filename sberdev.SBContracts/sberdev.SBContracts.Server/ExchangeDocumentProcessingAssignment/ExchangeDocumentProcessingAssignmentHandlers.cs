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

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.ResultIsRedistributionSberDev = false;
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
      var task = ExchangeDocumentProcessingTasks.As(_obj.Task);
      // так как невозможно переделать схему задачи, так чтобы задание создавалось по новой при ином результате кроме как "переадресовано"
      // этот флаг показывает что результат на самом деле "перераспределить"
      var resultIsRedistribution = _obj.ResultIsRedistributionSberDev == true;
      
      if (_obj.Result == Result.ReAddress && !resultIsRedistribution)
      {
        SberDev.ExchDocDistrib.PublicFunctions.Rule.Remote.CreateOrUpdate(_obj, true);
        task.OnRedistributionSberDev = false;
        task.AccessRights.Grant(_obj.Addressee, DefaultAccessRightsTypes.Change);
        task.AccessRights.Save();
        task.Save();
      }
      
      if (resultIsRedistribution)
      {
        SberDev.ExchDocDistrib.PublicFunctions.Rule.Remote.CreateOrUpdate(_obj, false);
        e.Result = sberdev.SBContracts.ExchangeDocumentProcessingAssignments.Resources.RedisributeResult;
        task.OnRedistributionSberDev = true;
        task.Save();
      }
      else
        base.BeforeComplete(e);
      
      PublicFunctions.ApprovalTask.ClearPublicComment(_obj);
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        e.Result = sberdev.SberContracts.Resources.NotApprove;
      }
    }
  }

}