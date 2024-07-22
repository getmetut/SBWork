using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.ContractualTask;

namespace SDev.BPCustom.Server.ContractualTaskBlocks
{
  partial class NaPodpisanieHandlers
  {

    public virtual void NaPodpisanieStartAssignment(SDev.BPCustom.INaPodpisanie assignment)
    {
      assignment.NOR = _obj.BaseAttachments.ContractualDocuments.FirstOrDefault().BusinessUnit;
      assignment.Counterparty = _obj.BaseAttachments.ContractualDocuments.FirstOrDefault().Counterparty;
    }
  }

  partial class TypesAssignmentHandlers
  {

    public virtual void TypesAssignmentStartAssignment(SDev.BPCustom.ITypesAssignment assignment)
    {
      assignment.NOR = _obj.BaseAttachments.ContractualDocuments.FirstOrDefault().BusinessUnit;
      assignment.Counterparty = _obj.BaseAttachments.ContractualDocuments.FirstOrDefault().Counterparty;
    }
  }

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
      task.Signatory = _obj.Signer;                
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

    public virtual void DoWorksStartAssignment(SDev.BPCustom.IDoWorks assignment)
    {

    }

    public virtual void DoWorksStart()
    {
      //_block.Performers.Add(_obj.Author);
    }
  }

}