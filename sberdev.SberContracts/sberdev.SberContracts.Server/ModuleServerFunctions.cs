using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using Sungero.Exchange;
using OfficeOpenXml;
using sberdev.SBContracts.Structures.Module;

namespace sberdev.SberContracts.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Функция пересоханения всех записей КА
    /// </summary>
    [Public]
    public string ReSaveServer()
    {
      var CompanyList = sberdev.SBContracts.Companies.GetAll(c => c.Status == sberdev.SBContracts.Company.Status.Active);
      string log = "";
      int ind = 0;
      foreach (var comp in CompanyList)
      {
        try
        {
          comp.Name = comp.Name;
          comp.Save();
        }
        catch (Exception e)
        {
          log += comp.Name.ToString() + "| Ошибка: " + e.Message.ToString();
        }
        ind += 1;
      }
      return log;
    }
    
    #region Минифункции
    
    /// <summary>
    /// Возвращает генеральный продукт для нашей организации документа
    /// </summary>
    /// <param name="doc">Документ</param>
    /// <returns></returns>
    [Public]
    public IProductsAndDevices GetOrCreateGeneralProduct(SBContracts.IOfficialDocument doc)
    {
      var genProd = ProductsAndDeviceses.GetAll(p => p.BusinessUnit == doc.BusinessUnit && p.Name == "General").FirstOrDefault();
      
      if (genProd == null)
      {
        var newGenProd = ProductsAndDeviceses.Create();
        newGenProd.BusinessUnit = doc.BusinessUnit;
        newGenProd.Name = "General";
        
        // Сохранение перед возвратом сущности
        newGenProd.Save();
        
        // Возвращаем сущность
        return newGenProd;
      }
      else
      {
        return genProd;
      }
    }
    
    /// <summary>
    /// Получить все цифры после указанного тега
    /// </summary>
    [Public]
    public string GetNumberTag(string input, string tag)
    {
      string result = "";
      int i = input.IndexOf(tag);
      if (i > 0)
      {
        i += tag.Length;
        while (i < input.Length && Char.IsDigit(input[i]))
        {
          result += input[i];
          i++;
        }
      }
      return result;
    }
    
    /// <summary>
    /// Возвращает список ИНН всех "Наших организаций"
    /// </summary>
    [Public]
    public List<string> GetAllBuisnessUnitTINs()
    {
      return Sungero.Company.BusinessUnits.GetAll().Select(b => b.TIN).ToList();
    }
    
    [Public, Remote]
    public static string DeleteAllContractsSrv()
    {
      var s = "";
      var contracts = Sungero.Contracts.Contracts.GetAll(); // sberdev.SBContracts.Contracts.GetAll();
      foreach (var contract in contracts)
      {
        try{
          var newcontract = Sungero.Contracts.Contracts.Get(contract.Id);
          //Transactions.Execute(() => {
          Sungero.Contracts.Contracts.Delete(newcontract) ;
          //                    });//sberdev.SBContracts.Contracts.Delete(contract);
        }
        catch (Exception ex)
        {
          s = s+System.Environment.NewLine+ex.Message;
        }
        
      }
      return s;
    }
    
    [Public, Remote]
    public static void DeleteAllContractsSrvNew(Sungero.Contracts.IContract contract)
    {
      Sungero.Contracts.Contracts.Delete(contract) ;
    }
    
    public static string GetCellValue(object cell)
    {
      return (cell != null) ? (cell.ToString() != "NULL" ? cell.ToString() : "") : "";
    }
    
    #endregion
    
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
            return !isAccounting && !isContractual;
            
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
      try
      {
        if (assignment == null)
          return false;
        
        if (string.IsNullOrEmpty(documentType) || documentType == "All")
          return true;
        
        bool isSubtaskAssignment = assignment.Task != assignment.MainTask;
        
      /*  if(!assignment.Attachments.Any())
        {
          Logger.Error($"AssignmentMatchesDocumentType: Нет вложений для задания {assignment?.Id}");
          return false;
        }*/
        
        if (isSubtaskAssignment)
          return SafeMatchesDocumentType(assignment.Attachments.FirstOrDefault(), documentType);
        else
        {
          var task = SBContracts.ApprovalTasks.As(assignment.Task);
          
          if (task == null)
            return SafeMatchesDocumentType(assignment.Attachments.FirstOrDefault(), documentType);
          
          if (task.DocumentTypeSungero == documentType)
            return true;
          else
            return SafeMatchesDocumentType(task.DocumentGroup.OfficialDocuments.FirstOrDefault(), documentType);
        }
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка в AssignmentMatchesDocumentType для задания {assignment?.Id}", ex);
        return false;
      }
    }

    /// <summary>
    /// Получить документ из задачи и проверить его тип
    /// </summary>
    [Public]
    public bool TaskMatchesDocumentType(SBContracts.IApprovalTask task, string documentType)
    {
      try
      {
        // Для всех типов документов возвращаем true без проверки
        if (string.IsNullOrEmpty(documentType) || documentType == "All")
          return true;
        
        // Если задача пуста, возвращаем false
        if (task == null)
          return false;
        
        // Проверяем, есть ли в задаче сохраненное значение DocumentType
        if (!string.IsNullOrEmpty(task.DocumentTypeSungero))
        {
          // Если типы совпадают, возвращаем true
          return task.DocumentTypeSungero == documentType;
        }
        
        // Безопасное получение документа с обработкой возможных исключений
        try
        {
          var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
          if (document == null)
            return false;
          
          // Определяем тип документа и сохраняем его в задаче для повторного использования
          bool result = SafeMatchesDocumentType(document, documentType);
          
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
    
    #region Удаленные функции
    
    [Public, Remote]
    public static void LinkDocs( Sungero.Docflow.IApprovalTask task )
    {
      var docs = task.AllAttachments.ToList();
      var mainDoc = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      foreach (var docInf in docs)
      {
        if (mainDoc.Id != docInf.Id )
        {
          var doc = Sungero.Content.ElectronicDocuments.Get(docInf.Id);
          doc.Relations.AddFrom("Simple relation", mainDoc );
        }
      }
    }
    
    [Public, Remote]
    public static IApprovalTask CreateApprovalTask(sberdev.SBContracts.IContract document)
    {
      var task = ApprovalTasks.Create();
      task.DocumentGroup.All.Add(document);
      
      return task;
    }

    #region Функции миграции
    
    [Public, Remote]
    public static Sungero.Core.IZip MigrationContractCard(string filePath)
    {
      FileInfo fi = new FileInfo(filePath);
      var logId = Guid.NewGuid().ToString();
      var logFilePath =  Path.GetTempPath() + logId + ".txt";
      
      using (ExcelPackage excelPackage = new ExcelPackage(fi))
      {
        
        ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];
        var row = 2;
        var errNum = 0;
        var messageList = new List<String>();
        var messageFormat = "Ошибка в строке {0} Excel файла. Текст ошибки: {1}";
        var idContract = GetCellValue(firstWorksheet.Cells[row, 1].Value);
        
        messageList.Add("==========================================================" + Environment.NewLine + "Старт процедуры миграции " + Sungero.Core.Calendar.Now.ToString() + Environment.NewLine + "==========================================================");
        
        while (idContract != "")
        {
          try
          {
            
            //Обязательные
            var contrType = GetCellValue(firstWorksheet.Cells[row, 2].Value);
            var contrCategory = GetCellValue(firstWorksheet.Cells[row, 3].Value);
            var contrNum = GetCellValue(firstWorksheet.Cells[row, 4].Value);
            var contrDate = GetCellValue(firstWorksheet.Cells[row, 5].Value);
            var contrSubj = GetCellValue(firstWorksheet.Cells[row, 6].Value);
            var contrAmount = GetCellValue(firstWorksheet.Cells[row, 7].Value);
            var contrCurrency = GetCellValue(firstWorksheet.Cells[row, 8].Value);
            var contactINN = GetCellValue(firstWorksheet.Cells[row, 9].Value);
            var contactKPP = GetCellValue(firstWorksheet.Cells[row, 10].Value);
            var contactGUID = GetCellValue(firstWorksheet.Cells[row, 11].Value);
            var failedTrig = false;
            
            //Ищем документ или создаем новый
            SBContracts.IContract contract;
            contract = SBContracts.Contracts.GetAll(c => c.GUID1C != null && c.GUID1C == idContract).FirstOrDefault();
            if (contract == null)
            {
              contract = SBContracts.Contracts.Create();
            }
            
            //Заполнить исторический ИД ИУС ПТ
            contract.GUID1C = idContract;
            
            //Заполнение типа документа (с проверкой)
            if (contrType != null)
            {
              if (contrType == "Доходный")
              {
                contract.ContrTypeBaseSberDev = SBContracts.Contract.ContrTypeBaseSberDev.Profitable;
                contract.MVPBaseSberDev = sberdev.SberContracts.MVZs.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
              }
              else
              {
                contract.ContrTypeBaseSberDev = SBContracts.Contract.ContrTypeBaseSberDev.Expendable;
                contract.MVZBaseSberDev = sberdev.SberContracts.MVZs.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();}
              
              //Заполнение Категории (с проверкой)
              if (contrCategory != null)
              {
                if (contrCategory == "С покупателем")
                {
                  contract.DocumentGroup = sberdev.SBContracts.ContractCategories.GetAll(c => c.Name == "С покупателем").FirstOrDefault();
                }
                else
                {
                  if (contrCategory == "С поставщиком")
                  {contract.DocumentGroup = sberdev.SBContracts.ContractCategories.GetAll(c => c.Name == "С поставщиком").FirstOrDefault();}
                  else
                  {contract.DocumentGroup = sberdev.SBContracts.ContractCategories.GetAll(c => c.Name == "С комиссионером").FirstOrDefault();}
                }
                if (contrDate == null || contrDate == "")
                {
                  string pattern = @"\d\d\.\d\d\.\d\d\d?\d?";
                  Match match = Regex.Match(contrSubj, pattern);
                  if (match.Success)
                  {
                    contrDate = match.Value;
                  }
                }
                
                if (contrDate != ""){
                  contract.DocumentDate = Convert.ToDateTime(contrDate);
                }
                
                //Заполнение темы (с проверкой)
                if (contrSubj != "")
                {
                  contract.Subject = contrSubj;

                  //Заполнение Суммы
                  if (contrAmount != "")
                    contract.TotalAmount = Convert.ToDouble(contrAmount.Replace('.',','));
                  
                  //Заполнение Валют
                  var currency = Sungero.Commons.Currencies.GetAll(c => c.AlphaCode == contrCurrency).FirstOrDefault();
                  if (currency != null)
                    contract.Currency = currency;
                  
                  //Заполнение журнала регистрации.
                  contract.DocumentRegister = Sungero.Docflow.DocumentRegisters.GetAll(c => c.Name == "Договоры").FirstOrDefault();
                  
                  //Заполнение Рег номера
                  if (contrNum != "")
                    contract.RegistrationNumber = contrNum;
                  else
                  {
                    string patternNum = @"№\s*\w+\s";
                    Match matchNum = Regex.Match(contrSubj, patternNum);
                    if (matchNum.Success)
                    {contract.RegistrationNumber = matchNum.Value.Substring(2);}
                  }
                  
                  //Заполнение даты регистрации
                  if (contrDate != "")
                    contract.RegistrationDate = Convert.ToDateTime(contrDate);
                  
                  //Заполнение состояния значением "Действующий"
                  contract.LifeCycleState = Sungero.Contracts.ContractualDocument.LifeCycleState.Active;
                  
                  //Заполнение ответственного подразделения
                  contract.Department = Sungero.Company.Departments
                    .GetAll(c => c.Name == "Администрация" || c.Name == "Администрация/Генеральный директор").FirstOrDefault();
                  
                  Sungero.Parties.ICompany contact = null;
                  
                  //Заполнение контрагента
                  if (!string.IsNullOrEmpty(contactGUID))
                  {
                    using (var connection = SQL.CreateConnection())
                    {
                      connection.Open();
                      
                      using (var command = connection.CreateCommand())
                      {
                        command.CommandText = string.Format(
                          "SELECT EntityId " +
                          "FROM [dbo].[Sungero_Commons_ExtEntityLinks] " +
                          "WHERE ExtEntityId = '{0}'", contactGUID);
                        
                        using (var reader = command.ExecuteReader())
                        {
                          while (reader.Read())
                          {
                            if (reader[0] != null)
                            {
                              var entityId = Convert.ToInt16(reader[0].ToString());
                              contact = Sungero.Parties.Companies.GetAll(r => r.Id == entityId).FirstOrDefault();

                              if (contact != null)
                                contract.Counterparty = contact;
                              else
                              {
                                var pers = Sungero.Parties.Counterparties.GetAll(r => r.Id == entityId).FirstOrDefault();
                                if (pers != null)
                                  contract.Counterparty = pers;
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                  else{}
                  
                  if (contract.Counterparty == null)
                  {
                    if (contactINN != "" && contactKPP != "")
                    {
                      contact = Sungero.Parties.Companies.GetAll(c => c.TIN == contactINN && c.TRRC == contactKPP).FirstOrDefault();
                      if (contact != null)
                        contract.Counterparty = contact;
                      else
                      {
                        messageList.Add(String.Format(messageFormat, row, "Не найден контрагент с ИНН " + contactINN + " КПП " + contactKPP + "."));
                        failedTrig = true;
                        errNum++;
                      }
                    }
                  }
                  else{}
                  
                  
                }
                else
                {
                  messageList.Add(String.Format(messageFormat, row, "Незаполнен о содержание. Создать договор невозможно."));
                  failedTrig = true;
                  errNum++;
                }
              }
              else
              {
                messageList.Add(String.Format(messageFormat, row, "Не заполнена категория договора. Создать договор невозможно."));
                failedTrig = true;
                errNum++;
              }
            }
            else
            {
              messageList.Add(String.Format(messageFormat, row, "Не заполнен тип договора. Создать договор невозможно."));
              failedTrig = true;
              errNum++;
            }
            contract.BusinessUnit = Sungero.Company.BusinessUnits.GetAll(c => c.Name == "ООО «ТД ЕПК" || c.Name == "ООО \"СберДевайсы\"").FirstOrDefault();
            contract.AccArtPrBaseSberDev = SberContracts.AccountingArticleses.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
            contract.ProdCollectionPrBaseSberDev.AddNew().Product = SberContracts.ProductsAndDeviceses.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
            contract.Save();
          }
          catch (Exception ex)
          {
            messageList.Add(String.Format(messageFormat, row, ex.Message));
            errNum++;
          }
          
          row++;
          idContract = GetCellValue(firstWorksheet.Cells[row, 1].Value);
        }
        
        
        using (StreamWriter sw = new StreamWriter(logFilePath, false, System.Text.Encoding.Default))
        {
          foreach(var text in messageList)
            sw.WriteLine(text);
          sw.WriteLine("==========================================================================");
          sw.WriteLine("Конец загрузки. Всего ошибок: " + errNum.ToString());
        }
      }
      
      //Заархивировать файл и вернуть в клиентский код
      IZip zip = Zip.Create();
      var zipFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".zip";
      using (FileStream fstream = File.OpenRead(logFilePath))
      {
        byte[] array = new byte[fstream.Length];
        fstream.Read(array, 0, array.Length);
        var today = Sungero.Core.Calendar.Now;
        var filePathName = "Log_" + logId + " от " + today.Day.ToString() + "_" +
          today.Month.ToString() + "_" + today.Year.ToString(); //Пока так, чтобы сэкономить время на обработку имени файла от нечитаемых символов и т.д.
        zip.Add(array, filePathName,".txt");
        zip.Save(zipFilePath);
      }
      
      System.IO.File.Delete(filePath);
      System.IO.File.Delete(logFilePath);
      System.IO.File.Delete(zipFilePath);
      return zip;
      
    }
    
    /// <summary>
    /// Загрузка файлов для раннее загруженных договоров
    /// </summary>
    [Public, Remote]
    public static Sungero.Core.IZip MigrationFile(string filePath)
    {
      FileInfo fi = new FileInfo(filePath);
      var logId = Guid.NewGuid().ToString();
      var logFilePath = Path.GetTempPath() + logId + ".txt";
      using (ExcelPackage excelPackage = new ExcelPackage(fi))
      {
        ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];
        var row = 2;
        var errNum = 0;
        var messageList = new List<String>();
        var messageFormat = "Ошибка в строке {0} Excel файла. Текст ошибки: {1}";
        var idContract = GetCellValue(firstWorksheet.Cells[row, 1].Value);
        
        messageList.Add("==========================================================" + Environment.NewLine + "Старт процедуры миграции "
                        + Sungero.Core.Calendar.Now.ToString() + Environment.NewLine + "==========================================================");
        while (idContract != "")
        {
          try
          {
            var contract = SBContracts.Contracts.GetAll(c => c.GUID1C != null && c.GUID1C == idContract).FirstOrDefault();
            
            if (contract != null)
            {
              var docFilePath = GetCellValue(firstWorksheet.Cells[row, 13].Value);
              
              //Конкатенируем путь и загружаем файл в документ
              if (docFilePath != ""){
                string[] subStrs = docFilePath.Split(';');
                var contrtrig = true;
                foreach (var str in subStrs)
                {
                  if (contrtrig)
                  {
                    contract.CreateVersion();
                    contract.Import(str);
                    contract.Save();
                    contrtrig = false;
                  }
                  else
                  {
                    var otherDoc = SberContracts.OtherContractDocuments.Create();
                    otherDoc.Subject = str;
                    otherDoc.LeadingDocument = contract;
                    otherDoc.DocumentKind = Sungero.Docflow.DocumentKinds.GetAll(c => c.Name == "Иные документы").FirstOrDefault();
                    otherDoc.CreateVersion();
                    otherDoc.Import(str);
                    otherDoc.Save();
                  }
                }
              }
              else
              {
                messageList.Add(String.Format(messageFormat, row, "Не заполнен путь к файлу."));
                errNum++;
              }
            }
            else
            {
              messageList.Add(String.Format(messageFormat, row, "Не найден договор с ИД " + idContract));
              errNum++;
            }
            
            
          }
          catch (Exception ex)
          {
            messageList.Add(String.Format(messageFormat, row, ex.Message));
            errNum++;
          }
          row++;
          idContract = GetCellValue(firstWorksheet.Cells[row, 1].Value);
        }
        
        using (StreamWriter sw = new StreamWriter(logFilePath, false, System.Text.Encoding.Default))
        {
          foreach(var text in messageList)
            sw.WriteLine(text);
          sw.WriteLine("==========================================================================");
          sw.WriteLine("Конец загрузки. Всего ошибок: " + errNum.ToString());
        }
      }
      
      //Заархивировать файл и вернуть в клиентский код
      IZip zip = Zip.Create();
      var zipFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".zip";
      using (FileStream fstream = File.OpenRead(logFilePath))
      {
        byte[] array = new byte[fstream.Length];
        fstream.Read(array, 0, array.Length);
        var today = Sungero.Core.Calendar.Now;
        var filePathName = "Log_" + logId + " от " + today.Day.ToString() + "_" +
          today.Month.ToString() + "_" + today.Year.ToString(); //Пока так, чтобы сэкономить время на обработку имени файла от нечитаемых символов и т.д.
        zip.Add(array, filePathName,".txt");
        zip.Save(zipFilePath);
      }
      
      System.IO.File.Delete(filePath);
      System.IO.File.Delete(logFilePath);
      System.IO.File.Delete(zipFilePath);
      return zip;
    }
    
    #endregion

    #region Функция распределения
    
    /// <summary>
    /// Общая функция для кнопки распределения в документах с калькуляцией
    /// </summary>
    [Public, Remote]
    public void DistributeToCalculation(Sungero.Docflow.IOfficialDocument doc)
    {
      var products = SberContracts.ProductsAndDeviceses.GetAll().Where(p => p.Name != "General" && p.BusinessUnit == doc.BusinessUnit
                                                                       && p.Status == SberContracts.ProductsAndDevices.Status.Active
                                                                       && (p.NoDistribute == false || p.NoDistribute == null));
      
      #region Договорные
      var contractual = sberdev.SBContracts.ContractualDocuments.As(doc);
      if (contractual != null)
      {
        contractual.CalculationBaseSberDev.Clear();
        
        if (contractual.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Devices)
          foreach (var product in products)
            if (product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Devices)
        {
          var line = contractual.CalculationBaseSberDev.AddNew();
          line.ProductCalc = product;
        }
        
        if (contractual.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Licenses)
          foreach (var product in products)
            if (product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Licenses)
        {
          var line = contractual.CalculationBaseSberDev.AddNew();
          line.ProductCalc = product;
        }
        
        if (contractual.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Products)
          foreach (var product in products)
            if (product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Devices
                || product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Licenses)
        {
          var line = contractual.CalculationBaseSberDev.AddNew();
          line.ProductCalc = product;
        }
        
        if (contractual.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.ProductUnitsSberDev)
        {
          var units = PublicFunctions.Module.CreateProductUnitDialog();
          if (units != null)
            foreach (var unit in units)
              foreach (var product in products)
                if (product.ProductUnit == unit)
          {
            var line = contractual.CalculationBaseSberDev.AddNew();
            line.ProductCalc = product;
          }
        }
        
        if (contractual.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.SelectedProds)
        {
          var listEx = contractual.ProdCollectionExBaseSberDev.Select(p => p.Product).ToList();
          var listPr = contractual.ProdCollectionPrBaseSberDev.Select(p => p.Product).ToList();
          var selectedProds = listEx.Union(listPr).Where(p => p.Name != "General").ToList();
          foreach (var product in selectedProds)
          {
            var line = contractual.CalculationBaseSberDev.AddNew();
            line.ProductCalc = product;
          }
        }
        
        if (contractual.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Tamplate)
        {
          var tamplate = PublicFunctions.Module.CreateCalcTamplateDialog(contractual.CalculationFlagBaseSberDev.Value.Value);
          if (tamplate != null)
            if (tamplate.CalculationFlag.Value == SberContracts.CalculationTamplate.CalculationFlag.Percent)
              foreach (var element in tamplate.ProdCollection)
          {
            var line = contractual.CalculationBaseSberDev.AddNew();
            line.ProductCalc = element.Product;
            line.PercentCalc = element.Percent;
          }
          else
          {
            contractual.TotalAmount = tamplate.Amount;
            foreach (var element in tamplate.ProdCollection)
            {
              var line = contractual.CalculationBaseSberDev.AddNew();
              line.ProductCalc = element.Product;
              line.AbsoluteCalc = element.Absolute;
            }
          }
        }
        
        
        if (contractual.CalculationFlagBaseSberDev.HasValue && contractual.CalculationBaseSberDev.Any()
            && contractual.CalculationDistributeBaseSberDev.Value != SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Tamplate)
          if (contractual.CalculationFlagBaseSberDev == SBContracts.Contract.CalculationFlagBaseSberDev.Absolute)
        {
          double amountPart = Math.Round(contractual.TotalAmount.Value / contractual.CalculationBaseSberDev.Count, 2, MidpointRounding.AwayFromZero);
          foreach (var prop in contractual.CalculationBaseSberDev)
            prop.AbsoluteCalc = amountPart;
          if (contractual.CalculationResidualAmountBaseSberDev != 0)
            contractual.CalculationBaseSberDev.Last().AbsoluteCalc += contractual.CalculationResidualAmountBaseSberDev;
        }
        else
        {
          int count = contractual.CalculationBaseSberDev.Count;
          double percentPart = Math.Round((double)100 / count, 6, MidpointRounding.AwayFromZero);
          foreach (var prop in contractual.CalculationBaseSberDev)
            prop.PercentCalc = percentPart;
          if (contractual.CalculationResidualAmountBaseSberDev != 0)
            contractual.CalculationBaseSberDev.Last().PercentCalc += contractual.CalculationResidualAmountBaseSberDev;
        }
      }
      #endregion
      
      #region Финансовые
      var accBase = sberdev.SBContracts.AccountingDocumentBases.As(doc);
      if (accBase != null)
      {
        accBase.CalculationBaseSberDev.Clear();
        
        if (accBase.CalculationDistributeBaseSberDev.Value == SBContracts.AccountingDocumentBase.CalculationDistributeBaseSberDev.Devices)
          foreach (var product in products)
            if (product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Devices)
        {
          var line = accBase.CalculationBaseSberDev.AddNew();
          line.ProductCalc = product;
        }
        
        if (accBase.CalculationDistributeBaseSberDev.Value == SBContracts.AccountingDocumentBase.CalculationDistributeBaseSberDev.Licenses)
          foreach (var product in products)
            if (product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Licenses)
        {
          var line = accBase.CalculationBaseSberDev.AddNew();
          line.ProductCalc = product;
        }
        
        if (accBase.CalculationDistributeBaseSberDev.Value == SBContracts.AccountingDocumentBase.CalculationDistributeBaseSberDev.Products)
          foreach (var product in products)
            if (product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Devices
                || product.Aggregation == SberContracts.ProductsAndDevices.Aggregation.Licenses)
        {
          var line = accBase.CalculationBaseSberDev.AddNew();
          line.ProductCalc = product;
        }
        
        if (accBase.CalculationDistributeBaseSberDev.Value == SBContracts.AccountingDocumentBase.CalculationDistributeBaseSberDev.ProductUnitsSberDev)
        {
          var units = PublicFunctions.Module.CreateProductUnitDialog();
          if (units != null)
            foreach (var unit in units)
              foreach (var product in products)
                if (product.ProductUnit == unit)
          {
            var line = accBase.CalculationBaseSberDev.AddNew();
            line.ProductCalc = product;
          }
        }
        
        if (accBase.CalculationDistributeBaseSberDev.Value == SBContracts.AccountingDocumentBase.CalculationDistributeBaseSberDev.SelectedProds)
        {
          var selectedProds = accBase.ProdCollectionBaseSberDev.Where(p => p.Product.Name != "General").ToList();
          foreach (var product in selectedProds)
          {
            var line = accBase.CalculationBaseSberDev.AddNew();
            line.ProductCalc = product.Product;
          }
        }
        
        if (accBase.CalculationDistributeBaseSberDev.Value == SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Tamplate)
        {
          var tamplate = PublicFunctions.Module.CreateCalcTamplateDialog(accBase.CalculationFlagBaseSberDev.Value.Value);
          if (tamplate != null)
            if (tamplate.CalculationFlag.Value == SberContracts.CalculationTamplate.CalculationFlag.Percent)
              foreach (var element in tamplate.ProdCollection)
          {
            var line = accBase.CalculationBaseSberDev.AddNew();
            line.ProductCalc = element.Product;
            line.PercentCalc = element.Percent;
          }
          else
          {
            accBase.TotalAmount = tamplate.Amount;
            foreach (var element in tamplate.ProdCollection)
            {
              var line = accBase.CalculationBaseSberDev.AddNew();
              line.ProductCalc = element.Product;
              line.AbsoluteCalc = element.Absolute;
            }
          }
        }
        
        if (accBase.CalculationFlagBaseSberDev.HasValue && accBase.CalculationBaseSberDev.Any()
            && accBase.CalculationDistributeBaseSberDev.Value != SBContracts.ContractualDocument.CalculationDistributeBaseSberDev.Tamplate)
          if (accBase.CalculationFlagBaseSberDev == SBContracts.ContractStatement.CalculationFlagBaseSberDev.Absolute)
        {
          double amountPart = Math.Round(accBase.TotalAmount.Value / accBase.CalculationBaseSberDev.Count, 2, MidpointRounding.AwayFromZero);
          foreach (var prop in accBase.CalculationBaseSberDev)
            prop.AbsoluteCalc = amountPart;
          if (accBase.CalculationResidualAmountBaseSberDev != 0)
            accBase.CalculationBaseSberDev.Last().AbsoluteCalc += accBase.CalculationResidualAmountBaseSberDev;
        }
        else
        {
          int count = accBase.CalculationBaseSberDev.Count;
          double percentPart = Math.Round((double)100 / count, 6, MidpointRounding.AwayFromZero);
          foreach (var prop in accBase.CalculationBaseSberDev)
            prop.PercentCalc = percentPart;
          if (accBase.CalculationResidualAmountBaseSberDev != 0)
            accBase.CalculationBaseSberDev.Last().PercentCalc += accBase.CalculationResidualAmountBaseSberDev;
        }
      }
      #endregion
    }

    #endregion
    
    #region Функции кнопок заполнения свойств в карточках
    
    
    /// <summary>
    /// Заполнить из предыдущего документа такого же типа (не стал дорабатывать так как это чисто моя инициатива)
    /// </summary>
    /// <param name="doc">Документ который заполняется</param>
    /// <param name="usr">Пользователь (для получения кэша)</param>
    [Public, Remote]
    public static void FillFromCasheSrv(Sungero.Docflow.IOfficialDocument doc , IUser usr)
    {
      IAnaliticsCashe cashe = null;
      var contractual = SBContracts.ContractualDocuments.As(doc);
      var accounting = SBContracts.AccountingDocumentBases.As(doc);
      
      if (contractual != null)
      {
        if (contractual.Counterparty != null)
        {
          cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(r => r.User == usr && r.Counterparty == contractual.Counterparty).FirstOrDefault();
          if (cashe == null)
            cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(r => r.User == usr).OrderByDescending(r => r.Modified).FirstOrDefault();
        }
        else
          cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(r => r.User == usr).OrderByDescending(r => r.Modified).FirstOrDefault();
      }
      
      if (accounting != null)
      {
        if (accounting.Counterparty != null)
        {
          cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(r => r.User == usr && r.Counterparty == accounting.Counterparty).FirstOrDefault();
          if (cashe == null)
            cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(r => r.User == usr).OrderByDescending(r => r.Modified).FirstOrDefault();
        }
        else
          cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(r => r.User == usr).OrderByDescending(r => r.Modified).FirstOrDefault();
      }
      
      if (cashe == null)
      {
        cashe = sberdev.SberContracts.AnaliticsCashes.Create();
        cashe.Counterparty = contractual != null ? contractual.Counterparty : accounting.Counterparty;
        cashe.Save();
      }
      else
      {
        if (contractual != null)
        {
          contractual.ContrTypeBaseSberDev = cashe.ContrType;
          contractual.DocumentGroup = cashe.DocumentGroup;
          contractual.IsStandard = cashe.IsStandard;
          contractual.AccArtPrBaseSberDev = cashe.AccArt;
          contractual.AccArtExBaseSberDev = cashe.AccArtMVZ;
          contractual.MVZBaseSberDev = cashe.MVZ;
          contractual.MVPBaseSberDev = cashe.MVP;
          contractual.MarketDirectSberDev = cashe.MarkDirection;
          contractual.ExitCommentBaseSberDev = cashe.ExitComment;
          contractual.Counterparty = cashe.Counterparty;
          if (cashe.ProdCollection.Count > 0)
          {
            contractual.ProdCollectionPrBaseSberDev.Clear();
            var collection = cashe.ProdCollection;
            foreach (var str in collection)
            {
              var i = contractual.ProdCollectionPrBaseSberDev.AddNew();
              i.Product = str.Prod;
            }
          }
          if (cashe.ProdMVZCollection.Count > 0)
          {
            contractual.ProdCollectionExBaseSberDev.Clear();
            foreach (var str in cashe.ProdMVZCollection)
            {
              var i = contractual.ProdCollectionExBaseSberDev.AddNew();
              i.Product = str.ProdMVZProp;
            }
          }
        }
        
        if (accounting != null)
        {
          if (cashe.ContrType != null)
          {
            accounting.PayTypeBaseSberDev = cashe.PayTypeBaseSberDev;
            accounting.ContrTypeBaseSberDev = cashe.ContrType;
            accounting.AccArtBaseSberDev= cashe.AccArt;
            accounting.MVZBaseSberDev = cashe.MVZ;
            accounting.MVPBaseSberDev = cashe.MVP;
            accounting.MarketDirectSberDev = cashe.MarkDirection;
            accounting.Counterparty = cashe.Counterparty;
            if (cashe.ProdCollection.Count > 0)
            {
              accounting.ProdCollectionBaseSberDev.Clear();
              var collection = cashe.ProdCollection;
              foreach (var str in collection)
              {
                var i = accounting.ProdCollectionBaseSberDev.AddNew();
                i.Product = str.Prod;
              }
            }
          }
        }
      }
    }
    
    
    /// <summary>
    /// Заполнить из любого предыдущего документа
    /// </summary>
    /// <param name="doc">Документ который заполняется</param>
    /// <param name="usr">Пользователь (для получения кэша)</param>
    [Public, Remote]
    public static void FillFromCasheGeneralSrv(Sungero.Docflow.IOfficialDocument doc , IUser usr)
    {
      IAnaliticsCasheGeneral cashe = null;
      var contractual = SBContracts.ContractualDocuments.As(doc);
      var accounting = SBContracts.AccountingDocumentBases.As(doc);
      
      if (contractual != null || accounting != null)
        cashe = sberdev.SberContracts.AnaliticsCasheGenerals.GetAll(r => r.User == usr).FirstOrDefault();
      
      if (cashe == null)
      {
        cashe = sberdev.SberContracts.AnaliticsCasheGenerals.Create();
        cashe.Save();
      }
      else
      {
        if (contractual != null)
        {
          contractual.ContrTypeBaseSberDev = cashe.ContrType;
          contractual.AccArtPrBaseSberDev = cashe.AccArtPr;
          contractual.AccArtExBaseSberDev = cashe.AccArtEx;
          contractual.MVZBaseSberDev = cashe.MVZ;
          contractual.MVPBaseSberDev = cashe.MVP;
          contractual.MarketDirectSberDev = cashe.MarkDirection;
          contractual.Counterparty = cashe.Counterparty;
          if (cashe.ProdCollectionPr.Count > 0)
          {
            contractual.ProdCollectionPrBaseSberDev.Clear();
            var collection = cashe.ProdCollectionPr;
            foreach (var str in collection)
            {
              var i = contractual.ProdCollectionPrBaseSberDev.AddNew();
              i.Product = str.Product;
            }
          }
          if (cashe.ProdCollectionEx.Count > 0)
          {
            contractual.ProdCollectionExBaseSberDev.Clear();
            foreach (var str in cashe.ProdCollectionEx)
            {
              var i = contractual.ProdCollectionExBaseSberDev.AddNew();
              i.Product = str.Product;
            }
          }
        }
        
        if (accounting != null)
        {
          accounting.PayTypeBaseSberDev = cashe.PayTypeBaseSberDev;
          if (cashe.ContrType.HasValue)
          {
            accounting.ProdCollectionBaseSberDev.Clear();
            switch (cashe.ContrType.Value.ToString())
            {
              case "Profitable":
                accounting.ContrTypeBaseSberDev = SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable;
                accounting.MVPBaseSberDev = cashe.MVP;
                accounting.AccArtBaseSberDev = cashe.AccArtPr;
                foreach(var prop in cashe.ProdCollectionPr)
                {
                  var target = accounting.ProdCollectionBaseSberDev.AddNew();
                  target.Product = prop.Product;
                }
                break;
              case "Expendable":
                accounting.ContrTypeBaseSberDev = SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable;
                accounting.MVZBaseSberDev = cashe.MVZ;
                accounting.AccArtBaseSberDev = cashe.AccArtEx;
                accounting.MarketDirectSberDev = cashe.MarkDirection;
                foreach(var prop in cashe.ProdCollectionEx)
                {
                  var target = accounting.ProdCollectionBaseSberDev.AddNew();
                  target.Product = prop.Product;
                }
                break;
              case "ExpendProfit":
                string type = SBContracts.PublicFunctions.AccountingDocumentBase.ShowContractChangedDialog(accounting);
                switch (type)
                {
                  case "Profitable":
                    accounting.ContrTypeBaseSberDev = SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable;
                    accounting.MVPBaseSberDev = cashe.MVP;
                    accounting.AccArtBaseSberDev = cashe.AccArtPr;
                    foreach(var prop in cashe.ProdCollectionPr)
                    {
                      var target = accounting.ProdCollectionBaseSberDev.AddNew();
                      target.Product = prop.Product;
                    }
                    break;
                  case "Expendable":
                    accounting.ContrTypeBaseSberDev = SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable;
                    accounting.MVZBaseSberDev = cashe.MVZ;
                    accounting.AccArtBaseSberDev = cashe.AccArtEx;
                    accounting.MarketDirectSberDev = cashe.MarkDirection;
                    foreach(var prop in cashe.ProdCollectionEx)
                    {
                      var target = accounting.ProdCollectionBaseSberDev.AddNew();
                      target.Product = prop.Product;
                    }
                    break;
                  case "":
                    break;
                }
                break;
            }
          }
          
          accounting.Counterparty = cashe.Counterparty;
        }
      }
    }
    
    /// <summary>
    /// Заполнить из любого выбранного документа
    /// </summary>
    /// <param name="doc">Документ который заполняется</param>
    /// <param name="docSelected">Документ из которого идут данные</param>
    [Public, Remote]
    public static void FillFromDocumentSrv(Sungero.Docflow.IOfficialDocument doc, Sungero.Docflow.IOfficialDocument docSelected)
    {
      var contractual = SBContracts.ContractualDocuments.As(doc);
      var accounting = SBContracts.AccountingDocumentBases.As(doc);
      var contractualSelected = SBContracts.ContractualDocuments.As(docSelected);
      var accountingSelected = SBContracts.AccountingDocumentBases.As(docSelected);
      
      #region Заполняется Договорной документ
      
      if (contractual != null)
      {
        if (contractualSelected != null)
        {
          FillGeneralProperties(contractual, contractualSelected);
          FillExpendableAnaliticsProperties(contractual, contractualSelected);
          FillProfitableAnaliticsProperties(contractual, contractualSelected);
          FillCalculation(contractual, contractualSelected);
        }
        
        if (accountingSelected != null)
        {
          FillGeneralProperties(contractual, accountingSelected);
          if (contractual.ContrTypeBaseSberDev.HasValue)
          {
            if (contractual.ContrTypeBaseSberDev.Value.Value == "Profitable")
            {
              FillProfitableAnaliticsProperties(contractual, accountingSelected);
            }
            
            if (contractual.ContrTypeBaseSberDev.Value.Value == "Expendable")
            {
              FillExpendableAnaliticsProperties(contractual, accountingSelected);
            }
          }
          FillCalculation(contractual, accountingSelected);
        }
      }
      
      #endregion
      
      #region Заполняется Финансовый документ
      if (accounting != null)
      {
        if (contractualSelected != null)
        {
          FillGeneralProperties(accounting, contractualSelected);
          
          if (contractualSelected.ContrTypeBaseSberDev.HasValue)
          {
            if (contractualSelected.ContrTypeBaseSberDev.Value.Value == "Profitable")
            {
              FillProfitableAnaliticsProperties(accounting, contractualSelected);
            }
            
            if (contractualSelected.ContrTypeBaseSberDev.Value.Value == "Expendable")
            {
              FillExpendableAnaliticsProperties(accounting, contractualSelected);
            }
            
            if (contractualSelected.ContrTypeBaseSberDev.Value.Value == "ExpendProfit")
            {
              string type = SBContracts.PublicFunctions.AccountingDocumentBase.ShowContractChangedDialog(accounting);
              switch (type)
              {
                case "Profitable":
                  FillProfitableAnaliticsProperties(accounting, contractualSelected);
                  break;
                case "Expendable":
                  FillExpendableAnaliticsProperties(accounting, contractualSelected);
                  break;
                case "":
                  break;
              }
            }
          }
          
          FillCalculation(accounting, contractualSelected);
        }
        
        if (accountingSelected != null)
        {
          FillGeneralProperties(accounting, accountingSelected);
          FillCalculation(accounting, accountingSelected);
        }
      }
      
      #endregion
      
    }
    
    #region Договор -> Договор
    
    /// <summary>
    /// Заполнить общие свойства из выбранного документа
    /// </summary>
    /// <param name="contract"></param>
    public static void FillGeneralProperties(SBContracts.IContractualDocument doc, SBContracts.IContractualDocument docSelected)
    {
      doc.BudItemBaseSberDev = docSelected.BudItemBaseSberDev ;
      if (docSelected.TotalAmount != null)
      {
        doc.TotalAmount = docSelected.TotalAmount;
        doc.Currency = docSelected.Currency;
      }
      doc.DeliveryMethod = docSelected.DeliveryMethod;
      if (!SberContracts.Purchases.Is(doc) && !SberContracts.AppProductPurchases.Is(doc))
      {
        doc.ContrTypeBaseSberDev = docSelected.ContrTypeBaseSberDev;
        doc.FrameworkBaseSberDev = docSelected.FrameworkBaseSberDev;
      }
      doc.Counterparty = docSelected.Counterparty;
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
    
    
    /// <summary>
    /// Универсальная функция для автоматической нумерации элементов в коллекции DirectumRX
    /// </summary>
    /// <param name="collectionPropertyName">Имя свойства коллекции</param>
    /// <param name="objInstance">Экземпляр объекта, содержащего коллекцию</param>
    [Public]
    public static void AutoNumberCollectionItem(string collectionPropertyName, object objInstance)
    {
      try
      {
        // Получаем саму коллекцию
        var collectionProperty = objInstance.GetType().GetProperty(collectionPropertyName);
        if (collectionProperty == null)
          return;
        
        var collection = collectionProperty.GetValue(objInstance, null);
        if (collection == null)
          return;
        
        // Получаем количество элементов
        var countProperty = collection.GetType().GetProperty("Count");
        int count = (int)countProperty.GetValue(collection, null);
        
        if (count <= 0)
          return;
        
        // Получаем последний элемент через индексатор
        var itemAccessMethod = collection.GetType().GetMethod("get_Item");
        var lastItem = itemAccessMethod.Invoke(collection, new object[] { count - 1 });
        var lastItemNumberProperty = lastItem.GetType().GetProperty("Number");
        
        // Используем GetEnumerator для обхода коллекции
        var getEnumeratorMethod = collection.GetType().GetMethod("GetEnumerator");
        if (getEnumeratorMethod == null)
          return;
        
        var enumerator = getEnumeratorMethod.Invoke(collection, null);
        var moveNextMethod = enumerator.GetType().GetMethod("MoveNext");
        var currentProperty = enumerator.GetType().GetProperty("Current");
        
        // Список существующих номеров
        var existingNumbers = new System.Collections.Generic.List<int>();
        int lastIndex = 0;
        
        // Перебираем все элементы, кроме последнего
        while ((bool)moveNextMethod.Invoke(enumerator, null))
        {
          if (lastIndex == count - 1)
            break;
          
          var item = currentProperty.GetValue(enumerator, null);
          if (item == null)
          {
            lastIndex++;
            continue;
          }
          
          var numberProperty = item.GetType().GetProperty("Number");
          if (numberProperty == null)
          {
            lastIndex++;
            continue;
          }
          
          var numberValue = numberProperty.GetValue(item, null);
          if (numberValue == null)
          {
            lastIndex++;
            continue;
          }
          
          try
          {
            // Проверяем, является ли Number Nullable
            var hasValueProperty = numberValue.GetType().GetProperty("HasValue");
            if (hasValueProperty != null)
            {
              bool hasValue = (bool)hasValueProperty.GetValue(numberValue, null);
              if (hasValue)
              {
                var valueProperty = numberValue.GetType().GetProperty("Value");
                int number = (int)valueProperty.GetValue(numberValue, null);
                existingNumbers.Add(number);
              }
            }
            else
            {
              // Если не Nullable, просто приводим к int
              int number = (int)numberValue;
              existingNumbers.Add(number);
            }
          }
          catch { }
          
          lastIndex++;
        }
        
        // Сортируем номера
        existingNumbers.Sort();
        
        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }
        
        // Проверяем, есть ли уже такой номер
        if (existingNumbers.Contains(missingNumber))
          missingNumber = existingNumbers.Count > 0 ? existingNumbers.Max() + 1 : 1;
        
        // Присваиваем найденный номер последнему элементу
        lastItemNumberProperty.SetValue(lastItem, missingNumber, null);
      }
      catch { }
    }
    
    #endregion
    
    #endregion
    
    #region Функции виджетов
    
    /// <summary>
    /// Вычисление времени выполнения задачи
    /// </summary>
    [Public]
    public int GetExecutionTaskTime(SBContracts.IApprovalTask task)
    {
      try
      {
        // Если уже рассчитано, используем сохранённое значение
        if (task.ExecutionInDaysSungero.HasValue)
          return task.ExecutionInDaysSungero.Value;
        
        if (task?.Started == null)
          return 0;
        
        var lastAssignment = SBContracts.ApprovalAssignments.GetAll()
          .Where(a => a.Task.Id == task.Id &&
                 a.Status == Sungero.Workflow.Assignment.Status.Completed &&
                 a.Completed != null &&
                 a.Completed >= task.Started)
          .OrderByDescending(a => a.Completed)
          .FirstOrDefault();
        
        if (lastAssignment?.Completed == null)
          return 0;
        
        // Проверяем корректность дат
        if (task.Started > lastAssignment.Completed)
          return 0;
        
        int days = SBContracts.PublicFunctions.Module.CalculateBusinessDays(task.Started, lastAssignment.Completed);
        return days;
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка в GetExecutionTaskTime для задачи {task?.Id}", ex);
        return 0;
      }
    }
    
    /// <summary>
    /// Оптимизированный расчет значений для AssignCompletedByDepart.
    /// </summary>
    [Public]
    public System.Collections.Generic.Dictionary<string, SBContracts.Structures.Module.IAssignApprSeriesInfo> CalculateAssignCompletedByDepartValues(
      SBContracts.Structures.Module.IDateRange dateRange, string documentType)
    {
      var result = new Dictionary<string, SBContracts.Structures.Module.IAssignApprSeriesInfo>();
      
      try
      {
        // Проверяем корректность диапазона дат
        if (dateRange == null || dateRange.StartDate > dateRange.EndDate)
          return result;
        
        // Запрос для получения всех сотрудников и их департаментов в одной операции
        var employeeDepartmentsRaw = Sungero.Company.Employees.GetAll()
          .Select(e => new {
                    Id = e.Id,
                    DepartmentName = e.Department != null ? e.Department.Name : "Без подразделения"
                  })
          .ToList(); // Сначала материализуем список
        
        var employeeDepartments = employeeDepartmentsRaw
          .ToDictionary(e => e.Id, e => e.DepartmentName);
        
        // Получаем базовый список заданий
        var baseQuery = Sungero.Workflow.Assignments.GetAll()
          .Where(a =>
                 a.Status == Sungero.Workflow.Assignment.Status.Completed &&
                 a.Created >= dateRange.StartDate &&
                 a.Completed <= dateRange.EndDate &&
                 a.Performer != null)
          .ToList();
        
        // Если не нужно фильтровать по типу документа, используем оптимизированный подход с группировкой
        if (string.IsNullOrEmpty(documentType) || documentType == "All")
        {
          // Теперь применяем проекцию к материализованному списку
          var assignments = baseQuery
            .Select(a => new {
                      AssignmentId = a.Id,
                      PerformerId = a.Performer.Id,
                      IsExpired = a.Deadline.HasValue && a.Completed > a.Deadline
                    })
            .ToList();
          
          // Группируем по департаментам после выгрузки данных из БД
          var assignmentsByDepartment = assignments
            .GroupBy(a => employeeDepartments.ContainsKey(a.PerformerId) ?
                     employeeDepartments[a.PerformerId] : "Без подразделения")
            .Select(g => new {
                      Department = g.Key,
                      Completed = g.Count(),
                      Expired = g.Count(a => a.IsExpired)
                    })
            .OrderByDescending(g => g.Completed)
            .Take(10)
            .ToList();
          
          // Заполняем результат
          foreach (var dept in assignmentsByDepartment)
          {
            var seriesInfo = SBContracts.Structures.Module.AssignApprSeriesInfo.Create();
            seriesInfo.Completed = dept.Completed;
            seriesInfo.Expired = dept.Expired;
            result[dept.Department] = seriesInfo;
          }
        }
        else // Нужно фильтровать по типу документа
        {
          // Получаем все задания, чтобы проверить тип документа
          var assignments = baseQuery.ToList();
          
          // Проверяем соответствие типу документа порциями
          Dictionary<string, int> departmentCompleted = new Dictionary<string, int>();
          Dictionary<string, int> departmentExpired = new Dictionary<string, int>();
          
          // Проверяем задания порциями
          const int batchSize = 500;
          var assignmentIds = assignments.Select(a => (int)a.Id).ToList();
          
          for (int i = 0; i < assignmentIds.Count; i += batchSize)
          {
            var batchIds = assignmentIds.Skip(i).Take(batchSize).ToList();
            
            // Получаем задания с прикрепленными документами
            var assignmentsBatch = Sungero.Workflow.Assignments.GetAll()
              .Where(a => batchIds.Contains((int)a.Id))
              .ToList();
            
            foreach (var assignment in assignmentsBatch)
            {
              // Проверяем соответствие типу документа
              if (!PublicFunctions.Module.AssignmentMatchesDocumentType(assignment, documentType))
                continue;
              
              // Определяем департамент
              string departmentName = "Без подразделения";
              string deptName = "Без подразделения";
              if (assignment.Performer != null && employeeDepartments.TryGetValue(assignment.Performer.Id, out deptName))
                departmentName = deptName;
              
              // Подсчитываем статистику
              if (!departmentCompleted.ContainsKey(departmentName))
              {
                departmentCompleted[departmentName] = 0;
                departmentExpired[departmentName] = 0;
              }
              
              departmentCompleted[departmentName]++;
              
              if (assignment.Deadline.HasValue && assignment.Completed > assignment.Deadline.Value)
                departmentExpired[departmentName]++;
            }
          }
          
          // Формируем результат
          foreach (var department in departmentCompleted.Keys)
          {
            var seriesInfo = SBContracts.Structures.Module.AssignApprSeriesInfo.Create();
            seriesInfo.Completed = departmentCompleted[department];
            seriesInfo.Expired = departmentExpired.ContainsKey(department) ? departmentExpired[department] : 0;
            
            result[department] = seriesInfo;
          }
          
          // Сортируем по количеству заданий и ограничиваем 10 департаментами
          result = result
            .OrderByDescending(kvp => kvp.Value.Completed)
            .Take(10)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в CalculateAssignCompletedByDepartValues", ex);
      }
      
      return result;
    }
    
    /// <summary>
    /// Оптимизированный расчет значений для TaskDeadline.
    /// </summary>
    [Public]
    public double CalculateTaskDeadlineChartPoints(SBContracts.Structures.Module.IDateRange dateRange, string serialType, string documentType)
    {
      try
      {
        if (dateRange == null || dateRange.StartDate == null || dateRange.EndDate == null)
          return 0;
        
        // Проверяем корректность диапазона дат
        if (dateRange.StartDate > dateRange.EndDate)
          return 0;
        
        // Базовый запрос без проекции
        var baseQuery = sberdev.SBContracts.ApprovalTasks.GetAll()
          .Where(t => t.Status == sberdev.SBContracts.ApprovalTask.Status.Completed &&
                 t.Started != null &&
                 t.Started >= dateRange.StartDate &&
                 t.Started <= dateRange.EndDate)
          .ToList();
        
        // Если нужно фильтровать по типу документа
        var tasksToCheck = documentType != "All" && !string.IsNullOrEmpty(documentType)
          ? baseQuery
          : null;
        
        List<int> executionDays = new List<int>();
        List<int> targetDays = new List<int>();
        
        // Если не нужно фильтровать по типу документа
        if (tasksToCheck == null)
        {
          // Выбираем задачи с заполненным ExecutionInDaysSungero
          var tasksWithExecutionTime = baseQuery
            .Where(t => t.ExecutionInDaysSungero != null && t.ExecutionInDaysSungero > 0)
            .Select(t => t.ExecutionInDaysSungero.Value)
            .ToList();
          
          // Для тех, у кого не заполнено, вычисляем время выполнения на месте
          var tasksWithoutExecutionTime = baseQuery
            .Where(t => t.ExecutionInDaysSungero == null || t.ExecutionInDaysSungero <= 0)
            .ToList();
          
          foreach (var task in tasksWithoutExecutionTime)
          {
            int days = GetExecutionTaskTime(task);
            if (days > 0)
              executionDays.Add(days);
          }
          
          // Объединяем результаты
          executionDays.AddRange(tasksWithExecutionTime);
          
          // Для целевого времени - применяем проекцию уже в памяти
          if (serialType.ToLower() == "target")
          {
            // Сначала получаем задачи с нужными датами
            var tasksWithDates = baseQuery
              .Where(t => t.MaxDeadline.HasValue && t.Started <= t.MaxDeadline.Value)
              .ToList()
              .Select(t => new {
                        StartDate = t.Started.Value,
                        MaxDeadlineDate = t.MaxDeadline.Value
                      })
              .ToList();
            
            // Затем применяем метод CalculateBusinessDays в памяти
            foreach (var taskData in tasksWithDates)
            {
              int days = sberdev.SBContracts.PublicFunctions.Module.CalculateBusinessDays(
                taskData.StartDate, taskData.MaxDeadlineDate);
              
              if (days > 0)
                targetDays.Add(days);
            }
          }
        }
        else // Нужно фильтровать по типу документа
        {
          // Фильтруем задачи по типу документа
          List<SBContracts.IApprovalTask> filteredTasks = new List<SBContracts.IApprovalTask>();
          
          foreach (var task in tasksToCheck)
          {
            if (PublicFunctions.Module.TaskMatchesDocumentType(task, documentType))
              filteredTasks.Add(task);
          }
          
          // Вычисляем время выполнения для отфильтрованных задач
          foreach (var task in filteredTasks)
          {
            // Используем сохраненное значение, если есть
            if (task.ExecutionInDaysSungero != null && task.ExecutionInDaysSungero > 0)
            {
              executionDays.Add(task.ExecutionInDaysSungero.Value);
            }
            else
            {
              int days = GetExecutionTaskTime(task);
              if (days > 0)
                executionDays.Add(days);
            }
            
            // Для целевого времени
            if (serialType.ToLower() == "target" &&
                task.MaxDeadline.HasValue &&
                task.Started <= task.MaxDeadline.Value)
            {
              int targetDay = sberdev.SBContracts.PublicFunctions.Module.CalculateBusinessDays(
                task.Started.Value, task.MaxDeadline.Value);
              
              if (targetDay > 0)
                targetDays.Add(targetDay);
            }
          }
        }
        
        // Вычисление значения в зависимости от типа серии
        serialType = serialType.ToLower();
        
        if (serialType == "average" && executionDays.Any())
          return executionDays.Average();
        
        if (serialType == "maximum" && executionDays.Any())
          return executionDays.Max();
        
        if (serialType == "minimum" && executionDays.Any())
          return executionDays.Min();
        
        if (serialType == "target" && targetDays.Any())
          return targetDays.Average();
        
        return 0;
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка в CalculateTaskDeadlineChartPoints", ex);
        return 0;
      }
    }
    
    /// <summary>
    /// Оптимизированный расчет значений для AssignAvgApprTimeByDepart.
    /// </summary>
    [Public]
    public System.Collections.Generic.Dictionary<string, double> CalculateAssignAvgApprTimeByDepartValues(SBContracts.Structures.Module.IDateRange dateRange, string documentType)
    {
      var result = new Dictionary<string, double>();
      
      try
      {
        // Проверка корректности диапазона дат
        if (dateRange == null || dateRange.StartDate > dateRange.EndDate)
          return result;
        
        // Создаем структуру для хранения времени выполнения по департаментам
        var departmentWorkDays = new Dictionary<string, List<double>>();
        
        // Оптимизация 1: Получаем данные о сотрудниках и их департаментах одним запросом
        var performers = Sungero.Company.Employees.GetAll()
          .Select(e => new {
                    Id = e.Id,
                    DepartmentId = e.Department != null ? e.Department.Id : (long?)null,
                    DepartmentName = e.Department != null ? e.Department.Name : null
                  })
          .ToList()
          .ToDictionary(e => e.Id, e => new {
                          DepartmentId = e.DepartmentId,
                          DepartmentName = e.DepartmentName ?? "Без подразделения"
                        });
        
        // Оптимизация 2: Сначала получаем базовые данные, потом проекцию
        var baseAssignments = Sungero.Workflow.Assignments.GetAll()
          .Where(a => a.Status == Sungero.Workflow.Assignment.Status.Completed &&
                 a.Created >= dateRange.StartDate &&
                 a.Completed <= dateRange.EndDate &&
                 a.Performer != null)
          .ToList();
        
        // Применяем проекцию уже в памяти
        var assignments = baseAssignments
          .Select(a => new {
                    Id = a.Id,
                    Created = a.Created,
                    Completed = a.Completed,
                    PerformerId = a.Performer.Id
                  })
          .ToList();
        
        // Если нужно фильтровать по типу документа
        List<int> filteredAssignmentIds = new List<int>();
        
        if (!string.IsNullOrEmpty(documentType) && documentType != "All")
        {
          // Получаем все задания, которые могут соответствовать типу документа
          var assignmentIdsToCheck = assignments.Select(a => (int)a.Id).ToList();
          
          // Выполняем оптимизированную проверку порциями
          const int batchSize = 500;
          for (int i = 0; i < assignmentIdsToCheck.Count; i += batchSize)
          {
            var batchIds = assignmentIdsToCheck.Skip(i).Take(batchSize).ToList();
            
            // Получаем соответствующие задания с их вложениями
            var assignmentsBatch = Sungero.Workflow.Assignments.GetAll()
              .Where(a => batchIds.Contains((int)a.Id))
              .ToList();  // Сначала получаем объекты в память
            
            // Проверяем соответствие типу документа
            foreach (var assignment in assignmentsBatch)
            {
              if (PublicFunctions.Module.AssignmentMatchesDocumentType(assignment, documentType))
              {
                filteredAssignmentIds.Add((int)assignment.Id);
              }
            }
          }
        }
        
        // Фильтруем задания, если нужно
        var assignmentsToProcess = !string.IsNullOrEmpty(documentType) && documentType != "All"
          ? assignments.Where(a => filteredAssignmentIds.Contains((int)a.Id)).ToList()
          : assignments;
        
        // Группируем задания по департаментам и вычисляем среднее время выполнения
        var departmentStats = assignmentsToProcess
          .Where(a => a.Created <= a.Completed) // Проверяем корректность дат
          .GroupBy(a => performers.ContainsKey(a.PerformerId)
                   ? performers[a.PerformerId].DepartmentName
                   : "Без подразделения")
          .Select(g => new {
                    Department = g.Key,
                    AvgDays = g.Average(a => sberdev.SBContracts.PublicFunctions.Module.CalculateBusinessDays(
                      a.Created.Value, a.Completed.Value))
                  })
          .Where(x => x.AvgDays > 0)
          .OrderByDescending(x => x.AvgDays)
          .Take(10);
        
        // Заполняем результат
        foreach (var stat in departmentStats)
        {
          result[stat.Department] = Math.Round(stat.AvgDays, 1);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Ошибка в CalculateAssignAvgApprTimeByDepartValues", ex);
      }
      
      return result;
    }

    /// <summary>
    /// Оптимизированный расчет значений для TaskFlowChart.
    /// </summary>
    [Public]
    public System.Collections.Generic.Dictionary<string, int> CalculateTaskFlowValues(SBContracts.Structures.Module.IDateRange dateRange, string documentType)
    {
      Dictionary<string, int> result = new Dictionary<string, int>
      {
        ["started"] = 0,
        ["completed"] = 0,
        ["inprocess"] = 0,
        ["expired"] = 0
      };
      
      try
      {
        if (dateRange == null || dateRange.StartDate > dateRange.EndDate)
          return result;
        
        var currentDate = Calendar.Now;
        
        // 1. Сначала получаем данные без проекции
        var baseTasks = sberdev.SBContracts.ApprovalTasks.GetAll()
          .Where(t => t.Started >= dateRange.StartDate && t.Started <= dateRange.EndDate)
          .ToList();
        
        // 2. Затем применяем проекцию уже в памяти
        var tasks = baseTasks.Select(t => new
                                     {
                                       t.Id,
                                       t.Status,
                                       t.MaxDeadline,
                                       t.DocumentTypeSungero
                                     })
          .ToList();
        
        // Словарь для кэширования соответствий задач типам документов
        Dictionary<long, bool> taskMatchesType = new Dictionary<long, bool>();
        
        // 3. Если нужна фильтрация по типу документов и не все задачи имеют кэшированный тип
        if (!string.IsNullOrEmpty(documentType) && documentType != "All")
        {
          // Получаем ID задач, для которых нужно проверить документы
          var taskIdsToCheck = tasks
            .Where(t => string.IsNullOrEmpty(t.DocumentTypeSungero))
            .Select(t => t.Id)
            .ToList();
          
          // Проверяем документы порциями для оптимизации
          const int batchSize = 500;
          for (int i = 0; i < taskIdsToCheck.Count; i += batchSize)
          {
            var batchIds = taskIdsToCheck.Skip(i).Take(batchSize).ToList();
            
            // Получаем задачи с документами для проверки
            var tasksBatch = sberdev.SBContracts.ApprovalTasks.GetAll()
              .Where(t => batchIds.Contains(t.Id))
              .ToList();  // Сначала загружаем в память
            
            // Проверяем соответствие типу документа
            foreach (var task in tasksBatch)
            {
              taskMatchesType[task.Id] = TaskMatchesDocumentType(task, documentType);
            }
          }
        }
        
        // 4. Подсчитываем статистику одним проходом
        foreach (var t in tasks)
        {
          // Применяем фильтр по типу документа, если нужно
          if (!string.IsNullOrEmpty(documentType) && documentType != "All")
          {
            bool matches = false;
            
            // Если тип документа уже сохранен в задаче
            if (!string.IsNullOrEmpty(t.DocumentTypeSungero))
            {
              matches = (t.DocumentTypeSungero == documentType);
            }
            // Иначе используем кэшированный результат проверки
            else if (taskMatchesType.ContainsKey(t.Id))
            {
              matches = taskMatchesType[t.Id];
            }
            
            if (!matches)
              continue;
          }
          
          // Подсчет статистики
          if (t.Status != sberdev.SBContracts.ApprovalTask.Status.Draft)
            result["started"]++;
          
          if (t.Status == sberdev.SBContracts.ApprovalTask.Status.Completed)
            result["completed"]++;
          
          if (t.Status == sberdev.SBContracts.ApprovalTask.Status.InProcess ||
              t.Status == sberdev.SBContracts.ApprovalTask.Status.UnderReview)
            result["inprocess"]++;
          
          if ((t.Status == sberdev.SBContracts.ApprovalTask.Status.InProcess ||
               t.Status == sberdev.SBContracts.ApprovalTask.Status.UnderReview) &&
              t.MaxDeadline.HasValue && t.MaxDeadline.Value < currentDate)
            result["expired"]++;
        }
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка в CalculateTaskFlowValues", ex);
      }
      
      return result;
    }

    #endregion
    
  }
}
