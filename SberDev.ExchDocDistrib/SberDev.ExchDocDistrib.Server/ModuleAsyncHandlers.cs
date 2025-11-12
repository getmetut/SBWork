using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace SberDev.ExchDocDistrib.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void DuplicateRuleCleaner(SberDev.ExchDocDistrib.Server.AsyncHandlerInvokeArgs.DuplicateRuleCleanerInvokeArgs args)
    {
      var items = Rules.GetAll()
        .Where(x => x.Author != null)
        .ToList();

      // Группируем по полю, по которому считаем дубль (например, Name + ИНН)
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

  }
}