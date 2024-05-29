using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core; 
using Sungero.CoreEntities;
using sberdev.SberContracts.GuaranteeLetter;

namespace sberdev.SberContracts
{
  partial class GuaranteeLetterServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
    }
  }


}