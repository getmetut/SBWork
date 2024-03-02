using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Shared
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Возвращает русское название агрегации
    /// </summary>
    [Public]
    public string GetRusAggregationName(Nullable<Enumeration> aggr)
    {
      if (aggr == null)
        return null;
      string result = null;
      switch (aggr.Value.Value)
      {
        case "Devices":
          result = "Устройства";
          break;
        case "Licenses":
          result = "Лицензии";
          break;
        case "Services":
          result = "Сервисы";
          break;
      }
      return result;
    }
    /// <summary>
    /// Функция убиарет из строки символы, которые нельзя использовать в названии файла
    /// </summary>
    [Public]
    public string NormalizeFileName(string name)
    {
      char[] exept = {'/','\\',':','*','?','"','<','>','|'};
      char[] chars = name.ToCharArray();
      var list = chars.Where(c => exept.Contains(c) == false);
      string newName = "";
      foreach (var ch in list)
        newName = newName + ch.ToString();
      return newName;
    }

  }
}