using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.ContractualDocumentBase;

namespace Sungero.ATS.Client
{
  partial class ContractualDocumentBaseActions
  {
    public virtual void SendDocInTMSDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.CloseFormAfterAction = true;
      var Task = SDev.BPCustom.ContractualTasks.Create();
      Task.BaseAttachments.ContractualDocuments.Add(sberdev.SBContracts.ContractualDocuments.Get(_obj.Id));
      string Theme = "Согласование: (" + _obj.Counterparty.Name.ToString() + ") - " + _obj.Name.ToString();
      if (Theme.Length > 249)
          Theme = Theme.Substring(0, 249);      
      Task.Subject = Theme;
      Task.Save();
      Task.Show();
    }

    public virtual bool CanSendDocInTMSDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}