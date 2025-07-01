using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts.Shared
{
  partial class OfficialDocumentFunctions
  {
    /// <summary>
    /// Проверяет откуда создан документ
    /// </summary>
    /// <param name="document"></param>
    /// <param name="source">Константы из OfficialDocument с приставкой "LS"</param>
    /// <returns>Словарь с результатами проверки вхождения строк</returns>
    [Public]
    public System.Collections.Generic.Dictionary<string, bool> CheckLocationState(string[] source)
    {
      var result = new Dictionary<string, bool>();
      
      if (source == null || source.Length == 0)
        return result;
      
      string locationState = _obj?.LocationState ?? string.Empty;
      
      foreach (string sourceItem in source)
      {
        if (!string.IsNullOrEmpty(sourceItem))
        {
          result[sourceItem] = locationState.Contains(sourceItem);
        }
      }
      
      return result;
    }
    
    /// <summary>
    /// Проверяет создан ли документ системой
    /// </summary>
    /// <param name="document">Документ для проверки</param>
    /// <returns></returns>
    public static bool IsCreatedBySystem(IOfficialDocument document)
    {
      long[] adminIds = new long[] {9, 10, 11, 12};
      return adminIds.Contains(document.Author.Id);
    }
    
    /// <summary>
    /// Выключает обязательность невидимых свойств
    /// </summary>
    [Public]
    public void ValidationRequierdProperties()
    {
      var unvisProps = _obj.State.Properties.Where(p => p.IsVisible == false);
      foreach (var prop in unvisProps)
        prop.IsRequired = false;
    }
    
    /// <summary>
    /// Получить список всех продуктов
    /// </summary>
    /// <returns></returns>
    [Public]
    public List<SberContracts.IProductsAndDevices> GetDocumentProducts()
    {
      var products = new List<SberContracts.IProductsAndDevices>();
      
      // Проверяем, является ли документ договорным
      var contract = SBContracts.ContractualDocuments.As(_obj);
      if (contract != null)
      {
        // Из коллекции расходов
        if (contract.ProdCollectionExBaseSberDev != null)
          products.AddRange(contract.ProdCollectionExBaseSberDev
                            .Where(x => x.Product != null && x.Product.Name != "General")
                            .Select(x => x.Product));

        // Из коллекции доходов (и прочих)
        if (contract.ProdCollectionPrBaseSberDev != null)
          products.AddRange(contract.ProdCollectionPrBaseSberDev
                            .Where(x => x.Product != null && x.Product.Name != "General")
                            .Select(x => x.Product));

        // Из расчетов
        if (contract.CalculationBaseSberDev != null)
          products.AddRange(contract.CalculationBaseSberDev
                            .Where(x => x.ProductCalc != null && x.ProductCalc.Name != "General")
                            .Select(x => x.ProductCalc));
      }

      // Проверяем, является ли документ учетным
      var accountingDoc = SBContracts.AccountingDocumentBases.As(_obj);
      if (accountingDoc != null)
      {
        // Основная коллекция продуктов
        if (accountingDoc.ProdCollectionBaseSberDev != null)
          products.AddRange(accountingDoc.ProdCollectionBaseSberDev
                            .Where(x => x.Product != null && x.Product.Name != "General")
                            .Select(x => x.Product));

        // Из расчетов
        if (accountingDoc.CalculationBaseSberDev != null)
          products.AddRange(accountingDoc.CalculationBaseSberDev
                            .Where(x => x.ProductCalc != null && x.ProductCalc.Name != "General")
                            .Select(x => x.ProductCalc));
      }

      // Убираем дубли, если объекты продуктов могут повторяться
      return products.Distinct().ToList();
    }
    
    public Nullable<Enumeration> GetIntApprEnum(string value)
    {
      switch (value)
      {
        case "OnApproval":
          return InternalApprovalState.OnApproval;
        case "OnRework":
          return InternalApprovalState.OnRework;
        case "PendingSign":
          return InternalApprovalState.PendingSign;
        case "Signed":
          return InternalApprovalState.Signed;
        case "Aborted":
          return InternalApprovalState.Aborted;
        default:
          return null;
      }
    }
    public Nullable<Enumeration> GetExtApprEnum(string value)
    {
      switch (value)
      {
        case "OnApproval":
          return ExternalApprovalState.OnApproval;
        case "UnSigned":
          return ExternalApprovalState.Unsigned;
        case "Signed":
          return ExternalApprovalState.Signed;
        default:
          return null;
      }
    }
  }
}