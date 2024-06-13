using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void DeleteContractById(sberdev.SberContracts.Server.AsyncHandlerInvokeArgs.DeleteContractByIdInvokeArgs args)
    {int contractId = args.ContrId;
      try
      {sberdev.SBContracts.Contracts.Delete(sberdev.SBContracts.Contracts.Get(contractId));}
      catch (Exception e)
      {Logger.Error(e.Message + contractId);
      }
      args.Retry = false;
    }

    public virtual void AddStageToTask(sberdev.SberContracts.Server.AsyncHandlerInvokeArgs.AddStageToTaskInvokeArgs args)
    {
      
      int taskid = args.taskId;
      Logger.DebugFormat("Добавление стадии в задачу {0}", taskid);
      try
      {
        var task = sberdev.SBContracts.ApprovalTasks.Get(taskid);
        
        var stage = task.ApprovalRule.Stages
          //.Where(s => s.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Execution)
          .FirstOrDefault(s => s.Number == task.StageNumber);
        if (stage != null)
        {
          var OurStage = sberdev.SBContracts.ApprovalStages.As(stage.Stage);
          
          if (OurStage != null)
          {
            if (OurStage.OneTime != null)
            {
              if (OurStage.OneTime == true)
              {

                var x = task.DoneStage.AddNew();
                x.Stage = OurStage;
                task.Save();
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
        args.Retry = true;
      }
      
    }

  }
}