using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ExchangeCore;
using Sungero.Parties;

namespace sberdev.SBContracts.Module.Exchange.Server
{
  partial class ModuleFunctions
  {/*
    protected override bool ProcessFormalizedTitlesAndSigns(NpoComputer.DCX.Common.IMessage message, IMessageQueueItem queueItem, List<IMessageQueueItem> queueItems, bool isIncoming, IBoxBase box, Enumeration historyOperation, string historyComment, string rootServiceDocumentId, string serviceDocumentId, byte[] reglamentDocumentContent)
    {
      var doc = Sungero.Exchange.PublicFunctions.ExchangeDocumentInfo.GetExDocumentInfoByExternalId(box, rootServiceDocumentId);
      
      if (doc == null)
      {
        if (doc.Document != null)
        {
          var accDoc = SBContracts.AccountingDocumentBases.As(doc.Document);
          if (accDoc != null && reglamentDocumentContent != null)
          {
            accDoc.MVPBaseSberDev = accDoc.State.Properties.MVPBaseSberDev.IsRequired ?
              SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVPStabGuid.ToString())).FirstOrDefault() : null;
            accDoc.MVZBaseSberDev = accDoc.State.Properties.MVZBaseSberDev.IsRequired ?
              SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVZStabGuid.ToString())).FirstOrDefault() : null;
            accDoc.AccArtBaseSberDev = accDoc.State.Properties.AccArtBaseSberDev.IsRequired ?
              SberContracts.AccountingArticleses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault() : null;
            if (accDoc.State.Properties.ProdCollectionBaseSberDev.IsRequired)
            {
              var prop = accDoc.ProdCollectionBaseSberDev.AddNew();
              prop.Product = SberContracts.ProductsAndDeviceses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault();
            }
          }
          accDoc.Save();
        }
      }
      return base.ProcessFormalizedTitlesAndSigns(message, queueItem, queueItems, isIncoming, box, historyOperation, historyComment, rootServiceDocumentId, serviceDocumentId, reglamentDocumentContent);
    }

    /// <summary>
    /// Заполняем обязательные поля заглушками
    /// </summary>
    protected override Sungero.Docflow.IOfficialDocument CreateContractStatementDocument(NpoComputer.DCX.Common.Document document, Sungero.Exchange.IExchangeDocumentInfo info, ICounterparty sender, IBoxBase box)
    {
      var accDoc = SBContracts.AccountingDocumentBases.As(base.CreateContractStatementDocument(document, info, sender, box));
      accDoc.MVPBaseSberDev = accDoc.State.Properties.MVPBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVPStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.MVZBaseSberDev = accDoc.State.Properties.MVZBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVZStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.AccArtBaseSberDev = accDoc.State.Properties.AccArtBaseSberDev.IsRequired ?
        SberContracts.AccountingArticleses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault() : null;
      if (accDoc.State.Properties.ProdCollectionBaseSberDev.IsRequired)
      {
        var prop = accDoc.ProdCollectionBaseSberDev.AddNew();
        prop.Product = SberContracts.ProductsAndDeviceses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault();
      }
      return accDoc;
    }
    
    protected override Sungero.Docflow.IOfficialDocument CreateTaxInvoice(NpoComputer.DCX.Common.Document document, Sungero.Exchange.IExchangeDocumentInfo info, ICounterparty sender, bool isIncoming, IBoxBase box)
    {
      var accDoc = SBContracts.AccountingDocumentBases.As(base.CreateTaxInvoice(document, info, sender, isIncoming, box));
      accDoc.MVPBaseSberDev = accDoc.State.Properties.MVPBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVPStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.MVZBaseSberDev = accDoc.State.Properties.MVZBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVZStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.AccArtBaseSberDev = accDoc.State.Properties.AccArtBaseSberDev.IsRequired ?
        SberContracts.AccountingArticleses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault() : null;
      if (accDoc.State.Properties.ProdCollectionBaseSberDev.IsRequired)
      {
        var prop = accDoc.ProdCollectionBaseSberDev.AddNew();
        prop.Product = SberContracts.ProductsAndDeviceses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault();
      }
      return accDoc;
      
    }
    
    protected override Sungero.Docflow.IOfficialDocument CreateWaybillDocument(NpoComputer.DCX.Common.Document document, Sungero.Exchange.IExchangeDocumentInfo info, ICounterparty sender, IBoxBase box)
    {
      var accDoc = SBContracts.AccountingDocumentBases.As(base.CreateWaybillDocument(document, info, sender, box));
      accDoc.MVPBaseSberDev = accDoc.State.Properties.MVPBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVPStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.MVZBaseSberDev = accDoc.State.Properties.MVZBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVZStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.AccArtBaseSberDev = accDoc.State.Properties.AccArtBaseSberDev.IsRequired ?
        SberContracts.AccountingArticleses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault() : null;
      if (accDoc.State.Properties.ProdCollectionBaseSberDev.IsRequired)
      {
        var prop = accDoc.ProdCollectionBaseSberDev.AddNew();
        prop.Product = SberContracts.ProductsAndDeviceses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault();
      }
      return accDoc;
    }

    protected override Sungero.Docflow.IOfficialDocument CreateUniversalTransferDocument(NpoComputer.DCX.Common.Document document, Sungero.Exchange.IExchangeDocumentInfo info, ICounterparty sender, IBoxBase box, List<NpoComputer.DCX.Common.DocumentType> universalDocumentTaxInvoiceAndBasicTypes)
    {
      var accDoc = SBContracts.AccountingDocumentBases.As(base.CreateUniversalTransferDocument(document, info, sender, box, universalDocumentTaxInvoiceAndBasicTypes));
      accDoc.MVPBaseSberDev = accDoc.State.Properties.MVPBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVPStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.MVZBaseSberDev = accDoc.State.Properties.MVZBaseSberDev.IsRequired ?
        SberContracts.MVZs.GetAll().Where(m => Equals(m.GUID, SberContracts.PublicConstants.Module.MVZStabGuid.ToString())).FirstOrDefault() : null;
      accDoc.AccArtBaseSberDev = accDoc.State.Properties.AccArtBaseSberDev.IsRequired ?
        SberContracts.AccountingArticleses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault() : null;
      if (accDoc.State.Properties.ProdCollectionBaseSberDev.IsRequired)
      {
        var prop = accDoc.ProdCollectionBaseSberDev.AddNew();
        prop.Product = SberContracts.ProductsAndDeviceses.GetAll().Where(m => Equals(m.Name, "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")).FirstOrDefault();
      }
      return accDoc;
    }
    */
    protected override Sungero.Exchange.IExchangeDocumentProcessingTask CreateExchangeTask(NpoComputer.DCX.Common.IMessage message, List<Sungero.Exchange.IExchangeDocumentInfo> infos, Sungero.Parties.ICounterparty sender, bool isIncoming, Sungero.ExchangeCore.IBoxBase box, string exchangeTaskActiveTextBoundedDocuments)
    {
      var task = base.CreateExchangeTask(message, infos, sender, isIncoming, box, exchangeTaskActiveTextBoundedDocuments);
      task.Author = Sungero.ExchangeCore.PublicFunctions.BoxBase.Remote.GetExchangeDocumentResponsible(box, sender, infos);
      task.Save();
      return task;
    }
  }
}