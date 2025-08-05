using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.DevSettings;

namespace sberdev.SberContracts.Server
{
  partial class DevSettingsFunctions
  {
    
    /// <summary>
    /// Получить элемент справочника "настройки разработке (DevSettings) разработки".
    /// </summary>
    /// <param name="name">Имя настройки разработке (DevSettings).</param>
    /// <returns>Элемент справочника "настройки разработке (DevSettings) разработки".</returns>
    [Public, Remote]
    public static sberdev.SberContracts.IDevSettings GetDevSetting(string name)
    {
      return SberContracts.DevSettingses.GetAll().Where(n => Equals(n.Name, name)).FirstOrDefault();
    }

    /// <summary>
    /// Получить список ID сущностей из текстового параметра элемента справочника "настройки разработке (DevSettings) разработки".
    /// </summary>
    /// <param name="name">Имя настройки разработке (DevSettings).</param>
    /// <returns>Список ID сущностей</returns>
    /// <exception cref="ArgumentNullException">Если name пустой</exception>
    /// <exception cref="InvalidOperationException">Если настройка разработке (DevSettings) не найдена или текст пуст</exception>
    /// <exception cref="FormatException">Если элемент не удалось распарсить</exception>
    [Public, Remote]
    public static List<long> GetDevSettingTextIDs(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name), "Имя настройки разработке (DevSettings) не задано");

      var devSetting = SberContracts.DevSettingses
        .GetAll()
        .FirstOrDefault(n => n.Name == name);

      if (devSetting == null)
      {
        Logger.Error($"Настройка разработки (DevSettings) с именем '{name}' не найдена");
        throw new InvalidOperationException($"Настройка разработки (DevSettings) '{name}' не найдена");
      }

      if (string.IsNullOrWhiteSpace(devSetting.Text))
      {
        Logger.Error($"Пустой параметр Text у настройки разработки (DevSettings) '{name}'");
        throw new InvalidOperationException($"Пустой параметр Text у настройки разработки (DevSettings) '{name}'");
      }

      var result = new List<long>();
      foreach (var part in devSetting.Text.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
      {
        long id;
        if (long.TryParse(part.Trim(), out id))
        {
          result.Add(id);
        }
        else
        {
          Logger.Error($"Невозможно преобразовать '{part}' в long для идентификации настройки разработки '{name}'");
          throw new FormatException($"Некорректный ID в параметре Text настройки разработки (DevSettings) '{name}': '{part}'");
        }
      }

      return result;
    }


  }
}