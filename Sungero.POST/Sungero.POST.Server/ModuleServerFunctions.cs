using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Metadata;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sungero.POST.Server
{
  
  public class ModuleFunctions
  {
    
    static DateTime? ParseDate(string dateString)
    {
        string[] dateFormats = {"yyyy-MM-dd","dd/MM/yyyy","MM/dd/yyyy","dd MMM yyyy","MMM dd, yyyy","yyyyMMdd","dd-MM-yyyy","MM-dd-yyyy","yyyy.MM.dd","dd.MM.yyyy"};
        DateTime parsedDate;
        bool success = DateTime.TryParseExact(dateString, dateFormats, 
            CultureInfo.InvariantCulture, 
            DateTimeStyles.None, out parsedDate);
        if (success)
          return parsedDate;

        return null;
    }
    
    /// <summary>
    /// Функция передачи актов по документам из внешнего запроса
    /// </summary>
    [Public(WebApiRequestType = RequestType.Post)]
    public List<Sungero.POST.Structures.Module.IStrucResult> ContractActsRequest(string  str_document_type,
                                                                                  string  str_document_kind,
                                                                                  string  str_receiver_cost_center,
                                                                                  string  str_reg_no,
                                                                                  string  str_start_date,
                                                                                  string  str_end_date,
                                                                                  string  str_counterparty_name,
                                                                                  string  str_status)
    {
      Sungero.POST.Structures.Module.IStructDog StructDog = Sungero.POST.Structures.Module.StructDog.Create();
      StructDog.document_type = str_document_type;
      StructDog.document_kind = str_document_kind;
      StructDog.receiver_cost_center = str_receiver_cost_center;
      StructDog.reg_no = str_reg_no;
      StructDog.start_date = str_start_date;
      StructDog.end_date = str_end_date;
      StructDog.counterparty_name = str_counterparty_name;
      StructDog.status = str_status;
      
      
      List<Sungero.POST.Structures.Module.IStrucResult> Resultat = new List<Sungero.POST.Structures.Module.IStrucResult>();
      var ContractList = sberdev.SBContracts.Contracts.GetAll();
      Logger.Debug("Начало отбора документов типа Договор. Собрано: " + ContractList.Count().ToString());
        long document_kind = long.Parse("0");
        if (StructDog.document_kind != "") 
          document_kind = long.Parse(StructDog.document_kind);
        
        DateTime start_date = DateTime.MinValue;
        if (StructDog.start_date != "")
        {
          start_date = ParseDate(StructDog.start_date).Value;
          ContractList = ContractList.Where(c => c.RegistrationDate.HasValue).Where(c => c.RegistrationDate > start_date);
        }
        Logger.Debug("Применен фильтр по дате регистрации Начало. Собрано: " + ContractList.Count().ToString());    
        DateTime end_date = DateTime.MaxValue;
        if (StructDog.end_date != "")
        {
          end_date = ParseDate(StructDog.end_date).Value;
          ContractList = ContractList.Where(c => c.RegistrationDate.HasValue).Where(c => c.RegistrationDate < end_date);
        }
        Logger.Debug("Применен фильтр по дате регистрации Конец. Собрано: " + ContractList.Count().ToString());
        string reg_no = "";
        if (StructDog.reg_no != "") 
        {
          reg_no = StructDog.reg_no;
          ContractList = ContractList.Where(c => c.RegistrationNumber == reg_no);
        }
        Logger.Debug("Применен фильтр по номеру регистрации. Собрано: " + ContractList.Count().ToString());
        if (StructDog.receiver_cost_center != "") // Фильтр для получения договоров по МВЗ
          ContractList = ContractList.Where(c => c.MVZBaseSberDev != null).Where(c => c.MVZBaseSberDev.Name == StructDog.receiver_cost_center);
        Logger.Debug("Применен фильтр по МВЗ. Собрано: " + ContractList.Count().ToString());
        if (StructDog.counterparty_name != "")  // Фильтр для получения договоров по контрагенту
          ContractList = ContractList.Where(c => c.Counterparty != null).Where(c => sberdev.SBContracts.Companies.Get(c.Counterparty.Id).Name1CSberDev == StructDog.counterparty_name);
        Logger.Debug("Применен фильтр по наименованию контрагента. Собрано: " + ContractList.Count().ToString());
        if (StructDog.status != "") //Статус действия договора
          ContractList = ContractList.Where(c => c.LifeCycleState.Value.ToString() == StructDog.status);
        Logger.Debug("Применен фильтр по статусу договора. ФИНАЛЬНЫЙ ФИЛЬТР. Собрано: " + ContractList.Count().ToString());
        if (ContractList.Count() > 0)
        {
            foreach (var elem in ContractList)
            {
              Logger.Debug("Начало работы с документом: " + elem.Name.ToString());
              var Struct = Sungero.POST.Structures.Module.StrucResult.Create();
              Struct.category = elem.DocumentGroup != null ? elem.DocumentGroup.Name.ToString() : "Нет категории";
              Struct.counterparty_name = elem.Counterparty != null ? elem.Counterparty.Name.ToString() : "Не указан";
              Struct.date_document = elem.RegistrationDate != null ? elem.RegistrationDate.Value.ToString() : "";
              Struct.name = elem.Name.ToString(); 
              Struct.reg_no = elem.RegistrationNumber != null ? elem.RegistrationNumber.ToString() : "";
              Logger.Debug("Собраны первичные данные по документу: " + elem.Name.ToString());
              List<Sungero.POST.Structures.Module.IStructlistdoc> acts = new List<Sungero.POST.Structures.Module.IStructlistdoc>();
              if (StructDog.document_type != "") // "УПД" / "Договора" / "Акты"
              { 
                var Related = elem.Relations.GetRelated();
                var RelatedFrom = elem.Relations.GetRelatedFrom();
                Logger.Debug("Найдено связанных документов: " + Related.Count().ToString() + " + " + RelatedFrom.Count().ToString());
              #region УПД
                if (StructDog.document_type == "УПД")
                {
                  Related = Related.Where(d => Sungero.FinancialArchive.UniversalTransferDocuments.Is(d));
                  RelatedFrom = RelatedFrom.Where(d => Sungero.FinancialArchive.UniversalTransferDocuments.Is(d));
                  Logger.Debug("Отбор по УПД: " + Related.Count().ToString() + " + " + RelatedFrom.Count().ToString());
                }        
              #endregion
              #region Договора
              
              if (StructDog.document_type == "Договора")
              {
                Related = Related.Where(d => Sungero.Docflow.ContractualDocumentBases.Is(d));
                RelatedFrom = RelatedFrom.Where(d => Sungero.Docflow.ContractualDocumentBases.Is(d));
                Logger.Debug("Отбор по договорам: " + Related.Count().ToString() + " + " + RelatedFrom.Count().ToString());
              } 
              #endregion
              #region Акты
      
              if (StructDog.document_type == "Акты")
              {
                Related = Related.Where(d => sberdev.SBContracts.AccountingDocumentBases.Is(d));
                RelatedFrom = RelatedFrom.Where(d => sberdev.SBContracts.AccountingDocumentBases.Is(d));
                Logger.Debug("Отбор по актам: " + Related.Count().ToString() + " + " + RelatedFrom.Count().ToString());
              }           
              #endregion
              if (Related.Count() > 0)
              {
                Logger.Debug("Начало отбора связанных документов. ");
                foreach (var str in Related)
                {
                  Sungero.POST.Structures.Module.IStructlistdoc doc = Sungero.POST.Structures.Module.Structlistdoc.Create();
                  if (Sungero.Docflow.ContractualDocumentBases.Is(str))
                  {
                    var tempDoc = Sungero.Docflow.ContractualDocumentBases.As(str);
                    doc.act_no = tempDoc.RegistrationNumber != null ? tempDoc.RegistrationNumber.ToString() : "Нет данных";
                    doc.acts_type = tempDoc.DocumentKind.Name.ToString();
                    doc.date_act = tempDoc.RegistrationDate != null ? tempDoc.RegistrationDate.ToString() : "Нет данных";
                    doc.total_amount = tempDoc.TotalAmount != null ? tempDoc.TotalAmount.ToString() : "Не указана";
                    doc.name = tempDoc.Name.ToString();
                    doc.include_VAT = true;     
                  }
                  if (Sungero.FinancialArchive.UniversalTransferDocuments.Is(str))
                  {
                    var tempDoc2 = Sungero.FinancialArchive.UniversalTransferDocuments.As(str);
                    doc.act_no = tempDoc2.RegistrationNumber != null ? tempDoc2.RegistrationNumber.ToString() : "Нет данных";
                    doc.acts_type = tempDoc2.DocumentKind.Name.ToString();
                    doc.date_act = tempDoc2.RegistrationDate != null ? tempDoc2.RegistrationDate.ToString() : "Нет данных";
                    doc.total_amount = tempDoc2.TotalAmount != null ? tempDoc2.TotalAmount.ToString() : "Не указана";
                    doc.name = tempDoc2.Name.ToString();
                    doc.include_VAT = true;     
                  }
                  if (sberdev.SBContracts.AccountingDocumentBases.Is(str))
                  {
                    var tempDoc3 = sberdev.SBContracts.AccountingDocumentBases.As(str);
                    doc.act_no = tempDoc3.RegistrationNumber != null ? tempDoc3.RegistrationNumber.ToString() : "Нет данных";
                    doc.acts_type = tempDoc3.DocumentKind.Name.ToString();
                    doc.date_act = tempDoc3.RegistrationDate != null ? tempDoc3.RegistrationDate.ToString() : "Нет данных";
                    doc.total_amount = tempDoc3.TotalAmount != null ? tempDoc3.TotalAmount.ToString() : "Не указана";
                    doc.name = tempDoc3.Name.ToString();
                    doc.include_VAT = true;     
                  }
                  acts.Add(doc);
                }
              }
              if (RelatedFrom.Count() > 0)
              {
                foreach (var str in RelatedFrom)
                {
                  Sungero.POST.Structures.Module.IStructlistdoc docf = Sungero.POST.Structures.Module.Structlistdoc.Create();
                  if (Sungero.Docflow.ContractualDocumentBases.Is(str))
                  {
                    var tempDocm = Sungero.Docflow.ContractualDocumentBases.As(str);
                    docf.act_no = tempDocm.RegistrationNumber != null ? tempDocm.RegistrationNumber.ToString() : "Нет данных";
                    docf.acts_type = tempDocm.DocumentKind.Name.ToString();
                    docf.date_act = tempDocm.RegistrationDate != null ? tempDocm.RegistrationDate.ToString() : "Нет данных";
                    docf.total_amount = tempDocm.TotalAmount != null ? tempDocm.TotalAmount.ToString() : "Не указана";
                    docf.name = tempDocm.Name.ToString();
                    docf.include_VAT = true;     
                  }
                  if (Sungero.FinancialArchive.UniversalTransferDocuments.Is(str))
                  {
                    var tempDocm2 = Sungero.FinancialArchive.UniversalTransferDocuments.As(str);
                    docf.act_no = tempDocm2.RegistrationNumber != null ? tempDocm2.RegistrationNumber.ToString() : "Нет данных";
                    docf.acts_type = tempDocm2.DocumentKind.Name.ToString();
                    docf.date_act = tempDocm2.RegistrationDate != null ? tempDocm2.RegistrationDate.ToString() : "Нет данных";
                    docf.total_amount = tempDocm2.TotalAmount != null ? tempDocm2.TotalAmount.ToString() : "Не указана";
                    docf.name = tempDocm2.Name.ToString();
                    docf.include_VAT = true;     
                  }
                  if (sberdev.SBContracts.AccountingDocumentBases.Is(str))
                  {
                    var tempDocm3 = sberdev.SBContracts.AccountingDocumentBases.As(str);
                    docf.act_no = tempDocm3.RegistrationNumber != null ? tempDocm3.RegistrationNumber.ToString() : "Нет данных";
                    docf.acts_type = tempDocm3.DocumentKind.Name.ToString();
                    docf.date_act = tempDocm3.RegistrationDate != null ? tempDocm3.RegistrationDate.ToString() : "Нет данных";
                    docf.total_amount = tempDocm3.TotalAmount != null ? tempDocm3.TotalAmount.ToString() : "Не указана";
                    docf.name = tempDocm3.Name.ToString();
                    docf.include_VAT = true;     
                  }
                  acts.Add(docf);
                }
              }
              Struct.acts = acts; 
              Logger.Debug("Занесение связанного документа: " + Struct.name.ToString());
              Resultat.Add(Struct);
            }
          Logger.Debug("Завершение обработки связанных документов. ");              
          }
        }
        Logger.Debug("Завершение работы скрипта"); 
        return Resultat;
    }
    
    /// <summary>
    /// Функция передачи актов выполненных работ
    /// </summary>
    [Public(WebApiRequestType = RequestType.Post)]
    public List<Sungero.POST.Structures.Module.IStructContractStatement> ContractStatements(string till, string from)
    {
      DateTime TILL = ParseDate(till).Value;
      DateTime FROM = ParseDate(from).Value;
      var DocList = sberdev.SBContracts.ContractStatements.GetAll(d => ((d.DocumentDate >= TILL) && (d.DocumentDate <= FROM)));
      List<Sungero.POST.Structures.Module.IStructContractStatement> RequestElem = new List<Sungero.POST.Structures.Module.IStructContractStatement>(); //Structures.Module.StructContractList.Create();
      if (DocList.Count() > 0)
      {
        foreach (var Doc in DocList)
        {
          var ContractSt = Structures.Module.StructContractStatement.Create();
          ContractSt.Id = Doc.Id != null ? Doc.Id.ToString() : "";
          ContractSt.Name = Doc.Name != null ? Doc.Name.ToString() : "";
          ContractSt.DocumentKind = Doc.DocumentKind != null ? Doc.DocumentKind.Name.ToString() : "";
          ContractSt.DocumentGroup = Doc.DocumentGroup != null ? Doc.DocumentGroup.Name.ToString() : "";
          ContractSt.Number = Doc.Number != null ? Doc.Number.ToString() : "";
          ContractSt.Date = Doc.Date != null ? Doc.Date.Value.ToString() : "";
          ContractSt.Counterparty = Doc.Counterparty != null ? Doc.Counterparty.ToString() : "";
          ContractSt.Contact = Doc.Contact != null ? Doc.Contact.Name.ToString() : "";
          ContractSt.CounterpartySignatory = Doc.CounterpartySignatory != null ? Doc.CounterpartySignatory.Name.ToString() : "";
          ContractSt.LeadingDocument = Doc.LeadingDocument != null ? Doc.LeadingDocument.Name.ToString() : "";
          ContractSt.BusinessUnit = Doc.BusinessUnit != null ? Doc.BusinessUnit.Name.ToString() : "";
          ContractSt.Department = Doc.Department != null ? Doc.Department.Name.ToString() : "";
          ContractSt.ResponsibleEmployee = Doc.ResponsibleEmployee != null ? Doc.ResponsibleEmployee.Name.ToString() : "";
          ContractSt.OurSignatory = Doc.OurSignatory != null ? Doc.OurSignatory.Name.ToString() : "";
          ContractSt.ExchangeState = Doc.ExchangeState != null ? Doc.ExchangeState.Value.ToString() : "";
          ContractSt.Subject = Doc.Subject != null ? Doc.Subject.ToString() : "";
          ContractSt.IsFormalized = Doc.IsFormalized != null ? Doc.IsFormalized.Value.ToString() : "";
          ContractSt.IsAdjustment = Doc.IsAdjustment != null ? Doc.IsAdjustment.Value.ToString() : "";
          ContractSt.Corrected = Doc.Corrected != null ? Doc.Corrected.Name.ToString() : "";
          ContractSt.VerificationState = Doc.VerificationState != null ? Doc.VerificationState.Value.ToString() : "";
          ContractSt.DocumentDate = Doc.DocumentDate != null ? Doc.DocumentDate.Value.ToString() : "";
          ContractSt.OurSigningReason = Doc.OurSigningReason != null ? Doc.OurSigningReason.Name.ToString() : "";
          ContractSt.PaperCount = Doc.PaperCount != null ? Doc.PaperCount.Value.ToString() : "";
          ContractSt.StoredIn = Doc.StoredIn != null ? Doc.StoredIn.ToString() : "";
          ContractSt.Topic = Doc.Topic != null ? Doc.Topic.Name.ToString() : "";
          ContractSt.Subtopic = Doc.Subtopic != null ? Doc.Subtopic.Name.ToString() : "";
          ContractSt.ManuallyCheckedSberDev = Doc.ManuallyCheckedSberDev != null ? Doc.ManuallyCheckedSberDev.Value.ToString() : "";
          ContractSt.FormalizedServiceType = Doc.FormalizedServiceType != null ? Doc.FormalizedServiceType.Value.ToString() : "";
          ContractSt.FormalizedFunction = Doc.FormalizedFunction != null ? Doc.FormalizedFunction.Value.ToString() : "";
          ContractSt.IsRevision = Doc.IsRevision != null ? Doc.IsRevision.Value.ToString() : "";
          ContractSt.CounterpartySigningReason = Doc.CounterpartySigningReason != null ? Doc.CounterpartySigningReason.ToString() : "";
          ContractSt.PurchaseOrderNumber = Doc.PurchaseOrderNumber != null ? Doc.PurchaseOrderNumber.ToString() : "";
          ContractSt.ContrTypeBaseSberDev = Doc.ContrTypeBaseSberDev != null ? Doc.ContrTypeBaseSberDev.Value.ToString() : "";
          ContractSt.FrameworkBaseSberDev = Doc.FrameworkBaseSberDev != null ? Doc.FrameworkBaseSberDev.Value.ToString() : "";
          ContractSt.AccDocSberDev = Doc.AccDocSberDev != null ? Doc.AccDocSberDev.Name.ToString() : "";
          ContractSt.MVZBaseSberDev = Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Name.ToString() : "";
          ContractSt.MVPBaseSberDev = Doc.MVPBaseSberDev != null ? Doc.MVPBaseSberDev.Name.ToString() : "";
          ContractSt.AccArtBaseSberDev = Doc.AccArtBaseSberDev != null ? Doc.AccArtBaseSberDev.Name.ToString() : "";
          ContractSt.InvoiceSberDev = Doc.InvoiceSberDev != null ? Doc.InvoiceSberDev.Name.ToString() : "";
          ContractSt.PayTypeBaseSberDev = Doc.PayTypeBaseSberDev != null ? Doc.PayTypeBaseSberDev.Value.ToString() : "";

          List<Sungero.POST.Structures.Module.IStructProdCollection> ProdCollection = new List<Sungero.POST.Structures.Module.IStructProdCollection>();
          if (Doc.ProdCollectionBaseSberDev.Count > 0)
          {
            foreach (var elem in Doc.ProdCollectionBaseSberDev)
            {
              var newstr = Sungero.POST.Structures.Module.StructProdCollection.Create();
              newstr.Product = elem.Product != null ? elem.Product.Name.ToString() : "";
              ProdCollection.Add(newstr);
            } 
          }
          ContractSt.ProdCollectionBaseSberDev = ProdCollection;
                
          ContractSt.MarketDirectSberDev = Doc.MarketDirectSberDev != null ? Doc.MarketDirectSberDev.Name.ToString() : "";
        
          List<Sungero.POST.Structures.Module.IStructCalculation> Calculation = new List<Sungero.POST.Structures.Module.IStructCalculation>();
          if (Doc.CalculationBaseSberDev.Count > 0)
          {
            foreach (var elem in Doc.CalculationBaseSberDev)
            {
              var newstr2 = Sungero.POST.Structures.Module.StructCalculation.Create();
              newstr2.AbsoluteCalc = elem.AbsoluteCalc != null ? elem.AbsoluteCalc.Value.ToString() : "";
              newstr2.AggregationCalc = elem.AggregationCalc != null ? elem.AggregationCalc.ToString() : "";
              newstr2.InterestCalc = elem.InterestCalc != null ? elem.InterestCalc.Value.ToString() : "";
              newstr2.PercentCalc = elem.PercentCalc != null ? elem.PercentCalc.Value.ToString() : "";
              newstr2.ProductCalc = elem.ProductCalc != null ? elem.ProductCalc.Name.ToString() : "";
              Calculation.Add(newstr2);
            } 
          }
          ContractSt.CalculationBaseSberDev = Calculation;
        
          ContractSt.CalculationFlagBaseSberDev = Doc.CalculationFlagBaseSberDev != null ? Doc.CalculationFlagBaseSberDev.Value.ToString() : "";
          ContractSt.CalculationAmountBaseSberDev = Doc.CalculationAmountBaseSberDev != null ? Doc.CalculationAmountBaseSberDev.Value.ToString() : "";
          ContractSt.CalculationDistributeBaseSberDev = Doc.CalculationDistributeBaseSberDev != null ? Doc.CalculationDistributeBaseSberDev.Value.ToString() : "";
          ContractSt.CalculationResidualAmountBaseSberDev = Doc.CalculationResidualAmountBaseSberDev != null ? Doc.CalculationResidualAmountBaseSberDev.Value.ToString() : "";
          ContractSt.NoticeSendBaseSberDev = Doc.NoticeSendBaseSberDev != null ? Doc.NoticeSendBaseSberDev.Value.ToString() : "";
          ContractSt.BudItemBaseSberDev = Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Name.ToString() : "";
          ContractSt.MarketingSberDev = Doc.MarketingSberDev != null ? Doc.MarketingSberDev.Name.ToString() : "";
          ContractSt.MarketingIDSberDev = Doc.MarketingIDSberDev != null ? Doc.MarketingIDSberDev.ToString() : "";
          ContractSt.ModifiedSberDev = Doc.ModifiedSberDev != null ? Doc.ModifiedSberDev.Value.ToString() : "";
          ContractSt.EstPaymentDateSberDev = Doc.EstPaymentDateSberDev != null ? Doc.EstPaymentDateSberDev.ToString() : "";
          ContractSt.Author = Doc.Author != null ? Doc.Author.Name.ToString() : "";
          ContractSt.Created = Doc.Created != null ? Doc.Created.Value.ToString() : "";
          ContractSt.Modified = Doc.Modified != null ? Doc.Modified.Value.ToString() : "";
          ContractSt.RegistrationNumber = Doc.RegistrationNumber != null ? Doc.RegistrationNumber.ToString() : "";
          ContractSt.Index = Doc.Index != null ? Doc.Index.Value.ToString() : "";
          ContractSt.RegistrationDate = Doc.RegistrationDate != null ? Doc.RegistrationDate.Value.ToString() : "";
          ContractSt.DocumentRegister = Doc.DocumentRegister != null ? Doc.DocumentRegister.ToString() : "";
          ContractSt.DeliveryMethod = Doc.DeliveryMethod != null ? Doc.DeliveryMethod.Name.ToString() : "";
          ContractSt.IsReturnRequired = Doc.IsReturnRequired != null ? Doc.IsReturnRequired.Value.ToString() : "";
          ContractSt.IsHeldByCounterParty = Doc.IsHeldByCounterParty != null ? Doc.IsHeldByCounterParty.Value.ToString() : "";
          ContractSt.DeliveredTo = Doc.DeliveredTo != null ? Doc.DeliveredTo.Name.ToString() : "";
          ContractSt.ReturnDeadline = Doc.ReturnDeadline != null ? Doc.ReturnDeadline.Value.ToString() : "";
          ContractSt.ReturnDate = Doc.ReturnDate != null ? Doc.ReturnDate.Value.ToString() : "";
          ContractSt.LifeCycleState = Doc.LifeCycleState != null ? Doc.LifeCycleState.Value.ToString() : "";
          ContractSt.RegistrationState = Doc.RegistrationState != null ? Doc.RegistrationState.Value.ToString() : "";
          ContractSt.InternalApprovalState = Doc.InternalApprovalState != null ? Doc.InternalApprovalState.Value.ToString() : "";
          ContractSt.ExternalApprovalState = Doc.ExternalApprovalState != null ? Doc.ExternalApprovalState.Value.ToString() : "";
          ContractSt.ExecutionState = Doc.ExecutionState != null ? Doc.ExecutionState.Value.ToString() : "";
          ContractSt.ControlExecutionState = Doc.ControlExecutionState != null ? Doc.ControlExecutionState.Value.ToString() : "";
          ContractSt.LocationState = Doc.LocationState != null ? Doc.LocationState.ToString() : "";
          ContractSt.ResponsibleForReturnEmployee = Doc.ResponsibleForReturnEmployee != null ? Doc.ResponsibleForReturnEmployee.Name.ToString() : "";
          ContractSt.ScheduledReturnDateFromCounterparty = Doc.ScheduledReturnDateFromCounterparty != null ? Doc.ScheduledReturnDateFromCounterparty.Value.ToString() : "";
          ContractSt.Note = Doc.Note != null ? Doc.Note.ToString() : "";
          ContractSt.Assignee = Doc.Assignee != null ? Doc.Assignee.Name.ToString() : "";
          ContractSt.PreparedBy = Doc.PreparedBy != null ? Doc.PreparedBy.Name.ToString() : "";
          ContractSt.TotalAmount = Doc.TotalAmount != null ? Doc.TotalAmount.Value.ToString() : "";
          ContractSt.VatRate = Doc.VatRate != null ? Doc.VatRate.Name.ToString() : "";
          ContractSt.VatAmount = Doc.VatAmount != null ? Doc.VatAmount.Value.ToString() : "";
          ContractSt.NetAmount = Doc.NetAmount != null ? Doc.NetAmount.Value.ToString() : "";
          ContractSt.Currency = Doc.Currency != null ? Doc.Currency.Name.ToString() : "";
          
          RequestElem.Add(ContractSt);
        }
        return RequestElem;
      }
      else
        return RequestElem;
    }

    /// <summary>
    /// Функция передачи допсоглашений
    /// </summary>
    [Public(WebApiRequestType = RequestType.Get)]
    public List<Structures.Module.IStructSupAgreement> SupAgreements(int iddoc)
    {
      var SupAgreements = sberdev.SBContracts.SupAgreements.GetAll();
      List<Structures.Module.IStructSupAgreement> RequestElem = new List<Sungero.POST.Structures.Module.IStructSupAgreement>(); //Structures.Module.StructContractList.Create();
      if (iddoc == 0)
      {
        foreach (var Doc in SupAgreements)
        {
          var StrSupAgreement = Structures.Module.StructSupAgreement.Create();
          StrSupAgreement.Id =  Doc.Id.ToString();
          StrSupAgreement.Name =  Doc.Name != null ? Doc.Name.ToString()  : "";
          StrSupAgreement.Created =  Doc.Created != null ? Doc.Created.Value.ToString()  : "";
          StrSupAgreement.TotalAmount =  Doc.TotalAmount != null ? Doc.TotalAmount.Value.ToString()  : "";
          StrSupAgreement.ValidFrom =  Doc.ValidFrom != null ? Doc.ValidFrom.Value.ToString()  : "";
          StrSupAgreement.ValidTill =  Doc.ValidTill != null ? Doc.ValidTill.Value.ToString()  : "";
          //StrSupAgreement.MVZOldSberDevSDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.ToString()  : "";
          //StrSupAgreement.AccArtExOldSberDevSDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.ToString()  : "";
          //StrSupAgreement.MVPOldSberDevSDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.ToString()  : "";
          //StrSupAgreement.AccArtPrOldSberDevSDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.ToString()  : "";
          //StrSupAgreement.BudItemOldSberDevSDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.ToString()  : "";
          //StrSupAgreement.ContrTypeOldSberDevSDev =  Doc.ContrTypeOldSberDev != null ? Doc.ContrTypeOldSberDev.ToString()  : "";
          //StrSupAgreement.OriginalOldSberDevSDev =  Doc.OriginalOldSberDev != null ? Doc.OriginalOldSberDev.ToString()  : "";
          //StrSupAgreement.SigningSDev =  Doc.Signing != null ? Doc.Signing.Value.ToString()  : "";
          //StrSupAgreement.DeliveryInfoOldSberDevSDev =  Doc.DeliveryInfoOldSberDev != null ? Doc.DeliveryInfoOldSberDev.ToString()  : "";
          //StrSupAgreement.NoticeSendOldSberDevSDev =  Doc.NoticeSendOldSberDev != null ? Doc.NoticeSendOldSberDev.ToString()  : "";
          //StrSupAgreement.FrameworkOldSberDevSDev =  Doc.FrameworkOldSberDev != null ? Doc.FrameworkOldSberDev.ToString()  : "";
          StrSupAgreement.SDSFSberDevSDev =  Doc.SDSFSberDev != null ? Doc.SDSFSberDev.ToString()  : "";
          StrSupAgreement.SRSberDevSDev =  Doc.SRSberDev != null ? Doc.SRSberDev.ToString()  : "";
          StrSupAgreement.GoogleDocsLinkSberDevSDev =  Doc.GoogleDocsLinkSberDev != null ? Doc.GoogleDocsLinkSberDev.ToString()  : "";
          StrSupAgreement.SubjectSpecificationSberDevSDev =  Doc.SubjectSpecificationSberDev != null ? Doc.SubjectSpecificationSberDev.ToString()  : "";
          StrSupAgreement.AccArtExBaseSberDev =  Doc.AccArtExBaseSberDev != null ? Doc.AccArtExBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.AccArtExOldSberDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrBaseSberDev =  Doc.AccArtPrBaseSberDev != null ? Doc.AccArtPrBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.AccArtPrOldSberDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.Id.ToString()  : "";
          StrSupAgreement.Assignee =  Doc.Assignee != null ? Doc.Assignee.Id.ToString()  : "";
          StrSupAgreement.AssociatedApplication =  Doc.AssociatedApplication != null ? Doc.AssociatedApplication.Id.ToString()  : "";
          StrSupAgreement.Author =  Doc.Author != null ? Doc.Author.Id.ToString()  : "";
          StrSupAgreement.BudItemBaseSberDev =  Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.BudItemOldSberDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.Id.ToString()  : "";
          StrSupAgreement.BusinessUnit =  Doc.BusinessUnit != null ? Doc.BusinessUnit.Id.ToString()  : "";
          StrSupAgreement.CalculationBaseSberDev = "";
          StrSupAgreement.CaseFile =  Doc.CaseFile != null ? Doc.CaseFile.Id.ToString()  : "";
          StrSupAgreement.Contact =  Doc.Contact != null ? Doc.Contact.Id.ToString()  : "";
          StrSupAgreement.Counterparty =  Doc.Counterparty != null ? Doc.Counterparty.Id.ToString()  : "";
          StrSupAgreement.CounterpartySignatory =  Doc.CounterpartySignatory != null ? Doc.CounterpartySignatory.Id.ToString()  : "";
          StrSupAgreement.Currency =  Doc.Currency != null ? Doc.Currency.Id.ToString()  : "";
          StrSupAgreement.DeliveredTo =  Doc.DeliveredTo != null ? Doc.DeliveredTo.Id.ToString()  : "";
          StrSupAgreement.DeliveryMethod =  Doc.DeliveryMethod != null ? Doc.DeliveryMethod.Id.ToString()  : "";
          StrSupAgreement.Department =  Doc.Department != null ? Doc.Department.Id.ToString()  : "";
          StrSupAgreement.DirectionMVZ = "";
          StrSupAgreement.DocumentGroup =  Doc.DocumentGroup != null ? Doc.DocumentGroup.Id.ToString()  : "";
          StrSupAgreement.DocumentKind =  Doc.DocumentKind != null ? Doc.DocumentKind.Id.ToString()  : "";
          StrSupAgreement.DocumentRegister =  Doc.DocumentRegister != null ? Doc.DocumentRegister.Id.ToString()  : "";
          StrSupAgreement.LeadingDocument =  Doc.LeadingDocument != null ? Doc.LeadingDocument.Id.ToString()  : "";
          StrSupAgreement.MarketDirectSberDev =  Doc.MarketDirectSberDev != null ? Doc.MarketDirectSberDev.Id.ToString()  : "";
          StrSupAgreement.Milestones = "";
          StrSupAgreement.MVPBaseSberDev =  Doc.MVPBaseSberDev != null ? Doc.MVPBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.MVPOldSberDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZBaseSberDev =  Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.MVZOldSberDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.Id.ToString()  : "";
          StrSupAgreement.OurSignatory =  Doc.OurSignatory != null ? Doc.OurSignatory.Id.ToString()  : "";
          StrSupAgreement.OurSigningReason =  Doc.OurSigningReason != null ? Doc.OurSigningReason.Id.ToString()  : "";
          StrSupAgreement.Parameters = "";
          StrSupAgreement.PreparedBy =  Doc.PreparedBy != null ? Doc.PreparedBy.Id.ToString()  : "";
          StrSupAgreement.ProdCollectionExBaseSberDev =  "";
          StrSupAgreement.ProdCollectionPrBaseSberDev =  "";
          StrSupAgreement.ProdSberDevCollection =  "";
          StrSupAgreement.Project =  Doc.Project != null ? Doc.Project.Id.ToString()  : "";
          StrSupAgreement.ResponsibleEmployee =  Doc.ResponsibleEmployee != null ? Doc.ResponsibleEmployee.Id.ToString()  : "";
          StrSupAgreement.ResponsibleForReturnEmployee =  Doc.ResponsibleForReturnEmployee != null ? Doc.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrSupAgreement.Subtopic =  Doc.Subtopic != null ? Doc.Subtopic.Id.ToString()  : "";
          StrSupAgreement.Topic =  Doc.Topic != null ? Doc.Topic.Id.ToString()  : "";
          StrSupAgreement.Tracking = "";
          StrSupAgreement.VatRate =  Doc.VatRate != null ? Doc.VatRate.Id.ToString()  : "";
          StrSupAgreement.Versions =  "";
          StrSupAgreement.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/265f2c57-6a8a-4a15-833b-ca00e8047fa5/" + Doc.Id.ToString();
          
          RequestElem.Add(StrSupAgreement);
        }
        return RequestElem;
      }
      else
      {
        var Doc = sberdev.SBContracts.SupAgreements.Get(long.Parse(iddoc.ToString()));
        var StrSupAgreement = Structures.Module.StructSupAgreement.Create();
        StrSupAgreement.Id =  Doc.Id.ToString();
          StrSupAgreement.Name =  Doc.Name != null ? Doc.Name.ToString()  : "";
          StrSupAgreement.Created =  Doc.Created != null ? Doc.Created.Value.ToString()  : "";
          StrSupAgreement.TotalAmount =  Doc.TotalAmount != null ? Doc.TotalAmount.Value.ToString()  : "";
          StrSupAgreement.ValidFrom =  Doc.ValidFrom != null ? Doc.ValidFrom.Value.ToString()  : "";
          StrSupAgreement.ValidTill =  Doc.ValidTill != null ? Doc.ValidTill.Value.ToString()  : "";
          //StrSupAgreement.MVZOldSberDevSDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.ToString()  : "";
          //StrSupAgreement.AccArtExOldSberDevSDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.ToString()  : "";
          //StrSupAgreement.MVPOldSberDevSDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.ToString()  : "";
          //StrSupAgreement.AccArtPrOldSberDevSDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.ToString()  : "";
          //StrSupAgreement.BudItemOldSberDevSDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.ToString()  : "";
          //StrSupAgreement.ContrTypeOldSberDevSDev =  Doc.ContrTypeOldSberDev != null ? Doc.ContrTypeOldSberDev.ToString()  : "";
          //StrSupAgreement.OriginalOldSberDevSDev =  Doc.OriginalOldSberDev != null ? Doc.OriginalOldSberDev.ToString()  : "";
          //StrSupAgreement.SigningSDev =  Doc.Signing != null ? Doc.Signing.Value.ToString()  : "";
          //StrSupAgreement.DeliveryInfoOldSberDevSDev =  Doc.DeliveryInfoOldSberDev != null ? Doc.DeliveryInfoOldSberDev.ToString()  : "";
          //StrSupAgreement.NoticeSendOldSberDevSDev =  Doc.NoticeSendOldSberDev != null ? Doc.NoticeSendOldSberDev.ToString()  : "";
          //StrSupAgreement.FrameworkOldSberDevSDev =  Doc.FrameworkOldSberDev != null ? Doc.FrameworkOldSberDev.ToString()  : "";
          StrSupAgreement.SDSFSberDevSDev =  Doc.SDSFSberDev != null ? Doc.SDSFSberDev.ToString()  : "";
          StrSupAgreement.SRSberDevSDev =  Doc.SRSberDev != null ? Doc.SRSberDev.ToString()  : "";
          StrSupAgreement.GoogleDocsLinkSberDevSDev =  Doc.GoogleDocsLinkSberDev != null ? Doc.GoogleDocsLinkSberDev.ToString()  : "";
          StrSupAgreement.SubjectSpecificationSberDevSDev =  Doc.SubjectSpecificationSberDev != null ? Doc.SubjectSpecificationSberDev.ToString()  : "";
          StrSupAgreement.AccArtExBaseSberDev =  Doc.AccArtExBaseSberDev != null ? Doc.AccArtExBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.AccArtExOldSberDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrBaseSberDev =  Doc.AccArtPrBaseSberDev != null ? Doc.AccArtPrBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.AccArtPrOldSberDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.Id.ToString()  : "";
          StrSupAgreement.Assignee =  Doc.Assignee != null ? Doc.Assignee.Id.ToString()  : "";
          StrSupAgreement.AssociatedApplication =  Doc.AssociatedApplication != null ? Doc.AssociatedApplication.Id.ToString()  : "";
          StrSupAgreement.Author =  Doc.Author != null ? Doc.Author.Id.ToString()  : "";
          StrSupAgreement.BudItemBaseSberDev =  Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.BudItemOldSberDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.Id.ToString()  : "";
          StrSupAgreement.BusinessUnit =  Doc.BusinessUnit != null ? Doc.BusinessUnit.Id.ToString()  : "";
          StrSupAgreement.CalculationBaseSberDev = "";
          StrSupAgreement.CaseFile =  Doc.CaseFile != null ? Doc.CaseFile.Id.ToString()  : "";
          StrSupAgreement.Contact =  Doc.Contact != null ? Doc.Contact.Id.ToString()  : "";
          StrSupAgreement.Counterparty =  Doc.Counterparty != null ? Doc.Counterparty.Id.ToString()  : "";
          StrSupAgreement.CounterpartySignatory =  Doc.CounterpartySignatory != null ? Doc.CounterpartySignatory.Id.ToString()  : "";
          StrSupAgreement.Currency =  Doc.Currency != null ? Doc.Currency.Id.ToString()  : "";
          StrSupAgreement.DeliveredTo =  Doc.DeliveredTo != null ? Doc.DeliveredTo.Id.ToString()  : "";
          StrSupAgreement.DeliveryMethod =  Doc.DeliveryMethod != null ? Doc.DeliveryMethod.Id.ToString()  : "";
          StrSupAgreement.Department =  Doc.Department != null ? Doc.Department.Id.ToString()  : "";
          StrSupAgreement.DirectionMVZ = "";
          StrSupAgreement.DocumentGroup =  Doc.DocumentGroup != null ? Doc.DocumentGroup.Id.ToString()  : "";
          StrSupAgreement.DocumentKind =  Doc.DocumentKind != null ? Doc.DocumentKind.Id.ToString()  : "";
          StrSupAgreement.DocumentRegister =  Doc.DocumentRegister != null ? Doc.DocumentRegister.Id.ToString()  : "";
          StrSupAgreement.LeadingDocument =  Doc.LeadingDocument != null ? Doc.LeadingDocument.Id.ToString()  : "";
          StrSupAgreement.MarketDirectSberDev =  Doc.MarketDirectSberDev != null ? Doc.MarketDirectSberDev.Id.ToString()  : "";
          StrSupAgreement.Milestones = "";
          StrSupAgreement.MVPBaseSberDev =  Doc.MVPBaseSberDev != null ? Doc.MVPBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.MVPOldSberDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZBaseSberDev =  Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Id.ToString()  : "";
          //StrSupAgreement.MVZOldSberDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.Id.ToString()  : "";
          StrSupAgreement.OurSignatory =  Doc.OurSignatory != null ? Doc.OurSignatory.Id.ToString()  : "";
          StrSupAgreement.OurSigningReason =  Doc.OurSigningReason != null ? Doc.OurSigningReason.Id.ToString()  : "";
          StrSupAgreement.Parameters = "";
          StrSupAgreement.PreparedBy =  Doc.PreparedBy != null ? Doc.PreparedBy.Id.ToString()  : "";
          StrSupAgreement.ProdCollectionExBaseSberDev =  "";
          StrSupAgreement.ProdCollectionPrBaseSberDev =  "";
          StrSupAgreement.ProdSberDevCollection =  "";
          StrSupAgreement.Project =  Doc.Project != null ? Doc.Project.Id.ToString()  : "";
          StrSupAgreement.ResponsibleEmployee =  Doc.ResponsibleEmployee != null ? Doc.ResponsibleEmployee.Id.ToString()  : "";
          StrSupAgreement.ResponsibleForReturnEmployee =  Doc.ResponsibleForReturnEmployee != null ? Doc.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrSupAgreement.Subtopic =  Doc.Subtopic != null ? Doc.Subtopic.Id.ToString()  : "";
          StrSupAgreement.Topic =  Doc.Topic != null ? Doc.Topic.Id.ToString()  : "";
          StrSupAgreement.Tracking = "";
          StrSupAgreement.VatRate =  Doc.VatRate != null ? Doc.VatRate.Id.ToString()  : "";
          StrSupAgreement.Versions =  "";
          StrSupAgreement.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/265f2c57-6a8a-4a15-833b-ca00e8047fa5/" + Doc.Id.ToString();
        RequestElem.Add(StrSupAgreement);
        return RequestElem;
      }
    }

    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
    [Public(WebApiRequestType = RequestType.Get)]
    public List<Structures.Module.IStructContract> Contracts(int iddoc) 
    {
      var Contracts = sberdev.SBContracts.Contracts.GetAll();
      List<Structures.Module.IStructContract> RequestElem = new List<Sungero.POST.Structures.Module.IStructContract>(); //Structures.Module.StructContractList.Create();
      if (iddoc == 0)
      {
        foreach (var Dog in Contracts)
        {
          var StrContract = Structures.Module.StructContract.Create();
          StrContract.ID = Dog.Id.ToString(); 
          StrContract.Name = Dog.Name; 
          StrContract.Created = Dog.Created.Value.ToString();
          StrContract.DocNum = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString()  : "";
          StrContract.DocDate = Dog.RegistrationDate != null ? Dog.RegistrationDate.Value.ToString() : "";
          StrContract.TotalAmount = Dog.TotalAmount != null ? Dog.TotalAmount.ToString() : "";
          StrContract.ValidFrom = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString() : "";
          StrContract.ValidTill = Dog.ValidTill != null ? Dog.ValidTill.Value.ToString() : "";
          StrContract.AccArtExBaseSberDev = Dog.AccArtExBaseSberDev != null ? Dog.AccArtExBaseSberDev.Id.ToString()  : "";
          //StrContract.AccArtMVZOldSberDev = Dog.AccArtMVZOldSberDev != null ? Dog.AccArtMVZOldSberDev.Id.ToString()  : "";
          StrContract.AccArtPrBaseSberDev = Dog.AccArtPrBaseSberDev != null ? Dog.AccArtPrBaseSberDev.Id.ToString()  : "";
          //StrContract.AccArtsberdevOldSberDev = Dog.AccArtsberdevOldSberDev != null ? Dog.AccArtsberdevOldSberDev.Id.ToString()  : "";
          StrContract.Assignee = Dog.Assignee != null ? Dog.Assignee.Id.ToString()  : "";
          StrContract.AssociatedApplication = Dog.AssociatedApplication != null ? Dog.AssociatedApplication.Id.ToString()  : "";
          StrContract.Author = Dog.Author != null ? Dog.Author.Id.ToString()  : "";
          StrContract.BudItemBaseSberDev = Dog.BudItemBaseSberDev != null ? Dog.BudItemBaseSberDev.Id.ToString()  : "";
          //StrContract.BudItemsberdevOldSberDev = Dog.BudItemsberdevOldSberDev != null ? Dog.BudItemsberdevOldSberDev.Id.ToString()  : "";
          StrContract.BusinessUnit = Dog.BusinessUnit != null ? Dog.BusinessUnit.Id.ToString()  : "";
          StrContract.CalculationBaseSberDev = "";
          StrContract.CaseFile = Dog.CaseFile != null ? Dog.CaseFile.Id.ToString()  : "";
          StrContract.CollectionProperty = "";
          StrContract.Contact = Dog.Contact != null ? Dog.Contact.Id.ToString()  : "";
          StrContract.Counterparty = Dog.Counterparty != null ? Dog.Counterparty.Id.ToString()  : "";
          StrContract.CounterpartySignatory = Dog.CounterpartySignatory != null ? Dog.CounterpartySignatory.Id.ToString()  : "";
          StrContract.Currency = Dog.Currency != null ? Dog.Currency.Id.ToString()  : "";
          StrContract.DeliveredTo = Dog.DeliveredTo != null ? Dog.DeliveredTo.Id.ToString()  : "";
          StrContract.DeliveryMethod = Dog.DeliveryMethod != null ? Dog.DeliveryMethod.Id.ToString()  : "";
          StrContract.Department = Dog.Department != null ? Dog.Department.Id.ToString()  : "";
          //StrContract.DirectionMVZ = Dog.DirectionMVZ != null ? Dog.DirectionMVZ.ToString()  : "";
          StrContract.DocumentGroup = Dog.DocumentGroup != null ? Dog.DocumentGroup.Id.ToString()  : "";
          StrContract.DocumentKind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString()  : "";
          StrContract.DocumentRegister = Dog.DocumentRegister != null ? Dog.DocumentRegister.Id.ToString()  : "";
          StrContract.LeadingDocument = Dog.LeadingDocument != null ? Dog.LeadingDocument.Id.ToString()  : "";
          StrContract.MarketDirectSberDev = Dog.MarketDirectSberDev != null ? Dog.MarketDirectSberDev.Id.ToString()  : "";
          StrContract.Milestones = "";
          StrContract.MVPBaseSberDev = Dog.MVPBaseSberDev != null ? Dog.MVPBaseSberDev.Id.ToString()  : "";
          //StrContract.MVPsberdevOldSberDev = Dog.MVPsberdevOldSberDev != null ? Dog.MVPsberdevOldSberDev.Id.ToString()  : "";
          StrContract.MVZBaseSberDev = Dog.MVZBaseSberDev != null ? Dog.MVZBaseSberDev.Id.ToString()  : "";
          //StrContract.MVZsberdevOldSberDev = Dog.MVZsberdevOldSberDev != null ? Dog.MVZsberdevOldSberDev.Id.ToString()  : "";
          StrContract.OurSignatory = Dog.OurSignatory != null ? Dog.OurSignatory.Id.ToString()  : "";
          StrContract.OurSigningReason = Dog.OurSigningReason != null ? Dog.OurSigningReason.Id.ToString()  : "";
          StrContract.Parameters = "";
          StrContract.PreparedBy = Dog.PreparedBy != null ? Dog.PreparedBy.Id.ToString()  : "";
          StrContract.ProdCollectionExBaseSberDev = "";
          StrContract.ProdCollectionPrBaseSberDev = "";
          StrContract.Project = Dog.Project != null ? Dog.Project.Id.ToString()  : "";
          StrContract.ResponsibleEmployee = Dog.ResponsibleEmployee != null ? Dog.ResponsibleEmployee.Id.ToString()  : "";
          StrContract.ResponsibleForReturnEmployee = Dog.ResponsibleForReturnEmployee != null ? Dog.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrContract.Subtopic = Dog.Subtopic != null ? Dog.Subtopic.Id.ToString()  : "";
          StrContract.Topic = Dog.Topic != null ? Dog.Topic.Id.ToString()  : "";
          StrContract.Tracking = "";
          StrContract.VatRate = Dog.VatRate != null ? Dog.VatRate.Id.ToString()  : "";
          StrContract.Versions = "";
          StrContract.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/f37c7e63-b134-4446-9b5b-f8811f6c9666/" + Dog.Id.ToString();
          
          RequestElem.Add(StrContract);
        }
        return RequestElem;
      }
      else
      {
        var Dog = sberdev.SBContracts.Contracts.Get(long.Parse(iddoc.ToString()));
        var StrContract = Structures.Module.StructContract.Create();
          StrContract.ID = Dog.Id.ToString(); 
          StrContract.Name = Dog.Name; 
          StrContract.Created = Dog.Created.Value.ToString();
          StrContract.DocNum = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString()  : "";
          StrContract.DocDate = Dog.RegistrationDate != null ? Dog.RegistrationDate.Value.ToString() : "";
          StrContract.TotalAmount = Dog.TotalAmount != null ? Dog.TotalAmount.ToString() : "";
          StrContract.ValidFrom = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString() : "";
          StrContract.ValidTill = Dog.ValidTill != null ? Dog.ValidTill.Value.ToString() : "";
          StrContract.AccArtExBaseSberDev = Dog.AccArtExBaseSberDev != null ? Dog.AccArtExBaseSberDev.Id.ToString()  : "";
          //StrContract.AccArtMVZOldSberDev = Dog.AccArtMVZOldSberDev != null ? Dog.AccArtMVZOldSberDev.Id.ToString()  : "";
          StrContract.AccArtPrBaseSberDev = Dog.AccArtPrBaseSberDev != null ? Dog.AccArtPrBaseSberDev.Id.ToString()  : "";
          //StrContract.AccArtsberdevOldSberDev = Dog.AccArtsberdevOldSberDev != null ? Dog.AccArtsberdevOldSberDev.Id.ToString()  : "";
          StrContract.Assignee = Dog.Assignee != null ? Dog.Assignee.Id.ToString()  : "";
          StrContract.AssociatedApplication = Dog.AssociatedApplication != null ? Dog.AssociatedApplication.Id.ToString()  : "";
          StrContract.Author = Dog.Author != null ? Dog.Author.Id.ToString()  : "";
          StrContract.BudItemBaseSberDev = Dog.BudItemBaseSberDev != null ? Dog.BudItemBaseSberDev.Id.ToString()  : "";
          //StrContract.BudItemsberdevOldSberDev = Dog.BudItemsberdevOldSberDev != null ? Dog.BudItemsberdevOldSberDev.Id.ToString()  : "";
          StrContract.BusinessUnit = Dog.BusinessUnit != null ? Dog.BusinessUnit.Id.ToString()  : "";
          StrContract.CalculationBaseSberDev = "";
          StrContract.CaseFile = Dog.CaseFile != null ? Dog.CaseFile.Id.ToString()  : "";
          StrContract.CollectionProperty = "";
          StrContract.Contact = Dog.Contact != null ? Dog.Contact.Id.ToString()  : "";
          StrContract.Counterparty = Dog.Counterparty != null ? Dog.Counterparty.Id.ToString()  : "";
          StrContract.CounterpartySignatory = Dog.CounterpartySignatory != null ? Dog.CounterpartySignatory.Id.ToString()  : "";
          StrContract.Currency = Dog.Currency != null ? Dog.Currency.Id.ToString()  : "";
          StrContract.DeliveredTo = Dog.DeliveredTo != null ? Dog.DeliveredTo.Id.ToString()  : "";
          StrContract.DeliveryMethod = Dog.DeliveryMethod != null ? Dog.DeliveryMethod.Id.ToString()  : "";
          StrContract.Department = Dog.Department != null ? Dog.Department.Id.ToString()  : "";
          //StrContract.DirectionMVZ = Dog.DirectionMVZ != null ? Dog.DirectionMVZ.ToString()  : "";
          StrContract.DocumentGroup = Dog.DocumentGroup != null ? Dog.DocumentGroup.Id.ToString()  : "";
          StrContract.DocumentKind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString()  : "";
          StrContract.DocumentRegister = Dog.DocumentRegister != null ? Dog.DocumentRegister.Id.ToString()  : "";
          StrContract.LeadingDocument = Dog.LeadingDocument != null ? Dog.LeadingDocument.Id.ToString()  : "";
          StrContract.MarketDirectSberDev = Dog.MarketDirectSberDev != null ? Dog.MarketDirectSberDev.Id.ToString()  : "";
          StrContract.Milestones = "";
          StrContract.MVPBaseSberDev = Dog.MVPBaseSberDev != null ? Dog.MVPBaseSberDev.Id.ToString()  : "";
          //StrContract.MVPsberdevOldSberDev = Dog.MVPsberdevOldSberDev != null ? Dog.MVPsberdevOldSberDev.Id.ToString()  : "";
          StrContract.MVZBaseSberDev = Dog.MVZBaseSberDev != null ? Dog.MVZBaseSberDev.Id.ToString()  : "";
          //StrContract.MVZsberdevOldSberDev = Dog.MVZsberdevOldSberDev != null ? Dog.MVZsberdevOldSberDev.Id.ToString()  : "";
          StrContract.OurSignatory = Dog.OurSignatory != null ? Dog.OurSignatory.Id.ToString()  : "";
          StrContract.OurSigningReason = Dog.OurSigningReason != null ? Dog.OurSigningReason.Id.ToString()  : "";
          StrContract.Parameters = "";
          StrContract.PreparedBy = Dog.PreparedBy != null ? Dog.PreparedBy.Id.ToString()  : "";
          StrContract.ProdCollectionExBaseSberDev = "";
          StrContract.ProdCollectionPrBaseSberDev = "";
          StrContract.Project = Dog.Project != null ? Dog.Project.Id.ToString()  : "";
          StrContract.ResponsibleEmployee = Dog.ResponsibleEmployee != null ? Dog.ResponsibleEmployee.Id.ToString()  : "";
          StrContract.ResponsibleForReturnEmployee = Dog.ResponsibleForReturnEmployee != null ? Dog.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrContract.Subtopic = Dog.Subtopic != null ? Dog.Subtopic.Id.ToString()  : "";
          StrContract.Topic = Dog.Topic != null ? Dog.Topic.Id.ToString()  : "";
          StrContract.Tracking = "";
          StrContract.VatRate = Dog.VatRate != null ? Dog.VatRate.Id.ToString()  : "";
          StrContract.Versions = "";
          StrContract.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/f37c7e63-b134-4446-9b5b-f8811f6c9666/" + Dog.Id.ToString();

          RequestElem.Add(StrContract);
          return RequestElem;
      }
    }

    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
   [Public(WebApiRequestType = RequestType.Post)]
    public List<Sungero.POST.Structures.Module.IMarcetingDoc> PostMarcetingDocuments(string IDDoc)
    {
      if (IDDoc != "")
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        POST.Structures.Module.IMarcetingDoc Struct = POST.Structures.Module.MarcetingDoc.Create();
        var Doc = Sungero.Custom.MarcetingDocs.GetAll(d => d.Id == int.Parse(IDDoc)).FirstOrDefault();
        if (Doc != null)
        {
          Struct.Name = Doc.Name.ToString();
          Struct.Kind = Doc.DocumentKind.ToString();
          Struct.ActionCode = Doc.ActionCode.Value.ToString();
          Struct.ActionName = Doc.ActionName.ToString();
          Struct.BudgetItem = Doc.BudgetItem.DisplayValue.ToString();
          Struct.StartDate = Doc.StartDate.Value.ToString();
          Struct.EndDate = Doc.EndDate.Value.ToString();
          Struct.MarketingKind = Doc.MarketingKind.DisplayValue.ToString();
          Struct.MarketingSubKind = Doc.MarketingSubKind.DisplayValue.ToString();
          Struct.OwnerEmployee = Doc.OwnerEmployee.DisplayValue.ToString();
          Struct.PlanKolInt = Doc.PlanKolInt.Value.ToString();
          Struct.PlannedRashod = Doc.PlannedRashod.Value.ToString();
          Struct.PlannedRashodCurrency = Doc.PlanRashCurrency != null ? Doc.PlanRashCurrency.NumericCode.ToString() : "";
          Struct.PlannedSumm = Doc.PlannedSummRub.Value.ToString();
          Struct.PlannedSummCurrency = Doc.PlanSummCurrency.NumericCode.ToString();
          Struct.FacktKolInt = Doc.FacktKolInt.HasValue ? Doc.FacktKolInt.Value.ToString() : "";
          Struct.FacktRashod = Doc.FacktRashod.HasValue ? Doc.FacktRashod.Value.ToString() : "";
          Struct.FacktRashodCurrency = Doc.FacktRashCurrency != null ? Doc.FacktRashCurrency.NumericCode.ToString() : "";
          Struct.FacktTotalSumm = Doc.FacktTotalSumm.HasValue ? Doc.FacktTotalSumm.Value.ToString() : "";
          Struct.FacktTotalSummCurrency = Doc.FacktSummCurrency != null ? Doc.FacktSummCurrency.NumericCode.ToString() : "";
          Struct.StagesOfApproval = Doc.StagesOfApproval != null ? Doc.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();
            newstr.PricePromo = elem.PricePromo.Value.ToString();
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();
            DevicesAction.Add(newstr);
          }          
          Struct.DevicesAction = DevicesAction;
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();
          foreach (var elem in Doc.Channels)
          {
            var newchan = Structures.Module.Channels.Create();
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newchan.Channel = elem.Channel.DisplayValue.ToString();
            newchan.Komission = elem.Komission.Value.ToString();
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();
            Channels.Add(newchan);
          }
          Struct.Channels = Channels;
          ListStruct.Add(Struct);
          return ListStruct;
        }
        else // Выдача полного списка документов
        {
          throw new WebException("Выдача полного списка документов");
        }
      }
      else
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct2 = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        var Docs = Sungero.Custom.MarcetingDocs.GetAll().ToList();
        foreach (var Doc2 in Docs)
        {
          Logger.Debug("Прошли 3-1");
          POST.Structures.Module.IMarcetingDoc Struct2 = POST.Structures.Module.MarcetingDoc.Create();
          Struct2.Name = Doc2.Name.ToString();
          Struct2.Kind = Doc2.DocumentKind.ToString();
          Struct2.ActionCode = Doc2.ActionCode.Value.ToString();
          Struct2.ActionName = Doc2.ActionName.ToString();
          Struct2.BudgetItem = Doc2.BudgetItem.DisplayValue.ToString();
          Struct2.StartDate = Doc2.StartDate.Value.ToString();
          Struct2.EndDate = Doc2.EndDate.Value.ToString();
          Struct2.MarketingKind = Doc2.MarketingKind.DisplayValue.ToString();
          Struct2.MarketingSubKind = Doc2.MarketingSubKind.DisplayValue.ToString();
          Struct2.OwnerEmployee = Doc2.OwnerEmployee.DisplayValue.ToString();
          Struct2.PlanKolInt = Doc2.PlanKolInt.Value.ToString();
          Struct2.PlannedRashod = Doc2.PlannedRashod.Value.ToString();
          Struct2.PlannedRashodCurrency = Doc2.PlanRashCurrency != null ? Doc2.PlanRashCurrency.NumericCode.ToString() : "";
          Struct2.PlannedSumm = Doc2.PlannedSummRub.Value.ToString();
          Struct2.PlannedSummCurrency = Doc2.PlanSummCurrency.NumericCode.ToString();
          Struct2.FacktKolInt = Doc2.FacktKolInt.HasValue ? Doc2.FacktKolInt.Value.ToString() : "";
          Struct2.FacktRashod = Doc2.FacktRashod.HasValue ? Doc2.FacktRashod.Value.ToString() : "";
          Struct2.FacktRashodCurrency = Doc2.FacktRashCurrency != null ? Doc2.FacktRashCurrency.NumericCode.ToString() : "";
          Struct2.FacktTotalSumm = Doc2.FacktTotalSumm.HasValue ? Doc2.FacktTotalSumm.Value.ToString() : "";
          Struct2.FacktTotalSummCurrency = Doc2.FacktSummCurrency != null ? Doc2.FacktSummCurrency.NumericCode.ToString() : "";
          Struct2.StagesOfApproval = Doc2.StagesOfApproval != null ? Doc2.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc2.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();                        
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();           
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();                   
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();                    
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();   
            newstr.PricePromo = elem.PricePromo.Value.ToString();                           
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();      
            DevicesAction.Add(newstr);                                                  
          }          
          Struct2.DevicesAction = DevicesAction;                                           
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();     
          foreach (var elem in Doc2.Channels)
          {
                                                                                          
            var newchan = Structures.Module.Channels.Create();                             
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();          
            newchan.Channel = elem.Channel.DisplayValue.ToString();                  
            newchan.Komission = elem.Komission.Value.ToString();                     
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();  
            Channels.Add(newchan);                                                         
          }                                                                                                                                                                        
          Struct2.Channels = Channels;                                                  
          ListStruct2.Add(Struct2);                                                     
        }
        return ListStruct2;
      } 
    }
    
    
    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
   [Public(WebApiRequestType = RequestType.Get)]
    public string GetClosedDocs()
    {
      var listDoc = sberdev.SBContracts.AccountingDocumentBases.GetAll(d => d.MarketingSberDev != null).ToList();
      string spisok = "";
      if (listDoc.Count > 0)
      {
        foreach (var str in listDoc)
        {
          spisok += str.Id.ToString() + ";";
        }
      }
      return spisok;
    }
    
    
    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
   [Public(WebApiRequestType = RequestType.Get)]
    public List<Sungero.POST.Structures.Module.IMarcetingDoc> GetMarcetingDocuments(int IDDoc)
    {
      if (IDDoc != 0)
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        POST.Structures.Module.IMarcetingDoc Struct = POST.Structures.Module.MarcetingDoc.Create();
        int idd = IDDoc;
        var Doc = Sungero.Custom.MarcetingDocs.GetAll(d => d.Id == idd).FirstOrDefault();                
        if (Doc != null)
        {
          Struct.Name = Doc.Name.ToString();
          Struct.Kind = Doc.DocumentKind.ToString();
          Struct.ActionCode = Doc.ActionCode.Value.ToString();
          Struct.ActionName = Doc.ActionName.ToString();
          Struct.BudgetItem = Doc.BudgetItem.DisplayValue.ToString();
          Struct.StartDate = Doc.StartDate.Value.ToString();
          Struct.EndDate = Doc.EndDate.Value.ToString();
          Struct.MarketingKind = Doc.MarketingKind.DisplayValue.ToString();
          Struct.MarketingSubKind = Doc.MarketingSubKind.DisplayValue.ToString();
          Struct.OwnerEmployee = Doc.OwnerEmployee.DisplayValue.ToString();
          Struct.PlanKolInt = Doc.PlanKolInt.Value.ToString();
          Struct.PlannedRashod = Doc.PlannedRashod.Value.ToString();
          Struct.PlannedRashodCurrency = Doc.PlanRashCurrency != null ? Doc.PlanRashCurrency.NumericCode.ToString() : "";
          Struct.PlannedSumm = Doc.PlannedSummRub.Value.ToString();
          Struct.PlannedSummCurrency = Doc.PlanSummCurrency.NumericCode.ToString();
          Struct.FacktKolInt = Doc.FacktKolInt.HasValue ? Doc.FacktKolInt.Value.ToString() : "";
          Struct.FacktRashod = Doc.FacktRashod.HasValue ? Doc.FacktRashod.Value.ToString() : "";
          Struct.FacktRashodCurrency = Doc.FacktRashCurrency != null ? Doc.FacktRashCurrency.NumericCode.ToString() : "";
          Struct.FacktTotalSumm = Doc.FacktTotalSumm.HasValue ? Doc.FacktTotalSumm.Value.ToString() : "";
          Struct.FacktTotalSummCurrency = Doc.FacktSummCurrency != null ? Doc.FacktSummCurrency.NumericCode.ToString() : "";
          Struct.StagesOfApproval = Doc.StagesOfApproval != null ? Doc.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();
            newstr.PricePromo = elem.PricePromo.Value.ToString();
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();
            DevicesAction.Add(newstr);
          }          
          Struct.DevicesAction = DevicesAction;
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();
          foreach (var elem in Doc.Channels)
          {
            var newchan = Structures.Module.Channels.Create();
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newchan.Channel = elem.Channel.DisplayValue.ToString();
            newchan.Komission = elem.Komission.Value.ToString();
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();
            Channels.Add(newchan);
          }
          Struct.Channels = Channels;
          ListStruct.Add(Struct);
          return ListStruct;
        }
        else // Выдача полного списка документов
        {
          return null;
        }
      }
      else
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct2 = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        var Docs = Sungero.Custom.MarcetingDocs.GetAll().ToList();
        foreach (var Doc2 in Docs)
        {
          POST.Structures.Module.IMarcetingDoc Struct2 = POST.Structures.Module.MarcetingDoc.Create();
          Struct2.Name = Doc2.Name.ToString();
          Struct2.Kind = Doc2.DocumentKind.ToString();
          Struct2.ActionCode = Doc2.ActionCode.Value.ToString();
          Struct2.ActionName = Doc2.ActionName.ToString();
          Struct2.BudgetItem = Doc2.BudgetItem.DisplayValue.ToString();
          Struct2.StartDate = Doc2.StartDate.Value.ToString();
          Struct2.EndDate = Doc2.EndDate.Value.ToString();
          Struct2.MarketingKind = Doc2.MarketingKind.DisplayValue.ToString();
          Struct2.MarketingSubKind = Doc2.MarketingSubKind.DisplayValue.ToString();
          Struct2.OwnerEmployee = Doc2.OwnerEmployee.DisplayValue.ToString();
          Struct2.PlanKolInt = Doc2.PlanKolInt.Value.ToString();
          Struct2.PlannedRashod = Doc2.PlannedRashod.Value.ToString();
          Struct2.PlannedRashodCurrency = Doc2.PlanRashCurrency != null ? Doc2.PlanRashCurrency.NumericCode.ToString() : "";
          Struct2.PlannedSumm = Doc2.PlannedSummRub.Value.ToString();
          Struct2.PlannedSummCurrency = Doc2.PlanSummCurrency.NumericCode.ToString();
          Struct2.FacktKolInt = Doc2.FacktKolInt.HasValue ? Doc2.FacktKolInt.Value.ToString() : "";
          Struct2.FacktRashod = Doc2.FacktRashod.HasValue ? Doc2.FacktRashod.Value.ToString() : "";
          Struct2.FacktRashodCurrency = Doc2.FacktRashCurrency != null ? Doc2.FacktRashCurrency.NumericCode.ToString() : "";
          Struct2.FacktTotalSumm = Doc2.FacktTotalSumm.HasValue ? Doc2.FacktTotalSumm.Value.ToString() : "";
          Struct2.FacktTotalSummCurrency = Doc2.FacktSummCurrency != null ? Doc2.FacktSummCurrency.NumericCode.ToString() : "";
          Struct2.StagesOfApproval = Doc2.StagesOfApproval != null ? Doc2.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc2.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();                        
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();           
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();                   
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();                    
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();   
            newstr.PricePromo = elem.PricePromo.Value.ToString();                           
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();      
            DevicesAction.Add(newstr);                                                  
          }          
          Struct2.DevicesAction = DevicesAction;                                           
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();     
          foreach (var elem in Doc2.Channels)
          {
                                                                                          
            var newchan = Structures.Module.Channels.Create();                             
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();          
            newchan.Channel = elem.Channel.DisplayValue.ToString();                  
            newchan.Komission = elem.Komission.Value.ToString();                     
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();  
            Channels.Add(newchan);                                                         
          }                                                                                                                                                                        
          Struct2.Channels = Channels;                                                  
          ListStruct2.Add(Struct2);                                                     
        }
        return ListStruct2;
      } 
    }
    
    /// <summary>
    /// Тестовая функция для проверки коннекта
    /// </summary>
    /// <param name="inputstring">Тестовая строка</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public POST.Structures.Module.IBaseStruct test(string inputstring)
    {
      POST.Structures.Module.IBaseStruct structur = POST.Structures.Module.BaseStruct.Create();
      structur.Name = "Результат";
      structur.Value = "Вы прислали строку: " + inputstring;
      return structur;
    }
    
    private Structures.Module.IBaseStruct AddPS(string name, string evalue)
    {
      var elem = Structures.Module.BaseStruct.Create();    
      elem.Name = name;
      elem.Value = evalue;
      return elem;
    }
    
    #region Выгрузка договора - пакет
    /// <summary>
    /// Выгрузка договора для 1С
    /// </summary>
    /// <param name="GUIDRX">ID Договора</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public Structures.Module.IFormatRequest loadDGD(string GUIDRX)
    {
      int IDCounter = int.Parse(GUIDRX);
      Logger.Debug("ЗАПРОС loadDGD: Начинается поиск документа с ИД = " + IDCounter.ToString() + ".");
      var Dog = Sungero.Contracts.Contracts.GetAll(d => d.Id == IDCounter).FirstOrDefault();
      if (Dog != null)
      {
        var list = new List<Structures.Module.IBaseStruct>();
        var Struct = Structures.Module.ContractRequest.Create();
            Struct.GUIDRX = Dog.Id.ToString();                       //" GUID "
            list.Add(AddPS("GUIDRX",Struct.GUIDRX));            
            Struct.Kind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString() : "Нет";            //" GUID "
            list.Add(AddPS("Kind",Struct.Kind));
            Struct.Name = Dog.Name != null ? Dog.Name.ToString() : "Нет";                       //"текст"	Наименование
            list.Add(AddPS("Name",Struct.Name));
            Struct.Counter = Dog.Counterparty != null ? Dog.Counterparty.Id.ToString() : "Нет";         //"GUID"	Контрагент
            list.Add(AddPS("Counter",Struct.Counter));
            Struct.Buyer = Dog.CounterpartySignatory != null ? Dog.CounterpartySignatory.Id.ToString() : "Нет";  //"GUID"	Покупатель
            list.Add(AddPS("Buyer",Struct.Buyer));
            Struct.NOR = Dog.BusinessUnit != null ? Dog.BusinessUnit.Id.ToString() : "Нет";             //"GUID"	Организация
            list.Add(AddPS("NOR",Struct.NOR));
            Struct.Departament = Dog.Department != null ? Dog.Department.Id.ToString() : "Нет";       //"GUID"	Подразделение
            list.Add(AddPS("Departament",Struct.Departament));
            Struct.Signatory = Dog.OurSignatory != null ? Dog.OurSignatory.Id.ToString() : "Нет";       //"GUID"	Подписал
            list.Add(AddPS("Signatory",Struct.Signatory));
            Struct.Summ = Dog.TotalAmount != null ? Dog.TotalAmount.ToString() : "0";                //"100,00"	Сумма договора фиксирована
            list.Add(AddPS("Summ",Struct.Summ));
            Struct.Currency = Dog.Currency != null ? Dog.Currency.NumericCode.ToString() : "Нет";   //"643"	Валюта
            list.Add(AddPS("Currency",Struct.Currency));
            Struct.Number = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString() : "Нет";          //"2023-СИМ-2233_NEW"	Номер договора
            list.Add(AddPS("Number",Struct.Number));
            Struct.Date = Dog.RegistrationDate != null ? Dog.RegistrationDate.ToString() : "Нет";            //"29.01.2022 0:00:00"	Дата заключения договора
            list.Add(AddPS("Date",Struct.Date));
            string LCS = Dog.LifeCycleState.ToString();
            if (LCS == "Active")
              LCS = "Действует";
            else
            {
              if (LCS == "Closed")
                LCS = "Закрыт";
              else
                LCS = "Не согласован";
            } 
            Struct.Status = LCS;          //"__________"	Статус договора
            list.Add(AddPS("Status",Struct.Status));
            Struct.ValidC = Dog.ValidFrom != null ? Dog.ValidFrom.ToString() : "Нет";          //"09.08.2023 0:00:00"	Действует с
            list.Add(AddPS("ValidC",Struct.ValidC));
            Struct.ValidPO = Dog.ValidTill != null ? Dog.ValidTill.ToString() : "Нет";         //"31.08.2023 0:00:00"	По
            list.Add(AddPS("ValidPO",Struct.ValidPO));
            
      
        var FormatRequest = Structures.Module.FormatRequest.Create();
        FormatRequest.Function = "Create";
        FormatRequest.Sysname = "RX";    
        FormatRequest.Entity = "Contract";       
        FormatRequest.Param = list;
        return FormatRequest;
      }
      else
      {
        Logger.Debug("ЗАПРОС loadDGD: Договор с указанным GUIDID (" + GUIDRX + ") не найден!");
        var FormatRequest = Structures.Module.FormatRequest.Create();
        FormatRequest.Function = "ERROR";
        FormatRequest.Sysname = "RX";    
        FormatRequest.Entity = "Contract"; 
        var list = new List<Structures.Module.IBaseStruct>();        
        FormatRequest.Param = list;
        list.Add(AddPS("Error","Договор с указанным GUIDID не найден!"));
        return FormatRequest;
        //throw new WebException(error.Value);
      } 
    }
    #endregion
    
    #region Выгрузка по счетам
    /// <summary>
    /// Выгрузка счетов для 1С
    /// </summary>
    /// <param name="GUIDRX">ID Договора</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public List<Structures.Module.IStructIncomingInvoices> loadIncomingInvoices(string DateStart, string DateStop)
    {
      var LstII = sberdev.SBContracts.IncomingInvoices.GetAll(i => i.LifeCycleState == sberdev.SBContracts.IncomingInvoice.LifeCycleState.Active).ToList();
      List<Structures.Module.IStructIncomingInvoices> FullList = new List<Sungero.POST.Structures.Module.IStructIncomingInvoices>();
      if (DateStart != "")
        LstII = LstII.Where(i => i.Created >= ParseDate(DateStart)).ToList();
      
      if (DateStop != "")
        LstII = LstII.Where(i => i.Created < ParseDate(DateStop)).ToList(); 
      
      if (LstII.Count > 0)
      {
        foreach (var elem in LstII)
        {
          var str = Structures.Module.StructIncomingInvoices.Create();
          str.CouterpartyTRRCSberDev = elem.CouterpartyTRRCSberDev != null ? elem.CouterpartyTRRCSberDev.ToString() : "";
          str.CardURLSberDev = elem.CardURLSberDev != null ? elem.CardURLSberDev.ToString() : "";;
          str.CounterpartyTINSberDev = elem.CounterpartyTINSberDev != null ? elem.CounterpartyTINSberDev.ToString() : "";
          str.EstPaymentDateSberDev = elem.EstPaymentDateSberDev != null ? elem.EstPaymentDateSberDev.ToString() : "";
          str.CalcListSDev = elem.CalcListSDev != null ? elem.CalcListSDev.ToString() : "";
          str.ModifiedSberDev = elem.ModifiedSberDev != null ? elem.ModifiedSberDev.ToString() : "";
          str.MarketingIDSberDev = elem.MarketingIDSberDev != null ? elem.MarketingIDSberDev.ToString() : "";
          str.MarketingSberDev = elem.MarketingSberDev != null ? elem.MarketingSberDev.Name.ToString() : "";
          str.BudItemBaseSberDev = elem.BudItemBaseSberDev != null ? elem.BudItemBaseSberDev.Name.ToString() : "";
          str.NoticeSendBaseSberDev = elem.NoticeSendBaseSberDev != null ? elem.NoticeSendBaseSberDev.Value.ToString() : "";
          str.CalculationResidualAmountBaseSberDev = elem.CalculationResidualAmountBaseSberDev != null ? elem.CalculationResidualAmountBaseSberDev.Value.ToString() : "";
          str.CalculationDistributeBaseSberDev = elem.CalculationDistributeBaseSberDev != null ? elem.CalculationDistributeBaseSberDev.Value.ToString() : "";
          str.CalculationAmountBaseSberDev = elem.CalculationAmountBaseSberDev != null ? elem.CalculationAmountBaseSberDev.Value.ToString() : "";
          str.CalculationFlagBaseSberDev = elem.CalculationFlagBaseSberDev != null ? elem.CalculationFlagBaseSberDev.Value.ToString() : "";
          str.MarketDirectSberDev = elem.MarketDirectSberDev != null ? elem.MarketDirectSberDev.Name.ToString() : "";
          str.PayTypeBaseSberDev = elem.PayTypeBaseSberDev != null ? elem.PayTypeBaseSberDev.Value.ToString() : "";
          str.InvoiceSberDev = elem.InvoiceSberDev != null ? elem.InvoiceSberDev.Name.ToString() : "";
          str.AccArtBaseSberDev = elem.AccArtBaseSberDev != null ? elem.AccArtBaseSberDev.Name.ToString() : "";
          str.MVPBaseSberDev = elem.MVPBaseSberDev != null ? elem.MVPBaseSberDev.Name.ToString() : "";
          str.MVZBaseSberDev = elem.MVZBaseSberDev != null ? elem.MVZBaseSberDev.Name.ToString() : "";          
          str.AccDocSberDev = elem.AccDocSberDev != null ? elem.AccDocSberDev.Name.ToString() : "";
          str.FrameworkBaseSberDev = elem.FrameworkBaseSberDev != null ? elem.FrameworkBaseSberDev.Value.ToString() : "";
          str.ContrTypeBaseSberDev = elem.ContrTypeBaseSberDev != null ? elem.ContrTypeBaseSberDev.Value.ToString() : "";
          str.PurchaseOrderNumber = elem.PurchaseOrderNumber != null ? elem.PurchaseOrderNumber.ToString() : "";
          str.NetAmount = elem.NetAmount != null ? elem.NetAmount.Value.ToString() : "";
          str.VatAmount = elem.VatAmount != null ? elem.VatAmount.Value.ToString() : "";
          str.VatRate = elem.VatRate != null ? elem.VatRate.Name.ToString() : "";
          str.CounterpartySigningReason = elem.CounterpartySigningReason != null ? elem.CounterpartySigningReason.ToString() : "";
          str.IsFormalizedSignatoryEmpty = elem.IsFormalizedSignatoryEmpty != null ? elem.IsFormalizedSignatoryEmpty.Value.ToString() : "";
          str.IsRevision = elem.IsRevision != null ? elem.IsRevision.Value.ToString() : "";          
          str.FormalizedFunction = elem.FormalizedFunction != null ? elem.FormalizedFunction.Value.ToString() : "";
          str.FormalizedServiceType = elem.FormalizedServiceType != null ? elem.FormalizedServiceType.Value.ToString() : "";
          str.BodyExtSberDev = elem.BodyExtSberDev != null ? elem.BodyExtSberDev.ToString() : "";
          str.ManuallyCheckedSberDev = elem.ManuallyCheckedSberDev != null ? elem.ManuallyCheckedSberDev.Value.ToString() : "";
          str.Subtopic = elem.Subtopic != null ? elem.Subtopic.Name.ToString() : "" ;
          str.Topic = elem.Topic != null ? elem.Topic.Name.ToString() : "";
          str.StoredIn = elem.StoredIn != null ? elem.StoredIn.ToString() : "";
          str.AddendaPaperCount = elem.AddendaPaperCount != null ? elem.AddendaPaperCount.Value.ToString() : "";
          str.ExternalId = elem.ExternalId != null ? elem.ExternalId.ToString() : "";
          str.OurSigningReason = elem.OurSigningReason != null ? elem.OurSigningReason.Name.ToString() : "";
          str.DocumentDate = elem.DocumentDate != null ? elem.DocumentDate.Value.ToString() : "";
          str.PaymentDateSberDevSDev = elem.PaymentDateSberDev != null ? elem.PaymentDateSberDev.Value.ToString() : "";          
          str.NoNeedLeadingDocsSDev = elem.NoNeedLeadingDocs != null ? elem.NoNeedLeadingDocs.Value.ToString() : "";
          str.ContractStatementSDev = elem.ContractStatement != null ? elem.ContractStatement.Name.ToString() : "";
          str.DeliveryInfoSDev = elem.DeliveryInfo != null ? elem.DeliveryInfo.ToString() : "";          
          str.OriginalSDev = elem.Original != null ? elem.Original.Value.ToString() : "";
          str.PayTypeSDev = elem.PayType != null ? elem.PayType.Value.ToString() : "";
          str.PaymentDueDate = elem.PaymentDueDate != null ? elem.PaymentDueDate.Value.ToString() : "";
          str.Corrected = elem.Corrected != null ? elem.Corrected.Name.ToString() : "";
          str.IsAdjustment = elem.IsAdjustment != null ? elem.IsAdjustment.Value.ToString() : "";
          str.BusinessUnitBox = elem.BusinessUnitBox != null ? elem.BusinessUnitBox.Name.ToString() : "";
          str.BuyerSignatureId = elem.BuyerSignatureId != null ? elem.BuyerSignatureId.Value.ToString() : "";
          str.SellerSignatureId = elem.SellerSignatureId != null ? elem.SellerSignatureId.Value.ToString() : "";
          str.BuyerTitleId = elem.BuyerTitleId != null ? elem.BuyerTitleId.Value.ToString() : "";
          str.SellerTitleId = elem.SellerTitleId != null ? elem.SellerTitleId.Value.ToString() : "";
          str.IsFormalized = elem.Topic != null ? elem.Topic.Name.ToString() : "";
          str.ExchangeState = elem.ExchangeState != null ? elem.ExchangeState.Value.ToString() : "";
          str.Contract = elem.Contract != null ? elem.Contract.Name.ToString() : "";
          str.Created = elem.Created != null ? elem.Created.Value.ToString() : "";
          str.Author = elem.Author != null ? elem.Author.Name.ToString() : "";
          str.Currency = elem.Currency != null ? elem.Currency.Name.ToString() : "";
          str.TotalAmount = elem.TotalAmount != null ? elem.TotalAmount.Value.ToString() : "";
          str.Date = elem.Date != null ? elem.Date.Value.ToString() : "";
          str.Number = elem.Number != null ? elem.Number.ToString() : "";
          str.Assignee = elem.Assignee != null ? elem.Assignee.Name.ToString() : "";
          str.PreparedBy = elem.PreparedBy != null ? elem.PreparedBy.Name.ToString() : "";
          str.DocumentGroup = elem.DocumentGroup != null ? elem.DocumentGroup.Name.ToString() : "";
          str.Note = elem.Note != null ? elem.Note.ToString() : "";
          str.Subject = elem.Subject != null ? elem.Subject.ToString() : "";          
          str.CounterpartySignatory = elem.CounterpartySignatory != null ? elem.CounterpartySignatory.Name.ToString() : "";
          str.LeadingDocument = elem.LeadingDocument != null ? elem.LeadingDocument.Name.ToString() : "";
          str.OurSignatory = elem.OurSignatory != null ? elem.OurSignatory.Name.ToString() : "";
          str.ResponsibleEmployee = elem.ResponsibleEmployee != null ? elem.ResponsibleEmployee.Name.ToString() : "";
          str.Department = elem.Department != null ? elem.Department.Name.ToString() : "";
          str.BusinessUnit = elem.BusinessUnit != null ? elem.BusinessUnit.Name.ToString() : "";
          str.Counterparty = elem.Counterparty != null ? elem.Counterparty.Name.ToString() : "";
          str.DocumentKind = elem.DocumentKind != null ? elem.DocumentKind.Name.ToString() : "";
          str.LifeCycleState = elem.LifeCycleState != null ? elem.LifeCycleState.Value.ToString() : "";
          str.RegistrationState = elem.RegistrationState != null ? elem.RegistrationState.Value.ToString() : "";
          str.ReturnDate = elem.ReturnDate != null ? elem.ReturnDate.Value.ToString() : "";
          str.DeliveredTo = elem.DeliveredTo != null ? elem.DeliveredTo.Name.ToString() : "";
          str.RegistrationNumber = elem.RegistrationNumber != null ? elem.RegistrationNumber.ToString() : "";
          str.Name = elem.Name.ToString();
          str.Id = elem.Id.ToString();

          FullList.Add(str);
        }
      }
      
      return FullList;
    }
    #endregion
  }
}