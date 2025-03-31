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
    
    #region Проверка документа на совпадение с выбранной группой типов
    /// <summary>
    /// Проверяет, соответствует ли документ выбранному типу
    /// </summary>
    [Public]
    public bool MatchesDocumentType(Sungero.Domain.Shared.IEntity document, string documentType)
    {
      if (document == null)
        return false;
      
      // Все типы документов
      if (string.IsNullOrEmpty(documentType) || documentType == "All")
        return true;
      
      // Преобразуем документ к возможным типам
      var accounting = sberdev.SBContracts.AccountingDocumentBases.As(document);
      var contractual = sberdev.SBContracts.ContractualDocuments.As(document);
      var incomingInvoice = sberdev.SBContracts.IncomingInvoices.As(document);
      var abstractSup = sberdev.SberContracts.AbstractsSupAgreements.As(document);
      
      switch (documentType)
      {
        case "Contractual":
          // SBContracts.Contract, SBContracts.SupAgreement (наследники ContractualDocument)
          return contractual != null && abstractSup == null;
          
        case "IncInvoce":
          // SBContracts.IncomingInvoice
          return incomingInvoice != null;
          
        case "Accounting":
          // SBContracts.AccountingDocumentBase кроме IncomingInvoice
          return accounting != null && incomingInvoice == null;
          
        case "AbstractContr":
          // SberContracts.AbstractsSupAgreement
          return abstractSup != null;
          
        case "Another":
          // Все остальные типы
          return document != null && accounting == null && contractual == null;
          
        default:
          return false;
      }
    }
    
    /// <summary>
    /// Безопасная проверка соответствия документа выбранному типу с обработкой возможных исключений
    /// </summary>
    [Public]
    public bool SafeMatchesDocumentType(Sungero.Domain.Shared.IEntity document, string documentType)
    {
      try
      {
        if (document == null)
          return false;
        
        // Все типы документов
        if (string.IsNullOrEmpty(documentType) || documentType == "All")
          return true;
        
        // Преобразуем документ к возможным типам с защитой от ошибок приведения типов
        bool isAccounting = false;
        bool isContractual = false;
        bool isIncomingInvoice = false;
        bool isAbstractSup = false;
        
        try { isAccounting = sberdev.SBContracts.AccountingDocumentBases.Is(document); } catch { }
        try { isContractual = sberdev.SBContracts.ContractualDocuments.Is(document); } catch { }
        try { isIncomingInvoice = sberdev.SBContracts.IncomingInvoices.Is(document); } catch { }
        try { isAbstractSup = sberdev.SberContracts.AbstractsSupAgreements.Is(document); } catch { }
        
        switch (documentType)
        {
          case "Contractual":
            // SBContracts.Contract, SBContracts.SupAgreement (наследники ContractualDocument)
            return isContractual && !isAbstractSup;
            
          case "IncInvoce":
            // SBContracts.IncomingInvoice
            return isIncomingInvoice;
            
          case "Accounting":
            // SBContracts.AccountingDocumentBase кроме IncomingInvoice
            return isAccounting && !isIncomingInvoice;
            
          case "AbstractContr":
            // SberContracts.AbstractsSupAgreement
            return isAbstractSup;
            
          case "Another":
            // Все остальные типы
            return document != null && !isAccounting && !isContractual;
            
          default:
            return false;
        }
      }
      catch (Exception ex)
      {
        Logger.Error($"SafeMatchesDocumentType: Ошибка при проверке типа {documentType} для документа {document?.Id}", ex);
        return false;
      }
    }

    /// <summary>
    /// Получить документ из задания и проверить его тип
    /// </summary>
    [Public]
    public bool AssignmentMatchesDocumentType(Sungero.Workflow.IAssignment assignment, string documentType)
    {
      if (string.IsNullOrEmpty(documentType) || documentType == "All")
        return true;
      
      var task = SBContracts.ApprovalTasks.As(assignment.Task);
      
      if (task.DocumentTypeSungero == documentType)
        return true;
      
      return MatchesDocumentType(task.DocumentGroup.OfficialDocuments.FirstOrDefault(), documentType);
    }

    /// <summary>
    /// Получить документ из задачи и проверить его тип
    /// </summary>
    [Public]
    public bool TaskMatchesDocumentType(Sungero.Workflow.ITask task, string documentType)
    {
      try
      {
        // Для всех типов документов возвращаем true без проверки
        if (string.IsNullOrEmpty(documentType) || documentType == "All")
          return true;
        
        // Если задача пуста, возвращаем false
        if (task == null)
          return false;
        
        // Преобразуем к типу задачи согласования
        var approvalTask = sberdev.SBContracts.ApprovalTasks.As(task);
        if (approvalTask == null || approvalTask.DocumentGroup == null)
          return false;
        
        // Проверяем, есть ли в задаче сохраненное значение DocumentType
        if (!string.IsNullOrEmpty(approvalTask.DocumentTypeSungero))
        {
          // Если типы совпадают, возвращаем true
          return approvalTask.DocumentTypeSungero == documentType;
        }
        
        // Безопасное получение документа с обработкой возможных исключений
        try
        {
          // Получаем первый документ из группы вложений
          var documents = approvalTask.DocumentGroup.OfficialDocuments.ToList();
          if (documents == null || !documents.Any())
            return false;
          
          var document = documents.FirstOrDefault();
          if (document == null)
            return false;
          
          // Определяем тип документа и сохраняем его в задаче для повторного использования
          bool result = MatchesDocumentType(document, documentType);
          
          // Сохраняем определенный тип в поле DocumentType для будущего использования
          if (result && string.IsNullOrEmpty(approvalTask.DocumentTypeSungero))
          {
            try
            {
              approvalTask.DocumentTypeSungero = documentType;
              approvalTask.Save();
            }
            catch (Exception ex)
            {
              Logger.Error($"Ошибка при сохранении типа документа для задачи {task.Id}", ex);
            }
          }
          
          return result;
        }
        catch (Exception ex)
        {
          Logger.Error($"Ошибка при получении документа из задачи {task.Id}", ex);
          return false;
        }
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка в TaskMatchesDocumentType для задачи {task?.Id}", ex);
        return false;
      }
    }
    #endregion
    
    /// <summary>
    /// Возвращает список завершенных рабочих периодов до текущей даты
    /// </summary>
    [Public]
    public List<IDateRange> GenerateCompletedDateRanges(DateTime currentDate, int numberOfSeries, string interval)
    {
      var dateRanges = new List<DateRange>();
      DateTime endDate = currentDate.Date;

      for (int i = 0; i < numberOfSeries; i++)
      {
        DateTime startDate;
        
        switch (interval.ToLower())
        {
          case "weeks":
            // Логика для недель остается без изменений
            endDate = endDate.AddDays(-(int)endDate.DayOfWeek);
            startDate = endDate.AddDays(-6);
            break;

          case "months":
            // Логика для месяцев остается без изменений
            endDate = new DateTime(endDate.Year, endDate.Month, 1).AddDays(-1);
            startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-1);
            break;

          case "quarters":
            int quarter = (endDate.Month - 1) / 3;
            int startMonth = quarter * 3 + 1;
            int endMonth = startMonth + 2;
            
            // Корректировка года если квартал в конце года
            int year = endDate.Month >= 10 ? endDate.Year : endDate.Year;
            
            endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));
            startDate = new DateTime(year, startMonth, 1);
            break;

          default:
            throw new ArgumentException("Недопустимый интервал. Поддерживаются: 'weeks', 'months', 'quarters'.");
        }

        dateRanges.Insert(0, new DateRange
                          {
                            StartDate = startDate,
                            EndDate = endDate
                          });

        // Переход к предыдущему периоду
        endDate = startDate.AddDays(-1);
      }

      return dateRanges.Cast<IDateRange>().ToList();
    }
    
    /// <summary>
    /// Функция создает список с информацией о сериях в графике ControlFlowChart виджета ApprovalAnalyticsWidget
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