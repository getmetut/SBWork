using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BProc.ExchangeDocumentProcessingTask;

namespace SDev.BProc.Server.ExchangeDocumentProcessingTaskBlocks
{
  partial class SendTaskIncInvATSDevHandlers
  {

    public virtual void SendTaskIncInvATSDevExecute()
    {
      var doc = _obj.IncomingInvoiceATSDev;
      if (doc != null)
      {
        var availableApprovalRules = Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetApprovalRules(doc).ToList();
        if (availableApprovalRules.Any())
        {
          try
          {
            var task = BProc.Functions.Module.CreateApprovalTask(doc, _obj);
            task.Start();
          }
          catch (Exception ex)
          {
            _obj.ActiveText += '\n' + string.Format("Счет на оплату не отправлен на согласование по причине: {0}",ex.Message.ToString());
          }
        }
      }
    }
  }

  partial class SearchIncomingInvoiATSDevHandlers
  {

    public virtual void SearchIncomingInvoiATSDevExecute()
    {
      var doc = _obj.AllAttachments.Where(t => Sungero.ATS.UniversalTransferDocuments.Is(t)).FirstOrDefault();
      if (doc != null)
      {
        if (_block.PostCasheATSDev.HasValue)
        {
          if (_block.PostCasheATSDev.Value)
          {
            var invoice = sberdev.SBContracts.IncomingInvoices.GetAll(i => ((i.AccDocSberDev.Id == doc.Id) && (i.PayType == sberdev.SBContracts.IncomingInvoice.PayType.Postpay))).FirstOrDefault();
            if (invoice != null)
            {
              _obj.IncomingInvoiceATSDev = invoice;
              _obj.Save();
            }
          }
          else
          {
            var invoice = sberdev.SBContracts.IncomingInvoices.GetAll(i => i.AccDocSberDev.Id == doc.Id).FirstOrDefault();
            if (invoice != null)
            {
              _obj.IncomingInvoiceATSDev = invoice;
              _obj.Save();
            }
          }
        }
      }
    }
  }

  partial class ExchangeDocumentProcessingAssignmentBlockHandlers
  {

  }
}