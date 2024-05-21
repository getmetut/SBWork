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
    // <summary>
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
  }
}