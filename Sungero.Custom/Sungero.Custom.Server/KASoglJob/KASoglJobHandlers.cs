using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.KASoglJob;

namespace Sungero.Custom
{
  partial class KASoglJobServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      var Task = Custom.NDATasks.Get(_obj.MainTask.Id);
      if (Task.TravelDoc == Custom.NDATask.TravelDoc.Kurier)
        _obj.TravelStr = Custom.KASoglJob.TravelStr.Paper;
      else
        _obj.TravelStr = Custom.KASoglJob.TravelStr.Electronic;
    }
  }

}