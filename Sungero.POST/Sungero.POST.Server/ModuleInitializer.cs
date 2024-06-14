using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.POST.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      var Role = Roles.GetAll().Where(r => r.Name == "Доступ к обложке Отчеты").FirstOrDefault();
      bool marker = false;
      if (Role != null)
      {
        foreach (var Sot in Role.RecipientLinks)
        {
          if (Users.Current.Id == Sot.Member.Id)
            marker = true;
        }
      }
      return marker;
    }
  }
}
