using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalTaskActions
  {
    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var doc = _obj.DocumentGroup.OfficialDocuments.First();
      var kind = SBContracts.DocumentKinds.As(doc.DocumentKind);
      bool noBodyFlag = kind != null ? kind.NoBodyApprovalSberDev.GetValueOrDefault() : true;
      if (!noBodyFlag && !doc.HasVersions)
      {
        Dialogs.ShowMessage(sberdev.SBContracts.ApprovalTasks.Resources.NoBodyError, MessageType.Error);
        return;
      }
        
      var invoice = SBContracts.IncomingInvoices.As(doc);
      if (invoice != null && !_obj.IsNeedManuallyCheckSberDev.Value)
      {
        bool flagContractOwn = false, flagContractCounter = false, flagActOwn = false, flagActCounter = false;
        string error = "";
        
        if (invoice.NoNeedLeadingDocs.HasValue && invoice.NoNeedLeadingDocs.Value)
          base.Start(e);
        else
        {
          if (invoice.PayType.Value == SBContracts.IncomingInvoice.PayType.Postpay)
          {
            var contract = invoice.LeadingDocument;
            if (contract != null)
            {
              if (contract.ManuallyCheckedSberDev == null || contract.ManuallyCheckedSberDev == false)
              {
                List<bool> list = PublicFunctions.ApprovalTask.Remote.CheckSignatures(_obj, contract, true);
                flagContractOwn = list[0];
                flagContractCounter = list[1];
              }
              else
              {
                flagContractOwn = true;
                flagContractCounter = true;
              }
            }
            else
              error += "Заполните поле \"Договор\" в карточке счета.\n";
            
            var act = invoice.AccDocSberDev;
            if (act != null)
            {
              if (act.ManuallyCheckedSberDev == null || act.ManuallyCheckedSberDev == false)
              {
                var list = PublicFunctions.ApprovalTask.Remote.CheckSignatures(_obj, act, true);
                flagActOwn = list[0];
                flagActCounter = list[1];
              }
              else
              {
                flagActOwn = true;
                flagActCounter = true;
              }
            }
            else
              error += "Заполните поле \"Дополнительный финансовый документ\" в карточке счета.";
            
            if (flagContractOwn && flagContractCounter && flagActOwn && flagActCounter)
              base.Start(e);
            else
              if (error != "")
                Dialogs.ShowMessage(error, MessageType.Error);
              else
                Dialogs.ShowMessage(PublicFunctions.ApprovalTask.ShowCheckSignaturesResult(_obj, flagContractOwn, flagContractCounter, flagActOwn, flagActCounter),
                                    MessageType.Error);
          }
          else
          {
            var contract = invoice.LeadingDocument;
            if (contract != null)
            {
              if (contract.ManuallyCheckedSberDev == null || contract.ManuallyCheckedSberDev == false)
              {
                List<bool> list = PublicFunctions.ApprovalTask.Remote.CheckSignatures(_obj, contract, true);
                flagContractOwn = list[0];
                flagContractCounter = list[1];
              }
              else
              {
                flagContractOwn = true;
                flagContractCounter = true;
              }
              
              if (flagContractOwn && flagContractCounter)
                base.Start(e);
              else
                Dialogs.ShowMessage(PublicFunctions.ApprovalTask.ShowCheckSignaturesResult(_obj, flagContractOwn, flagContractCounter),
                                    MessageType.Error);
            }
            else
              Dialogs.ShowMessage("Заполните поле \"Договор\" в карточке счета.", MessageType.Error);
          }
        }
      }
      else
        base.Start(e);
    }


    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }

  }

}