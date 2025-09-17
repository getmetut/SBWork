using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Company.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// Установить статусы КонутрФокус для контрагнетов.
    /// </summary>
    public virtual void SetFocusStatusSberDev()
    {
      var counterparties = SBContracts.Companies.GetAll().Where(cp => cp.FocusCheckedDateSberDev != null);
      foreach (var cp in counterparties)
      {
        // Легкая первая проверка. Был ли ка успешно проверен
        if (cp.FocusCheckedSberDev.HasValue && cp.FocusCheckedSberDev.Value)
        {
          if (cp.FocusCheckedDateSberDev.HasValue && cp.FocusCheckedDateSberDev.Value.Year == Calendar.Now.Year)
          {
            cp.FocusCheckStatusSberDev = SBContracts.Company.FocusCheckStatusSberDev.Actual;
            continue;
          }
          else
          {
            cp.FocusCheckStatusSberDev = SBContracts.Company.FocusCheckStatusSberDev.DateOutdated;
            continue;
          }
        }
        
        // Сложная проверка. Был ли ка забракован
        
      }
    }

  }
}