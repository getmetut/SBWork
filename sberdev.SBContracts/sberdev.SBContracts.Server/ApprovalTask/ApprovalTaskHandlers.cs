using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SBContracts.ApprovalTask;
using System.IO;

namespace sberdev.SBContracts
{
  partial class ApprovalTaskServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var document = _obj.DocumentGroup.OfficialDocuments.First();
      if (document != null && _obj.DeliveryMethod != document.DeliveryMethod && document.DeliveryMethod != null)
      {
        _obj.DeliveryMethod = document.DeliveryMethod;
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsNeedManuallyCheckSberDev = false;
    }

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      var doc = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      base.BeforeStart(e);
      Functions.ApprovalTask.SendDiadocSettingsTask(_obj, doc);
      _obj.NeedAbort = false;
    }

    public override void BeforeAbort(Sungero.Workflow.Server.BeforeAbortEventArgs e)
    {
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        var document = _obj.DocumentGroup.OfficialDocuments.First();
        var subject = string.Format(Sungero.Exchange.Resources.TaskSubjectTemplate, "Прекращено согласование", Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(document.Name));
        var task = SimpleTasks.CreateAsSubtask(_obj);
        task.Subject = subject ;
        task.ThreadSubject = "Прекращено согласование";
        task.ActiveText = _obj.AbortingReason;
        task.NeedsReview = false;
        task.Author = _obj.Author;
        var routeStep = task.RouteSteps.AddNew();
        routeStep.AssignmentType = Sungero.Workflow.SimpleTaskRouteSteps.AssignmentType.Notice;
        routeStep.Performer = _obj.Author;
        routeStep.Deadline = null;
        task.Start();
      }
      else
      {
        base.BeforeAbort(e);
      }
    }
  }



}