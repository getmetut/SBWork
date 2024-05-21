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
    /// Функция возваращает русское название месяца по его номеру
    /// </summary>
    [Public]
    public string GetMonthGenetiveName(int monthNumber)
    {
        switch (monthNumber)
        {
            case 1:
                return "января";
            case 2:
                return "февраля";
            case 3:
                return "марта";
            case 4:
                return "апреля";
            case 5:
                return "мая";
            case 6:
                return "июня";
            case 7:
                return "июля";
            case 8:
                return "августа";
            case 9:
                return "сентября";
            case 10:
                return "октября";
            case 11:
                return "ноября";
            case 12:
                return "декабря";
            default:
                throw new ArgumentOutOfRangeException("Номер месяца должен быть от 1 до 12");
        }
    }
    
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