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
        if (Doc.OurSignatory == null)
          e.AddError("В исходном документе Не указан подписант от нашей организации - внесите его и повторите запуск на согласование!");
        else
          _obj.Signer = Doc.OurSignatory;
        
        if (Doc.HasRelations)
        {
          foreach (var sdoc in Doc.Relations.GetRelated())
          {
            _obj.OtherAttachments.ElectronicDocuments.Add(sdoc);
          }
        }
      }
      
      if (_obj.Signer == null)
      {
        _obj.State.Properties.Signer.HighlightColor = Colors.Common.Red;
        e.AddError("Перед стартом задачи внесите подписанта данного документа!");
      }
    }
  }

}