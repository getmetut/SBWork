using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ATS.Module.Shell.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Применить к списку заданий фильтры по статусу и периоду.
    /// </summary>
    /// <param name="query">Список заданий.</param>
    /// <param name="inProcess">Признак показа заданий "В работе".</param>
    /// <param name="shortPeriod">Фильтр по короткому периоду (30 дней).</param>
    /// <param name="middlePeriod">Фильтр по среднему периоду (90 дней).</param>
    /// <param name="longPeriod">Фильтр по длинному периоду (180 дней).</param>
    /// <param name="longPeriod">Фильтр по самому длинному периоду (10 лет).</param>
    /// <param name="longPeriodToCompleted">Фильтр по длинному периоду (180 дней) для завершённых заданий.</param>
    /// <returns>Отфильтрованный список заданий.</returns>
    [Public]
    public IQueryable<Sungero.Workflow.IAssignmentBase> ApplyCommonSubfolderFilters(IQueryable<Sungero.Workflow.IAssignmentBase> query,
                                                                                    bool inProcess,
                                                                                    bool shortPeriod,
                                                                                    bool middlePeriod,
                                                                                    bool longPeriod,
                                                                                    bool verylongPeriod,
                                                                                    bool longPeriodToCompleted)
    {
      // Фильтр по статусу.
      if (inProcess)
        return query.Where(a => a.Status == Workflow.AssignmentBase.Status.InProcess);
      
      // Фильтр по периоду.
      DateTime? periodBegin = null;
      if (shortPeriod)
        periodBegin = Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(Calendar.UserToday.AddDays(-30));
      else if (middlePeriod)
        periodBegin = Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(Calendar.UserToday.AddDays(-90));
      else if (longPeriod || longPeriodToCompleted)
        periodBegin = Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(Calendar.UserToday.AddDays(-180));
      else if (verylongPeriod || longPeriodToCompleted)
        periodBegin = Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(Calendar.UserToday.AddDays(-3654));
      
      if (shortPeriod || middlePeriod || longPeriod)
        query = query.Where(a => a.Created >= periodBegin);
      else if (longPeriodToCompleted)
        query = query.Where(a => a.Created >= periodBegin || a.Status == Workflow.AssignmentBase.Status.InProcess);

      return query;
    }
  }
}