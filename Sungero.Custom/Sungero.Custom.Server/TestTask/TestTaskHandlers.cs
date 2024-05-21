using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.TestTask;

namespace Sungero.Custom
{
  partial class TestTaskServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      Custom.Functions.Module.CreateTaskRef(int.Parse(_obj.Id.ToString()));
    }
  }

}