using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalTaskFunctions
  {
    public override void UpdateDocumentApprovalState(Sungero.Docflow.IOfficialDocument document, Nullable<Enumeration> state)
    {
      var invoice = SBContracts.IncomingInvoices.As(document);
      if (invoice == null)
        base.UpdateDocumentApprovalState(document, state);
    }
    
    public void SendDiadocSettingsTask()
    {
      var doc = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var contr = SBContracts.ContractualDocuments.As(doc);
      SBContracts.ICounterparty cp = null;
      if (contr != null)
        cp = SBContracts.Counterparties.As(contr.Counterparty);
      else
        return;
      if (doc.DeliveryMethod == null)
        return;
      if (cp.DiadocIsSetSberDev.Value ||  doc.DeliveryMethod.Id != 1)
        return;
      if (cp.Nonresident.HasValue && cp.Nonresident.Value)
        return;
      if (cp.CanExchange.HasValue && cp.CanExchange.Value)
        return;
      var task = SberContracts.DiadocSettingsTasks.Create();
      task.Counterparty = cp;
      task.Subject = sberdev.SBContracts.ApprovalTasks.Resources.SendDiadocSettingsTaskSubject + task.Counterparty.Name + sberdev.SBContracts.ApprovalTasks.Resources.SendDiadocSettingsTaskSubject2;
      task.NeedsReview = false;
      task.Start();
    }

    /// <summary>
    /// Функция проверяет наличие подписи от контрагента и от одного человека из группы
    /// "Обязательные подписанты счета перед согласованием" у документа и возвращает булевой массив где первый элемент флаг нашей подписи, второй контрагента.
    /// <param name="document">Документ.</param>
    /// <param name="isNotNeedValid">Необходимость проверки валидности.</param>
    /// </summary>
    [Public, Remote]
    public List<bool> CheckSignatures(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      List<bool> flags = new List<bool>() {false, false};
      var signatures = Signatures.Get(document.LastVersion);
      // Проверка на двойную версию
      if (document.LastVersion.Note == "Титул покупателя")
        signatures = Signatures.Get(document);
      var needSignGroup = PublicFunctions.Module.Remote.GetGroup("Обязательные подписанты счета перед согласованием");
      var ddd = signatures.Count();
      if (signatures.Any())
      {
        var contractual = SBContracts.ContractualDocuments.As(document);
        if (contractual != null)
        {
          foreach(var sign in signatures)
          {
            if (sign.IsExternal.HasValue && sign.IsExternal.Value)
            {
              var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
              string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyOrgTIN(certificateInfo.SubjectInfo);
              if (Equals(tin, contractual.Counterparty.TIN) && (sign.IsValid || !isNeedValid))
                flags[1] = true;
              if (Equals(tin, contractual.BusinessUnit.TIN) && (sign.IsValid || !isNeedValid))
                flags[0] = true;
            }
            else
              if (sign.SignatureType == SignatureType.Approval && ((sign.Signatory != null && sign.Signatory.IncludedIn(needSignGroup))
                                                                   || (sign.SubstitutedUser != null && sign.SubstitutedUser.IncludedIn(needSignGroup))))
                flags[0] = true;
          }
        }
        
        var accounting = SBContracts.AccountingDocumentBases.As(document);
        if (accounting != null)
        {
          foreach(var sign in signatures)
          {
            if (sign.IsExternal.HasValue && sign.IsExternal.Value)
            {
              var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
              string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyOrgTIN(certificateInfo.SubjectInfo);
              if (Equals(tin, accounting.Counterparty.TIN) && (sign.IsValid || !isNeedValid))
                flags[1] = true;
              if (Equals(tin, accounting.BusinessUnit.TIN) && (sign.IsValid || !isNeedValid))
                flags[0] = true;
            }
            else
              if (sign.SignatureType == SignatureType.Approval && ((sign.Signatory != null && sign.Signatory.IncludedIn(needSignGroup))
                                                                   || (sign.SubstitutedUser != null && sign.SubstitutedUser.IncludedIn(needSignGroup))))
                flags[0] = true;
          }
        }
      }
      return flags;
    }
  }
}