using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.NDA;

namespace Sungero.Custom.Client
{
  partial class NDAActions
  {
    public virtual void SendNDATask(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      var Task = Custom.NDATasks.Create();
      Task.BaseDocNDA.NDAs.Add(_obj);
      Task.Subject = "Согласование: " + _obj.Name.ToString();
      e.CloseFormAfterAction = true;
      Task.Show();
    }

    public virtual bool CanSendNDATask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.State.IsInserted)
        return false;
      else
        return true;
    }

  }

}