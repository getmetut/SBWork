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
    /// Установить статусы КонутрФокус для контрагнетов.
    /// </summary>
    public virtual void SetFocusStatusSberDev()
    {
      var counterparties = SBContracts.Companies.GetAll().Where(cp => cp.FocusCheckedDateSberDev != null);
      foreach (var cp in counterparties)
      {
        try
        {
        PublicFunctions.Module.Remote.UnblockCardByDatabase(cp);
        // Легкая первая проверка. Был ли ка успешно проверен
        if (cp.FocusCheckedSberDev == true)
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
        else
        {
          var hasCheckingAssign= ApprovalAssignments.GetAll().Where(a => ApprovalStages.As(a.Stage).SidSberDev == PublicConstants.Docflow.ApprovalTask.CheckingCPStage
                                                                   && ApprovalTasks.As(a.Task).DocumentTypeSungero == "Contractual" 
                                                                   && ContractualDocuments.As(a.DocumentGroup.OfficialDocuments.FirstOrDefault()).Counterparty == cp).Any();
          if (hasCheckingAssign)
             cp.FocusCheckStatusSberDev = SBContracts.Company.FocusCheckStatusSberDev.Failed;
        }
        cp.Save();
        }
      }
    }

  }
}