using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;
using System.IO;

namespace sberdev.SBContracts.Server
{
  partial class OfficialDocumentFunctions
  {

    /// <summary>
    /// Переносит тело с подписями по ид
    /// </summary>   
    [Public, Remote]
    public void TransferBody(int idDoc)
    {
          var doc = Sungero.Content.ElectronicDocuments.GetAll(d => d.Id == idDoc).First();
          Stream strmCommon = _obj.LastVersion.Body.Read();
          Stream strmPublic = _obj.LastVersion.PublicBody.Read();
          doc.CreateVersionFrom(strmCommon, _obj.LastVersion.AssociatedApplication.Extension);
          doc.LastVersion.PublicBody.Write(strmPublic);
          doc.Save();
          var signInfos = Signatures.Get(_obj.LastVersion);
          foreach(var signInfo in signInfos)
          {
            var signaturesBytes = signInfo.GetDataSignature();
            Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, signInfo.SigningDate, doc.LastVersion);
          }
          strmCommon.Close();
          strmPublic.Close();
    }

    /// <summary>
    /// Обнуляет флаг ручной проверки
    /// </summary>  
    [Remote, Public]    
    public void NullingManuallyChecked()
    {
      _obj.ManuallyCheckedSberDev = null;
    }
      
  }
}