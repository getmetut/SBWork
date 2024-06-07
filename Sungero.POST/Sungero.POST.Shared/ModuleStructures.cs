using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.POST.Structures.Module
{

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