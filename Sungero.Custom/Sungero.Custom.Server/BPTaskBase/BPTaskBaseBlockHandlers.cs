using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using Sungero.Custom.BPTaskBase;

namespace Sungero.Custom.Server.BPTaskBaseBlocks
{
  partial class StandartBlockHandlers
  {

    public virtual void StandartBlockStart()
    {
      if (_block.DelSogl.HasValue)
      {
        var listempl = new List<Sungero.Company.IEmployee>();      
        if (_obj.SoglComplete.Count > 0)
        {
          foreach (var elem in _obj.SoglComplete)
          {
            listempl.Add(elem.Sogl);
          }
        }
        
        if (_block.Performers.Count > 0)
        {
          foreach (var str in _block.Performers)
          {
            if (ContrEmplList(listempl, Sungero.Company.Employees.Get(str.Id)))
              _block.Performers.Remove(str);
          }
        }
      }
    }
    #region Функция сравнения сотрудников
    
    private bool ContrEmplList(List<Sungero.Company.IEmployee> ls, Sungero.Company.IEmployee emp)
    {
      bool tech = true;
      if (ls.Count > 0)
      {
        foreach (var e1 in ls)
        {
          if (e1.Login == emp.Login)
            tech = false;
        }
      }
      return tech;
    }
    #endregion
  }

  partial class IniciatorHandlers
  {

    public virtual void IniciatorStart()
    {
      _block.Performers.Add(Sungero.Company.Employees.GetAll(r => r.Login == _obj.Author.Login).FirstOrDefault());
    }
  }

  partial class MonitoringBlockHandlers
  {

        public virtual bool MonitoringBlockResult()
    {
      var Subtask = Sungero.Docflow.ApprovalTasks.Get(_block.SubTask.Id);
      //_obj.ActiveText += '\n' + Calendar.Now.ToString() + ": " + Subtask.Status.Value.ToString();
      if (Subtask.Status != Sungero.Docflow.ApprovalTask.Status.InProcess)
        return true;
      else
        return false;
    }

    public virtual void MonitoringBlockStart()
    {
      var task = Sungero.Docflow.ApprovalTasks.CreateAsSubtask(_obj);        
      if (_obj.OsnDoc.OfficialDocuments.FirstOrDefault() != null)
      {
        task.DocumentGroup.OfficialDocuments.Add(_obj.OsnDoc.OfficialDocuments.FirstOrDefault());
      }  
      
      if (_obj.OsnDoc.ElectronicDocuments != null)
        task.Attachments.Add(_obj.OsnDoc.ElectronicDocuments.FirstOrDefault());
      
      if (_obj.OtherAttachment.All.Count > 0)
      {
        foreach (var doc in _obj.OtherAttachment.ElectronicDocuments)
        {
          task.Attachments.Add(doc);
        }        
      }
      task.Author = _obj.Author;      
      task.ApprovalRule = _block.ApprovalRule;     
      task.Save();
      task.Start();
	    _block.SubTask = task;
	    _obj.Save();
    }
  }

  partial class GenlistSoglHandlers
  {

    public virtual void GenlistSoglExecute()
    {
      var document = Sungero.Docflow.SimpleDocuments.Create();
      document.Subject = "Лист согласования по " + _obj.Subject.ToString();
      document.DocumentKind = Sungero.Docflow.DocumentKinds.Get(1);
      var Autr = Sungero.Company.Employees.GetAll().Where(r => r.Login == _obj.Author.Login).FirstOrDefault();
      document.PreparedBy = Autr;
      document.BusinessUnit  = Autr.Department.BusinessUnit;
      document.Department = Autr.Department;      
      document.Save();
      var Rep = Reports.GetListSogl();      
      if (TasksRefers.GetAll().Where(t => t.IDTask == _obj.Id).FirstOrDefault() != null)
        Rep.Entity = TasksRefers.GetAll().Where(t => t.IDTask == _obj.Id).FirstOrDefault();
      else
        Rep.Entity = TasksRefers.Get(_obj.Id);
      Rep.ExportTo(document);
      document.Save();
      document.Name = "Лист согласования по " + _obj.Subject.ToString();
      document.Save();
      _obj.OtherAttachment.ElectronicDocuments.Add(document);
      _obj.Save();
    }
  }

}