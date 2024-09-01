using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ATS.Module.Shell.Server
{
  partial class FromKASDevFolderHandlers
  {

    public virtual IQueryable<sberdev.SBContracts.IExchangeDocument> FromKASDevDataQuery(IQueryable<sberdev.SBContracts.IExchangeDocument> query)
    {
      if (_filter != null)
      {
        var ListDocKA = sberdev.SBContracts.ExchangeDocuments.GetAll(d => d.Counterparty != null).Where(d => sberdev.SBContracts.Counterparties.Get(d.Counterparty.Id).OtvBuchSDevSDev != null).
          Where(d => sberdev.SBContracts.Counterparties.Get(d.Counterparty.Id).OtvBuchSDevSDev.Login == Users.Current.Login).ToList();
          
        if (_filter.MySDev == true)
          query = query.Where(k => ListDocKA.Contains(k));
      }
      return query;
    }
  }

  partial class ApprovalFolderHandlers
  {

    public override IQueryable<Sungero.Workflow.ITask> ApprovalDataQuery(IQueryable<Sungero.Workflow.ITask> query)
    {
      return base.ApprovalDataQuery(query);
    }

    public virtual IQueryable<Sungero.Parties.ICounterparty> ApprovalCounterpartySDevFiltering(IQueryable<Sungero.Parties.ICounterparty> query)
    {
      query = query.Where(c => c.Status == Sungero.Parties.Counterparty.Status.Active);
      return query;
    }
  }

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