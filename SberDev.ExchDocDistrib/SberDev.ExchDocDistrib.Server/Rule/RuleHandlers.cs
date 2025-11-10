using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SberDev.ExchDocDistrib.Rule;

namespace SberDev.ExchDocDistrib
{
  partial class RuleServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.AttemptsFailed = 0;
      _obj.AttemptsSuccessful = 0;
    }
  }

}