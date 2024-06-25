using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.Specs;

namespace Sungero.Custom.Client
{
  partial class SpecsActions
  {
    public virtual void StartTask(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      e.CloseFormAfterAction = true;
      var task = Custom.SpecsTasks.Create();
      task.BaseAttachments.Specses.Add(_obj);
      task.BaseAttachments.ContractualDocuments.Add(sberdev.SBContracts.ContractualDocuments.Get(_obj.LeadingDocument.Id));
      task.Subject = "Согласование: " + _obj.Name;
      task.Show();
    }

    public virtual bool CanStartTask(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}