using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CheckDocumentSignTask;

namespace sberdev.SberContracts.Server
{
  partial class CheckDocumentSignTaskFunctions
  {
    [Remote]
    public string GetFirstAttachment()
    {
      var attach = SBContracts.OfficialDocuments.As(_obj.Attachments.FirstOrDefault());
      return attach.Name;
    }

    [Remote]
    public string GetFirstAllAttachment()
    {
      var attach = SBContracts.OfficialDocuments.As(_obj.AllAttachments.FirstOrDefault());
      return attach.Name;
    }
  }
}