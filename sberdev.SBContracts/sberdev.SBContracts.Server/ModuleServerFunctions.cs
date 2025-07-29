using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts;
using System.Net;
using Aspose.Words;
using Aspose.Words.Reporting;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Sungero.Metadata;
using Sungero.Domain.Shared;
using sberdev.SBContracts.OfficialDocument;
using Newtonsoft.Json;

namespace sberdev.SBContracts.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Продлить срок задания
    /// </summary>
    [Public, Remote]
    public void ExtendAssignmentDeadline(Sungero.Workflow.IAssignment assignment, DateTime newDeadline)
    {
      //  PublicFunctions.Module.Remote.UnblockCardByDatabase(assignment);
      assignment.Deadline = newDeadline;
      assignment.Save();
    }
    
    [Public]
    public string GetPaymentType(SberContracts.IPurchase purch)
    {
      return GetPaymentKind(purch.PaymentKind) + (purch.PrepaymentPercent != null ? " c " + purch.PrepaymentPercent.ToString() + " % авансом" : "");
    }
    
    #region Разные геты и сеты
    
    /// <summary>
    /// Записать ИД документа в метаданные документа
    /// </summary>
    [Public, Remote]
    public void CreatePDFSetMetadataID(Sungero.Docflow.IOfficialDocument doc)
    {
      if (!doc.HasVersions)
        return;

      string guid1 = Guid.NewGuid().ToString();
      string guid2 = Guid.NewGuid().ToString();
      string guid3 = Guid.NewGuid().ToString();
      string ext = "pdf"; // Всегда используем pdf для экспорта

      string tempPdfPathCopy = $"C:\\TempDocs\\docDirectumTemp{guid3}.{ext}";
      string tempPdfPath = $"C:\\TempDocs\\docDirectumTemp{guid1}.{ext}";
      string tempPath = $"C:\\TempDocs\\docDirectum{guid2}.{doc.LastVersion.AssociatedApplication.Extension}";
      // Конвертируем документ в PDF, если он не в PDF формате
      if (doc.LastVersion.AssociatedApplication.Extension != "pdf")
      {
        doc.LastVersion.Export(tempPath);

        Aspose.Words.Document docAsp = new Aspose.Words.Document(tempPath);
        docAsp.Save(tempPdfPath, SaveFormat.Pdf);
      }
      else
        doc.LastVersion.Export(tempPdfPath);

      // Добавляем ИД в метаданные PDF
      PdfReader reader = new PdfReader(tempPdfPath);
      PdfReader.unethicalreading = true;
      string idPdf = null;
      if (reader.Info.TryGetValue("DirectumID", out idPdf) && idPdf != doc.Id.ToString())
        reader.Info.Remove("DirectumID");
      
      iTextSharp.text.Document document = new iTextSharp.text.Document();
      PdfCopy copy = new PdfCopy(document, new System.IO.FileStream(tempPdfPathCopy, System.IO.FileMode.Create));
      document.Open();
      copy.AddDocument(reader);
      copy.Info.Put(new PdfName("DirectumID"), new PdfString(doc.Id.ToString()));
      copy.Close();
      document.Close();

      reader.Close();

      // Сохраняем PDF с ИД обратно в систему и возвращаем пользователю
      doc.CreateVersionFrom(tempPdfPathCopy);
      doc.Save();
      var signInfos = Signatures.Get(doc.LastVersion);
      foreach(var signInfo in signInfos)
      {
        var signaturesBytes = signInfo.GetDataSignature();
        Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, signInfo.SigningDate, doc.LastVersion);
      }
    }
    
    /// <summary>
    /// Записать ИД документа в метаданные документа
    /// </summary>
    [Public, Remote]
    public void SetMetadataID(Sungero.Docflow.IOfficialDocument doc)
    {
      if (!doc.HasVersions)
        return;
      string ext = doc.LastVersion.AssociatedApplication.Extension;
      if (ext != "doc" && ext != "docx" && ext != "pdf")
        return;
      string guid = Guid.NewGuid().ToString();
      string path = "C:\\TempDocs\\docDirectum" + guid + "." + ext;
      string tempPath = "C:\\TempDocs\\docDirectumTemp" + guid + "." + ext;
      doc.LastVersion.Export(path);
      if (ext == "doc" || ext == "docx")
      {
        bool isNeedSave = false;
        Aspose.Words.Document docAsp = new Aspose.Words.Document(path);
        var props = docAsp.CustomDocumentProperties;
        var prop = props.FirstOrDefault(p => p.Name == "DirectumID");
        
        if (prop == null)
        {
          props.Add("DirectumID", doc.Id.ToString());
          isNeedSave = true;
        }
        
        if (prop != null && prop.Value.ToString() != doc.Id.ToString())
        {
          prop.Value = doc.Id.ToString();
          isNeedSave = true;
        }
        
        if (isNeedSave)
        {
          switch (ext)
          {
            case ("doc"):
              docAsp.Save(tempPath, SaveFormat.Doc);
              break;
            case ("docx"):
              docAsp.Save(tempPath, SaveFormat.Docx);
              break;
            case ("pdf"):
              docAsp.Save(tempPath, SaveFormat.Pdf);
              break;
            default:
              return;
          }
        }
      }
      
      if (ext == "pdf")
      {
        PdfReader reader = new PdfReader(path);
        PdfReader.unethicalreading = true;
        string idPdf = null;
        if (reader.Info.TryGetValue("DirectumID", out idPdf) && idPdf != doc.Id.ToString())
        {
          reader.Info.Remove("DirectumID");
          iTextSharp.text.Document document = new iTextSharp.text.Document();
          PdfCopy copy = new PdfCopy(document, new System.IO.FileStream(tempPath, System.IO.FileMode.Create));
          document.Open();
          copy.AddDocument(reader);
          copy.Info.Put(new PdfName("DirectumID"), new PdfString(doc.Id.ToString()));
          copy.Close();
          document.Close();
        }
        if (!reader.Info.ContainsKey("DirectumID"))
        {
          iTextSharp.text.Document document = new iTextSharp.text.Document();
          PdfCopy copy = new PdfCopy(document, new System.IO.FileStream(tempPath, System.IO.FileMode.Create));
          document.Open();
          copy.AddDocument(reader);
          copy.Info.Put(new PdfName("DirectumID"), new PdfString(doc.Id.ToString()));
          copy.Close();
          document.Close();
        }
      }
      
      FileInfo fi = new FileInfo(tempPath);
      if (fi.Exists)
      {
        try
        {
          doc.LastVersion.Import(tempPath);
          doc.Save();
        }
        catch (Exception ex)
        {
          Logger.Debug("Этап сценария. Неуспешный импорт версии с метаИД (" + doc.Id.ToString() + "). Будет созданна новая версия");
          var signInfos = Signatures.Get(doc.LastVersion);
          doc.CreateVersionFrom(tempPath);
          doc.Save();
          foreach(var signInfo in signInfos)
          {
            var signaturesBytes = signInfo.GetDataSignature();
            Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, signInfo.SigningDate, doc.LastVersion);
          }
        }
      }
    }
    
    public static void SaveStreamToFile(System.IO.Stream inputStream, string filePath)
    {
      using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        inputStream.CopyTo(fileStream);
      }
    }
    
    /// <summary>
    /// Получить ИД документа из метаданных документа
    /// </summary>
    [Public, Remote]
    public string GetMetadataID(Sungero.Docflow.IOfficialDocument doc)
    {
      if (!doc.HasVersions)
        return null;
      var sbDoc = SBContracts.OfficialDocuments.As(doc);
      string ext = sbDoc.BodyExtSberDev != null ? sbDoc.BodyExtSberDev : doc.LastVersion.AssociatedApplication.Extension;
      if (ext != "doc" && ext != "docx" && ext != "pdf")
        return null;
      string guid = Guid.NewGuid().ToString();
      string path = "C:\\TempDocs\\docDirectum" + guid + "." + ext;
      using (var fileStr = doc.LastVersion.Body.Read())
      {
        SaveStreamToFile(fileStr, path);
      }
      
      if (ext == "doc" || ext == "docx")
      {
        Aspose.Words.Document docAsp = new Aspose.Words.Document(path);
        var props = docAsp.CustomDocumentProperties;
        var prop = props.FirstOrDefault(p => p.Name == "DirectumID");
        if (prop != null)
          return (string)prop.Value;
      }
      
      try
      {
        if (ext == "pdf")
        {
          PdfReader reader = new PdfReader(path);
          PdfReader.unethicalreading = true;
          var info = reader.Info;
          if (info.ContainsKey("DirectumID"))
            return info["DirectumID"];
          reader.Close();
        }
      }
      catch
      {
        ext = "docx";
        guid = Guid.NewGuid().ToString();
        path = "C:\\TempDocs\\docDirectum" + guid + "." + ext;
        using (var fileStr = doc.LastVersion.Body.Read())
        {
          SaveStreamToFile(fileStr, path);
        }
        Aspose.Words.Document docAsp = new Aspose.Words.Document(path);
        var props = docAsp.CustomDocumentProperties;
        var prop = props.FirstOrDefault(p => p.Name == "DirectumID");
        if (prop != null)
          return (string)prop.Value;
      }
      
      return null;
      
    }

    /// <summary>
    /// Функция получения подразделения по имени
    /// </summary>
    /// <param name="name">Имя подразделения</param>
    /// <returns></returns>
    [Remote, Public]
    public Sungero.Company.IDepartment GetDepartment(string name)
    {
      return Sungero.Company.Departments.GetAll().Where(d => Equals(d.Name, name)).First();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">Expendable или profitable</param>
    /// <returns></returns>
    [Public]
    public SberContracts.IAccountingArticles GetAccountingArticleStab(string type)
    {
      if (type.ToLower() == "expendable")
        return AccountingArticleses.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                            && m.ContrType.ToString() == "Expendable");
      if (type.ToLower() == "profitable")
        return AccountingArticleses.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                            && m.ContrType.ToString() == "Profitable");
      return null;
    }
    
    /// <summary>
    /// Пулучить заглушку для продукта
    /// </summary>
    ///<param name="type">Expendable или profitable</param>
    /// <returns></returns>
    [Public]
    public SberContracts.IProductsAndDevices GetProductStab()
    {
      return ProductsAndDeviceses.GetAll().FirstOrDefault(p => p.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)");
    }
    
    
    /// <summary>
    /// Получить заглушку для мвз
    /// </summary>
    /// <param name="type">Expendable или profitable</param>
    [Public]
    public SberContracts.IMVZ GetMVZStab(string type)
    {
      if (type.ToLower() == "expendable")
        return MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                            && m.ContrType.ToString() == "Expendable");
      if (type.ToLower() == "profitable")
        return MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                            && m.ContrType.ToString() == "Profitable");
      return null;
    }
    
    /// <summary>
    /// Получить кеш по пользователю и если его нет, создать новый
    /// </summary>
    [Remote, Public]
    public SberContracts.IBusinessUnitFilteringCashe GetOrCreateBusinessUnitFilteringCashe(Sungero.CoreEntities.IUser user)
    {
      var cashe = BusinessUnitFilteringCashes.GetAll().Where(c => Equals(user, Users.Current)).FirstOrDefault();
      if (cashe == null)
      {
        cashe = BusinessUnitFilteringCashes.Create();
        cashe.User = user;
        cashe.Name = user.Info.Name;
        cashe.BusinessUnit = Sungero.Company.Employees.As(user).Department.BusinessUnit;
        cashe.Save();
      }
      return cashe;
    }
    
    /// <summary>
    /// Возвращает группу по названию
    /// </summary>
    /// <param name="name">Имя группы</param>
    /// <returns>Возварщает группу</returns>
    [Public, Remote]
    public IGroup GetGroup(string name)
    {
      return Sungero.CoreEntities.Groups.GetAll().Where(g => Equals(g.Name, name)).FirstOrDefault();
    }
    
    /// <summary>
    /// Возвращает группу по названию и Нашей организации
    /// </summary>
    /// <param name="name">Имя группы</param>
    /// <returns>Возварщает группу</returns>
    [Public, Remote]
    public IGroup GetGroup(string name, Sungero.Company.IBusinessUnit bU)
    {
      return Sungero.CoreEntities.Groups.GetAll().Where(g => Equals(g.Name, name)
                                                        && Equals(Sungero.Company.Departments.As(g).BusinessUnit, bU)).FirstOrDefault();
    }
    
    /// <summary>
    /// Возвращает список ИНН всех "Наших организаций"
    /// </summary>
    [Public]
    public List<string> GetAllBuisnessUnitTINs()
    {
      return Sungero.Company.BusinessUnits.GetAll().Select(b => b.TIN).ToList();
    }
    
    #endregion
    
    #region Сертификаты и подписи

    [Public]
    public System.Collections.Generic.IEnumerable<Sungero.Domain.Shared.ISignature> GetSignatures(Sungero.Domain.Shared.IEntity entity)
    {
      var signs = Signatures.Get(entity);
      return signs;
    }
    
    /// <summary>
    /// Функция возвращает красный цвет если документ не подписан с двух сторон
    /// </summary>
    /// <param name="document"></param>
    /// <param name="isNeedValid"></param>
    /// <returns></returns>
    [Public]
    public Color HighlightUnsignedDocument(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      var realSigs = CheckRealSignatures(document, isNeedValid);
      var propSigs = CheckPropertySignatures(document);
      if ((realSigs[0] || propSigs[0]) && (realSigs[1] || propSigs[1]))
        return Colors.Empty;
      else
        return Colors.Common.Red;
    }
    
    /// <summary>
    /// Функция проверяет поля согласования и возвращает булевой список, первый где первый элемент флаг нашей подписи, второй контрагента.
    /// </summary>
    /// <param name="document">Документ</param>
    [Public, Remote]
    public List<bool> CheckPropertySignatures(SBContracts.IOfficialDocument document)
    {
      List<bool> flags = new List<bool>() {false, false};
      flags[0] = document.InternalApprovalState == SBContracts.OfficialDocument.InternalApprovalState.Signed ? true : false;
      flags[1] = document.ExternalApprovalState == SBContracts.OfficialDocument.ExternalApprovalState.Signed ? true : false;
      return flags;
    }
    
    /// <summary>
    /// Функция проверяет поля согласования и возвращает общий результат
    /// </summary>
    /// <param name="document">Документ</param>
    [Public, Remote]
    public bool CheckPropertySignaturesGeneral(SBContracts.IOfficialDocument document)
    {
      List<bool> flags = new List<bool>() {false, false};
      flags[0] = document.InternalApprovalState == SBContracts.OfficialDocument.InternalApprovalState.Signed ? true : false;
      flags[1] = document.ExternalApprovalState == SBContracts.OfficialDocument.ExternalApprovalState.Signed ? true : false;
      return flags[0] && flags[1];
    }
    
    /// <summary>
    /// Функция проверяет согласование документа. Возвращает true,
    /// если есть согласование от начальника данного подразделения.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="depart">Подразделение.</param>
    /// <returns>Признак наличия согласования от руководителя подразделения.</returns>
    [Public]
    public bool CheckDepartmentApproval(SBContracts.IOfficialDocument document, Sungero.CoreEntities.IGroup depart)
    {
      if (document == null || !document.HasVersions)
        return false;
      
      var signatures = Signatures.Get(document.LastVersion);
      var signer = Sungero.CoreEntities.Users.As(Sungero.Company.Departments.As(depart).Manager);
      
      if (signer == null)
        return false;
      
      return signatures.Any(sign => sign.SignatureType == SignatureType.Endorsing &&
                            (Equals(sign.Signatory, signer) || Equals(sign.SubstitutedUser, signer)));
    }

    /// <summary>
    /// Функция проверяет подпись документа.
    /// Возвращает true если есть внешняя от Нашей организации или
    /// внутренняя от одного человека из группы "Обязательные подписанты счета перед согласованием".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="isNeedValid">Необходимость проверки валидности.</param>
    /// <returns>Признак наличия требуемой подписи.</returns>
    [Public]
    public bool CheckSpecialGroupSignature(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      if (document == null || !document.HasVersions)
        return false;
      
      var signatures = Signatures.Get(document.LastVersion);
      var needSignGroup = PublicFunctions.Module.Remote.GetGroup("Обязательные подписанты счета перед согласованием");
      
      return signatures.Any(sign =>
                            (sign.IsExternal.HasValue && sign.IsExternal.Value &&
                             IsSignatureFromBusinessUnit(sign, document.BusinessUnit.TIN, isNeedValid)) ||
                            (sign.SignatureType == SignatureType.Approval &&
                             (UserIncludedInGroup(sign.Signatory, needSignGroup) ||
                              UserIncludedInGroup(sign.SubstitutedUser, needSignGroup))));
    }

    /// <summary>
    /// Проверяет, что пользователь входит в указанную группу.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <param name="group">Группа.</param>
    /// <returns>True, если пользователь входит в группу.</returns>
    [Public]
    private bool UserIncludedInGroup(Sungero.CoreEntities.IUser user, Sungero.CoreEntities.IGroup group)
    {
      return user != null && user.IncludedIn(group);
    }

    /// <summary>
    /// Проверяет, что подпись принадлежит указанной организации.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <param name="tin">ИНН организации.</param>
    /// <param name="isNeedValid">Проверять валидность подписи.</param>
    /// <returns>True, если подпись принадлежит указанной организации.</returns>
    [Public]
    private bool IsSignatureFromBusinessUnit(Sungero.Domain.Shared.ISignature signature, string tin, bool isNeedValid)
    {
      try
      {
        var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(signature.GetDataSignature());
        var signatureTin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyTIN(certificateInfo.SubjectInfo);
        return Equals(signatureTin, tin) && (signature.IsValid || !isNeedValid);
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Функция проверяет наличие подписи от контрагента и от одного человека из группы
    /// "Обязательные подписанты счета перед согласованием".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="isNeedValid">Необходимость проверки валидности.</param>
    /// <returns>Список [наша подпись, подпись контрагента].</returns>
    [Public, Remote]
    public List<bool> CheckRealSignatures(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      var flags = new List<bool>() { false, false };
      
      if (document == null || !document.HasVersions)
        return flags;
      
      var signatures = Signatures.Get(document.LastVersion);
      
      // Проверка на двойную версию
      if (document.LastVersion.Note == "Титул покупателя")
        signatures = Signatures.Get(document);
      
      var needSignGroup = PublicFunctions.Module.Remote.GetGroup("Обязательные подписанты счета перед согласованием");
      
      if (!signatures.Any())
        return flags;
      
      var counterpartyTin = string.Empty;
      var businessUnitTin = string.Empty;
      
      var contractual = SBContracts.ContractualDocuments.As(document);
      var accounting = SBContracts.AccountingDocumentBases.As(document);
      
      if (contractual != null)
      {
        counterpartyTin = contractual.Counterparty.TIN;
        businessUnitTin = contractual.BusinessUnit.TIN;
      }
      else if (accounting != null)
      {
        counterpartyTin = accounting.Counterparty.TIN;
        businessUnitTin = accounting.BusinessUnit.TIN;
      }
      else
      {
        return flags;
      }
      
      // Проверка наличия нашей подписи
      flags[0] = signatures.Any(sign =>
                                (sign.IsExternal.HasValue && sign.IsExternal.Value && IsSignatureFromOrganization(sign, businessUnitTin, isNeedValid)) ||
                                (sign.SignatureType == SignatureType.Approval &&
                                 (UserIncludedInGroup(sign.Signatory, needSignGroup) || UserIncludedInGroup(sign.SubstitutedUser, needSignGroup))));
      
      // Проверка наличия подписи контрагента
      flags[1] = signatures.Any(sign =>
                                sign.IsExternal.HasValue && sign.IsExternal.Value && IsSignatureFromOrganization(sign, counterpartyTin, isNeedValid));
      
      return flags;
    }

    /// <summary>
    /// Проверяет, что подпись принадлежит указанной организации.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <param name="organizationTin">ИНН организации.</param>
    /// <param name="isNeedValid">Проверять валидность подписи.</param>
    /// <returns>True, если подпись принадлежит указанной организации.</returns>
    [Public]
    private bool IsSignatureFromOrganization(Sungero.Domain.Shared.ISignature signature, string organizationTin, bool isNeedValid)
    {
      try
      {
        var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(signature.GetDataSignature());
        var signatureTin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyTIN(certificateInfo.SubjectInfo);
        return Equals(signatureTin, organizationTin) && (signature.IsValid || !isNeedValid);
      }
      catch
      {
        return false;
      }
    }
    
    
    /// <summary>
    /// Получить структуру с информацией о владельце сертификата.
    /// </summary>
    /// <param name="subjectInfo">Информация о владельце сертификата.</param>
    /// <returns>Структура с информацией о владельце сертификата.</returns>
    [Public, Remote]
    public virtual string ParseCertificateSubjectOnlyTIN(string subjectInfo)
    {
      var subject = SBContracts.Functions.Module.GetOidValues(subjectInfo);
      var orgTin = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.OrganizationTIN)).FirstOrDefault().Value;
      var tin = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.TIN)).FirstOrDefault().Value;
      if (!string.IsNullOrEmpty(orgTin))
        return orgTin;
      if (!string.IsNullOrEmpty(tin))
        return tin;
      return "";
    }
    
    /// <summary>
    /// Получить информацию о сертификате по содержимому подписи.
    /// </summary>
    /// <param name="signatureContent">Подпись.</param>
    /// <returns>Информация о сертификате.</returns>
    [Public]
    public virtual Sungero.Core.IX509CertificateInfo GetSignatureCertificateInfo(byte[] signatureContent)
    {
      var signatureInfo = ExternalSignatures.GetSignatureInfo(signatureContent);
      if (signatureInfo.SignatureFormat == SignatureFormat.Hash)
        throw AppliedCodeException.Create(Resources.IncorrectSignatureFormat);
      var cadesBesSignatureInfo = signatureInfo.AsCadesBesSignatureInfo();
      return cadesBesSignatureInfo.CertificateInfo;
    }
    
    /// <summary>
    /// Получить структуру с информацией о владельце сертификата.
    /// </summary>
    /// <param name="subjectInfo">Информация о владельце сертификата.</param>
    /// <returns>Структура с информацией о владельце сертификата.</returns>
    [Public, Remote]
    public virtual Sungero.Docflow.Structures.Module.ICertificateSubject ParseCertificateSubject(string subjectInfo)
    {
      var subject = SBContracts.Functions.Module.GetOidValues(subjectInfo);
      
      var parsedSubject = Sungero.Docflow.Structures.Module.CertificateSubject.Create();
      parsedSubject.CounterpartyName = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.CommonName)).FirstOrDefault().Value;
      parsedSubject.Country = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.Country)).FirstOrDefault().Value;
      parsedSubject.State = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.State)).FirstOrDefault().Value;
      parsedSubject.Locality = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.Locality)).FirstOrDefault().Value;
      parsedSubject.Street = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.Street)).FirstOrDefault().Value;
      parsedSubject.Department = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.Department)).FirstOrDefault().Value;
      parsedSubject.Surname = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.Surname)).FirstOrDefault().Value;
      parsedSubject.GivenName = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.GivenName)).FirstOrDefault().Value;
      parsedSubject.JobTitle = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.JobTitle)).FirstOrDefault().Value;
      parsedSubject.OrganizationName = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.OrganizationName)).FirstOrDefault().Value;
      parsedSubject.Email = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.Email)).FirstOrDefault().Value;
      var valueTIN = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.TIN)).FirstOrDefault().Value;
      if (!string.IsNullOrEmpty(valueTIN))
        parsedSubject.TIN = valueTIN.StartsWith("00") ? valueTIN.Substring(2) : valueTIN;
      
      return parsedSubject;
    }
    
    /// <summary>
    /// Получить идентификаторы объектов и их значения.
    /// </summary>
    /// <param name="certificateInfo">Информация о сертификате.</param>
    /// <returns>Идентификаторы объектов и их значения.</returns>
    public virtual System.Collections.Generic.IDictionary<string, string> GetOidValues(string certificateInfo)
    {
      var parseCertificate = certificateInfo.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      Dictionary<string, string> oidDict = new Dictionary<string, string>();
      foreach (var item in parseCertificate)
      {
        var itemElements = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
        if (itemElements.Count() < 2)
          continue;
        
        var oidKey = itemElements[0].Trim();
        var oidValue = itemElements[1].Trim();
        
        if (!oidDict.ContainsKey(oidKey))
          oidDict.Add(oidKey, oidValue);
        else
          oidDict[oidKey] = string.Format("{0}, {1}", oidDict[oidKey], oidValue);
      }
      return oidDict;
    }
    
    #endregion
    
    #region Прочее
    
    /// <summary>
    /// Сравнить значение свойства объекта
    /// </summary>
    /// <param name="obj">Объект</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <param name="newValue">Значение для сравнения</param>
    [Public]
    public bool IsPropertyValueChanged(object obj, string propertyName, object newValue)
    {
      var propertyInfo = obj.GetType().GetProperty(propertyName);
      if (propertyInfo == null) return false;
      
      var currentValue = propertyInfo.GetValue(obj);
      return !Equals(currentValue, newValue);
    }
    
    /// <summary>
    /// Получить свойство объекта по имени
    /// </summary>
    /// <param name="obj">Объект</param>
    /// <param name="propertyName">Имя свойства</param>
    [Public]
    public object GetPropertyValue(object obj, string propertyName)
    {
      var propertyInfo = obj.GetType().GetProperty(propertyName);
      return propertyInfo?.GetValue(obj);
    }
    
    /// <summary>
    /// Функция послыает запрос на удаление записи блокировки сушности в базу
    /// </summary>
    [Public, Remote]
    public void UnblockCardByDatabase(Sungero.Domain.Shared.IEntity entity)
    {
      if (entity != null)
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand("delete from Sungero_System_Locks where EntityId = "
                                                                 + entity.Id.ToString() + " and EntityTypeGuid = '"
                                                                 + entity.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "'");
    }

    /// <summary>
    /// Проверяет существование контрагента по ИНН
    /// </summary>
    /// <param name="tin">ИНН</param>
    /// <returns>Возварщает true если контрагент существует</returns>
    [Public, Remote]
    public bool IsExistCounterparty(string tin)
    {
      return Sungero.Parties.Counterparties.GetAll().Where(p => Equals(p.TIN, tin)).Any();
    }
    
    /// <summary>
    /// Функция посылает запрос на удаление записи блокировки бинарной версии в базу.
    /// </summary>
    [Public, Remote]
    public void UnblockVersionByDatabase(Sungero.Content.IElectronicDocumentVersions version)
    {
      if (version != null)
      {
        UnblockCardByDatabase(version.RootEntity);
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand("delete from Sungero_System_BinDataLocks where EntityId = "
                                                                 + version.Id.ToString() + " and EntityTypeGuid = '"
                                                                 + version.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "'");
      }
    }

    #endregion
    
    #region Создание и заполнение документа
    /// <summary>
    /// Функция создает новое тело документа основываясь на указанных в карточке свойствах
    /// </summary>
    [Remote, Public]
    public void CreateBodyByProperties(IOfficialDocument doc)
    {
      string pathTemplate = SberContracts.PublicFunctions.DevSettings.Remote.GetDevSetting("Путь к папке с шаблонами").Text;
      Aspose.Words.Document body = null;
      var typeName = doc.GetEntityTypeFullName();
      switch (typeName)
      {
        case "sberdev.SberContracts.Purchase":
          var purch = SberContracts.Purchases.As(doc);
          if (purch.PurchaseAmount > 500000)
          {
            body = new Aspose.Words.Document(pathTemplate + SBContracts.PublicConstants.Module.PurchaseTemplateDocxName);
            CreateBodyByPropertiesPurchase(purch, body);
          }
          else
          {
            body = new Aspose.Words.Document(pathTemplate + SBContracts.PublicConstants.Module.PurchaseShortTemplateDocxName);
            CreateBodyByPropertiesPurchaseShort(purch, body);
          }
          break;
        case "sberdev.SBContracts.Contract":
          if (doc.DocumentKind == null)
            break;
          if (doc.DocumentKind.Name == "Договор Xiongxin" )
          {
            body = new Aspose.Words.Document(pathTemplate + SBContracts.PublicConstants.Module.ContractXiongxinTemplateDocxName);
            CreateBodyByPropertiesContractXiongxin(SBContracts.ContractualDocuments.As(doc), body);
          }
          break;
        case "sberdev.SBContracts.SupAgreement":
          if (doc.DocumentKind == null)
            break;
          if (doc.DocumentKind.Name == "Заказ Xiongxin")
          {
            body = new Aspose.Words.Document(pathTemplate + SBContracts.PublicConstants.Module.OrderXiongxinTemplateDocxName);
            CreateBodyByPropertiesOrderXiongxin(SBContracts.SupAgreements.As(doc), body);
          }
          break;
        case "sberdev.SberContracts.AppProductPurchase":
          body = new Aspose.Words.Document(pathTemplate + SBContracts.PublicConstants.Module.AppProductPurchaseTemplateDocxName);
          CreateBodyByPropertiesAppProductPurchase(SberContracts.AppProductPurchases.As(doc), body);
          break;
      };
      
      var gPath = Guid.NewGuid().ToString();
      string pathNewDoc = "C:TempDocs//" + gPath + ".docx";
      body.Save(pathNewDoc);
      doc.CreateVersionFrom(pathNewDoc);
      doc.Save();
    }
    
    #region Заказ Xiongxin
    public void CreateBodyByPropertiesOrderXiongxin(SBContracts.ISupAgreement order, Aspose.Words.Document body)
    {
      var contract = order.LeadingDocument;
      if (contract.RegistrationNumber != null)
        body.Range.Replace("[ContractNumber]", contract.RegistrationNumber);
      else
        body.Range.Replace("[ContractNumber]", "");
      if (order.RegistrationNumber != null)
        body.Range.Replace("[Number]", order.RegistrationNumber);
      else
        body.Range.Replace("[Number]", "");
      var contrDate = contract.DocumentDate.Value;
      body.Range.Replace("[ContractYear]", contrDate.Year.ToString());
      body.Range.Replace("[ContractMonth]", contrDate.Month.ToString());
      body.Range.Replace("[ContractDay]", contrDate.Day.ToString());
      body.Range.Replace("[ContractDate]", contrDate.ToShortDateString());
      var delDate = order.DeliveryDateSberDev.Value;
      body.Range.Replace("[DeliveryYear]", delDate.Year.ToString());
      body.Range.Replace("[DeliveryMonth]", delDate.Month.ToString());
      body.Range.Replace("[DeliveryDay]", delDate.Day.ToString());
      body.Range.Replace("[DeliveryDate]", delDate.ToShortDateString());
      body.Range.Replace("[DeliveryConditionEn]", order.DelConditionEnSberDev);
      if (order.DelConditionChSberDev != null)
        body.Range.Replace("[DeliveryConditionCh]", order.DelConditionChSberDev);
      body.Range.Replace("[Agent]", order.AgentSaluteSberDev.Name);
      body.Range.Replace("[AgentTranslit]", Functions.Module.Transliterate(order.AgentSaluteSberDev.Name));
      
      List<int> boldRows = new List<int>(){};
      List<int> columnWidths = new List<int>(){10, 80, 30, 15, 15, 20};
      boldRows.Add(0);
      boldRows.Add(order.OrderXXTableSberDev.Count + 1);
      string[,] table = new string[order.OrderXXTableSberDev.Count + 2, 6];
      table[0, 0] = "/项目编 / № п/п.";
      table[0, 1] = "描述 /Описание Товара";
      table[0, 2] = "型号 / Модель";
      table[0, 3] = "数量 / Количество";
      table[0, 4] = "单价（人民币¥）/ Цена за ед. в Юанях";
      table[0, 5] = "未税金额（人民币¥) / Общая стоимость в Юанях (без НДС)";
      int counter = 1;
      foreach (var elem in order.OrderXXTableSberDev)
      {
        table[counter, 0] = counter.ToString();
        table[counter, 1] = elem.ProductDescrip;
        table[counter, 2] = elem.Model;
        table[counter, 3] = elem.Amount.Value.ToString();
        table[counter, 4] = elem.UnitPrice.Value.ToString();
        table[counter, 5] = (elem.Amount.Value * elem.UnitPrice.Value).ToString();
        counter++;
      }
      table[order.OrderXXTableSberDev.Count + 1, 1] = "ИТОГО / TOTAL";
      table[order.OrderXXTableSberDev.Count + 1, 3] = order.OrderXXTableSberDev.Select(p => p.Amount).Sum().ToString();
      table[order.OrderXXTableSberDev.Count + 1, 5] = order.OrderXXTableSberDev.Select(p => p.Amount * p.UnitPrice).Sum().ToString();
      ReplacePlaceholderWithTable(body, "[Table]", CreateTableByArray(body, table, boldRows, columnWidths, "Times New Roman", 9));
    }
    #endregion
    
    #region Договор Xiongxin
    public void CreateBodyByPropertiesContractXiongxin(SBContracts.IContractualDocument contr, Aspose.Words.Document body)
    {
      if (contr.RegistrationNumber != null)
        body.Range.Replace("[Number]", contr.RegistrationNumber);
      else
        body.Range.Replace("[Number]", "");
      body.Range.Replace("[ValidFrom]", contr.ValidFrom.Value.ToShortDateString());
      body.Range.Replace("[Agent]", contr.AgentSaluteSberDev.Name);
      body.Range.Replace("[AgentTranslit]", Functions.Module.Transliterate(contr.AgentSaluteSberDev.Name));
      if (contr.PhoneNumberSberDev != null)
        body.Range.Replace("[PhoneNumber]", contr.PhoneNumberSberDev);
      else
        body.Range.Replace("[PhoneNumber]", "");
      if (contr.EmailSberDev != null)
        body.Range.Replace("[Email]", contr.EmailSberDev);
      else
        body.Range.Replace("[Email]", "");
      body.Range.Replace("[ShortCurrency]", contr.Currency.ShortName);
      body.Range.Replace("[Currency]", contr.Currency.Name);
      body.Range.Replace("[DelPeriodNumber]", contr.DelPeriodSberDev.Value.ToString());
      string str = null;
      body.Range.Replace("[DelPeriodText]", Functions.Module.NumberToWords(contr.DelPeriodSberDev.Value));
      body.Range.Replace("[AmountPostpayNumber]", (100 - contr.AmountPrepaySberDev.Value).ToString());
      body.Range.Replace("[AmountPostpayText]", Functions.Module.NumberToWords((int)(100 - contr.AmountPrepaySberDev.Value)));
      body.Range.Replace("[AmountPrepayNumber]", contr.AmountPrepaySberDev.Value.ToString());
      body.Range.Replace("[AmountPrepayText]", Functions.Module.NumberToWords((int)contr.AmountPrepaySberDev.Value));
      body.Range.Replace("[DeadlinePrepayNumber]", contr.DeadlinePrepaySberDev.Value.ToString());
      str = Functions.Module.NumberToWords(contr.DeadlinePrepaySberDev.Value);
      if (str != null)
        body.Range.Replace("[DeadlinePrepayText]", str);
      else
        body.Range.Replace("[DeadlinePrepayText]", "");
      body.Range.Replace("[ValidTill]", contr.ValidTill.Value.ToShortDateString());
      var bank = contr.Counterparty.Bank;
      if (bank != null)
      {
        body.Range.Replace("[BankName]", bank.Name);
        body.Range.Replace("[BankBIC]", bank.BIC);
        body.Range.Replace("[BankCode]", bank.Code);
      }
      else
      {
        body.Range.Replace("[BankName]", "Не указан банк в карточке контрагента!");
        body.Range.Replace("[BankBIC]", "Не указан банк в карточке контрагента!");
        body.Range.Replace("[BankCode]", "Не указан банк в карточке контрагента!");
      }
      if (contr.Counterparty.Account != null)
        body.Range.Replace("[ACNumber]", contr.Counterparty.Account);
      else
        body.Range.Replace("[ACNumber]", "Не указан счет в карточке контрагента!");
    }
    #endregion
    
    #region Закупка
    public void CreateBodyByPropertiesPurchase(SberContracts.IPurchase purch, Aspose.Words.Document body)
    {
      List<string> addendums = new List<string>();
      if (purch.Specification != null)
      {
        string link = "https://directum.sberdevices.ru/DrxWeb/#/sat/card/"
          + purch.Specification.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "/" + purch.Specification.Id.ToString();
        body.Range.Replace("[Specification]", link);
        addendums.Add("Техническое задание - " + link);
      }
      else
        body.Range.Replace("[Specification]", "отсутсвует");
      
      if (purch.CommercialOffer != null)
      {
        string link = "https://directum.sberdevices.ru/DrxWeb/#/sat/card/"
          + purch.CommercialOffer.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "/" + purch.CommercialOffer.Id.ToString();
        body.Range.Replace("[CommercialOffer]", link);
        addendums.Add("Коммерческое предложение - " + link);
      }
      if (purch.UpgradedCommercialOffer != null)
      {
        string link = "https://directum.sberdevices.ru/DrxWeb/#/sat/card/"
          + purch.UpgradedCommercialOffer.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "/" + purch.UpgradedCommercialOffer.Id.ToString();
        body.Range.Replace("[UpgradedCommercialOffer]", "Улучшенное технико-коммерческое предложение (" + link + ")");
        addendums.Add("Улучшенное коммерческое предложение - " + link);
      }
      else
        body.Range.Replace("[UpgradedCommercialOffer]", "");
      
      body.Range.Replace("[Currency]", purch.Currency.ShortName);
      body.Range.Replace("[KindPurchase]", GetPurchaseKind(purch.KindPurchase));
      body.Range.Replace("[CPName]", purch.Counterparty.Name);
      body.Range.Replace("[SubjectPurchase]", purch.SubjectPurchase);
      body.Range.Replace("[SubjectPurchaseGen]", purch.SubjectPurchaseGen);
      var nowDate = Sungero.Core.Calendar.Now;
      body.Range.Replace("[CreatedDay]", nowDate.Day.ToString());
      body.Range.Replace("[CreatedMonth]", PublicFunctions.Module.GetMonthGenetiveName(nowDate.Month));
      body.Range.Replace("[CreatedYear]", nowDate.Year.ToString());
      body.Range.Replace("[MethodPurchase]", GetMethodPurchase(purch.MethodPurchase.Value));
      body.Range.Replace("[PurchaseAmount]", purch.PurchaseAmount.ToString());
      body.Range.Replace("[MVZ]", purch.MVZBaseSberDev != null ? purch.MVZBaseSberDev.Name : purch.MVPBaseSberDev.Name);
      
      body.Range.Replace("[ProjectName]", purch.ProjectName);
      body.Range.Replace("[TargetPurchase]", purch.TargetPurchase);
      
      if (purch.Necessary != null)
        body.Range.Replace("[Necessary]", purch.Necessary);
      else
        body.Range.Replace("[Necessary]", "");
      
      var realizeList = purch.RealizeCollection.Select(p => p.Text).ToList();
      realizeList.Add("проект \"" + purch.ProjectName + "\":");
      ReplacePlaceholderWithMarkedParagraphs(body, "[Realize]", realizeList, Aspose.Words.Lists.ListTemplate.BulletDisk);
      
      body.Range.Replace("[ProjectName]", "\"" + purch.ProjectName + "\":");
      body.Range.Replace("[InfoSubjectPurchase]", purch.InfoSubjectPurchase);
      body.Range.Replace("[HistoryPurchase]", purch.HistoryPurchase);
      body.Range.Replace("[TasksPurchase]", purch.TasksPurchase);
      body.Range.Replace("[IfNoPurchase]", purch.IfNoPurchase);
      body.Range.Replace("[JustifImpossibInhouse]", purch.JustifImpossibInhouse);
      body.Range.Replace("[ChooseCpJustif]", purch.ChooseCpJustif);
      if (purch.ScreenBusinessPlan != null)
      {
        body.Range.Replace("[ScreenBusinessPlanText]", "Скрин строки из Бизнес-плана прилагается.");
        using (MemoryStream stream = new MemoryStream(purch.ScreenBusinessPlan))
        {
          ReplacePlaceholderWithImage(body, "[ScreenBusinessPlan]", stream);
        }
      }
      else
      {
        body.Range.Replace("[ScreenBusinessPlan]", "");
        body.Range.Replace("[ScreenBusinessPlanText]", "");
      }
      
      if (purch.NegotiationsDiscount.HasValue)
        body.Range.Replace("[NegotiationsDiscount]", "По итогам переговоров цена была снижена на " + purch.NegotiationsDiscount.ToString() + "%");
      else
        body.Range.Replace("[NegotiationsDiscount]", "");
      
      body.Range.Replace("[PayType]", GetPaymentType(purch));
      
      string linkС = null;
      if (purch.LeadingDocument != null)
      {
        linkС = "https://directum.sberdevices.ru/DrxWeb/#/sat/card/"
          + purch.LeadingDocument.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "/" + purch.LeadingDocument.Id.ToString();
        addendums.Add("Договор - " + linkС);
        body.Range.Replace("[ApprovalProjectContract]", linkС);
      }
      else
        body.Range.Replace("[ApprovalProjectContract]", "");
      
      if (purch.ConcludedContractsKind == SberContracts.Purchase.ConcludedContractsKind.Yes)
        body.Range.Replace("[ConcludedContracts]", $"а также с использованием информации из действующих договоров, например: " +
                           $"Согласно {purch.LeadingDocument.Name} ({linkС}) стоимость {purch.SubjectPurchaseGen} составляет " +
                           $"{purch.TotalAmount.Value.ToString()} {purch.Currency.ShortName} [VAT], что соответствует среднерыночной стоимости.");
      if (purch.ConcludedContractsKind == SberContracts.Purchase.ConcludedContractsKind.NoChanges)
        body.Range.Replace("[ConcludedContracts]", "а также с использованием информации из действующих договоров, например:\n" +
                           "Указанная стоимость услуг остается неизменной с " + purch.LeadingDocument.DocumentDate + ", что подтверждено" + purch.LeadingDocument.Name
                           + ". (" + linkС + "), что соответствует среднерыночной стоимости.");
      if (purch.ConcludedContractsKind == SberContracts.Purchase.ConcludedContractsKind.No)
        body.Range.Replace("[ConcludedContracts]", "Действующие договоры с " + purch.Counterparty.Name + " отсутствуют.");
      
      if (purch.PrepaymentPercent != null)
        body.Range.Replace("[PrepaymentJustification]", "Аванс предусмотрен в размере " + purch.PrepaymentPercent + "% в связи с тем, что " + purch.PrepaymentJustification);
      else
        body.Range.Replace("[PrepaymentJustification]", "Аванс не предусмотрен.");
      
      body.Range.Replace("[BusinessUnit]", purch.BusinessUnit.Name);
      body.Range.Replace("[Authorized]", purch.Authorized.Name);
      body.Range.Replace("[VAT]", (purch.VAT.Value ? " с учетом НДС 20%" : " без учета НДС 20%"));
      
      if (purch.NecessaryConclude == SberContracts.Purchase.NecessaryConclude.Contract)
        body.Range.Replace("[NecessaryConclude]", "Договор");
      else
        body.Range.Replace("[NecessaryConclude]", "Дополнительное Соглашение к " + purch.LeadingDocument.Name + " от " + purch.LeadingDocument.DocumentDate.Value.Year.ToString() +" года");
      body.Range.Replace("[DepartmentPurchase]", purch.DepartmentPurchase.Name);
      
      if (purch.TotalAmount != null && purch.TotalAmount != purch.PurchaseAmount)
        body.Range.Replace("[TotalAmount]", $", при этом общая стоимость договора составит {purch.TotalAmount.Value.ToString()} {purch.Currency.ShortName}");
      else
        body.Range.Replace("[TotalAmount]", "");
      
      var endDate = purch.ServiceEndDate.Value;
      var startDate = purch.ServiceStartDate.Value;
      int monthsDifference = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;

      // Если день начала больше дня конца, убираем 1 месяц
      if (endDate.Day < startDate.Day)
      {
        monthsDifference--;
      }
      
      body.Range.Replace("[DateDiff]", Math.Abs(monthsDifference).ToString());
      body.Range.Replace("[PurchaseKindExt]", GetPurchaseKindExt(purch.KindPurchase));
      body.Range.Replace("[ServiceStartDate]", purch.ServiceStartDate.Value.ToShortDateString());
      body.Range.Replace("[ServiceEndDate]", purch.ServiceEndDate.Value.ToShortDateString());
      body.Range.Replace("[InitiatorManager]", Sungero.Company.Employees.As(purch.Author).Department.Manager.Name);
      body.Range.Replace("[MVZBudgetOwner]", (purch.MVZBaseSberDev.MainMVZ != null ? purch.MVZBaseSberDev.MainMVZ.BudgetOwner.Person.ShortName : (purch.MVZBaseSberDev != null ?
                                                                                                                                                  purch.MVZBaseSberDev.BudgetOwner.Person.ShortName : null)));
      if (purch.BusinessUnit.CEO != null)
        body.Range.Replace("[CEO]", purch.BusinessUnit.CEO.Name);
      else
        body.Range.Replace("[CEO]", "Руководитель Нашей организации отсутвует");
      
      if (addendums.Count > 0)
        ReplacePlaceholderWithMarkedParagraphs(body, "[Addendums]", addendums, Aspose.Words.Lists.ListTemplate.NumberDefault);
      else
        body.Range.Replace("[Addendums]", "Отсутвует");
      
      #region Таблицы
      List<int> boldRows = new List<int>(){};
      List<int> columnWidths = new List<int>(){100, 25, 25};
      boldRows.Add(0);
      boldRows.Add(purch.StagesPurchaseCollection.Count + 1);
      string[,] stages = new string[purch.StagesPurchaseCollection.Count + 2, 3];
      stages[0, 0] = "Название этапа";
      stages[0, 1] = "Стоимость";
      stages[0, 2] = "Длительность (дн.)";
      int counter = 1;
      foreach (var stage in purch.StagesPurchaseCollection)
      {
        stages[counter, 0] = stage.Name;
        stages[counter, 1] = stage.Cost.Value.ToString();
        stages[counter, 2] = stage.Duration.Value.ToString();
        counter++;
      }
      stages[purch.StagesPurchaseCollection.Count + 1, 0] = "ИТОГО длительность и стоимость реализации проекта";
      stages[purch.StagesPurchaseCollection.Count + 1, 1] = purch.StagesPurchaseCollection.Select(p => p.Cost).Sum().ToString();
      stages[purch.StagesPurchaseCollection.Count + 1, 2] = purch.StagesPurchaseCollection.Select(p => p.Duration).Sum().ToString();
      ReplacePlaceholderWithTable(body, "[StagesPurchaseCollection]", CreateTableByArray(body, stages, boldRows, columnWidths, "Times New Roman", 12));
      
      boldRows[1] = 0;
      columnWidths = new List<int>(){90, 20, 20, 20};
      string[,] costAnalysis = new string[purch.CostAnalysisCollection.Count + 1, 4];
      costAnalysis[0, 0] = "Наименование работ/услуг";
      costAnalysis[0, 1] = "Цена по КП выбр. контр.";
      costAnalysis[0, 2] = "Цена по КП 2";
      costAnalysis[0, 3] = "Цена по КП 3";
      counter = 1;
      foreach (var row in purch.CostAnalysisCollection)
      {
        costAnalysis[counter, 0] = row.Name;
        costAnalysis[counter, 1] = row.COCost1.HasValue ? row.COCost1.Value.ToString() : "";
        costAnalysis[counter, 2] = row.COCost2.HasValue ? row.COCost2.Value.ToString() : "";
        costAnalysis[counter, 3] = row.COCost3.HasValue ? row.COCost3.Value.ToString() : "";
        counter++;
      }
      if (purch.CostAnalysisCollection.Count > 0)
        ReplacePlaceholderWithTable(body, "[CostAnalysisCollection]", CreateTableByArray(body, costAnalysis, boldRows, columnWidths, "Times New Roman", 12));
      else
        body.Range.Replace("[CostAnalysisCollection]", "");
      #endregion
    }
    #endregion
    
    #region Закупка меньше 500к
    public void CreateBodyByPropertiesPurchaseShort(SberContracts.IPurchase purch, Aspose.Words.Document body)
    {
      body.Range.Replace("[SubjectPurchase]", purch.SubjectPurchase);
      body.Range.Replace("[TargetPurchase]", purch.TargetPurchase);
      var budgetOwner = SBContracts.PublicFunctions.ContractualDocument.GetMVZBudgetOwner(purch);
      if (budgetOwner != null)
        body.Range.Replace("[BudgetOwner]", budgetOwner.Name);
      else
        body.Range.Replace("[BudgetOwner]", "Отсутствует");
      string str = "";
      var exProd = purch.ProdCollectionExBaseSberDev.FirstOrDefault()?.Product;
      var prProd = purch.ProdCollectionPrBaseSberDev.FirstOrDefault()?.Product;
      if (exProd?.Name == "General" || prProd?.Name == "General")
      {
        if (purch.CalculationFlagBaseSberDev == SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Absolute)
          foreach(var prod in purch.CalculationBaseSberDev)
            str += prod.ProductCalc.Name + " - " + prod.AbsoluteCalc.ToString() + " \n";
        if (purch.CalculationFlagBaseSberDev == SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Percent)
          foreach(var prod in purch.CalculationBaseSberDev)
            str += prod.ProductCalc + " - " + prod.InterestCalc.ToString() + "\n";
      }
      else
      {
        str += exProd?.Name;
        str += prProd?.Name;
      }
      body.Range.Replace("[Products]", str);
      body.Range.Replace("[AccArt]", purch.AccArtExBaseSberDev.Name);
      if (purch.MarketDirectSberDev != null)
        body.Range.Replace("[MarketDirect]", purch.MarketDirectSberDev.Name);
      else
        body.Range.Replace("[MarketDirect]", "Отсутствует");
      body.Range.Replace("[Amount]", purch.PurchaseAmount.Value.ToString() + " " + purch.Currency.ShortName + (purch.VAT.Value ? " с учетом НДС" : " без учета НДС"));
      body.Range.Replace("[Counterparty]", purch.Counterparty.Name);
      body.Range.Replace("[ChooseCpJustif]", purch.ChooseCpJustif);
      body.Range.Replace("[PaymentType]", GetPaymentType(purch));
    }
    #endregion
    
    #region Заявка нп производ. закупку
    public void CreateBodyByPropertiesAppProductPurchase(SberContracts.IAppProductPurchase purch, Aspose.Words.Document body)
    {
      // Удаление неиспользуемых плейсхолдеров
      List<string> placeholdersToRemove = new List<string>();
      
      var contractText = purch.LeadingDocument != null
        ? $"{purch.LeadingDocument.RegistrationNumber ?? "Не зарегистрирован"}, {purch.LeadingDocument.DocumentDate?.ToShortDateString()}"
        : "Договор не указан";
      body.Range.Replace("[Contract]", contractText);
      
      body.Range.Replace("[NDA]", purch.NDA != null ? $"https://directum.sberdevices.ru/DrxWeb/#/sat/card/d62141c7-715c-47bb-82b0-cf2ffbe8b6e7/{purch.NDA.Id}" : "NDA не указан");

      var cpContactsText = $"ФИО: {purch.AgentCP ?? "Нет"}" +
        $" Телефон: {purch.PhoneNumberSberDev ?? "Нет"}\n" +
        $"Email: {purch.EmailSberDev ?? "Нет"}" +
        $"{(purch.CPContactComment != null ? $"\nКомментарий: {purch.CPContactComment}" : "")}";
      body.Range.Replace("[CPContacts]", cpContactsText);

      body.Range.Replace("[PlanDelDate]", purch.PlanDelDate.Value.ToShortDateString());
      
      var prodPeriodText = purch.ProdPeriod.Value.ToShortDateString() +
        $"{(purch.ProdPeriodComment != null ? $"\nКомментарий: {purch.ProdPeriodComment}" : "")}";
      body.Range.Replace("[ProdPeriod]", prodPeriodText);
      
      body.Range.Replace("[Deposit]", purch.Deposit.ToString());
      body.Range.Replace("[DepositDays]",  $"{(purch.DepositDays > 0 ? $"% в течении {purch.DepositDays} р. д." : "")}");
      body.Range.Replace("[Balance]", purch.Balance.ToString());
      body.Range.Replace("[BalanceDays]", $"{(purch.BalanceDays > 0 ? $"% в течении {purch.BalanceDays} р. д." : "")}");
      var paymentMethod = purch.PaymentMethod.Value.Value;
      body.Range.Replace("[PaymentMethod]", SberContracts.PublicFunctions.AppProductPurchase.TranslatePaymentMethod(purch, paymentMethod));
      if (paymentMethod == "Agent")
      {
        body.Range.Replace("[AgencyContract]", $"https://directum.sberdevices.ru/DrxWeb/#/sat/card/f37c7e63-b134-4446-9b5b-f8811f6c9666/{purch.NDA.Id}");
        body.Range.Replace("[AgencyPercent]", $"{purch.AgencyPercent.Value.ToString()}%");
        body.Range.Replace("[AgencyPayDate]", purch.AgencyPayDate.Value.ToShortDateString());
        body.Range.Replace("[AgencyFlagPAO]", purch.AgencyFlagPAO.Value ? "Да" : "Нет");
      }
      else
      {
        body.Range.Replace("[AgencyContract]", "Счет в ВТБ");
        body.Range.Replace("[AgencyPercent]", "Счет в ВТБ");
        body.Range.Replace("[AgencyPayDate]", "Счет в ВТБ");
        body.Range.Replace("[AgencyFlagPAO]", "Счет в ВТБ");
      }
      body.Range.Replace("[Incoterms]", purch.Incoterms.Value.Value);
      body.Range.Replace("[PickupAddress]", purch.PickupAddress != null ? purch.PickupAddress : "Не указан");
      body.Range.Replace("[PlanDelType]", SberContracts.PublicFunctions.AppProductPurchase.TranslatePlanDelType(purch, purch.PlanDelType.Value.Value));
      body.Range.Replace("[Responsible]", purch.ResponsibleEmployee.Name);
      string str = "";
      var exProd = purch.ProdCollectionExBaseSberDev.FirstOrDefault()?.Product;
      if (exProd?.Name == "General")
      {
        if (purch.CalculationFlagBaseSberDev == SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Absolute)
          foreach(var prod in purch.CalculationBaseSberDev)
            str += prod.ProductCalc.Name + " - " + prod.AbsoluteCalc.ToString() + "\n";
        if (purch.CalculationFlagBaseSberDev == SBContracts.ContractualDocument.CalculationFlagBaseSberDev.Percent)
          foreach(var prod in purch.CalculationBaseSberDev)
            str += prod.ProductCalc + " - " + prod.InterestCalc.ToString() + "\n";
      }
      else
        str += exProd?.Name;
      
      body.Range.Replace("[Products]", str);
      body.Range.Replace("[AccArt]", purch.AccArtExBaseSberDev.Name);
      body.Range.Replace("[MVZ]", purch.MVZBaseSberDev.Name);
      if (purch.Note != null)
        body.Range.Replace("[Note]", "Примечание: " + purch.Note);
      placeholdersToRemove.Add("[Note]");
      
      #region Таблица продуктов
      // Настройки таблицы
      var boldRows = new List<int> { 0, purch.PurchasesCollection.Count + 1 };
      var columnWidths = new List<int> { 10, 80, 20, 15, 25, 15 };

      // Инициализация таблицы
      int rowCount = purch.PurchasesCollection.Count + 2;
      string[,] purchTable = new string[rowCount, 6];

      // Заполнение заголовка таблицы
      string[] headers = { "№", "Наименование продукции", "Цена за ед. " + (purch.FlagVAT.Value ? "с НДС" : "без НДС"), "Кол-во, шт.", "Полн. стоим.", "Валюта" };
      for (int i = 0; i < headers.Length; i++)
        purchTable[0, i] = headers[i];

      // Заполнение строк данных
      int counter = 1;
      foreach (var elem in purch.PurchasesCollection)
      {
        purchTable[counter, 0] = counter.ToString();
        purchTable[counter, 1] = elem.Product;
        purchTable[counter, 2] = elem.PriceUnit.Value.ToString();
        purchTable[counter, 3] = elem.Quantity.Value.ToString();
        purchTable[counter, 4] = (elem.PriceUnit.Value * elem.Quantity.Value).ToString();
        purchTable[counter, 5] = purch.Currency?.ShortName;
        counter++;
      }

      // Заполнение итоговой строки
      purchTable[rowCount - 1, 1] = "ИТОГО / TOTAL";
      purchTable[rowCount - 1, 3] = purch.PurchasesCollection.Sum(p => p.Quantity.Value).ToString();
      purchTable[rowCount - 1, 4] = purch.PurchasesCollection.Sum(p => p.PriceUnit.Value * p.Quantity.Value).ToString();

      // Вставка таблицы в документ
      ReplacePlaceholderWithTable(
        body,
        "[PurchTable]",
        CreateTableByArray(body, purchTable, boldRows, columnWidths, "Times New Roman", 10)
       );
      #endregion
      
      #region Закомменчено
     /*
      
      #region Таблица ресипиентов
      if (purch.ParticipantsCollection.Any())
      {
        // Настройки таблицы
        boldRows = new List<int> { 0};
        columnWidths = new List<int> { 10, 40, 60 };

        // Инициализация таблицы
        rowCount = purch.ParticipantsCollection.Count + 1;
        string[,] recipTable = new string[rowCount, 3];

        // Заполнение заголовка таблицы
        string[] headers1 = { "№", "Наименование компании", "Контакты"};
        for (int i = 0; i < headers1.Length; i++)
          recipTable[0, i] = headers1[i];

        // Заполнение строк данных
        counter = 1;
        foreach (var elem in purch.ParticipantsCollection)
        {
          recipTable[counter, 0] = counter.ToString();
          recipTable[counter, 1] = elem.Counterparty;
          recipTable[counter, 2] = elem.Contacts;
          counter++;
        }
        
        // Вставка таблицы в документ
        ReplacePlaceholderWithTable(
          body,
          "[RecipTable]",
          CreateTableByArray(body, recipTable, boldRows, columnWidths, "Times New Roman", 10)
         );
      }
      #endregion #region Таблицы по контрагентам
      // Настройки таблиц
      boldRows = new List<int> { 0 }; // Общая настройка для всех таблиц
      columnWidths = new List<int> { 5, 25, 7, 10, 15, 15, 20, 20, 20 };
      string[] headers3 = { "№", "Наименование продукта", "Кол-во", "Цена за шт, без НДС", "Сумма, без НДС", "Условия оплаты",
        "Срок поставки", "Преимущества", "Риски" };

      // Определение количества таблиц
      var numberMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
      {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
      };

      int tableCount = 0;
      numberMapping.TryGetValue(purch.CpNumber.Value.Value.ToLower(), out tableCount);

      // Определение выбранного контрагента
      int selectedCounterpartyIndex = -1;
      for (int i = 1; i <= 7; i++)
      {
        var selectedProperty = typeof(SberContracts.IAppProductPurchase).GetProperty($"SelectedCounterparty{i}");
        if (selectedProperty != null)
        {
          bool isSelected = (bool)selectedProperty.GetValue(purch);
          if (isSelected)
          {
            selectedCounterpartyIndex = i;
            break;
          }
        }
      }

      // Генерация таблиц
      for (int tableIndex = 1; tableIndex <= tableCount; tableIndex++)
      {
        // Получение коллекции для текущей таблицы
        var collectionProperty = typeof(SberContracts.IAppProductPurchase).GetProperty($"ComparativeCollection{tableIndex}");
        if (collectionProperty == null)
          throw new InvalidOperationException($"Не найдено свойство ComparativeCollection{tableIndex}");

        var comparativeCollection = (IEnumerable<object>)collectionProperty.GetValue(purch);
        if (comparativeCollection == null)
          throw new InvalidOperationException($"Свойство ComparativeCollection{tableIndex} имеет значение null");

        // Инициализация таблицы
        rowCount = comparativeCollection.Count() + 1;
        string[,] tableData = new string[rowCount, headers3.Length];

        // Заполнение заголовка таблицы
        for (int i = 0; i < headers3.Length; i++)
          tableData[0, i] = headers3[i];

        // Заполнение строк данных
        counter = 1;
        foreach (var elem in comparativeCollection)
        {
          var product = elem.GetType().GetProperty("Product")?.GetValue(elem, null).ToString();
          var quantity = (int?)elem.GetType().GetProperty("Quantity")?.GetValue(elem, null);
          var priceUnit = (double?)elem.GetType().GetProperty("PriceUnit")?.GetValue(elem, null);
          var paymentTerms = elem.GetType().GetProperty("PaymentTerms")?.GetValue(elem, null)?.ToString();
          var deliveryPer = (DateTime)elem.GetType().GetProperty("DeliveryPer")?.GetValue(elem, null);
          var advantages = elem.GetType().GetProperty("Advantages")?.GetValue(elem, null)?.ToString();
          var risks = elem.GetType().GetProperty("Risks")?.GetValue(elem, null)?.ToString();

          tableData[counter, 0] = counter.ToString();
          tableData[counter, 1] = product;
          tableData[counter, 2] = quantity.ToString();
          tableData[counter, 3] = priceUnit.ToString();
          tableData[counter, 4] = (priceUnit.GetValueOrDefault() * quantity.GetValueOrDefault()).ToString();
          tableData[counter, 5] = paymentTerms;
          tableData[counter, 6] = deliveryPer.ToShortDateString();
          tableData[counter, 7] = advantages;
          tableData[counter, 8] = risks;
          counter++;
        }

        // Вставка таблицы в документ
        string placeholder = $"[CP{tableIndex}Table]";
        ReplacePlaceholderWithTable(
          body,
          placeholder,
          CreateTableByArray(body, tableData, boldRows, columnWidths, "Times New Roman", 10)
         );
        
        // Замена наименования контрагента
        var cpProp = typeof(SberContracts.IAppProductPurchase).GetProperty($"Counterparty{tableIndex}");
        string cpPropValue = (string)cpProp.GetValue(purch);
        body.Range.Replace($"[Counterparty{tableIndex}]", cpPropValue);
        
        // Если это выбранный контрагент, дополнительно заменяем плейсхолдер [SelectedCounterparty]
        if (tableIndex == selectedCounterpartyIndex)
        {
          // Создаем дубликат таблицы для выбранного контрагента
          ReplacePlaceholderWithTable(
            body,
            "[SelectedCounterparty]",
            CreateTableByArray(body, tableData, boldRows, columnWidths, "Times New Roman", 10)
           );
          
          // Дополнительно заменяем заголовок выбранного контрагента
          body.Range.Replace("[SelectedCounterpartyName]", cpPropValue);
        }
      }
      
      // Добавляем плейсхолдеры для неиспользуемых таблиц
      for (int i = tableCount + 1; i <= 7; i++)
      {
        placeholdersToRemove.Add($"[CP{i}Table]");
        placeholdersToRemove.Add($"[Counterparty{i}]");
      }
      
      // Если ни один из контрагентов не выбран или выбранный контрагент вне допустимого диапазона,
      // добавляем плейсхолдер выбранного контрагента в список для удаления
      if (selectedCounterpartyIndex <= 0 || selectedCounterpartyIndex > tableCount)
      {
        placeholdersToRemove.Add("[SelectedCounterparty]");
        placeholdersToRemove.Add("[SelectedCounterpartyName]");
      }
      
      // Удаление всех неиспользуемых плейсхолдеров
      RemovePlaceholders(body, placeholdersToRemove);
      
      #endregion */
    }
      #endregion 
      
    #endregion
    
    #region Вспомогательные функции для построения документа
    
    // Метод для удаления плейсхолдеров из документа
    static void RemovePlaceholders(Aspose.Words.Document document, List<string> placeholders)
    {
      // Проходим по всем секциям документа
      foreach (Aspose.Words.Section section in document.Sections)
      {
        // Получаем тело секции
        Aspose.Words.Body sectionBody = section.Body;

        // Проходим по всем абзацам в теле секции
        foreach (Aspose.Words.Paragraph paragraph in sectionBody.Paragraphs.ToArray())
        {
          // Проверяем, содержит ли абзац какой-либо из плейсхолдеров
          bool containsPlaceholder = placeholders.Any(p => paragraph.Range.Text.Contains(p));
          
          if (containsPlaceholder)
          {
            paragraph.Remove();
          }
        }
      }
    }
    
    static void ReplacePlaceholderWithMarkedParagraphs(Aspose.Words.Document doc, string placeholder, List<string> texts, Aspose.Words.Lists.ListTemplate listTemplate)
    {
      // Получаем все параграфы, содержащие текстовый заполнитель
      NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
      foreach (Aspose.Words.Paragraph paragraph in paragraphs)
      {
        if (paragraph.GetText().Contains(placeholder))
        {
          // Создаем новый список для маркированных абзацев
          var list = doc.Lists.Add(listTemplate);
          
          // Заменяем текстовый заполнитель на несколько абзацев
          for (int i = texts.Count - 1; i >= 0; i--)
          {
            Aspose.Words.Paragraph newParagraph = new Aspose.Words.Paragraph(doc);
            newParagraph.ListFormat.List = list;
            newParagraph.AppendChild(new Run(doc, texts[i]));
            paragraph.ParentNode.InsertAfter(newParagraph, paragraph);
          }

          // Удаляем параграф с текстовым заполнителем
          paragraph.Remove();
        }
      }
    }
    
    static void ReplacePlaceholderWithMarkedParagraphs(Aspose.Words.Document doc, string placeholder, List<string> texts, string prestring)
    {
      // Получаем все параграфы, содержащие текстовый заполнитель
      NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
      foreach (Aspose.Words.Paragraph paragraph in paragraphs)
      {
        if (paragraph.GetText().Contains(placeholder))
        {
          // Создаем новый список для маркированных абзацев
          var list = doc.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletDisk);
          
          // Заменяем текстовый заполнитель на несколько абзацев
          for (int i = texts.Count - 1; i >= 0; i--)
          {
            Aspose.Words.Paragraph newParagraph = new Aspose.Words.Paragraph(doc);
            newParagraph.ListFormat.List = list;
            newParagraph.AppendChild(new Run(doc, prestring + " " + texts[i]));
            paragraph.ParentNode.InsertAfter(newParagraph, paragraph);
          }

          // Удаляем параграф с текстовым заполнителем
          paragraph.Remove();
        }
      }
    }
    
    static void ReplacePlaceholderWithImage(Aspose.Words.Document doc, string placeholder, Stream imageStrm)
    {
      // Получаем все параграфы, содержащие текстовый заполнитель
      NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
      foreach (Aspose.Words.Paragraph paragraph in paragraphs)
      {
        if (paragraph.GetText().Contains(placeholder))
        {
          // Удаляем текстовый заполнитель из параграфа
          paragraph.Range.Replace(placeholder, "");
          
          // Вставляем изображение вместо текстового заполнителя
          var builder = new Aspose.Words.DocumentBuilder(doc);
          builder.MoveTo(paragraph);
          builder.InsertImage(imageStrm);
        }
      }
    }
    
    static void ReplacePlaceholderWithTable(Aspose.Words.Document doc, string placeholder, Aspose.Words.Tables.Table table)
    {
      // Получаем все параграфы, содержащие текстовый заполнитель
      NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
      foreach (Aspose.Words.Paragraph paragraph in paragraphs)
      {
        if (paragraph.GetText().Contains(placeholder))
        {
          // Удаляем текстовый заполнитель из параграфа
          paragraph.Range.Replace(placeholder, "");
          paragraph.ParentNode.InsertAfter(table, paragraph);
        }
      }
    }
    
    static Aspose.Words.Tables.Table CreateTableByArray(Aspose.Words.Document doc, string[,] values, List<int> boldRows, List<int> columnWidths, string font, byte fontSize)
    {
      // Создаем новую таблицу
      var table = new Aspose.Words.Tables.Table(doc);
      
      // Добавляем строки и ячейки в таблицу
      for (int row = 0; row < values.GetLength(0); row++)
      {
        var tableRow = new Aspose.Words.Tables.Row(doc);
        for (int col = 0; col < values.GetLength(1); col++)
        {
          var cell = new Aspose.Words.Tables.Cell(doc);
          cell.CellFormat.Width = columnWidths[col];
          var dfd = columnWidths[col];
          // Создаем пустой параграф
          var paragraph = new Aspose.Words.Paragraph(doc);
          var run = new Run(doc, values[row, col] ?? "");

          // Настройка шрифта и размера текста
          run.Font.Name = font;
          run.Font.Size = fontSize;

          // Добавляем текст в параграф
          paragraph.AppendChild(run);
          // Если номер текущей строки содержится в списке boldRows, делаем текст жирным
          if (boldRows.Contains(row))
          {
            foreach (Run r in paragraph.Runs)
            {
              r.Font.Bold = true;
            }
          }

          cell.AppendChild(paragraph);
          tableRow.Cells.Add(cell);
        }
        table.AppendChild(tableRow);
      }
      table.AllowAutoFit = false;
      return table;
    }

    static Aspose.Words.Tables.Table CreateTableByArray(Aspose.Words.Document doc, string[,] values)
    {
      // Создаем новую таблицу
      var table = new Aspose.Words.Tables.Table(doc);
      
      // Добавляем строки и ячейки в таблицу
      for (int row = 0; row < values.GetLength(0); row++)
      {
        var tableRow = new Aspose.Words.Tables.Row(doc);
        for (int col = 0; col < values.GetLength(1); col++)
        {
          var cell = new Aspose.Words.Tables.Cell(doc);
          cell.CellFormat.Width = 50;
          cell.AppendChild(new Aspose.Words.Paragraph(doc));
          cell.FirstParagraph.AppendChild(new Run(doc, values[row,col]));
          tableRow.Cells.Add(cell);
        }
        table.AppendChild(tableRow);
      }
      return table;
    }
    
    string GetPurchaseKind(Enumeration? kind)
    {
      if (kind.Value == SberContracts.Purchase.KindPurchase.Service)
        return "услуг";
      else
        return "работ";
    }
    
    string GetPurchaseKindExt(Enumeration? kind)
    {
      if (kind.Value == SberContracts.Purchase.KindPurchase.Service)
        return "оказания услуг";
      else
        return "выполнения работ";
    }
    
    string GetPaymentKind(Enumeration? kind)
    {
      if (kind.Value == SberContracts.Purchase.PaymentKind.Monthly)
        return "ежемесячно ";
      if (kind.Value == SberContracts.Purchase.PaymentKind.OnePayment)
        return "одним платежом ";
      if (kind.Value == SberContracts.Purchase.PaymentKind.Quarterly)
        return "ежеквартально ";
      if (kind.Value == SberContracts.Purchase.PaymentKind.Stagely)
        return "поэтапно ";
      return "";
    }
    
    string GetMethodPurchase(Enumeration? kind)
    {
      if (kind.Value == SberContracts.Purchase.MethodPurchase.OneCP)
        return "Единственный поставщик";
      else
        return "Запрос предложений";
    }
    
    #endregion
    
    #endregion
  }
}