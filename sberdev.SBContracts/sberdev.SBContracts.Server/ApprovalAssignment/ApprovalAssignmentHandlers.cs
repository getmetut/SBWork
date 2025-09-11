using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalAssignmentServerHandlers
  {

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      PublicFunctions.ApprovalTask.ClearPublicComment(_obj);
      if (_obj.NeedAbort.GetValueOrDefault())
      {
        e.Result = sberdev.SberContracts.Resources.NotApprove;
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.InvApprByTreasSberDev = false;
      _obj.IsNeedSupStageSberDev = false;
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);

      var task = sberdev.SBContracts.ApprovalTasks.As(_obj.Task);
      var attach = GetMainDocument();

      UpdateDocumentId(attach);

      if (attach == null)
      {
        CopyTaskShadowFields(task);
        return;
      }

      var accounting   = sberdev.SBContracts.AccountingDocumentBases.As(attach);
      var contractual  = sberdev.SBContracts.ContractualDocuments.As(attach);
      var incomingInv  = sberdev.SBContracts.IncomingInvoices.As(attach);

      if (accounting != null)
        FillFromAccounting(accounting);

      if (contractual != null)
        FillFromContractual(contractual, task, /*persistChangesToDoc*/ true);

      if (incomingInv != null)
        UpdateNonContractInvoiceCounter(incomingInv);

      // Для бухгалтерских, но не входящих счетов, проверяем подпись казначея.
      if (accounting != null && incomingInv == null)
        UpdateTreasuryApprovalFlag(accounting);

      CopyTaskShadowFields(task);
    }

    #region Helpers

    /// <summary>
    /// Первый прикреплённый официальный документ.
    /// </summary>
    private Sungero.Docflow.IOfficialDocument GetMainDocument()
    {
      return _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
    }

    /// <summary>
    /// Проставить строковый Id документа в карточку.
    /// </summary>
    private void UpdateDocumentId(Sungero.Docflow.IOfficialDocument attach)
    {
      _obj.DocumentIDSberDev = attach?.Id.ToString();
    }

    /// <summary>
    /// Заполнить поля из бухгалтерского документа.
    /// </summary>
    private void FillFromAccounting(sberdev.SBContracts.IAccountingDocumentBase accounting)
    {
      _obj.InternalApprovalStateSberDev = accounting.LeadingDocument?.InternalApprovalState;
      _obj.ExternalApprovalStateSberDev = accounting.LeadingDocument?.ExternalApprovalState;
      _obj.MVZSberDev                   = accounting.MVZBaseSberDev ?? accounting.MVPBaseSberDev;
      _obj.AccArtSberDev                = accounting.AccArtBaseSberDev;
    }

    /// <summary>
    /// Заполнить поля и признаки из договорного документа.
    /// </summary>
    /// <param name="persistChangesToDoc">
    /// Если true — сохраняем изменённый договорной документ (как было в исходном коде).
    /// Учтите риски сохранения связанных сущностей в событии Saving.
    /// </param>
    private void FillFromContractual(sberdev.SBContracts.IContractualDocument contractual,
                                     sberdev.SBContracts.IApprovalTask task,
                                     bool persistChangesToDoc)
    {
      _obj.MVZSberDev    = contractual.MVZBaseSberDev ?? contractual.MVPBaseSberDev;
      _obj.AccArtSberDev = contractual.AccArtExBaseSberDev ?? contractual.AccArtPrBaseSberDev;

      if (task != null)
      {
        var isCheckingCpStage =
          PublicFunctions.ApprovalTask.IsNecessaryStage(task, PublicConstants.Docflow.ApprovalTask.CheckingCPStage);

        contractual.IsNeedLegalInfoSberDev = isCheckingCpStage;

        // В исходнике был явный Save(); оставляю за вами решение, уместно ли это в данном событии.
        if (persistChangesToDoc)
          contractual.Save();
      }
    }

    /// <summary>
    /// Обновить счётчик "вне договора" по входящему счёту-фактуре.
    /// </summary>
    private void UpdateNonContractInvoiceCounter(sberdev.SBContracts.IIncomingInvoice incomingInv)
    {
      var counter = sberdev.SberContracts.NonContractInvoiceCounters
        .GetAll()
        .Where(c => c.Counterparty == incomingInv.Counterparty && c.Employee == incomingInv.Author)
        .FirstOrDefault();

      if (counter != null)
      {
        _obj.NonContractInvoiceCounterSberDev      = counter.Counter;
        _obj.NonContractInvoiceCounterMoreSberDev  = counter;
      }
      else
      {
        _obj.NonContractInvoiceCounterSberDev      = 0;
        _obj.NonContractInvoiceCounterMoreSberDev  = null;
      }
    }

    /// <summary>
    /// Выставить признак согласования казначеем по подписям.
    /// </summary>
    private void UpdateTreasuryApprovalFlag(sberdev.SBContracts.IAccountingDocumentBase accounting)
    {
      _obj.InvApprByTreasSberDev = IsApprovedByTreasurer(accounting);
    }

    /// <summary>
    /// Проверить, есть ли среди подписантов казначей (не визирующая подпись).
    /// </summary>
    private bool IsApprovedByTreasurer(sberdev.SBContracts.IAccountingDocumentBase accounting)
    {
      var treasurer = GetTreasurerUser();
      if (treasurer == null)
        return false;

      var lastVersion = accounting.InvoiceSberDev?.LastVersion;
      if (lastVersion == null)
        return false;

      var signs = Signatures.Get(lastVersion);
      if (signs == null)
        return false;

      foreach (var sign in signs)
      {
        var signatoryId    = sign.Signatory != null ? sign.Signatory.Id : 0;
        var substitutedId  = sign.SubstitutedUser != null ? sign.SubstitutedUser.Id : 0;

        if ((signatoryId == treasurer.Id || substitutedId == treasurer.Id) &&
            sign.SignatureType != SignatureType.NotEndorsing)
          return true;
      }

      return false;
    }

    /// <summary>
    /// Найти пользователя-казначея по имени (как в исходной логике).
    /// </summary>
    private Sungero.CoreEntities.IUser GetTreasurerUser()
    {
      // Оставил Contains по имени — обычно транслируется в LIKE. При желании замените на роль/группу.
      return Sungero.CoreEntities.Users.GetAll()
        .Where(u => u.Name.Contains("Казначей"))
        .FirstOrDefault();
    }

    /// <summary>
    /// Скопировать вспомогательные поля из задачи.
    /// </summary>
    private void CopyTaskShadowFields(sberdev.SBContracts.IApprovalTask task)
    {
      if (task == null)
        return;

      _obj.AmountATSDevATSDev        = task.AmountATSDev;
      _obj.ContractTypeATSDevATSDev  = task.ContractTypeATSDev;
    }

    #endregion
  }

}