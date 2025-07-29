using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSigningAssignment;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalSigningAssignmentActions
  {
    public override void EditActiveText(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.EditActiveText(e);
    }

    public override bool CanEditActiveText(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanEditActiveText(e);
    }

    public virtual void UnblockAttachSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var attach = SBContracts.OfficialDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault());
      PublicFunctions.Module.Remote.UnblockCardByDatabase(attach);
      if (attach.HasVersions == true)
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand("delete from Sungero_System_BinDataLocks where EntityId = "
                                                                 + attach.LastVersion.Id.ToString() + " and EntityTypeGuid = '"
                                                                 + attach.LastVersion.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "'");
    }

    public virtual bool CanUnblockAttachSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Company.Employees.Current.IncludedIn(Roles.GetAll(r => r.Sid == SberContracts.PublicConstants.Module.AdminButtonsUserRoleGuid).FirstOrDefault());
    }

  }

  partial class ApprovalSigningAssignmentCollectionActions
  {

    public virtual bool CanActionSignedSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ActionSignedSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      foreach (var job in _objs.Where(j => j.Status == SBContracts.ApprovalSigningAssignment.Status.InProcess).ToList())
      {
        job.Complete(ApprovalSigningAssignment.Result.Sign);
      }
    }
  }

}