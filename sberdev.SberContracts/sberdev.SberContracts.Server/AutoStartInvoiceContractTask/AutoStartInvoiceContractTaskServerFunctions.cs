using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AutoStartInvoiceContractTask;

namespace sberdev.SberContracts.Server
{
  partial class AutoStartInvoiceContractTaskFunctions
  {
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      try
      {
        var contract = SBContracts.Contracts.As(approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault());
        if (contract != null)
        {
          var invoice = SBContracts.IncomingInvoices.Create();
          invoice.LeadingDocument = contract;
          invoice.Number = "test";
          invoice.PayType = SBContracts.IncomingInvoice.PayType.Prepayment;
          invoice.Date = Calendar.Now;
          invoice.Save();
          SBContracts.PublicFunctions.OfficialDocument.Remote.TransferBody(contract, invoice.Id);
          var newTask = Sungero.Docflow.ApprovalTasks.Create();
          newTask.DocumentGroup.OfficialDocuments.Add(invoice);
          newTask.ApprovalRule = Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetApprovalRules(invoice).FirstOrDefault();
          if (newTask.State.Properties.Signatory.IsRequired)
            newTask.Signatory = approvalTask.Signatory;
          newTask.StartedBy = approvalTask.StartedBy;
          newTask.Author = approvalTask.Author;
          newTask.Save();
          newTask.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Этап сценария. Создание входящего счета к договору-оферте и запуск его на  Результат: Неуспешно.  Причина:" + ex.ToString(), approvalTask);
      }
      return base.Execute(approvalTask);
    }
  }
}