using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom.Server
{
  partial class FolderFolderHandlers
  {

    public virtual IQueryable<Sungero.Custom.IAuthorJob> FolderDataQuery(IQueryable<Sungero.Custom.IAuthorJob> query)
    {
      return query;
    }
  }

  partial class CustomHandlers
  {
  }
}