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

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsNeedManuallyCheckSberDev = false;
    }

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      // Механика подтверждения суммы закупки
      if (document != null)
      {
        var gurantee = SberContracts.GuaranteeLetters.As(document);
        if (gurantee != null && gurantee.TotalAmount < 5000000 && gurantee.AddendumDocument == null)
        {
          if (!PublicFunctions.ApprovalTask.ShowConfirmationAmountDialog(_obj))
            e.AddError(sberdev.SBContracts.ApprovalTasks.Resources.NotConfirmedPurchAmount);
        }
        
        base.BeforeStart(e);
        Functions.ApprovalTask.SendDiadocSettingsTask(_obj, document);
        _obj.NeedAbort = false;
        
        // Механика проверки бездоговорных счетов
        var incInv = SBContracts.IncomingInvoices.As(document);
        if (incInv != null && incInv.NoNeedLeadingDocs.HasValue && incInv.NoNeedLeadingDocs.Value)
        {
          var counter = SberContracts.NonContractInvoiceCounters.GetAll().Where(c => c.Counterparty == incInv.Counterparty
                                                                                && c.Employee == incInv.Author).FirstOrDefault();
          if (counter == null)
          {
            counter = SberContracts.NonContractInvoiceCounters.Create();
            counter.Employee = incInv.Author;
            counter.Counterparty = incInv.Counterparty;
            counter.Counter = 1;
          }
          else if (!counter.Tasks.Select(t => SBContracts.ApprovalTasks.As(t.Task).DocumentGroup.OfficialDocuments.First()).Where(d => d.Id == incInv.Id).Any())
          {
            var task = counter.Tasks.AddNew();
            task.Task = _obj;
            counter.Counter++;
          }
        }
        
        // Механика переноса значения поля Способ доставки
        if (!_obj.State.Properties.DeliveryMethod.IsVisible)
        {
          return;
        }
        if (_obj.DeliveryMethod != document.DeliveryMethod && document.DeliveryMethod != null
            && document.DeliveryMethod.Sid != Sungero.Docflow.Constants.MailDeliveryMethod.Exchange)
        {
          _obj.DeliveryMethod = document.DeliveryMethod;
          _obj.ExchangeService = null;
        }
      }
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