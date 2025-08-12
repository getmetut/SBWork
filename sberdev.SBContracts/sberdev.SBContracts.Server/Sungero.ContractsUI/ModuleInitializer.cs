using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace sberdev.SBContracts.Module.ContractsUI.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      GrantRightsOnFolder();
    }
    
    public void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Выдача прав на папки ContractsUI");
      var allUsers = Roles.AllUsers;
      
      InitializationLogger.Debug("Init: Выдача прав на папку \"Реестр закупок\"");
      SpecialFolders.PurchaseRegistry.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Change);
      InitializationLogger.Debug("Init: Выдача прав на папку \"Реестр учредительных документов\"");
      SpecialFolders.FoundDocRegistrySBDEV.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Change);
      
      SpecialFolders.FoundDocRegistrySBDEV.AccessRights.Save();
      SpecialFolders.PurchaseRegistry.AccessRights.Save();
    }
  }
}
