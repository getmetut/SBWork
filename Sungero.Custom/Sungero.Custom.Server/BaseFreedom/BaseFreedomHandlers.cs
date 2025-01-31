using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.BaseFreedom;

namespace Sungero.Custom
{
  partial class BaseFreedomAssignyEmplEmployeePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AssignyEmplEmployeeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      _obj.DogDocument = Sungero.Contracts.ContractualDocuments.As(_obj.BaseFreedom.OtherAttachment.ElectronicDocuments.FirstOrDefault());
      return query;
    }
  }

}