using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.PowerOfAttorney;

namespace Sungero.ATS.Client
{
  partial class PowerOfAttorneyActions
  {
    public virtual void SendPoAToBPATSDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      e.CloseFormAfterAction = true;
      var task = Sungero.Custom.TaskToPowerOfAttorneys.Create();
      task.BaseAttachments.PowerOfAttorneys.Add(_obj);
      task.Subject = "Согласование: " + _obj.Name;
      task.Show();
    }

    public virtual bool CanSendPoAToBPATSDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual bool CanSendATSDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

  }

}