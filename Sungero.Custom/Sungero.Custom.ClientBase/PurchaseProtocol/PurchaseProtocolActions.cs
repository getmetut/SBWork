using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PurchaseProtocol;

namespace Sungero.Custom.Client
{
  partial class PurchaseProtocolActions
  {
    public virtual void SendToTM(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      var Task = SDev.BPCustom.ProtocolPurchTasks.Create();
      Task.BaseAttachment.PurchaseProtocols.Add(_obj);
      if (_obj.HasRelations)
      {
        foreach (var doc in _obj.Relations.GetRelated())
        {
          Task.OtherAttachment.ElectronicDocuments.Add(doc);
        }
      }
      Task.Subject = "Согласование " + _obj.Name;
      Task.Show();
    }

    public virtual bool CanSendToTM(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}