using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SDev.BPCustom.ContractualTask;

namespace SDev.BPCustom
{
  partial class ContractualTaskServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      var Doc = _obj.BaseAttachments.ContractualDocuments.FirstOrDefault();
      if (Doc != null)
      {
        if (Doc.HasRelations)
        {
          foreach (var sdoc in Doc.Relations.GetRelated())
          {
            _obj.OtherAttachments.ElectronicDocuments.Add(sdoc);
          }
        }
      }
    }
  }

}