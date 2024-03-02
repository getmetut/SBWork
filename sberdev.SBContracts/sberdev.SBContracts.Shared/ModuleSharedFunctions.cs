using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Shared
{
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Функция проверяет является ли пользователь системным
    /// </summary>
    [Public]
    public bool IsSystemUser()
    {
      return Users.Current.Name == "Service User" || Users.Current.Name == "Administrator"
        || (Users.Current.IsSystem.HasValue && Users.Current.IsSystem.Value);
    }
    
  }
}