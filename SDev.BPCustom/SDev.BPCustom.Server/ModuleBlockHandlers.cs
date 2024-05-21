using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace SDev.BPCustom.Server.BPCustomBlocks
{
  partial class ReglamentBlockHandlers
  {

    public virtual bool ReglamentBlockResult()
    {
      if (_block.SubTask != null)
      {
        var Subtask = Sungero.Workflow.Tasks.Get(_block.SubTask.Id);
        if (Subtask.Status != Sungero.Workflow.Task.Status.InProcess)
          return true;
        else
          return false;
      }
      else
        return false;
    }

    public virtual void ReglamentBlockStart()
    {
      if (_block.StandartRoute != null)
      {
        if (_block.StandartRoute.Value.Value == "Contracts")
        {
          var tsk = ContractualTasks.CreateAsSubtask(_obj);
          tsk.Author = _obj.Author;
          tsk.Save();
          foreach (var elem in _obj.Attachments.ToList())
          {
            if (Sungero.Content.ElectronicDocuments.Is(elem))
              tsk.OtherAttachments.ElectronicDocuments.Add(Sungero.Content.ElectronicDocuments.Get(elem.Id));
          }
          tsk.Start();
    	    _block.SubTask = tsk;
    	    _obj.Save();
        }
                
        if (_block.StandartRoute.Value.Value == "Zakupka")
        {
          var tskz = PurchaseTasks.CreateAsSubtask(_obj);
          tskz.Author = _obj.Author;
          tskz.Save();
          foreach (var elem in _obj.Attachments.ToList())
          {
            if (Sungero.Content.ElectronicDocuments.Is(elem))
              tskz.OtherAttachments.ElectronicDocuments.Add(Sungero.Content.ElectronicDocuments.Get(elem.Id));
          }
          tskz.Start();
    	    _block.SubTask = tskz;
    	    _obj.Save();
        }
      }
    }
  }

}