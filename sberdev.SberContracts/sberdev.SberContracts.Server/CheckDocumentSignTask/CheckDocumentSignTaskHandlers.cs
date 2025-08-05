using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CheckDocumentSignTask;

namespace sberdev.SberContracts
{
  partial class CheckDocumentSignTaskServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      var devSet = SberContracts.PublicFunctions.DevSettings.Remote.GetDevSetting("Настройка рассылки подзадач по контролю возврата").Text.Split(',');
      int period;
      if (!int.TryParse(devSet[1], out period))
        throw new ArgumentException("Укажите корректные значениея в текстовом параметре. Модуль Договоры -> Системные настройки -> Настройка рассылки подзадач по контролю возврата");
      _obj.ReminderCount = 0;
      _obj.MaxDeadline = Calendar.AddWorkingDays(Calendar.Now, period);
    }
  }



}