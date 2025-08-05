using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SberContracts.CorpEndorseTask;

namespace sberdev.SberContracts.Server
{
  partial class CorpEndorseTaskRouteHandlers
  {
    // Доработка в рамках задачи DRX-868.
    public virtual void StartAssignment3(sberdev.SberContracts.ICorpEndorseAssignment assignment, sberdev.SberContracts.Server.CorpEndorseAssignmentArguments e)
    {
      assignment.Deadline = _obj.Deadline;
    }

    public virtual void StartBlock3(sberdev.SberContracts.Server.CorpEndorseAssignmentArguments e)
    {
      e.Block.Performers.Add(_obj.Performer);
    }

  }
}