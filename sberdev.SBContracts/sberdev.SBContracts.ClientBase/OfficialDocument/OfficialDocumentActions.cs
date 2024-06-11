using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;
using System.IO;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Client
{
  partial class OfficialDocumentVersionsActions
  {
    public override void CreateDocumentFromVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(SBContracts.OfficialDocuments.As(_obj.ElectronicDocument));
      base.CreateDocumentFromVersion(e);
    }

    public override bool CanCreateDocumentFromVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var doc = SBContracts.OfficialDocuments.As(_obj.ElectronicDocument);
      if (doc != null && PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(doc))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanCreateDocumentFromVersion(e);
    }

    public override void CreateVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(SBContracts.OfficialDocuments.As(_obj.ElectronicDocument));
      base.CreateVersion(e);
    }

    public override bool CanCreateVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var doc = SBContracts.OfficialDocuments.As(_obj.ElectronicDocument);
      if (doc != null && PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(doc))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanCreateVersion(e);
    }

  }

  partial class OfficialDocumentActions
  {
    public virtual void UnblockVersionSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.HasVersions == true)
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand("delete from Sungero_System_BinDataLocks where EntityId = "
                                                                 + _obj.LastVersion.Id.ToString() + " and EntityTypeGuid = '"
                                                                 + _obj.LastVersion.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "'");
    }

    public virtual bool CanUnblockVersionSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }


    public virtual void UnblockCardSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.Module.Remote.UnblockCardByDatabase(_obj);
    }

    public virtual bool CanUnblockCardSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void GetMetadataSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var id = PublicFunctions.Module.Remote.GetMetadataID(_obj);
      if (id != null)
        Dialogs.ShowMessage(id);
      else
        Dialogs.ShowMessage("А нету тут метаданных братан");
    }

    public virtual bool CanGetMetadataSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));
    }

    public virtual void SetMetadataSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.Module.Remote.SetMetadataID(_obj);
    }

    public virtual bool CanSetMetadataSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));
    }

    public virtual void TransferBodySberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ExchangeDocumentProcessingTasks.Resources.DialogCopyBody);
      dialog.Width = 200;
      var idDoc = dialog.AddInteger(sberdev.SBContracts.ExchangeDocumentProcessingTasks.Resources.ID, true);
      if (dialog.Show() == DialogButtons.Ok)
      {
        bool flag = true;
        try
        {
          PublicFunctions.OfficialDocument.Remote.TransferBodyWithSignatures(_obj, idDoc.Value.Value);
        }
        catch
        {
          Dialogs.ShowMessage(sberdev.SBContracts.ExchangeDocumentProcessingTasks.Resources.Wrong, MessageType.Error);
          flag = false;
        }
        if (flag)
          Dialogs.ShowMessage(sberdev.SBContracts.ExchangeDocumentProcessingTasks.Resources.Succsess, MessageType.Information);
      }
    }

    public virtual bool CanTransferBodySberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Администраторы"));
    }

    public override bool CanImportInLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanImportInLastVersion(e);
    }

    public override void ImportInLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.ImportInLastVersion(e);
    }

    public override void ImportInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.ImportInNewVersion(e);
    }

    public override bool CanImportInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanImportInNewVersion(e);
    }

    public override bool CanScanInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanScanInNewVersion(e);
    }

    public override void ScanInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.ScanInNewVersion(e);
    }

    public override void CreateFromScanner(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.CreateFromScanner(e);
    }

    public override bool CanCreateFromScanner(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanCreateFromScanner(e);
    }

    public override void CreateFromFile(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.CreateFromFile(e);
    }

    public override bool CanCreateFromFile(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanCreateFromFile(e);
    }

    public override void CreateVersionFromLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.CreateVersionFromLastVersion(e);
    }

    public override bool CanCreateVersionFromLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanCreateVersionFromLastVersion(e);
    }

    public override void CreateFromTemplate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      PublicFunctions.OfficialDocument.Remote.NullingManuallyChecked(_obj);
      base.CreateFromTemplate(e);
    }

    public override bool CanCreateFromTemplate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (PublicFunctions.Module.Remote.CheckPropertySignaturesGeneral(_obj))
        return Sungero.Company.Employees.Current.IncludedIn(PublicFunctions.Module.Remote.GetGroup("Делопроизводители"));
      else
        return base.CanCreateFromTemplate(e);
    }

    public virtual void ImportSignatureSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.HasVersions && !_obj.State.IsInserted)
      {
        var inputFileDialog = Dialogs.CreateInputDialog("Выберите файл подписи");
        var file = inputFileDialog.AddFileSelect("Файл", false);
        file.WithFilter(string.Empty, "sgn");
        if (inputFileDialog.Show() == DialogButtons.Ok)
        {
          var signatureInfo = ExternalSignatures.GetSignatureInfo(file.Value.Content);
          var signatureUpgraded = signatureInfo.AsCadesTSignatureInfo();
          var parsedInfo = PublicFunctions.Module.Remote.ParseCertificateSubject(signatureUpgraded.CertificateInfo.SubjectInfo);
          Signatures.Import(_obj, SignatureType.Approval, parsedInfo.Surname + " " + parsedInfo.GivenName, file.Value.Content, _obj.LastVersion);
          _obj.Save();
          Dialogs.ShowMessage("Подпись успешно импортирована.", MessageType.Information);
        }
      }
      else
        Dialogs.ShowMessage("Невозможно импортитровать подпись. Документ не имеет версий или карточка только что создана и не сохранена.", MessageType.Error);

    }

    public virtual bool CanImportSignatureSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}