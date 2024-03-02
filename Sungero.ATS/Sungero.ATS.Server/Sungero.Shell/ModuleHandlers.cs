using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ATS.Module.Shell.Server
{
  partial class NoticesSDevFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.INotice> NoticesSDevDataQuery(IQueryable<Sungero.Workflow.INotice> query)
    {
      // Фильтр по статусу.
      if (_filter == null)
        return query;
      
      if (_filter.OnlyReadSDev)
        return query.Where(n => n.IsRead != true);
      else
        return query;
    }
  }

  partial class NoticesFolderHandlers
  {

    public override bool IsNoticesVisible()
    {
      return false; 
    }
  }


  partial class ShellHandlers
  {
  }
}