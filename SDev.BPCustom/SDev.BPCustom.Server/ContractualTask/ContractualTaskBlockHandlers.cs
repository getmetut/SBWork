using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.ContractualTask;

namespace SDev.BPCustom.Server.ContractualTaskBlocks
{
  partial class MonitoringBlockHandlers
  {

    public virtual bool MonitoringBlockResult()
    {
      var Subtask = Sungero.Docflow.ApprovalTasks.Get(_block.SubTask.Id);
      if (Subtask.Status != Sungero.Docflow.ApprovalTask.Status.InProcess)
        return true;
      else
        return false;
    }

    public virtual void MonitoringBlockStart()
    {
      var task = Sungero.Docflow.ApprovalTasks.CreateAsSubtask(_obj);        
      task.DocumentGroup.OfficialDocuments.Add(_obj.BaseAttachments.ContractualDocuments.FirstOrDefault());
      task.Signatory = _obj.BaseAttachments.ContractualDocuments.FirstOrDefault().OurSignatory;                
      task.Author = _obj.Author;      
      task.ApprovalRule = _block.ApprovalRule;     
      task.Save();
      task.Start();
	    _block.SubTask = task;
	    _obj.Save();
    }
  }

  partial class DoWorksHandlers
  {

    public virtual void DoWorksStart()
    {
      _block.Performers.Add(_obj.Author);
    }
  }

}