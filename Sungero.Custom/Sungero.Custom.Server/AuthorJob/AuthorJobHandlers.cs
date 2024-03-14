using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.AuthorJob;

namespace Sungero.Custom
{
  partial class AuthorJobServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      var Task = Custom.NDATasks.Get(_obj.MainTask.Id);
      if (Task.TravelDoc == Custom.NDATask.TravelDoc.Kurier)
        _obj.TravelStr = Custom.AuthorJob.TravelStr.Paper;
      else
        _obj.TravelStr = Custom.AuthorJob.TravelStr.Electronic;
    }
  }

}