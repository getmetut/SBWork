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


namespace sberdev.SBContracts.Server
{
  public class ModuleFunctions
  {
    
    #region Разные геты и сеты
    
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
    
    /// <summary>
    /// Получить ИД документа из метаданных документа
    /// </summary>
    [Public, Remote]
    public string GetMetadataID(Sungero.Docflow.IOfficialDocument doc)
    {
      if (!doc.HasVersions)
        return null;
      string ext = doc.LastVersion.AssociatedApplication.Extension;
      if (ext != "doc" && ext != "docx" && ext != "pdf")
        return null;
      string guid = Guid.NewGuid().ToString();
      string path = "C:\\TempDocs\\docDirectum" + guid + "." + ext;
      doc.LastVersion.Export(path);
      if (ext == "doc" || ext == "docx")
      {
        Aspose.Words.Document docAsp = new Aspose.Words.Document(path);
        var props = docAsp.CustomDocumentProperties;
        var prop = props.FirstOrDefault(p => p.Name == "DirectumID");
        if (prop != null)
          return (string)prop.Value;
      }
      
      if (ext == "pdf")
      {
        PdfReader reader = new PdfReader(path);
        PdfReader.unethicalreading = true;
        var info = reader.Info;
        if (info.ContainsKey("DirectumID"))
          return info["DirectumID"];
        reader.Close();
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
    /// Получить элемент справочника "Настройки разработки".
    /// </summary>
    /// <param name="name">Имя настройки.</param>
    /// <returns>Элемент справочника "Настройки разработки".</returns>
    [Public, Remote]
    public sberdev.SberContracts.IDevSettings GetDevSetting(string name)
    {
      return SberContracts.DevSettingses.GetAll().Where(n => Equals(n.Name, name)).FirstOrDefault();
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
    /// <param name="document">Документ.</param>
    /// <param name="isNotNeedValid">Необходимость проверки валидности.</param>
    /// </summary>
    [Public]
    public bool CheckDepartmentApproval(SBContracts.IOfficialDocument document, Sungero.CoreEntities.IGroup depart)
    {
      bool flag = false;
      if (document == null)
        return false;
      var signatures = Signatures.Get(document.LastVersion);
      var signer = Sungero.CoreEntities.Users.As(Sungero.Company.Departments.As(depart).Manager);
      if (signer == null)
        return false;
      
      if (signatures.Any())
      {
        if (document != null)
        {
          foreach(var sign in signatures)
          {
            if (sign.SignatureType == SignatureType.Endorsing && (sign.Signatory != null && Equals(sign.Signatory, signer)
                                                                  || sign.SubstitutedUser != null && Equals(sign.SubstitutedUser, signer)));
            flag = true;
          }
        }
      }
      return flag;
    }
    
    /// <summary>
    /// Функция проверяет подпись документа. Возвращает true если есть внешняя от Нашей организации или
    /// внутренняя от одного человека из группы "Обязательные подписанты счета перед согласованием" у документа.
    /// <param name="document">Документ.</param>
    /// <param name="isNotNeedValid">Необходимость проверки валидности.</param>
    /// </summary>
    [Public]
    public bool CheckSpecialGroupSignature(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      bool flag = false;
      var signatures = Signatures.Get(document.LastVersion);
      var needSignGroup = PublicFunctions.Module.Remote.GetGroup("Обязательные подписанты счета перед согласованием");
      
      if (signatures.Any())
      {
        if (document != null)
        {
          foreach(var sign in signatures)
          {
            if (sign.IsExternal.HasValue && sign.IsExternal.Value)
            {
              var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
              string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyOrgTIN(certificateInfo.SubjectInfo);
              if (Equals(tin, document.BusinessUnit.TIN) && (sign.IsValid || !isNeedValid))
                flag = true;
            }
            else
              if (sign.SignatureType == SignatureType.Approval &&
                  ((sign.Signatory != null && sign.Signatory.IncludedIn(needSignGroup)) || (sign.SubstitutedUser != null && sign.SubstitutedUser.IncludedIn(needSignGroup))))
                flag = true;
          }
        }
      }
      return flag;
    }
    
    /// <summary>
    /// Функция проверяет наличие подписи от контрагента и от одного человека из группы
    /// "Обязательные подписанты счета перед согласованием" у документа и возвращает булевой список где первый элемент флаг нашей подписи, второй контрагента.
    /// <param name="document">Документ.</param>
    /// <param name="isNotNeedValid">Необходимость проверки валидности.</param>
    /// </summary>
    [Public, Remote]
    public List<bool> CheckRealSignatures(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      List<bool> flags = new List<bool>() {false, false};
      if (!document.HasVersions)
        return flags;
      var signatures = Signatures.Get(document.LastVersion);
      // Проверка на двойную версию
      if (document.LastVersion.Note == "Титул покупателя")
        signatures = Signatures.Get(document);
      var needSignGroup = PublicFunctions.Module.Remote.GetGroup("Обязательные подписанты счета перед согласованием");
      if (signatures.Any())
      {
        var contractual = SBContracts.ContractualDocuments.As(document);
        if (contractual != null)
        {
          foreach(var sign in signatures)
          {
            if (sign.IsExternal.HasValue && sign.IsExternal.Value)
            {
              try
              {
                var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
                string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyOrgTIN(certificateInfo.SubjectInfo);
                if (Equals(tin, contractual.Counterparty.TIN) && (sign.IsValid || !isNeedValid))
                  flags[1] = true;
                if (Equals(tin, contractual.BusinessUnit.TIN) && (sign.IsValid || !isNeedValid))
                  flags[0] = true;
              }
              catch (Exception ex)
              {
                Logger.Error("Проверка подписи. Ошибка при извлечении сертификата: " + ex.ToString());
                continue;
              }
            }
            else
              if (sign.SignatureType == SignatureType.Approval && ((sign.Signatory != null && sign.Signatory.IncludedIn(needSignGroup))
                                                                   || (sign.SubstitutedUser != null && sign.SubstitutedUser.IncludedIn(needSignGroup))))
                flags[0] = true;
          }
        }
        
        var accounting = SBContracts.AccountingDocumentBases.As(document);
        if (accounting != null)
        {
          foreach(var sign in signatures)
          {
            if (sign.IsExternal.HasValue && sign.IsExternal.Value)
            {
              try
              {
                var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
                string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyOrgTIN(certificateInfo.SubjectInfo);
                if (Equals(tin, accounting.Counterparty.TIN) && (sign.IsValid || !isNeedValid))
                  flags[1] = true;
                if (Equals(tin, accounting.BusinessUnit.TIN) && (sign.IsValid || !isNeedValid))
                  flags[0] = true;
              }
              catch (Exception ex)
              {
                Logger.Error("Проверка подписи. Ошибка при извлечении сертификата: " + ex.ToString());
                continue;
              }
            }
            else
              if (sign.SignatureType == SignatureType.Approval && ((sign.Signatory != null && sign.Signatory.IncludedIn(needSignGroup))
                                                                   || (sign.SubstitutedUser != null && sign.SubstitutedUser.IncludedIn(needSignGroup))))
                flags[0] = true;
          }
        }
      }
      return flags;
    }
    
    
    /// <summary>
    /// Получить структуру с информацией о владельце сертификата.
    /// </summary>
    /// <param name="subjectInfo">Информация о владельце сертификата.</param>
    /// <returns>Структура с информацией о владельце сертификата.</returns>
    [Public, Remote]
    public virtual string ParseCertificateSubjectOnlyOrgTIN(string subjectInfo)
    {
      var subject = SBContracts.Functions.Module.GetOidValues(subjectInfo);
      var tin = subject.Where(c => Equals(c.Key, Constants.Module.CertificateOid.OrganizationTIN)).FirstOrDefault().Value;
      if (!string.IsNullOrEmpty(tin))
        return tin;
      else
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
    /// Функция послыает запрос на удаление записи блокировки сушности в базу
    /// </summary>
    [Public, Remote]
    public void UnblockEntityByDatabase(Sungero.Domain.Shared.IEntity entity)
    {
      if (entity != null)
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand("delete from Sungero_System_Locks where EntityId = "
                                                                 + entity.Id.ToString() + " and EntityTypeGuid = '"
                                                                 + entity.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "'");
    }
    
    #endregion
    
    #region Создание и заполнение документа
    /// <summary>
    /// Функция создает новое тело документа основываясь на указанных в карточке свойствах
    /// </summary>
    [Remote, Public]
    public void CreateBodyByProperties(IOfficialDocument doc)
    {
      string pathTemplate = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("Путь к папке с шаблонами").Text;
      Aspose.Words.Document body = new Aspose.Words.Document(pathTemplate + SberContracts.PublicConstants.Purchase.TemplateDocxName);
      var typeName = doc.GetEntityTypeFullName();
      switch (typeName)
      {
        case "sberdev.SberContracts.Purchase":
          CreateBodyByPropertiesPurchase(SberContracts.Purchases.As(doc), body);
          break;
      };
      
      var gPath = Guid.NewGuid().ToString();
      string pathNewDoc = "C:TempDocs//" + gPath + ".docx";
      body.Save(pathNewDoc);
      doc.CreateVersionFrom(pathNewDoc);
      doc.Save();
    }
    
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
      
      body.Range.Replace("[KindPurchase]", GetPurchaseKind(purch.KindPurchase));
      body.Range.Replace("[CPName]", purch.Counterparty.Name);
      body.Range.Replace("[SubjectPurchase]", purch.SubjectPurchase);
      body.Range.Replace("[SubjectPurchaseGen]", purch.SubjectPurchaseGen);
      body.Range.Replace("[ForNeeds]", purch.ForNeeds);
      var nowDate = Calendar.Now;
      body.Range.Replace("[CreatedDay]", nowDate.Day.ToString());
      body.Range.Replace("[CreatedMonth]", PublicFunctions.Module.GetMonthGenetiveName(nowDate.Month));
      body.Range.Replace("[CreatedYear]", nowDate.Year.ToString());
      body.Range.Replace("[MethodPurchase]", purch.MethodPurchase);
      body.Range.Replace("[PurchaseAmount]", purch.PurchaseAmount.ToString());
      body.Range.Replace("[MVZ]", purch.MVZBaseSberDev != null ? purch.MVZBaseSberDev.Name : purch.MVPBaseSberDev.Name);
      body.Range.Replace("[ApprovalProjectContract]", purch.ApprovalProjectContract);
      body.Range.Replace("[NameTaskProject]", purch.NameTaskProject);
      body.Range.Replace("[RelizableKind]", GetRelizableKind(purch.RelizableKind));
      body.Range.Replace("[TargetPurchase]", purch.TargetPurchase);
      body.Range.Replace("[TargetAndDepartCp]", purch.TargetAndDepartCp);
      
      if (purch.Necessary != null)
        body.Range.Replace("[Necessary]", purch.Necessary);
      
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
      
      if (purch.NegotiationsDiscount.HasValue)
        body.Range.Replace("[NegotiationsDiscount]", "По итогам переговоров цена была снижена на " + purch.NegotiationsDiscount.ToString() + "%");
      
      body.Range.Replace("[PayType]", GetPaymentType(purch));
      
      string linkС = null;
      if (purch.LeadingDocument != null)
      {
        linkС = "https://directum.sberdevices.ru/DrxWeb/#/sat/card/"
          + purch.LeadingDocument.GetEntityMetadata().GetOriginal().NameGuid.ToString() + "/" + purch.LeadingDocument.Id.ToString();
        addendums.Add("Договор - " + linkС);
      }
      
      if (purch.ConcludedContractsKind == SberContracts.Purchase.ConcludedContractsKind.Yes)
        body.Range.Replace("[ConcludedContracts]", "а также с использованием информации из действующих договоров, например:\n" +
                           "Согласно " + purch.LeadingDocument.Name + " (" + linkС + ") стоимость " + purch.SubjectPurchaseGen + " составляет "
                           + purch.TotalAmount.Value.ToString() + " рублей без НДС, что соответствует среднерыночной стоимости.");
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
      
      if (purch.TotalAmount != purch.PurchaseAmount)
        body.Range.Replace("[TotalAmount]", "при этом общая стоимость договора составит " + purch.TotalAmount.Value.ToString() + (purch.VAT.Value ? "руб. с учетом НДС 20%" : "руб. без учета НДС 20%"));
      
      body.Range.Replace("[PurchaseKindExt]", GetPurchaseKindExt(purch.KindPurchase));
      body.Range.Replace("[ServiceStartDate]", purch.ServiceStartDate.Value.ToShortDateString());
      body.Range.Replace("[ServiceEndDate]", purch.ServiceEndDate.Value.ToShortDateString());
      body.Range.Replace("[InitiatorManager]", Sungero.Company.Employees.As(purch.Author).Department.Manager.Name);
      body.Range.Replace("[MVZBudgetOwner]", (purch.MVZBaseSberDev.MainMVZ != null ? purch.MVZBaseSberDev.MainMVZ.BudgetOwner.Name : (purch.MVZBaseSberDev != null ?
                                                                                                                                      purch.MVZBaseSberDev.BudgetOwner.Name : null)));
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
      ReplacePlaceholderWithTable(body, "[StagesPurchaseCollection]", CreateTableByArray(body, stages, boldRows));
      
      boldRows[1] = 0;
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
      ReplacePlaceholderWithTable(body, "[CostAnalysisCollection]", CreateTableByArray(body, costAnalysis, boldRows));
      #endregion
    }
    
    #region Вспомогателбные функции для построения документа
    
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
    
    static Aspose.Words.Tables.Table CreateTableByArray(Aspose.Words.Document doc, string[,] values, List<int> boldRows)
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

          // Создаем пустой параграф
          var paragraph = new Aspose.Words.Paragraph(doc);

          // Добавляем текст в параграф
          paragraph.AppendChild(new Run(doc, (values[row, col] == null ? "" : values[row, col])));

          // Если номер текущей строки содержится в списке boldRows, делаем текст жирным
          if (boldRows.Contains(row))
          {
            foreach (Run run in paragraph.Runs)
            {
              run.Font.Bold = true;
            }
          }

          cell.AppendChild(paragraph);
          tableRow.Cells.Add(cell);
        }
        table.AppendChild(tableRow);
      }
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
    
    string GetRelizableKind(Enumeration? kind)
    {
      if (kind.Value == SberContracts.Purchase.RelizableKind.Project)
        return "проекта";
      else
        return "задачи";
    }
    
    [Public]
    public string GetPaymentType(SberContracts.IPurchase purch)
    {
      return GetPaymentKind(purch.PaymentKind) + (purch.PrepaymentPercent != null ? " c " + purch.PrepaymentPercent.ToString() + " % авансом" : "");
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
    
    #endregion
    
    #endregion
  }
}