using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom.Shared
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создание записи справочника по задачам
    /// </summary>
    [Public]
    public void CreateTaskRef(int ID)
    {
      var Ref = Custom.TasksRefers.GetAll().Where(i => i.IDTask == ID);
      if (Ref.Count() == 0)
      {
        var Task = Sungero.Workflow.Tasks.Get(ID);
        var Refs = Custom.TasksRefers.Create();
        Refs.IDTask = ID;
        Refs.Author = Sungero.Company.Employees.GetAll().Where(empl => empl.Login == Task.Author.Login).FirstOrDefault();
        Refs.Name = Task.Subject;
        Refs.Sost = Task.Status.ToString();
        Refs.LinkObject = Sungero.Core.Hyperlinks.Get(Task.Info); 
        Refs.Save();
      }
    }
    
    /// <summary>
    /// Создание записиd в таблицу справочника по задачам
    /// </summary>
    [Public]
    public void CreateJobRef(int IDTask, Sungero.Company.IEmployee Performen, DateTime TimeResult, string Result, string Comment)
    {
      var Ref = Custom.TasksRefers.GetAll().Where(i => i.IDTask == IDTask).FirstOrDefault();
      if (Ref != null)
      {
        var Job = Ref.InfoJobs.AddNew();
        Job.Performens = Performen;
        Job.TimeResult = TimeResult; 
        Job.Result = Result;
        Job.Comment = Comment;
        Ref.Save();
      }
    }
    
    /// <summary>
    /// Очистка записей таблицы справочника по задачам
    /// </summary>
    [Public]
    public void ClearJobRef(int IDTask)
    {
      var Ref = Custom.TasksRefers.GetAll().Where(i => i.IDTask == IDTask).FirstOrDefault();
      if (Ref != null)
      {
        if (Ref.InfoJobs.Count > 0)
        {
          Ref.InfoJobs.Clear();
          Ref.Save();
        }        
      }
    }

  }
}