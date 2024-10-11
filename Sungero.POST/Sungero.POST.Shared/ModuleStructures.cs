using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.POST.Structures.Module
{

  /// <summary>
  /// Структура ответа при запросе из внешней системы
  /// </summary>
  [Public]
  partial class StrucResult
  {
    public string document_type { get; set; } //": "", // тип договора
    public string category { get; set; } // { get; set; } //": "", // категория договора
    public string reg_no { get; set; } //": "", // регистрационный номер
    public string name { get; set; } //": "", // Имя договора
    public string counterparty_name { get; set; } //": "", //контрагент
    public string date_document { get; set; } //": "", // Дата документа
    public List<Sungero.POST.Structures.Module.IStructlistdoc> acts { get; set; } //"
  }
  
  /// <summary>
  /// Струтура акта
  /// </summary>
  [Public]
  partial class Structlistdoc
  {
    public string acts_type { get; set; } //": "", // Вид документа
    public string name { get; set; } //": "", // имя документа
    public string act_no { get; set; } //": "", // Номер документа
    public string date_act { get; set; } //": "", //Дата документа
    public string total_amount { get; set; } //": "" //общая сумма
    public bool include_VAT { get; set; } //": TRUE
  }

  /// <summary>
  /// Структура получения данных по документам из внешних систем
  /// </summary>
  [Public]
  partial class StructDog
  {
    public string  document_type  { get; set; } //": "", // Фильтр для получения договоров по типу
    public string  document_kind  { get; set; } //": "", // Фильтр для получения договоров по виду
    public string  receiver_cost_center  { get; set; } //"", // Фильтр для получения договоров по МВЗ
    public string  reg_no  { get; set; } //": "", // Фильтр для получения информации по указанному договору
    public string  start_date  { get; set; } //": "", // Фильтр для получения документов с датой от
    public string  end_date  { get; set; } //": "", // Фильтр для получения документов с датой по
    public string  counterparty_name  { get; set; } //": ["string"], // Фильтр для получения договоров по контрагенту
    public string  status  { get; set; } //": "", //Статус действия договора
  }
  /// <summary>
  /// Структура Акта выполненных работ
  /// </summary>
  [Public]
  partial class StructContractStatement
  {
    public string  Id  { get; set; }
    public string  Name  { get; set; }
    public string  DocumentKind  { get; set; }
    public string  DocumentGroup  { get; set; }
    public string  Number  { get; set; }
    public string  Date  { get; set; }
    public string  Counterparty  { get; set; }
    public string  Contact  { get; set; }
    public string  CounterpartySignatory  { get; set; }
    public string  LeadingDocument  { get; set; }
    public string  BusinessUnit  { get; set; }
    public string  Department  { get; set; }
    public string  ResponsibleEmployee  { get; set; }
    public string  OurSignatory  { get; set; }
    public string  ExchangeState  { get; set; }
    public string  Subject  { get; set; }
    public string  IsFormalized  { get; set; }
    public string  IsAdjustment  { get; set; }
    public string  Corrected  { get; set; }
    public string  VerificationState  { get; set; }
    public string  DocumentDate  { get; set; }
    public string  OurSigningReason  { get; set; }
    public string  PaperCount  { get; set; }
    public string  StoredIn  { get; set; }
    public string  Topic  { get; set; }
    public string  Subtopic  { get; set; }
    public string  ManuallyCheckedSberDev  { get; set; }
    public string  FormalizedServiceType  { get; set; }
    public string  FormalizedFunction  { get; set; }
    public string  IsRevision  { get; set; }
    public string  CounterpartySigningReason  { get; set; }
    public string  PurchaseOrderNumber  { get; set; }
    public string  ContrTypeBaseSberDev  { get; set; }
    public string  FrameworkBaseSberDev  { get; set; }
    public string  AccDocSberDev  { get; set; }
    public string  MVZBaseSberDev  { get; set; }
    public string  MVPBaseSberDev  { get; set; }
    public string  AccArtBaseSberDev  { get; set; }
    public string  InvoiceSberDev  { get; set; }
    public string  PayTypeBaseSberDev  { get; set; }
    public List<Sungero.POST.Structures.Module.IStructProdCollection>  ProdCollectionBaseSberDev  { get; set; }
    public string  MarketDirectSberDev  { get; set; }
    public List<Sungero.POST.Structures.Module.IStructCalculation>  CalculationBaseSberDev  { get; set; }
    public string  CalculationFlagBaseSberDev  { get; set; }
    public string  CalculationAmountBaseSberDev  { get; set; }
    public string  CalculationDistributeBaseSberDev  { get; set; }
    public string  CalculationResidualAmountBaseSberDev  { get; set; }
    public string  NoticeSendBaseSberDev  { get; set; }
    public string  BudItemBaseSberDev  { get; set; }
    public string  MarketingSberDev  { get; set; }
    public string  MarketingIDSberDev  { get; set; }
    public string  ModifiedSberDev  { get; set; }
    public string  EstPaymentDateSberDev  { get; set; }
    public string  Author  { get; set; }
    public string  Created  { get; set; }
    public string  Modified  { get; set; }
    public string  RegistrationNumber  { get; set; }
    public string  Index  { get; set; }
    public string  RegistrationDate  { get; set; }
    public string  DocumentRegister  { get; set; }
    public string  DeliveryMethod  { get; set; }
    public string  IsReturnRequired  { get; set; }
    public string  IsHeldByCounterParty  { get; set; }
    public string  DeliveredTo  { get; set; }
    public string  ReturnDeadline  { get; set; }
    public string  ReturnDate  { get; set; }
    public string  LifeCycleState  { get; set; }
    public string  RegistrationState  { get; set; }
    public string  InternalApprovalState  { get; set; }
    public string  ExternalApprovalState  { get; set; }
    public string  ExecutionState  { get; set; }
    public string  ControlExecutionState  { get; set; }
    public string  LocationState  { get; set; }
    public string  ResponsibleForReturnEmployee  { get; set; }
    public string  ScheduledReturnDateFromCounterparty  { get; set; }
    public string  Note  { get; set; }
    public string  Assignee  { get; set; }
    public string  PreparedBy  { get; set; }
    public string  TotalAmount  { get; set; }
    public string  VatRate  { get; set; }
    public string  VatAmount  { get; set; }
    public string  NetAmount  { get; set; }
    public string  Currency  { get; set; }
  }
  
  /// <summary>
  /// Структура списка Продуктов
  /// </summary>
  [Public]
  partial class StructProdCollection
  {
    public string Product {get; set; }
  }
  
  /// <summary>
  /// Структура Счетов для 1С
  /// </summary>
  [Public]
  partial class StructIncomingInvoices
  {
    public string CouterpartyTRRCSberDev {get; set; }
    public string CardURLSberDev {get; set; }
    public string CounterpartyTINSberDev {get; set; }
    public string EstPaymentDateSberDev {get; set; }
    public string CalcListSDev {get; set; }
    public string ModifiedSberDev {get; set; }
    public string MarketingIDSberDev {get; set; }
    public string MarketingSberDev {get; set; }
    public string BudItemBaseSberDev {get; set; }
    public string NoticeSendBaseSberDev {get; set; }
    public string CalculationResidualAmountBaseSberDev {get; set; }
    public string CalculationDistributeBaseSberDev {get; set; }
    public string CalculationAmountBaseSberDev {get; set; }
    public string CalculationFlagBaseSberDev {get; set; }
    //public string /------CalculationBaseSberDev {get; set; }
    //public string AggregationCalc {get; set; }
    //public string ProductCalc {get; set; }
    //public string AbsoluteCalc {get; set; }
    //public string PercentCalc {get; set; }
    //public string InterestCalc {get; set; }
    //public string ----------------------------- {get; set; }
    public string MarketDirectSberDev {get; set; }
    //public string =======ProdCollectionBaseSberDev {get; set; }
    //public string Product {get; set; }
    //public string ================================ {get; set; }
    public string PayTypeBaseSberDev {get; set; }
    public string InvoiceSberDev {get; set; }
    public string AccArtBaseSberDev {get; set; }
    public string MVPBaseSberDev {get; set; }
    public string MVZBaseSberDev {get; set; }
    public string AccDocSberDev {get; set; }
    public string FrameworkBaseSberDev {get; set; }
    public string ContrTypeBaseSberDev {get; set; }
    public string PurchaseOrderNumber {get; set; }
    public string NetAmount {get; set; }
    public string VatAmount {get; set; }
    public string VatRate {get; set; }
    public string CounterpartySigningReason {get; set; }
    public string IsFormalizedSignatoryEmpty {get; set; }
    public string IsRevision {get; set; }
    public string FormalizedFunction {get; set; }
    public string FormalizedServiceType {get; set; }
    public string BodyExtSberDev {get; set; }
    public string ManuallyCheckedSberDev {get; set; }
    public string Subtopic {get; set; }
    public string Topic {get; set; }
    public string StoredIn {get; set; }
    public string AddendaPaperCount {get; set; }
    public string ExternalId {get; set; }
    public string OurSigningReason {get; set; }
    public string DocumentDate {get; set; }
    public string PaymentDateSberDevSDev {get; set; }
    public string NoNeedLeadingDocsSDev {get; set; }
    public string ContractStatementSDev {get; set; }
    public string DeliveryInfoSDev {get; set; }
    public string OriginalSDev {get; set; }
    public string PayTypeSDev {get; set; }
    public string PaymentDueDate {get; set; }
    public string Corrected {get; set; }
    public string IsAdjustment {get; set; }
    public string BusinessUnitBox {get; set; }
    public string BuyerSignatureId {get; set; }
    public string SellerSignatureId {get; set; }
    public string BuyerTitleId {get; set; }
    public string SellerTitleId {get; set; }
    public string IsFormalized {get; set; }
    public string ExchangeState {get; set; }
    public string Contract {get; set; }
    public string Created {get; set; }
    public string Author {get; set; }
    public string Currency {get; set; }
    public string TotalAmount {get; set; }
    public string Date {get; set; }
    public string Number {get; set; }
    public string Assignee {get; set; }
    public string PreparedBy {get; set; }
    public string DocumentGroup {get; set; }
    public string Note {get; set; }
    public string Subject {get; set; }
    public string CounterpartySignatory {get; set; }
    public string LeadingDocument {get; set; }
    public string OurSignatory {get; set; }
    public string ResponsibleEmployee {get; set; }
    public string Department {get; set; }
    public string BusinessUnit {get; set; }
    public string Counterparty {get; set; }
    public string DocumentKind {get; set; }
    public string LifeCycleState {get; set; }
    public string RegistrationState {get; set; }
    public string ReturnDate {get; set; }
    public string DeliveredTo {get; set; }
    public string RegistrationNumber {get; set; }
    public string Name {get; set; }
    public string Id {get; set; }

  }
  
  /// <summary>
  /// Структура списка Калькуляций
  /// </summary>
  [Public]
  partial class StructCalculation
  {
    public string AggregationCalc {get; set; }
    public string ProductCalc {get; set; }
    public string AbsoluteCalc {get; set; }
    public string PercentCalc {get; set; }
    public string InterestCalc {get; set; }
  }
    
  /// <summary>
  /// Структура допсоглашения
  /// </summary>
  [Public]
  partial class StructSupAgreement
  {
    public string  Id  { get; set; }
    public string  Name  { get; set; }
    public string  Created  { get; set; }
    public string  TotalAmount  { get; set; }
    public string  ValidFrom  { get; set; }
    public string  ValidTill  { get; set; }
    public string  MVZOldSberDevSDev  { get; set; }
    public string  AccArtExOldSberDevSDev  { get; set; }
    public string  MVPOldSberDevSDev  { get; set; }
    public string  AccArtPrOldSberDevSDev  { get; set; }
    public string  BudItemOldSberDevSDev  { get; set; }
    public string  ContrTypeOldSberDevSDev  { get; set; }
    public string  OriginalOldSberDevSDev  { get; set; }
    public string  SigningSDev  { get; set; }
    public string  DeliveryInfoOldSberDevSDev  { get; set; }
    public string  NoticeSendOldSberDevSDev  { get; set; }
    public string  FrameworkOldSberDevSDev  { get; set; }
    public string  SDSFSberDevSDev  { get; set; }
    public string  SRSberDevSDev  { get; set; }
    public string  GoogleDocsLinkSberDevSDev  { get; set; }
    public string  SubjectSpecificationSberDevSDev  { get; set; }
    public string  AccArtExBaseSberDev  { get; set; }
    public string  AccArtExOldSberDev  { get; set; }
    public string  AccArtPrBaseSberDev  { get; set; }
    public string  AccArtPrOldSberDev  { get; set; }
    public string  Assignee  { get; set; }
    public string  AssociatedApplication  { get; set; }
    public string  Author  { get; set; }
    public string  BudItemBaseSberDev  { get; set; }
    public string  BudItemOldSberDev  { get; set; }
    public string  BusinessUnit  { get; set; }
    public string  CalculationBaseSberDev  { get; set; }
    public string  CaseFile  { get; set; }
    public string  Contact  { get; set; }
    public string  Counterparty  { get; set; }
    public string  CounterpartySignatory  { get; set; }
    public string  Currency  { get; set; }
    public string  DeliveredTo  { get; set; }
    public string  DeliveryMethod  { get; set; }
    public string  Department  { get; set; }
    public string  DirectionMVZ  { get; set; }
    public string  DocumentGroup  { get; set; }
    public string  DocumentKind  { get; set; }
    public string  DocumentRegister  { get; set; }
    public string  LeadingDocument  { get; set; }
    public string  MarketDirectSberDev  { get; set; }
    public string  Milestones  { get; set; }
    public string  MVPBaseSberDev  { get; set; }
    public string  MVPOldSberDev  { get; set; }
    public string  MVZBaseSberDev  { get; set; }
    public string  MVZOldSberDev  { get; set; }
    public string  OurSignatory  { get; set; }
    public string  OurSigningReason  { get; set; }
    public string  Parameters  { get; set; }
    public string  PreparedBy  { get; set; }
    public string  ProdCollectionExBaseSberDev  { get; set; }
    public string  ProdCollectionPrBaseSberDev  { get; set; }
    public string  ProdSberDevCollection  { get; set; }
    public string  Project  { get; set; }
    public string  ResponsibleEmployee  { get; set; }
    public string  ResponsibleForReturnEmployee  { get; set; }
    public string  Subtopic  { get; set; }
    public string  Topic  { get; set; }
    public string  Tracking  { get; set; }
    public string  VatRate  { get; set; }
    public string  Versions  { get; set; }
    public string  Link  { get; set; }
  }

  /// <summary>
  /// Структура списка договоров
  /// </summary>
  [Public]
  partial class StructContractList
  {
    public List<Sungero.POST.Structures.Module.IStructContract> ContractList {get; set; }
  }
  
  /// <summary>
  /// Структура договора
  /// </summary>
  [Public]
  partial class StructContract
  {
    public string ID { get; set; }
    public string Name { get; set; }
    public string Created { get; set; }
    public string DocNum { get; set; }
    public string DocDate { get; set; }
    public string TotalAmount { get; set; }
    public string ValidFrom { get; set; }
    public string ValidTill { get; set; }
    public string AccArtExBaseSberDev { get; set; } // { get; set; } //Target="IAccountingArticless"/>
    public string AccArtMVZOldSberDev { get; set; } //Target="IAccountingArticless"/>
    public string AccArtPrBaseSberDev { get; set; } //Target="IAccountingArticless"/>
    public string AccArtsberdevOldSberDev { get; set; } //Target="IAccountingArticless"/>
    public string Assignee { get; set; } //Target="IEmployees"/>
    public string AssociatedApplication { get; set; } //Target="IAssociatedApplications"/>
    public string Author { get; set; } //Target="IUsers"/>
    public string BudItemBaseSberDev { get; set; } //Target="IBudgetItems"/>
    public string BudItemsberdevOldSberDev { get; set; } //Target="IBudgetItems"/>
    public string BusinessUnit { get; set; } //Target="IBusinessUnits"/>
    public string CalculationBaseSberDev { get; set; } //Target="IContractualDocumentCalculationBaseSberDevs"/>
    public string CaseFile { get; set; } //Target="ICaseFiles"/>
    public string CollectionProperty { get; set; } //Target="IContractCollectionProperties"/>
    public string Contact { get; set; } //Target="IContacts"/>
    public string Counterparty { get; set; } //Target="ICounterparties"/>
    public string CounterpartySignatory { get; set; } //Target="IContacts"/>
    public string Currency { get; set; } //Target="ICurrencies"/>
    public string DeliveredTo { get; set; } //Target="IEmployees"/>
    public string DeliveryMethod { get; set; } //Target="IMailDeliveryMethods"/>
    public string Department { get; set; } //Target="IDepartments"/>
    public string DirectionMVZ { get; set; } //Target="IContractDirectionMVZs"/>
    public string DocumentGroup { get; set; } //Target="IDocumentGroupBases"/>
    public string DocumentKind { get; set; } //Target="IDocumentKinds"/>
    public string DocumentRegister { get; set; } //Target="IDocumentRegisters"/>
    public string LeadingDocument { get; set; } //Target="IOfficialDocuments"/>
    public string MarketDirectSberDev { get; set; } //Target="ISberContractsMarketingDirections"/>
    public string Milestones { get; set; } //Target="IContractualDocumentMilestoness"/>
    public string MVPBaseSberDev { get; set; } //Target="IMVZs"/>
    public string MVPsberdevOldSberDev { get; set; } //Target="IMVZs"/>
    public string MVZBaseSberDev { get; set; } //Target="IMVZs"/>
    public string MVZsberdevOldSberDev { get; set; } //Target="IMVZs"/>
    public string OurSignatory { get; set; } //Target="IEmployees"/>
    public string OurSigningReason { get; set; } //Target="ISignatureSettings"/>
    public string Parameters { get; set; } //Target="IElectronicDocumentParameterss"/>
    public string PreparedBy { get; set; } //Target="IEmployees"/>
    public string ProdCollectionExBaseSberDev { get; set; } //Target="IContractualDocumentProdCollectionExBaseSberDevs"/>
    public string ProdCollectionPrBaseSberDev { get; set; } //Target="IContractualDocumentProdCollectionPrBaseSberDevs"/>
    public string Project { get; set; } //Target="IProjectBases"/>
    public string ResponsibleEmployee { get; set; } //Target="IEmployees"/>
    public string ResponsibleForReturnEmployee { get; set; } //Target="IEmployees"/>
    public string Subtopic { get; set; } //Target="ITopics"/>
    public string Topic { get; set; } //Target="ITopics"/>
    public string Tracking { get; set; } //Target="IOfficialDocumentTrackings"/>
    public string VatRate { get; set; } //Target="IVatRates"/>
    public string Versions { get; set; } //Target="IElectronicDocumentVersionss"/>
    public string Link { get; set; }
  }

  /// <summary>
  /// Структура выдачи маркетингового документа
  /// </summary>
  [Public]
  partial class MarcetingDoc
  {
    public string Name { get; set; } 
    public string Kind { get; set; }   
    public string ActionCode { get; set; }
    public string ActionName { get; set; } 
    public string MarketingKind { get; set; }   
    public string ActionType { get; set; }
    public string MarketingSubKind { get; set; } 
    public string OwnerEmployee { get; set; }   
    public string StartDate { get; set; }
    public string EndDate { get; set; } 
    public string BudgetItem { get; set; }   
    public string AccountingArticles { get; set; }
    public string PlannedSumm { get; set; } 
    public string PlannedSummCurrency { get; set; }   
    public string PlannedRashod { get; set; }
    public string PlannedRashodCurrency { get; set; }
    public string PlanKolInt { get; set; }
    public string FacktTotalSumm { get; set; }
    public string FacktTotalSummCurrency { get; set; }
    public string FacktRashod { get; set; }
    public string FacktRashodCurrency { get; set; }
    public string FacktKolInt { get; set; }
    public string StagesOfApproval { get; set; }
    public List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction { get; set; }
    public List<Sungero.POST.Structures.Module.IChannels> Channels { get; set; }
    // Добавить связанные документы с текущим - можно ссылками
  }
  /// <summary>
  /// Структура Списка структур по маркетинговым документам
  /// </summary>
  [Public]
  partial class MarcetingDocsList
  {
    public List<Sungero.POST.Structures.Module.IMarcetingDoc> MarcetingDocumentStruct { get; set; }
  }

  /// <summary>
  /// Базовая структура Таблицы Продуктов и девайсов
  /// </summary>
  [Public]
  partial class DevicesAction
  {
    public string Product { get; set; } 
    public string KolVoDevices { get; set; }
    public string PriceNoPromo { get; set; }
    public string PriceNoPromoCurrency { get; set; }
    public string PricePromo { get; set; }
    public string PricePromoCurrency { get; set; }
  }
  
  /// <summary>
  /// Базовая структура Таблицы Каналов
  /// </summary>
  [Public]
  partial class Channels
  {
    public string Channel { get; set; } 
    public string Product { get; set; }
    public string Komission { get; set; }
    public string KomissionCurrency { get; set; }
  }

  /// <summary>
  /// Базовая структура Name=Value
  /// </summary>
  [Public]
  partial class BaseStruct
  {
    public string Name { get; set; } 
    public string Value { get; set; }
  }
  
  /// <summary>
  /// Образец структуры с описанием функции, системы и сущности
  /// </summary>
  [Public]
  partial class FormatRequest
  {
    public string Sysname { get; set; } 
    public string Function { get; set; }   
    public string Entity { get; set; }  
    public List<Sungero.POST.Structures.Module.IBaseStruct> Param { get; set; }
  
  /* {
    "Sysname": "RX",
    "Function": "Create",
    "Entity": "Contract",
    "Param": [
        {
            "Name": "Doc.version",
            "Value": "2"
        },
        {
            "Name": "Extension",
            "Value": "jpeg"
        }
     ]
  } */
  
  }
  
  /// <summary>
  /// Запрос в 1С Информацию по договору
  /// </summary>
  [Public]
  partial class ContractRequest
  {  
    public string GUIDRX { get; set; }          // { get; set; } //GUID "	
    public string Kind { get; set; }            // { get; set; } //GUID "    	
    public string Name { get; set; }            //"текст"	Наименование
    public string Counter { get; set; }         //"GUID"	Контрагент
    public string Buyer { get; set; }           //"GUID"	Покупатель
    public string NOR { get; set; }             //"GUID"	Организация
    public string Departament { get; set; }     //"GUID"	Подразделение
    public string Signatory { get; set; }       //"GUID"	Подписал
    public string Summ { get; set; }            //"100,00"	Сумма договора фиксирована
    public string Currency { get; set; }        //"643"	Валюта
    public string Number { get; set; }          //"2023-СИМ-2233_NEW"	Номер договора
    public string Date { get; set; }            //"29.01.2022 0:00:00"	Дата заключения договора
    public string Status { get; set; }          //"__________"	Статус договора
    public string ValidC { get; set; }          //"09.08.2023 0:00:00"	Действует с
    public string ValidPO { get; set; }         //"31.08.2023 0:00:00"	По	
  }
}