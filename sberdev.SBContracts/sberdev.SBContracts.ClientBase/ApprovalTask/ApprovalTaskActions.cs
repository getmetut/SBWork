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
      
      
      if (PublicFunctions.ApprovalTask.OldAnaliticsInDocument(_obj, doc))
      {
        PublicFunctions.Module.ShowErrorMessage(sberdev.SBContracts.ApprovalTasks.Resources.BanOldAnalitics);
        return;
      }
      
      // Проверка на возможность согласования документа без версий
      var kind = SBContracts.DocumentKinds.As(doc.DocumentKind);
      bool noBodyFlag = kind != null ? kind.NoBodyApprovalSberDev.GetValueOrDefault() : true;
      if (!noBodyFlag && !doc.HasVersions)
      {
        Dialogs.ShowMessage(sberdev.SBContracts.ApprovalTasks.Resources.NoBodyError, MessageType.Error);
        return;
      }
      
      // Проверка подписей сопуствующих документов счета
      var invoice = SBContracts.IncomingInvoices.As(doc);
      if (invoice != null && !_obj.IsNeedManuallyCheckSberDev.Value)
      {
        bool flagContractOwn = false, flagContractCounter = false, flagActOwn = false, flagActCounter = false;
        string error = "";
        
        if (invoice.NoNeedLeadingDocs.HasValue && invoice.NoNeedLeadingDocs.Value)
          base.Start(e);
        else
        {
          if (invoice.PayTypeBaseSberDev.Value == SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay)
          {
            var contract = invoice.LeadingDocument;
            if (contract != null)
            {
              List<bool> realSig = PublicFunctions.Module.Remote.CheckRealSignatures(contract, false);
              List<bool> propSig = PublicFunctions.Module.Remote.CheckPropertySignatures(contract);
              flagContractOwn = realSig[0] || propSig[0];
              flagContractCounter = realSig[1] || propSig[1];
            }
            else
              error += "Заполните поле \"Договор\" в карточке счета.\n";
            
            var act = invoice.AccDocSberDev;
            if (act != null)
            {
              List<bool> realSig = PublicFunctions.Module.Remote.CheckRealSignatures(act, false);
              List<bool> propSig = PublicFunctions.Module.Remote.CheckPropertySignatures(act);
              flagActOwn = realSig[0] || propSig[0];
              flagActCounter = realSig[1] || propSig[1];
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
              List<bool> realSig = PublicFunctions.Module.Remote.CheckRealSignatures(contract, false);
              List<bool> propSig = PublicFunctions.Module.Remote.CheckPropertySignatures(contract);
              flagContractOwn = realSig[0] || propSig[0];
              flagContractCounter = realSig[1] || propSig[1];
            }
            else
              Dialogs.ShowMessage("Заполните поле \"Договор\" в карточке счета.", MessageType.Error);
            
            if (flagContractOwn && flagContractCounter)
              base.Start(e);
            else
              Dialogs.ShowMessage(PublicFunctions.ApprovalTask.ShowCheckSignaturesResult(_obj, flagContractOwn, flagContractCounter),
                                  MessageType.Error);
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