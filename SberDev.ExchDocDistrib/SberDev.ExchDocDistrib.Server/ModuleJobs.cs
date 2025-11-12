using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace SberDev.ExchDocDistrib.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Процесс удаляет дубликаты в справочнике правил распределения вх. док-ов
    /// </summary>
    public virtual void DuplicateRuleCleaner()
    {
      var items = Rules.GetAll()
        .Where(x => x.Author != null)
        .ToList();

      // Группируем по полям, по которому считаем дубль
      var duplicates = items
        .GroupBy(x => new { x.Author, x.Counterparty, x.Responsible })
        .Where(g => g.Count() > 1);

      foreach (var group in duplicates)
      {
        // Сортируем, чтобы оставить первый (например, по Id)
        var ordered = group.OrderBy(x => x.Id).ToList();
        var keep = ordered.First();
        var toDelete = ordered.Skip(1);

        foreach (var dup in toDelete)
        {
          try
          {
            Rules.Delete(dup);
          }
          catch (Exception ex)
          {
            Logger.DebugFormat("Не удалось удалить дубль {0}: {1}", dup.Id, ex.Message);
          }
        }
      }
    }

    /// <summary>
    /// Процесс очищает справочник LogEntry от записей с датой старше месяца
    /// </summary>
    public virtual void LogCleaner()
    {
      // вычисляем пороговую дату — всё, что раньше этой даты, удаляем
      var threshold = Calendar.Now.AddDays(-30);

      // сначала получаем список записей (ToList — материализует запрос)
      var oldEntries = LogEntries.GetAll()
        .Where(l => l.Date != null && l.Date < threshold)
        .ToList();

      // можно залогировать сколько будет удалено
      Logger.DebugFormat("ExchDocDistribLogCleaner: will delete {0} entries older than {1}", oldEntries.Count, threshold);

      // удаляем
      foreach (var entry in oldEntries)
        LogEntries.Delete(entry);
    }


  }
}