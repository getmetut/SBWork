using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom
{
  partial class ListSoglServerHandlers
  {

    public virtual IQueryable<Sungero.Custom.ITasksRefer> GetReference()
    {
      return Sungero.Custom.TasksRefers.GetAll().Where(r => r.Id == ListSogl.Entity.Id);
    }
  }

}