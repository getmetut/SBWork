using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Фоновый процесс рассылки уведомлений по договорам с истекающим сроком действия
    /// </summary>
    public virtual void ControlSkorContracts()
    {
       DateTime currentDate = Calendar.Today;   
        var targetDates = new List<DateTime>
        {
            currentDate.AddDays(14),
            currentDate.AddDays(7),
            currentDate.AddDays(3),
            currentDate.AddDays(1)
        };
    
        foreach (var targetDate in targetDates)
        {
          if (targetDate.DayOfWeek == DayOfWeek.Saturday)
              targetDate.AddDays(-1);
          else if (targetDate.DayOfWeek == DayOfWeek.Sunday)
              targetDate.AddDays(-2);
        }
        var Contractuals = sberdev.SBContracts.Contracts.GetAll(d => d.ValidTill.HasValue).Where(c => ((targetDates.Contains(c.ValidTill.Value)) && (c.LifeCycleState == sberdev.SBContracts.Contract.LifeCycleState.Active)));
      if (Contractuals.Count() > 0)
      {
        foreach (var cons in Contractuals)
        {
          var Empl = Sungero.Company.Employees.GetAll(r => r.Login == cons.Author.Login).FirstOrDefault();
          if (Empl != null)
          {
            var task = FreedomicTasks.Create();
            task.Employee = Empl;
            task.Subject = "Заканчивается срок действия договора: " + cons.Name;
            task.OtherAttachment.ElectronicDocuments.Add(cons);
            task.Start();
          }          
        }
      }
    }

    /// <summary>
    /// Обработка заданий для занесения их в список заданий в справочнике
    /// </summary>
    public virtual void JobsinReference()
    {
      var ActualList = DatabookJobses.GetAll();
      List<long> ListIds = new List<long>();
      foreach (var elem in ActualList)
      {
        ListIds.Add(long.Parse(elem.IDJob.Value.ToString()));
      }
      var Jobs = Sungero.Workflow.Assignments.GetAll(a => ((a.Status == Sungero.Workflow.Assignment.Status.InProcess) && (!ListIds.Contains(a.Id)))).ToList();
      foreach (var job in Jobs)
      {
        var DJ = DatabookJobses.Create();
        try
        {          
          DJ.IDJob = int.Parse(job.Id.ToString());
          DJ.Save();
        }
        catch (Exception e)
        {
          Logger.Debug("Фоновый процесс сбора заданий завершился ошибкой: " + e.Message.ToString());
        }
      }
    }

  }
}