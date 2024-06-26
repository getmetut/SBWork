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
      Task.Subject = "Согласование: " + _obj.Name.ToString();
      Task.Save();
      Task.Show();
    }

    public virtual bool CanSendDocInTMSDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var User = Users.Current;
      var ctr = false;
      var Rol = Roles.GetAll(r => r.Name == "Тестировщики").FirstOrDefault();
      if (Rol != null)
      {
        foreach (var us in Rol.RecipientLinks)
        {
          if (us.Member.Name == User.Name)
            ctr = true;
        }
      }
      return ctr;
    }

  }

}