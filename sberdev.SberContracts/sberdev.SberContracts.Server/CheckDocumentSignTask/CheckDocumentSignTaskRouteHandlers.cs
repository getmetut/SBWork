using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SberContracts.CheckDocumentSignTask;

namespace sberdev.SberContracts.Server
{
  partial class CheckDocumentSignTaskRouteHandlers
  {

    public virtual void StartAssignment3(sberdev.SberContracts.ICheckDocumentSignAssignment assignment, sberdev.SberContracts.Server.CheckDocumentSignAssignmentArguments e)
    {
      var devSet = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("Настройка рассылки подзадач по контролю возврата").Text.Split(',');
      int period;
      if (!int.TryParse(devSet[1], out period))
        throw new ArgumentException("Укажите корректные значениея в текстовом параметре. Модуль Договоры -> Системные настройки -> Настройка рассылки подзадач по контролю возврата");
      var deadline = Calendar.AddWorkingDays(Calendar.Now, period);
      assignment.Deadline = deadline;
      _obj.Deadline = deadline;
    }

    public virtual void StartBlock3(sberdev.SberContracts.Server.CheckDocumentSignAssignmentArguments e)
    {
      e.Block.Performers.Add(_obj.Performer);
    }

  }
}