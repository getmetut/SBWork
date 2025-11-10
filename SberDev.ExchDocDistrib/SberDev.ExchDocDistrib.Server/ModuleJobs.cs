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