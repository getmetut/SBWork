using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.DatabookJobs;

namespace Sungero.Custom.Client
{
  partial class DatabookJobsFunctions
  {
    /// <summary>
    /// Открытие карточки задания
    /// </summary>  
    [Public]
    public void OpenJobInCard()
    {
      var job = Sungero.Workflow.Assignments.Get(long.Parse(_obj.IDJob.ToString()));
      job.Show();
    }

  }
}