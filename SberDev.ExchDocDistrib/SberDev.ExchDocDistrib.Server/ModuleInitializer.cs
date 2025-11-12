using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace SberDev.ExchDocDistrib.Server
{
  public partial class ModuleInitializer
  {
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      var ruleResp = Functions.Module.GetAllBoxesResponsibleEmployees();
      var allUsers = Roles.AllUsers;
      Rules.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      LogEntries.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      foreach (var resp in ruleResp)
      {
        Rules.AccessRights.Grant(resp, DefaultAccessRightsTypes.FullAccess);
      }
      Rules.AccessRights.Save();
      LogEntries.AccessRights.Save();
    }
  }
}
