using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.DatabookJobs;

namespace Sungero.Custom.Client
{
  partial class DatabookJobsActions
  {
    public virtual void OpenJob(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.DatabookJobs.OpenJobInCard(_obj);
    }

    public virtual bool CanOpenJob(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}