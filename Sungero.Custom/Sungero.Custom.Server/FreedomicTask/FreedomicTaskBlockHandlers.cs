using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using Sungero.Custom.FreedomicTask;

namespace Sungero.Custom.Server.FreedomicTaskBlocks
{
  partial class BaseFreedomHandlers
  {

    public virtual void BaseFreedomStartAssignment(Sungero.Custom.IBaseFreedom assignment)
    {
      foreach (var elem in _obj.OtherAttachment.ElectronicDocuments)
      {
        if (Sungero.Contracts.ContractualDocuments.Is(elem))
        {
          var job = assignment.AssignyEmpl.AddNew();
          job.DogDocument = Sungero.Contracts.ContractualDocuments.As(elem);
          job.Employee = _obj.Employee;
        }
      }
    }
  }


  partial class SendNotificationHandlers
  {

    public virtual void SendNotificationExecute()
    {
      if (_obj.AssignyEmpl.Count > 0)
      {
        foreach (var str in _obj.AssignyEmpl)
        {
          var mintask = Custom.NotiffDocContracts.Create();
          mintask.Employee = str.Employee;
          mintask.BaseAttachment.ElectronicDocuments.Add(str.DogDocument);
          mintask.Subject = "Заканчивается срок действия договора: " + str.DogDocument.Name;
          mintask.Start();
        }
      }
    }
  }

}