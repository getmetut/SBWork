using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalStage;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalStageActions
  {
    public virtual void SetSidDevSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Укажите Sid");
      var sid = dialog.AddString("Sid", true);
      if (dialog.Show() == DialogButtons.Ok)
      {
        _obj.SidSberDev = sid.Value;
        _obj.Save();
      }
    }

    public virtual bool CanSetSidDevSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Company.Employees.Current.IncludedIn(Roles.GetAll(r => r.Sid == SberContracts.PublicConstants.Module.AdminButtonsUserRoleGuid).FirstOrDefault())
        && !_obj.State.IsInserted;
    }

  }


}