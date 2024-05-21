using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using Sungero.Custom.TestTask;

namespace Sungero.Custom.Server.TestTaskBlocks
{
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
      _obj.ListSogl.ElectronicDocuments.Add(document);
      _obj.Save();
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
      if (_obj.ContractOne.Contracts.FirstOrDefault() != null)
      {
        task.DocumentGroup.OfficialDocuments.Add(_obj.ContractOne.Contracts.FirstOrDefault());
        task.Signatory = _obj.ContractOne.Contracts.FirstOrDefault().OurSignatory;
      }
      else
      {
        task.DocumentGroup.OfficialDocuments.Add(_obj.ContractOne.SupAgreements.FirstOrDefault());
        task.Signatory = _obj.ContractOne.SupAgreements.FirstOrDefault().OurSignatory;
      }                  
      task.Author = _obj.Author;      
      task.ApprovalRule = _block.ApprovalRule;     
      task.Save();
      task.Start();
	    _block.SubTask = task;
	    _obj.Save();
    }
  }

}