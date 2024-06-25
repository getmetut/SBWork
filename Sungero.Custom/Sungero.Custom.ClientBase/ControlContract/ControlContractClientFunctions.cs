using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.ControlContract;

namespace Sungero.Custom.Client
{
  partial class ControlContractFunctions
  {

    /// <summary>
    /// Функция вывода диалога для пользователя
    /// </summary> 
    [Public]
    public void ShowMessage(string message)
    {
      Dialogs.ShowMessage(message);
    }

  }
}