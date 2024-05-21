using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Contract;

namespace sberdev.SBContracts.Server
{
  partial class ContractFunctions
  {
    [Remote(IsPure=true)]
    public static bool CurrentUserInKZ()
    {   var recIds = Users.UserAndDirectSubstitutionRecipientIds;
      var UserInrole = Users.GetAll().ToList().Where(l => l.IncludedIn(sberdev.SberContracts.PublicConstants.Module.KZTypeGuid) && recIds.Contains(l.Id) );
      return UserInrole.Any();
    }
    
  }
}
