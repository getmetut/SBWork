using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CheckDocumentSignAssignment;

namespace sberdev.SberContracts
{
  partial class CheckDocumentSignAssignmentSharedHandlers
  {

    public override void DeadlineChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      var task = SberContracts.CheckDocumentSignTasks.As(_obj.Task);
      if (task?.Deadline != _obj.Deadline)
      {
        SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(task);
        task.Deadline = _obj.Deadline;
        task.Save();
      }
    }

  }
}