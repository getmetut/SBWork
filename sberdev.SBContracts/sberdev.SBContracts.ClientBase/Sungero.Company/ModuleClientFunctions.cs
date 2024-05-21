using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Company.Client
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Кнопка установки фильтрации аналитик по Нашей организации в диалоге глобального поиска
    /// </summary>
    public virtual void DialogUpdateBusinessUnitFilteringCashe()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.Module.Company.Resources.UnitFilteringTitle);
      var cashe = PublicFunctions.Module.Remote.GetOrCreateBusinessUnitFilteringCashe(Users.Current);
      var bU = dialog.AddSelect(sberdev.SBContracts.Module.Company.Resources.DialogUpdateBusinessUnitFilteringCasheBusinessUnit, true, cashe.BusinessUnit);
      if (dialog.Show() == DialogButtons.Ok)
      {
        cashe.BusinessUnit = bU.Value;
        cashe.Save();
      }
    }
  }
}