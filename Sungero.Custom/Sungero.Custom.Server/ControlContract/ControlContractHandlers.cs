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
      if (_obj.Limit.HasValue)
      {
        double limit = _obj.Limit.Value;
        if (DDS.Count > 0)
        {
          foreach (var str in DDS)
          {
            if (str.Summ.HasValue)
              limit -= str.Summ.Value;
          }
        }
        
        if (_obj.TotalLimit != limit)
          _obj.TotalLimit = limit;
      }
    }
  }

}