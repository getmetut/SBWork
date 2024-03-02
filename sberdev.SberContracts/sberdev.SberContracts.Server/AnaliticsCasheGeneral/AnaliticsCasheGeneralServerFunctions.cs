using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AnaliticsCasheGeneral;

namespace sberdev.SberContracts.Server
{
  partial class AnaliticsCasheGeneralFunctions
  {
    [Public]
    public void ClearByUser(IUser user)
    {
      var cashe = AnaliticsCasheGenerals.GetAll().FirstOrDefault(c => Equals(c.User, user));
      _obj.Counterparty = null;
      _obj.AccArtEx = null;
      _obj.AccArtPr = null;
      _obj.ContrType = null;
      _obj.MVP = null;
      _obj.MVZ = null;
      _obj.ProdCollectionEx.Clear();
      _obj.ProdCollectionPr.Clear();
    }
  }
}