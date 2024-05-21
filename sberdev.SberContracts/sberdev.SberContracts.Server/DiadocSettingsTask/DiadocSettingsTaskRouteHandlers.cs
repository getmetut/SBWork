using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using sberdev.SberContracts.DiadocSettingsTask;

namespace sberdev.SberContracts.Server
{
  partial class DiadocSettingsTaskRouteHandlers
  {

    public virtual void StartBlock3(sberdev.SberContracts.Server.DiadocSettingsAssignmentArguments e)
    {
      e.Block.Performers.Add(Recipients.As(Users.GetAll(u => u.Login.LoginName == "delopp").First()));
    }
  }

}