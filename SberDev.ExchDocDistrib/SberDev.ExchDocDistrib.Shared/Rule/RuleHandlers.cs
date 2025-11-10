using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SberDev.ExchDocDistrib.Rule;

namespace SberDev.ExchDocDistrib
{
  partial class RuleSharedHandlers
  {

    public virtual void AttemptsFailedChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if (_obj.AttemptsSuccessful != 0 && e.NewValue != null && e.NewValue != 0)
        _obj.SuccessRate = (int)((double)_obj.AttemptsSuccessful / (e.NewValue + _obj.AttemptsSuccessful) * 100);
      else
        _obj.SuccessRate = 0;
    }

    public virtual void AttemptsSuccessfulChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if (e.NewValue != 0)
        _obj.SuccessRate = (int)((double)e.NewValue / (e.NewValue + _obj.AttemptsFailed) * 100);
      else
        _obj.SuccessRate = 0;
    }

  }
}