using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.IncomingInvoice;

namespace sberdev.SBContracts.Server
{
  partial class IncomingInvoiceFunctions
  {

    /// <summary>
    /// Установить валюту если есть сумма
    /// </summary>       
    public void SetCurrency()
    {
      if (SBContracts.PublicFunctions.Module.IsSystemUser() && _obj.TotalAmount != null && _obj.Currency == null)
      {
        _obj.Currency = Sungero.Commons.Currencies.Get(1);
      }
    }
    
    /// <summary>
    /// Установить способ доставки по умолчанию
    /// </summary>       
    public void SetDeliveryMethod()
    {
      var DelMeth = Sungero.Docflow.MailDeliveryMethods.GetAll(d => d.Name == "Не определено").FirstOrDefault();
      if (DelMeth != null)
        _obj.DeliveryMethod = DelMeth;
    }
    
    /// <summary>
    /// Заполнить поле "Оплатить до"
    /// </summary>       
    public void SetPaymentDueDate()
    {
      var date = _obj.Date.Value;
      if (_obj.PaymentDueDate == null)
      {
        if (date.DayOfWeek != DayOfWeek.Tuesday && date.DayOfWeek != DayOfWeek.Thursday)
          _obj.PaymentDueDate = (date.DayOfWeek > DayOfWeek.Thursday || date.DayOfWeek == DayOfWeek.Sunday) ?
            Calendar.EndOfWeek(date).AddDays(2) : date.AddDays(1);
        else
          _obj.PaymentDueDate = date;
      }   
    }
    
    public void SendNotice()
    {
      if (_obj.LocationState == "Получен через сервис обмена Диадок. Подписан обеими сторонами" && !_obj.NoticeSendBaseSberDev.GetValueOrDefault())
      {
        var subject = string.Format(Sungero.Exchange.Resources.TaskSubjectTemplate, "Подписан обеими сторонами:", Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(_obj.Name));
        var task = Sungero.Workflow.SimpleTasks.Create();
        task.Subject = subject.Length > 248 ? subject.Substring(0, 247) : subject;
        task.NeedsReview = false;
        task.Attachments.Add(_obj);
        var routeStep = task.RouteSteps.AddNew();
        routeStep.AssignmentType = Sungero.Workflow.SimpleTaskRouteSteps.AssignmentType.Notice;
        routeStep.Performer = Roles.GetAll(l => l.Sid == sberdev.SberContracts.PublicConstants.Module.NoticeBySing ).FirstOrDefault();
        routeStep.Deadline = null;
        task.Start();
        _obj.NoticeSendBaseSberDev = true;
      }
    }
  }
}