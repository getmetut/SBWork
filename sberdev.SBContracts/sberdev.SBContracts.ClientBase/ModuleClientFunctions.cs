using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Функция выводит информационное сообщение в диалоговом окне
    /// </summary>
    /// <param name="message">Текс сообщения</param>
    [Public]
    public void ShowInfoMessage(string message)
    {
      Dialogs.ShowMessage(message, MessageType.Information);
    }
  }
}