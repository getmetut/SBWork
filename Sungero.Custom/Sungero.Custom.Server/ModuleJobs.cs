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