using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SBContracts.CounterpartyConflictProcessingTask;

namespace sberdev.SBContracts.Server
{
  partial class CounterpartyConflictProcessingTaskRouteHandlers
  {

    public override void StartBlock3(Sungero.ExchangeCore.Server.CounterpartyConflictProcessingAssignmentArguments e)
    {
      base.StartBlock3(e);
      e.Block.Performers.Clear();
      e.Block.Performers.Add(Roles.GetAll().Where(n => n.Sid == sberdev.SberContracts.PublicConstants.Module.CpSyncConflictRoleGuid).FirstOrDefault());
      
    }

  }
}