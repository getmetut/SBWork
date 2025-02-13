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
    /// Сбрасываем статус соглаосвания (комментарий) в документе
    /// </summary>
    [Remote, Public]
    public void ClearPublicComment()
    {
      if (_obj != null && _obj.PublicCommentSberDev != null)
      {
        PublicFunctions.Module.Remote.UnblockCardByDatabase(_obj);
        _obj.PublicCommentSberDev = null;
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Функция устанавливает комментарий в документе (статус)
    /// </summary>
    [Remote]
    public void SetPublicComment(string text)
    {
      if (text != "")
      {
        _obj.PublicCommentSberDev = "Статус по задаче: " + text;
        _obj.Save();
      }
    }
    
    /// <summary>
    /// Переносит тело с подписями по ид
    /// </summary>
    [Public, Remote]
    public void TransferBodyWithSignatures(long idDoc)
    {
      var doc = Sungero.Docflow.OfficialDocuments.GetAll(d => d.Id == idDoc).First();
      TransferBody(doc);
      TransferExternalSignatures(doc);
    }
    
    /// <summary>
    /// Переносит тело документа
    /// </summary>
    /// <param name="idDoc"></param>
    [Remote, Public]
    public void TransferBody(Sungero.Docflow.IOfficialDocument doc)
    {
      Stream strmCommon = _obj.LastVersion.Body.Read();
      doc.CreateVersionFrom(strmCommon, _obj.LastVersion.AssociatedApplication.Extension);
      if (_obj.LastVersion.PublicBody.Size > 0)
      {
        Stream strmPublic = _obj.LastVersion.PublicBody.Read();
        doc.LastVersion.PublicBody.Write(strmPublic);
        strmPublic.Close();
      }
      doc.Save();
      strmCommon.Close();
    }
    
    /// <summary>
    /// Переносит внешние подписи последней версии документа
    /// </summary>
    /// <param name="id"></param>
    [Remote, Public]
    public void TransferExternalSignatures(Sungero.Docflow.IOfficialDocument doc)
    {
      var signInfos = Signatures.Get(_obj.LastVersion);
      foreach(var signInfo in signInfos)
      {
        var signaturesBytes = signInfo.GetDataSignature();
        if (signInfo.IsExternal.HasValue && signInfo.IsExternal.Value)
          Signatures.Import(doc, signInfo.SignatureType, signInfo.SignatoryFullName, signaturesBytes, signInfo.SigningDate, doc.LastVersion);
      }
    }

    /// <summary>
    /// Обнуляет флаг ручной проверки
    /// </summary>
    [Remote, Public]
    public void NullingManuallyChecked()
    {
      _obj.InternalApprovalState = null;
      _obj.ExternalApprovalState = null;
    }
    
  }
}