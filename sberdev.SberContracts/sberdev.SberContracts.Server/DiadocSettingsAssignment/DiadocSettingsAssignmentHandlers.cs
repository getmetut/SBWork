using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.DiadocSettingsAssignment;

namespace sberdev.SberContracts
{
  partial class DiadocSettingsAssignmentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.ThreadSubject = sberdev.SberContracts.DiadocSettingsAssignments.Resources.DIadocSettingAssigmentThreadSubject;
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      _obj.Counterparty = SberContracts.DiadocSettingsTasks.As(_obj.MainTask).Counterparty;
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      if (Locks.TryLock(_obj.Counterparty))
      {
        SBContracts.Counterparties.As(_obj.Counterparty).DiadocIsSetSberDev = true;
        _obj.Counterparty.Save();
        Locks.Unlock(_obj.Counterparty);
      }
      else
        e.AddError(sberdev.SberContracts.DiadocSettingsAssignments.Resources.ErrorBlockCa);
    }
  }
}