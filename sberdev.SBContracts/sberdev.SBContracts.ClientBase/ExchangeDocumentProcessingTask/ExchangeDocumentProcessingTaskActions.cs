using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingTask;
using System.IO;

namespace sberdev.SBContracts.Client
{
  partial class ExchangeDocumentProcessingTaskActions
  {
    public virtual void TransferBodySberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var attach = _obj.Attachments.FirstOrDefault();
      var incomingDoc = Sungero.Content.ElectronicDocuments.As(attach);
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ExchangeDocumentProcessingTasks.Resources.DialogCopyBody);
      dialog.Width = 200;
      var idDoc = dialog.AddInteger(sberdev.SBContracts.ExchangeDocumentProcessingTasks.Resources.ID, true);
      bool flag = true;
      // этот код можно раскоментить и нормально оформить если пойдут ошибки с процессом возврата
      /*   List<int> idsTasks = new List<int>();
      if (setting.Text != null)
      {
        foreach(string id in setting.Text.Split(','))
          idsTasks.Add(Int32.Parse(id));
        foreach(int idTask in idsTasks)
        {
          var task = SBContracts.ExchangeDocumentProcessingTasks.GetAll().Where(t => Equals(t.Id, idTask)).First();
          task.IsNeedComeback = true;
          task.NumberOfAttempsComeback = 0;
          task.Save();
        }
        else
      }*/
      if (dialog.Show() == DialogButtons.Ok)
      {
        try
        {
          var doc = Sungero.Content.ElectronicDocuments.GetAll(d => d.Id == idDoc.Value.Value).First();
          Stream strmCommon = incomingDoc.LastVersion.Body.Read();
          Stream strmPublic = incomingDoc.LastVersion.PublicBody.Read();
          doc.CreateVersionFrom(strmCommon, incomingDoc.LastVersion.AssociatedApplication.Extension);
          doc.LastVersion.PublicBody.Write(strmPublic);
          doc.Save();
          var signInfos = Signatures.Get(incomingDoc.LastVersion);
          foreach(var signInfo in signInfos)
          {
            var signaturesBytes = signInfo.GetDataSignature();
            Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, signInfo.SigningDate, doc.LastVersion);
          }
          strmCommon.Close();
          strmPublic.Close();
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
      return true;
    }

    public virtual void ShowCertInfo(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var attach = _obj.Attachments.FirstOrDefault();
      var signs = Signatures.Get(Sungero.Content.ElectronicDocuments.As(attach).LastVersion);
      foreach (var sign in signs)
      {
        var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
        Dialogs.ShowMessage(certificateInfo.SubjectInfo, MessageType.Information);
      }
    }

    public virtual bool CanShowCertInfo(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}