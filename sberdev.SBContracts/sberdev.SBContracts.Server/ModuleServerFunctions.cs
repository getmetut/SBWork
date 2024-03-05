using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts;
using System.Net;
using Aspose.Words;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace sberdev.SBContracts.Server
{
  public class ModuleFunctions
  {
    
    #region Разные геты
    
    /// <summary>
    /// Записать ИД документа в метаданные документа
    /// </summary>
    [Public, Remote]
    public void SetMetadataID(Sungero.Docflow.IOfficialDocument doc)
    {
      /*
      POIFSFileSystem fs = new POIFSFileSystem();
      DirectoryEntry dir = fs.Root;
      DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
      CustomProperties customProperties = dsi.CustomProperties;
      if (customProperties != null && customProperties.ContainsKey("DirectumID"))
      {
        return;
      }
      else
      {
        customProperties = new CustomProperties();
        customProperties.Put("DirectumID", doc.Id.ToString());
        dsi.CustomProperties = customProperties;
        dsi.Write(dir, DocumentSummaryInformation.DEFAULT_STREAM_NAME);
        doc.LastVersion.Export("C:\\Temp\\doc.doc");
        FileStream output = new FileStream("C:\\Temp\\doc.doc", FileMode.OpenOrCreate);
        fs.WriteFileSystem(output);
        doc.LastVersion.Body.Write(output);
        output.Close();
        
        doc.Save();
      }*/
      if (!doc.HasVersions)
        return;
      string ext = doc.LastVersion.AssociatedApplication.Extension;
      if (ext != "doc" && ext != "docx" && ext != "pdf")
        return;
      string guid = Guid.NewGuid().ToString();
      string path = "C:\\TempDocs\\docDirectum" + guid + "." + ext;
      string tempPath = "C:\\TempDocs\\docDirectumTemp" + guid + "." + ext;
      doc.LastVersion.Export(path);/*
      using (Stream strmCommon = doc.LastVersion.Body.Read())
      {
        if (ext == "doc" || ext == "docx")
        {
          Aspose.Words.Document docAsp = new Aspose.Words.Document(path);
          var props = docAsp.CustomDocumentProperties;
          var prop = props.FirstOrDefault(p => p.Name == "DirectumID");
          if (prop == null)
          {
            props.Add("DirectumID", doc.Id.ToString());
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
          doc.LastVersion.Import(tempPath);
          doc.Save();
        }
      }
      
      
      using (Stream strmCommon = doc.LastVersion.Body.Read())
      {*/
        if (ext == "doc" || ext == "docx")
        {
          Aspose.Words.Document docAsp = new Aspose.Words.Document(path);
          var props = docAsp.CustomDocumentProperties;
          var prop = props.FirstOrDefault(p => p.Name == "DirectumID");
          if (prop == null)
          {
            props.Add("DirectumID", doc.Id.ToString());
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
          doc.LastVersion.Import(tempPath);
          doc.Save();
        }
     // }
    }
    
    /// <summary>
    /// Получить ИД документа из метаданных документа
    /// </summary>
    [Public, Remote]
    public string GetMetadataID(Sungero.Docflow.IOfficialDocument doc)
    {
      /*
      using (Stream strmCommon = doc.LastVersion.Body.Read())
      {
        POIFSFileSystem fs = new POIFSFileSystem(strmCommon);
        DirectoryEntry dir = fs.Root;
        DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        CustomProperties customProperties = dsi.CustomProperties;
        if (customProperties != null && customProperties.ContainsKey("DirectumID"))
        {
          Property idProperty = (Property)dsi.CustomProperties["DirectumID"];
          return (double?)idProperty.Value;
        }
        else
          return null;
      }*/
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
    /// Функция проверяет согласование документа. Возвращает true,
    /// если есть согласование от начальника данного подразделения.
    /// <param name="document">Документ.</param>
    /// <param name="isNotNeedValid">Необходимость проверки валидности.</param>
    /// </summary>
    [Public]
    public bool CheckDepartmentApproval(SBContracts.IOfficialDocument document, Sungero.CoreEntities.IGroup depart)
    {
      bool flag = false;
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
    /// Функция проверяет подпись контрактного документа. Возвращает true если есть внешняя от Нашей организации или
    /// внутренняя от одного человека из группы "Обязательные подписанты счета перед согласованием" у документа.
    /// <param name="document">Документ.</param>
    /// <param name="isNotNeedValid">Необходимость проверки валидности.</param>
    /// </summary>
    [Public]
    public bool CheckSpecialGroupSignatureContractual(SBContracts.IOfficialDocument document, bool isNeedValid)
    {
      bool flag = false;
      var signatures = Signatures.Get(document.LastVersion);
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
              var certificateInfo = Sungero.Docflow.PublicFunctions.Module.GetSignatureCertificateInfo(sign.GetDataSignature());
              string tin = SBContracts.PublicFunctions.Module.Remote.ParseCertificateSubjectOnlyOrgTIN(certificateInfo.SubjectInfo);
              if (Equals(tin, contractual.BusinessUnit.TIN) && (sign.IsValid || !isNeedValid))
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

    

    #endregion
    
    #region Сертификаты

    
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
  }
}