using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractStatement;

namespace sberdev.SBContracts
{

  partial class ContractStatementServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
        base.BeforeSave(e);
        Functions.ContractStatement.BeforeSaveFunction(_obj);
      
    }
  }

}