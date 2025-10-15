using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts;

namespace sberdev.SBContracts.Module.Company.Server
{
  partial class ModuleJobs
  {
    
    public virtual void SetManualCheckStatusSberDev()
    {
      var swTotal = System.Diagnostics.Stopwatch.StartNew();
      Sungero.Core.Logger.Debug("SetFocusStatusJob. НАЧАЛО.");

      try
      {
        var candidates = Integration.PublicFunctions.Module.GetCompaniesToFillInMarkersPublic()
          .Select(cp => new
                  {
                    cp.Id,
                    cp.ManualCheckedSberDev,
                    cp.ManualCheckedDateSberDev
                  })
          .ToList();
        Sungero.Core.Logger.Debug($"SetFocusStatusJob. Найдено компаний: {candidates.Count}.");

        int processed = 0, updated = 0, skipped = 0, errors = 0;

        foreach (var row in candidates)
        {
          processed++;

          try
          {
            var company = SBContracts.Companies.Get(row.Id);
            if (company == null)
            {
              skipped++;
              Sungero.Core.Logger.Debug($"SetFocusStatusJob. Компания Id={row.Id} не найдена. Пропуск.");
              continue;
            }

            // Снимаем блокировку, если есть.
            PublicFunctions.Module.Remote.UnblockCardByDatabase(company);

            // Получаем задания.
            var assignments = PublicFunctions.Company.GetCheckingAssignments(company);
            if (assignments == null)
            {
              assignments = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
            }

            var assignmentIds = assignments.Select(a => a.Id.ToString()).ToList();
            var hasAssignments = assignmentIds.Count > 0;

            // Определяем новый статус.
            var status = PublicFunctions.Company.GetManualCheckStatus(company, hasAssignments);

            bool changed = false;
            if (company.ManualCheckStatusSberDev != status)
            {
              company.ManualCheckStatusSberDev = status;
              changed = true;
            }

            if (hasAssignments)
            {
              var idsString = string.Join(",", assignmentIds);
              if (!string.Equals(company.ManualCheckAssignIDsSberDev ?? string.Empty, idsString, System.StringComparison.Ordinal))
              {
                company.ManualCheckAssignIDsSberDev = idsString;
                changed = true;
              }
            }

            if (changed)
            {
              company.Save();
              updated++;
              Sungero.Core.Logger.Debug($"SetFocusStatusJob. Компания Id={company.Id} обновлена. Новый статус={status}.");
            }
            else
            {
              Sungero.Core.Logger.Debug($"SetFocusStatusJob. Компания Id={company.Id} без изменений.");
            }
          }
          catch (System.Exception ex)
          {
            errors++;
            Sungero.Core.Logger.Error($"SetFocusStatusJob. Ошибка при обработке компании Id={row.Id}: {ex}");
          }
        }

        Sungero.Core.Logger.Debug(
          $"SetFocusStatusJob. Завершено. Обработано={processed}, Обновлено={updated}, Пропущено={skipped}, Ошибок={errors}.");
      }
      catch (System.Exception ex)
      {
        Sungero.Core.Logger.Error($"SetFocusStatusJob. КРИТИЧЕСКАЯ ОШИБКА: {ex}");
      }
      finally
      {
        swTotal.Stop();
        Sungero.Core.Logger.Debug($"SetFocusStatusJob. КОНЕЦ (время {swTotal.ElapsedMilliseconds} мс).");
      }
    }

  }
}