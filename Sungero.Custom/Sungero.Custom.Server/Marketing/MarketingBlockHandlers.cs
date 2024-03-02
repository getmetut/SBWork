using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using Sungero.Custom.Marketing;

namespace Sungero.Custom.Server.MarketingBlocks
{
  partial class DroductSoglHandlers
  {

    public virtual void DroductSoglStart()
    {
      var doc = _obj.Baseattachment.MarcetingDocs.FirstOrDefault();
      if (doc.DevicesAction.Count > 0)
      {
        foreach (var elem in doc.DevicesAction)
        {
          if (elem.ProductsAndDevices.ProductUnit.Responsible != null)
            _block.Performers.Add(elem.ProductsAndDevices.ProductUnit.Responsible);
        }
      }
    }
  }

  partial class ScriptEditStageHandlers
  {

    public virtual void ScriptEditStageExecute()
    {
      if (_block != null)
      {
        var doc = _obj.Baseattachment.MarcetingDocs.FirstOrDefault();
        if (_block.Stage.Value.ToString() == "Draft")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.Draft;
        
        if (_block.Stage.Value.ToString() == "Completed")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.Completed;
        
        if (_block.Stage.Value.ToString() == "Confirmed")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.Confirmed;
        
        if (_block.Stage.Value.ToString() == "AtWork")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.AtWork;
        
        if (_block.Stage.Value.ToString() == "NewObj")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.NewObj;
        
        if (_block.Stage.Value.ToString() == "OnApproval")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.OnApproval;
        
        if (_block.Stage.Value.ToString() == "SummingResult")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.SummingResult;
        
        if (_block.Stage.Value.ToString() == "Approval")
          doc.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.Approval;
        
        doc.Save();
      }
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
      //if (_obj.OsnDoc.OfficialDocuments.FirstOrDefault() != null)
      //{
      //  task.DocumentGroup.OfficialDocuments.Add(_obj.OsnDoc.OfficialDocuments.FirstOrDefault());
      //}  
      
      if (_obj.Baseattachment.MarcetingDocs != null)
        task.Attachments.Add(_obj.Baseattachment.MarcetingDocs.FirstOrDefault());
      
      if (_obj.OtherAttachments.All.Count > 0)
      {
        foreach (var doc in _obj.OtherAttachments.ElectronicDocuments)
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

}