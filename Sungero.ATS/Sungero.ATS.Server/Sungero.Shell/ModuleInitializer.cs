using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.ATS.Module.Shell.Server
{
  public partial class ModuleInitializer
  {
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GrantRightsOnFolder();
    }
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      // Доработка в рамках задачи DRX-824.
      var allUsers = Roles.AllUsers;
      SpecialFolders.NoticesSDev.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SpecialFolders.NoticesSDev.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Уведомления'");
    }
  }
}
