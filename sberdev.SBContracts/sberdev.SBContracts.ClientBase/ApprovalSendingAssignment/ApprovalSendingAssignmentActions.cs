using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSendingAssignment;
using Sungero.Company;
using Sungero.Content;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalSendingAssignmentActions
  {
    public virtual void CreateVersionFromFileSBDEV(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Доработка в рамках задачи DRX-700.
      var dialog = Dialogs.CreateInputDialog("Выберите файл");
      var file = dialog.AddFileSelect("Файл", true);
      if (dialog.Show() == DialogButtons.Ok) {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (document != null) {
          document.CreateVersion();
          var version = document.LastVersion;
          using (var stream = new MemoryStream(file.Value.Content)) {
            version.PublicBody.Write(stream);
            version.AssociatedApplication = AssociatedApplications.GetByExtension("pdf");
            document.Save();
          }
        }
        e.AddInformation("Файл импортирован в новую версию документа.");
      }
    }

    public virtual bool CanCreateVersionFromFileSBDEV(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Доработка в рамках задачи DRX-700.
      // Делопроизводитель ПП.
      var emp = Sungero.Company.Employees.Get(431);
      // Все замещения на Делопроизводителя ПП.
      var subs = Sungero.CoreEntities.Substitutions.GetAll()
        .Where(s => s.User.Login != null && s.User.Login == emp.Login)
        .Select(s => s.Substitute.Login)
        .ToList();
      
      return _obj.Performer != null && _obj.Performer.Login == emp.Login
        && (subs.Contains(Employees.Current.Login) || Employees.Current.IncludedIn(Roles.Administrators));
    }

    public virtual void ExtendDeadlineSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ApprovalSendingAssignments.Resources.DialogTitleExtDeadline);
      var newDeadline = dialog.AddDate(sberdev.SBContracts.ApprovalSendingAssignments.Resources.DialogPropNewDeadline, true);
      if (dialog.Show() == DialogButtons.Ok)
      {
        PublicFunctions.Module.Remote.ExtendAssignmentDeadline(_obj, newDeadline.Value.Value);
      }
    }

    public virtual bool CanExtendDeadlineSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Status == Status.InProcess;
    }

  }

}