using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.ControlContract;

namespace Sungero.Custom
{
  partial class ControlContractServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var DDS = _obj.CollectionDocs;
      double limit = _obj.Limit.Value;
      foreach (var str in DDS)
      {
        limit -= str.Summ.Value;
      }
      if (_obj.TotalLimit != limit)
        _obj.TotalLimit = limit;
    }
  }

}