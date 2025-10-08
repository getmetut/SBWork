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
          var assignmentsIds = PublicFunctions.Company.Remote.GetCheckingAssignments(cp)?.Select(a => a.Id.ToString());
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
    }

  }
}