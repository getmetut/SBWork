using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Structures.Module;

namespace sberdev.SberContracts.Shared
{
  public class ModuleFunctions
  {
    #region Заполнение, копирование свойств
    
    #region Договор -> Договор
    
    /// <summary>
    /// Заполнить общие свойства из выбранного документа
    /// </summary>
    /// <param name="contract"></param>
    public static void FillGeneralProperties(SBContracts.IContractualDocument doc, SBContracts.IContractualDocument docSelected)
    {
      doc.BudItemBaseSberDev = docSelected.BudItemBaseSberDev;
      doc.Counterparty = docSelected.Counterparty;
      doc.DeliveryMethod = docSelected.DeliveryMethod;
      if (docSelected.AgentSaluteSberDev != null)
      {
        doc.AgentSaluteSberDev = docSelected.AgentSaluteSberDev;
      }
      if (docSelected.TotalAmount != null)
      {
        doc.TotalAmount = docSelected.TotalAmount;
        doc.Currency = docSelected.Currency;
      }
      if (!SberContracts.Purchases.Is(doc) && !SberContracts.AppProductPurchases.Is(doc))
      {
        doc.ContrTypeBaseSberDev = docSelected.ContrTypeBaseSberDev;
        doc.FrameworkBaseSberDev = docSelected.FrameworkBaseSberDev;
      }
    }
    
    /// <summary>
    /// Заполнить доходные аналитики из выбранного документа
    /// </summary>
    /// <param name="lead">Ведущий документ</param>
    public static void FillProfitableAnaliticsProperties(SBContracts.IContractualDocument doc, SBContracts.IContractualDocument docSelected)
    {
      if (docSelected.AccArtPrBaseSberDev != null)
        doc.AccArtPrBaseSberDev = docSelected.AccArtPrBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtPrBaseSberDev : null;
      if (docSelected.AccArtPrBaseSberDev != null)
        doc.MVPBaseSberDev = docSelected.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVPBaseSberDev : null;
      var collection = docSelected.ProdCollectionPrBaseSberDev;
      if (collection.Count > 0)
      {
        doc.ProdCollectionPrBaseSberDev.Clear();
        foreach (var prod in collection)
        {
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          {
            var target = doc.ProdCollectionPrBaseSberDev.AddNew();
            target.Product = prod.Product;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить расходные аналитики из выбранного документа
    /// </summary>
    /// <param name="lead">Ведущий документ</param>
    [Public]
    public static void FillExpendableAnaliticsProperties(SBContracts.IContractualDocument doc, SBContracts.IContractualDocument docSelected)
    {
      if (docSelected.AccArtExBaseSberDev != null)
        doc.AccArtExBaseSberDev = docSelected.AccArtExBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtExBaseSberDev : null;
      if (docSelected.MVZBaseSberDev != null)
        doc.MVZBaseSberDev = docSelected.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVZBaseSberDev : null;
      if (docSelected.MarketDirectSberDev != null)
        doc.MarketDirectSberDev = docSelected.MarketDirectSberDev.Status == SberContracts.MarketingDirection.Status.Active ?
          docSelected.MarketDirectSberDev : null;
      var collection = docSelected.ProdCollectionExBaseSberDev;
      if (collection.Count > 0)
      {
        doc.ProdCollectionExBaseSberDev.Clear();
        foreach (var prod in collection)
        {
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          {
            var target = doc.ProdCollectionExBaseSberDev.AddNew();
            target.Product = prod.Product;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить калькуляцию из выбранного документа
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="docSelected"></param>
    public static void FillCalculation(SBContracts.IContractualDocument doc, SBContracts.IContractualDocument docSelected)
    {
      var calc = doc.CalculationBaseSberDev;
      var calcSelected = docSelected.CalculationBaseSberDev;
      if (calcSelected.Count > 0)
      {
        calc.Clear();
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Percent")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Percent;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.PercentCalc = prop.PercentCalc;
            }
          }
        }
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Absolute")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Absolute;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.AbsoluteCalc = prop.AbsoluteCalc;
            }
          }
        }
      }
    }
    
    #endregion
    
    #region Финаносвый -> Договор
    
    /// <summary>
    /// Заполнить общие свойства из выбранного документа
    /// </summary>
    /// <param name="contract"></param>
    public static void FillGeneralProperties(SBContracts.IContractualDocument doc, SBContracts.IAccountingDocumentBase docSelected)
    {
      doc.BudItemBaseSberDev = docSelected.BudItemBaseSberDev ;
      doc.ContrTypeBaseSberDev = docSelected.ContrTypeBaseSberDev;
      if (docSelected.TotalAmount != null)
      {
        doc.TotalAmount = docSelected.TotalAmount;
        doc.Currency = docSelected.Currency;
      }
      doc.DeliveryMethod = docSelected.DeliveryMethod;
      doc.Counterparty = docSelected.Counterparty;
    }
    
    /// <summary>
    /// Заполнить доходные аналитики из выбранного документа
    /// </summary>
    /// <param name="lead">Ведущий документ</param>
    public static void FillProfitableAnaliticsProperties(SBContracts.IContractualDocument doc, SBContracts.IAccountingDocumentBase docSelected)
    {
      if (docSelected.AccArtBaseSberDev != null)
        doc.AccArtPrBaseSberDev = docSelected.AccArtBaseSberDev != null && docSelected.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtBaseSberDev : null;
      if (docSelected.MVPBaseSberDev != null)
        doc.MVPBaseSberDev = docSelected.MVPBaseSberDev != null && docSelected.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVPBaseSberDev : null;
      var collection = docSelected.ProdCollectionBaseSberDev;
      if (collection.Count > 0)
      {
        doc.ProdCollectionPrBaseSberDev.Clear();
        foreach (var prod in collection)
        {
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          {
            var target = doc.ProdCollectionPrBaseSberDev.AddNew();
            target.Product = prod.Product;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить расходные аналитики из выбранного документа
    /// </summary>
    /// <param name="lead">Ведущий документ</param>
    public static void FillExpendableAnaliticsProperties(SBContracts.IContractualDocument doc, SBContracts.IAccountingDocumentBase docSelected)
    {
      if (docSelected.AccArtBaseSberDev != null)
        doc.AccArtExBaseSberDev = docSelected.AccArtBaseSberDev != null && docSelected.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtBaseSberDev : null;
      if (docSelected.MVZBaseSberDev != null)
        doc.MVZBaseSberDev = docSelected.MVZBaseSberDev != null && docSelected.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVZBaseSberDev : null;
      if (docSelected.MarketDirectSberDev != null)
        doc.MarketDirectSberDev = docSelected.MarketDirectSberDev.Status == SberContracts.MarketingDirection.Status.Active ?
          docSelected.MarketDirectSberDev : null;
      var collection = docSelected.ProdCollectionBaseSberDev;
      if (collection.Count > 0)
      {
        doc.ProdCollectionExBaseSberDev.Clear();
        foreach (var prod in collection)
        {
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          {
            var target = doc.ProdCollectionExBaseSberDev.AddNew();
            target.Product = prod.Product;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить калькуляцию из выбранного документа
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="docSelected"></param>
    public static void FillCalculation(SBContracts.IContractualDocument doc, SBContracts.IAccountingDocumentBase docSelected)
    {
      var calc = doc.CalculationBaseSberDev;
      var calcSelected = docSelected.CalculationBaseSberDev;
      if (calcSelected.Count > 0)
      {
        calc.Clear();
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Percent")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Percent;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.PercentCalc = prop.PercentCalc;
            }
          }
        }
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Absolute")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Absolute;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.AbsoluteCalc = prop.AbsoluteCalc;
            }
          }
        }
      }
    }
    #endregion
    
    #region Договор -> Финансовый
    
    /// <summary>
    /// Заполнить общие свойства из выбранного документа
    /// </summary>
    /// <param name="contract"></param>
    public static void FillGeneralProperties(SBContracts.IAccountingDocumentBase doc, SBContracts.IContractualDocument docSelected)
    {
      doc.BudItemBaseSberDev = docSelected.BudItemBaseSberDev;
      if (docSelected.TotalAmount != null)
      {
        doc.TotalAmount = docSelected.TotalAmount;
        doc.Currency = docSelected.Currency;
      }
      doc.DeliveryMethod = docSelected.DeliveryMethod;
      doc.Counterparty = docSelected.Counterparty;
    }
    
    /// <summary>
    /// Заполнить доходные аналитики из выбранного документа
    /// </summary>
    /// <param name="lead">Ведущий документ</param>
    public static void FillProfitableAnaliticsProperties(SBContracts.IAccountingDocumentBase doc, SBContracts.IContractualDocument docSelected)
    {
      doc.ContrTypeBaseSberDev = SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable;
      if (docSelected.AccArtPrBaseSberDev != null)
        doc.AccArtBaseSberDev = docSelected.AccArtPrBaseSberDev != null && docSelected.AccArtPrBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtPrBaseSberDev : null;
      if (docSelected.MVPBaseSberDev != null)
        doc.MVPBaseSberDev = docSelected.MVPBaseSberDev != null && docSelected.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVPBaseSberDev : null;
      var collection = docSelected.ProdCollectionPrBaseSberDev;
      if (collection.Count > 0)
      {
        doc.ProdCollectionBaseSberDev.Clear();
        foreach (var prod in collection)
        {
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          {
            var i = doc.ProdCollectionBaseSberDev.AddNew();
            i.Product = prod.Product;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить расходные аналитики из выбранного документа
    /// </summary>
    /// <param name="lead">Ведущий документ</param>
    public static void FillExpendableAnaliticsProperties(SBContracts.IAccountingDocumentBase doc, SBContracts.IContractualDocument docSelected)
    {
      doc.ContrTypeBaseSberDev = SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable;
      if (docSelected.MVZBaseSberDev != null)
        doc.MVZBaseSberDev = docSelected.MVZBaseSberDev != null && docSelected.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVZBaseSberDev : null;
      if (docSelected.AccArtExBaseSberDev != null)
        doc.AccArtBaseSberDev = docSelected.AccArtExBaseSberDev != null && docSelected.AccArtExBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtExBaseSberDev : null;
      if (docSelected.MarketDirectSberDev != null)
        doc.MarketDirectSberDev = docSelected.MarketDirectSberDev.Status == SberContracts.MarketingDirection.Status.Active ?
          docSelected.MarketDirectSberDev : null;
      var collection = docSelected.ProdCollectionExBaseSberDev;
      if (collection.Count > 0)
      {
        doc.ProdCollectionBaseSberDev.Clear();
        foreach (var prod in collection)
        {
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          {
            var i = doc.ProdCollectionBaseSberDev.AddNew();
            i.Product = prod.Product;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить калькуляцию из выбранного документа
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="docSelected"></param>
    public static void FillCalculation(SBContracts.IAccountingDocumentBase doc, SBContracts.IContractualDocument docSelected)
    {
      var calc = doc.CalculationBaseSberDev;
      var calcSelected = docSelected.CalculationBaseSberDev;
      if (calcSelected.Count > 0)
      {
        calc.Clear();
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Percent")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.AccountingDocumentBase.CalculationFlagBaseSberDev.Percent;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.PercentCalc = prop.PercentCalc;
            }
          }
        }
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Absolute")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.AccountingDocumentBase.CalculationFlagBaseSberDev.Absolute;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.AbsoluteCalc = prop.AbsoluteCalc;
            }
          }
        }
      }
    }
    #endregion
    
    #region Финансовый -> Финансовый
    
    /// <summary>
    /// Заполнить общие свойства из выбранного документа
    /// </summary>
    /// <param name="contract"></param>
    public static void FillGeneralProperties(SBContracts.IAccountingDocumentBase doc, SBContracts.IAccountingDocumentBase docSelected)
    {
      doc.PayTypeBaseSberDev = docSelected.PayTypeBaseSberDev;
      doc.BudItemBaseSberDev = docSelected.BudItemBaseSberDev;
      doc.ContrTypeBaseSberDev = docSelected.ContrTypeBaseSberDev;
      doc.LeadingDocument = docSelected.LeadingDocument;
      if (docSelected.TotalAmount != null)
      {
        doc.TotalAmount = docSelected.TotalAmount;
        doc.Currency = docSelected.Currency;
      }
      doc.DeliveryMethod = docSelected.DeliveryMethod;
      if (docSelected.AccArtBaseSberDev != null)
        doc.AccArtBaseSberDev = docSelected.AccArtBaseSberDev != null && docSelected.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active ?
          docSelected.AccArtBaseSberDev : null;
      if (docSelected.MVPBaseSberDev != null)
        doc.MVPBaseSberDev = docSelected.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVPBaseSberDev : null;
      if (docSelected.MVZBaseSberDev != null)
        doc.MVZBaseSberDev = docSelected.MVZBaseSberDev != null && docSelected.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Active ?
          docSelected.MVZBaseSberDev : null;
      if (docSelected.MarketDirectSberDev != null)
        doc.MarketDirectSberDev = docSelected.MarketDirectSberDev.Status == SberContracts.MarketingDirection.Status.Active ?
          docSelected.MarketDirectSberDev : null;
      doc.Counterparty = docSelected.Counterparty;
      doc.ProdCollectionBaseSberDev.Clear();
      var collection = docSelected.ProdCollectionBaseSberDev;
      foreach (var prod in collection)
      {
        if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
        {
          var target = doc.ProdCollectionBaseSberDev.AddNew();
          target.Product = prod.Product;
        }
      }
    }
    
    /// <summary>
    /// Заполнить калькуляцию из выбранного документа
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="docSelected"></param>
    public static void FillCalculation(SBContracts.IAccountingDocumentBase doc, SBContracts.IAccountingDocumentBase docSelected)
    {
      var calc = doc.CalculationBaseSberDev;
      var calcSelected = docSelected.CalculationBaseSberDev;
      if (calcSelected.Count > 0)
      {
        calc.Clear();
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Percent")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.AccountingDocumentBase.CalculationFlagBaseSberDev.Percent;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.PercentCalc = prop.PercentCalc;
            }
          }
        }
        if (docSelected.CalculationFlagBaseSberDev.Value.Value == "Absolute")
        {
          doc.CalculationFlagBaseSberDev = SBContracts.AccountingDocumentBase.CalculationFlagBaseSberDev.Absolute;
          foreach(var prop in calcSelected)
          {
            if (prop.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Active)
            {
              var target = calc.AddNew();
              target.ProductCalc = prop.ProductCalc;
              target.AbsoluteCalc = prop.AbsoluteCalc;
            }
          }
        }
      }
    }
    #endregion
    
    #endregion
    
    /// <summary>
    /// Возвращает список завершенных рабочих периодов до текущей даты
    /// </summary>
    [Public]
    public List<IDateRange> GenerateCompletedDateRanges(DateTime currentDate, int numberOfSeries, string interval)
    {
      var dateRanges = new List<DateRange>();
      
      for (int i = 0; i < numberOfSeries; i++)
      {
        DateTime startDate;
        DateTime endDate;
        
        switch (interval.ToLower())
        {
          case "weeks":
            // Находим последнее завершенное воскресенье
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
              endDate = currentDate.Date.AddDays(-7 - (i * 7));  // Если сегодня воскресенье, берем предыдущее воскресенье
            else
              endDate = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek - (i * 7));  // Иначе берем ближайшее предыдущее воскресенье
            
            // Начало недели - понедельник (за 6 дней до воскресенья)
            startDate = endDate.AddDays(-6);
            break;
            
          case "months":
            // Находим последний завершенный месяц
            // Вычисляем первый день текущего месяца
            var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            
            // Затем вычисляем последний день предыдущего месяца с учетом смещения i
            endDate = firstDayOfCurrentMonth.AddMonths(-(i + 1)).AddDays(-1);
            
            // Начало месяца
            startDate = new DateTime(endDate.Year, endDate.Month, 1);
            break;
            
          case "quarters":
            // Вычисляем текущий квартал
            int currentQuarter = (currentDate.Month - 1) / 3 + 1;
            
            // Вычисляем, сколько кварталов назад нам нужно
            int quartersBack = i + 1;  // +1 потому что мы всегда берем предыдущий квартал
            
            // Вычисляем целевой год и квартал
            int targetYear = currentDate.Year;
            int targetQuarter = currentQuarter - quartersBack;
            
            // Корректируем год, если нужно
            while (targetQuarter <= 0)
            {
              targetQuarter += 4;
              targetYear--;
            }
            
            // Вычисляем последний месяц квартала
            int endMonth = targetQuarter * 3;
            // Вычисляем первый месяц квартала
            int startMonth = endMonth - 2;
            
            // Конец квартала
            endDate = new DateTime(targetYear, endMonth, DateTime.DaysInMonth(targetYear, endMonth));
            // Начало квартала
            startDate = new DateTime(targetYear, startMonth, 1);
            break;
            
          default:
            throw new ArgumentException("Недопустимый интервал. Поддерживаются: 'weeks', 'months', 'quarters'.");
        }
        
        dateRanges.Add(new DateRange
                       {
                         StartDate = startDate,
                         EndDate = endDate
                       });
      }
      
      // Переворачиваем список, чтобы самый старый период был первым
      dateRanges.Reverse();
      
      return dateRanges.Cast<IDateRange>().ToList();
    }
    
    /// <summary>
    /// Функция создает список с информацией о сериях в графике ControlFlowChart виджета
    /// </summary>
    /// <returns></returns>
    [Public]
    public List<sberdev.SBContracts.Structures.Module.IControlFlowSeriesInfo> CreateControlFlowSeriesInfosList()
    {
      List<ControlFlowSeriesInfo> list = new List<ControlFlowSeriesInfo>();
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "started",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription1,
                 R = 100, G = 149, B = 237
               });
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "completed",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription2,
                 R = 46, G = 159, B = 12
               });
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "inprocess",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription3,
                 R = 255, G = 165, B = 0
               });
      list.Add(new ControlFlowSeriesInfo
               {
                 ValueId = "expired",
                 Label = sberdev.SberContracts.Resources.PeriodDiscription4,
                 R = 217, G = 63, B = 60
               });
      return list.Cast<IControlFlowSeriesInfo>().ToList();
    }
    
    /// <summary>
    /// Возвращает русское название агрегации
    /// </summary>
    [Public]
    public string GetRusAggregationName(Nullable<Enumeration> aggr)
    {
      if (aggr == null)
        return null;
      string result = null;
      switch (aggr.Value.Value)
      {
        case "Devices":
          result = "Устройства";
          break;
        case "Licenses":
          result = "Лицензии";
          break;
        case "Services":
          result = "Сервисы";
          break;
      }
      return result;
    }
    /// <summary>
    /// Функция убиарет из строки символы, которые нельзя использовать в названии файла
    /// </summary>
    [Public]
    public string NormalizeFileName(string name)
    {
      char[] exept = {'/','\\',':','*','?','"','<','>','|'};
      char[] chars = name.ToCharArray();
      var list = chars.Where(c => exept.Contains(c) == false);
      string newName = "";
      foreach (var ch in list)
        newName = newName + ch.ToString();
      return newName;
    }

  }
}