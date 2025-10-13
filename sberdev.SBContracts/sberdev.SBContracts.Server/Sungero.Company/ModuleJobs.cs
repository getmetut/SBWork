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
    
    public virtual void SetFocusStatusSberDev()
    {
      var swTotal = System.Diagnostics.Stopwatch.StartNew();
      Sungero.Core.Logger.Debug("[SetFocusStatusSberDev] НАЧАЛО.");

      try
      {
        var swQuery = System.Diagnostics.Stopwatch.StartNew();
        var candidates = SBContracts.Companies.GetAll()
          .Where(cp => cp.FocusCheckedDateSberDev != null)
          .Select(cp => new
                  {
                    cp.Id,
                    cp.FocusCheckedSberDev,
                    cp.FocusCheckedDateSberDev
                  })
          .ToList();
        swQuery.Stop();
        Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] Найдено компаний: {candidates.Count} (время {swQuery.ElapsedMilliseconds} мс).");

        int processed = 0, updated = 0, skipped = 0, errors = 0;

        foreach (var row in candidates)
        {
          processed++;
          var swStep = System.Diagnostics.Stopwatch.StartNew();

          try
          {
            var company = SBContracts.Companies.Get(row.Id);
            if (company == null)
            {
              skipped++;
              Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] Компания Id={row.Id} не найдена. Пропуск.");
              continue;
            }

            // Снимаем блокировку, если есть.
            PublicFunctions.Module.Remote.UnblockCardByDatabase(company);

            // Получаем задания.
            var assignments = PublicFunctions.Company.GetCheckingAssignments(company);
            if (assignments == null)
            {
              Sungero.Core.Logger.Error($"[SetFocusStatusSberDev] GetCheckingAssignments вернула NULL для компании Id={company.Id}. Использую пустой список.");
              assignments = new System.Collections.Generic.List<sberdev.SBContracts.IApprovalAssignment>();
            }

            var assignmentIds = assignments.Select(a => a.Id.ToString()).ToList();
            var hasAssignments = assignmentIds.Count > 0;

            Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] Компания Id={company.Id}: найдено заданий {assignmentIds.Count}.");

            // Определяем новый статус.
            var status = PublicFunctions.Company.GetFocusStatus(company, hasAssignments);

            bool changed = false;
            if (company.FocusCheckStatusSberDev != status)
            {
              company.FocusCheckStatusSberDev = status;
              changed = true;
            }

            if (hasAssignments)
            {
              var idsString = string.Join(",", assignmentIds);
              if (!string.Equals(company.FocusCheckAssignIDsSberDev ?? string.Empty, idsString, System.StringComparison.Ordinal))
              {
                company.FocusCheckAssignIDsSberDev = idsString;
                changed = true;
              }
            }

            if (changed)
            {
              company.Save();
              updated++;
              Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] Компания Id={company.Id} обновлена. Новый статус={status}.");
            }
            else
            {
              Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] Компания Id={company.Id} без изменений.");
            }
          }
          catch (System.Exception ex)
          {
            errors++;
            Sungero.Core.Logger.Error($"[SetFocusStatusSberDev] Ошибка при обработке компании Id={row.Id}: {ex}");
          }
          finally
          {
            swStep.Stop();
            Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] Компания Id={row.Id} обработана за {swStep.ElapsedMilliseconds} мс.");
          }
        }

        Sungero.Core.Logger.Debug(
          $"[SetFocusStatusSberDev] Завершено. Обработано={processed}, Обновлено={updated}, Пропущено={skipped}, Ошибок={errors}.");
      }
      catch (System.Exception ex)
      {
        Sungero.Core.Logger.Error($"[SetFocusStatusSberDev] КРИТИЧЕСКАЯ ОШИБКА: {ex}");
      }
      finally
      {
        swTotal.Stop();
        Sungero.Core.Logger.Debug($"[SetFocusStatusSberDev] КОНЕЦ (время {swTotal.ElapsedMilliseconds} мс).");
      }
    }


    /*
    /// <summary>
    /// 
    /// </summary>
    public virtual void SetFocusStatusSberDev()
    {
      // 1) Отдельно делаем "тяжёлый" запрос к БД: берём только Id и нужные поля.
      var candidates = SBContracts.Companies.GetAll()
        .Where(cp => cp.FocusCheckedDateSberDev != null)
        .Select(cp => new
                {
                  cp.Id,
                  cp.FocusCheckedSberDev,
                  cp.FocusCheckedDateSberDev
                })
        .ToList();

      foreach (var row in candidates)
      {
        try
        {
          var cp = Companies.Get(row.Id);
          if (cp == null)
            continue;

          // Если карточка бывает под блокировкой — разблокируем.
          PublicFunctions.Module.Remote.UnblockCardByDatabase(cp);
          // Получаем  задания на проверку
          var assignmentsIds = PublicFunctions.Company.GetCheckingAssignments(cp)?.Select(a => a.Id.ToString());
          // Проставляем статус строго по правилам.
          var status = PublicFunctions.Company.GetFocusStatus(cp, assignmentsIds.Any());
          if (cp.FocusCheckStatusSberDev != status)
          {
            cp.FocusCheckStatusSberDev = status;
            if(assignmentsIds.Any())
            {
              cp.FocusCheckAssignIDsSberDev = string.Join(",", assignmentsIds);
            }
            cp.Save();
          }
        }
        catch (System.Exception x)
        {
          Logger.Error(x.ToString());
        }
      }
    }*/

  }
}