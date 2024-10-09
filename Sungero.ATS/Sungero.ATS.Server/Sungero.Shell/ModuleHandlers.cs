using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ATS.Module.Shell.Server
{
  partial class ExchangeDocumentProcessingFolderHandlers
  {

    public virtual IQueryable<Sungero.CoreEntities.IRecipient> ExchangeDocumentProcessingBuchKASDevFiltering(IQueryable<Sungero.CoreEntities.IRecipient> query)
    {
      var OBrole = Roles.GetAll(r => r.Name == "Ответственный бухгалтер за Контрагентов").FirstOrDefault();
      List<Sungero.CoreEntities.IRecipient> Userlist = new List<Sungero.CoreEntities.IRecipient>();
      if (OBrole != null)
      {
        if (OBrole.RecipientLinks.Count > 0)
        {
          foreach (var elem in OBrole.RecipientLinks)
          {
            Userlist.Add(elem.Member);
          }
          query = query.Where(q => Userlist.Contains(q));
        }
      }
      return query;
    }

    public override IQueryable<Sungero.Workflow.IAssignmentBase> ExchangeDocumentProcessingDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var filtred = base.ExchangeDocumentProcessingDataQuery(query);
      if (_filter != null)
      {
        if (_filter.IncomingSberDev)
          filtred = query.Select(p => sberdev.SBContracts.ExchangeDocumentProcessingAssignments.As(p)).Where(e => sberdev.SBContracts.ExchangeDocumentProcessingTasks.As(e.Task).IsIncoming == true);
        if (_filter.OutgoingSberDev)
          filtred = query.Select(p => sberdev.SBContracts.ExchangeDocumentProcessingAssignments.As(p)).Where(e => sberdev.SBContracts.ExchangeDocumentProcessingTasks.As(e.Task).IsIncoming == false);
        
        if (_filter.BuchKASDev != null)
        {
          var listCP = sberdev.SBContracts.Counterparties.GetAll();
          var filterdata = listCP.Select(c => c.Id);
          if (_filter.NoSDev)
          {
            listCP = listCP.Where(c => c.OtvBuchSberDev != null); // Sungero.Parties.Counterparty
            if (_filter.BuchKASDev != null)
              filterdata = listCP.Where(c => c.OtvBuchSberDev.Id == _filter.BuchKASDev.Id).Select(c => c.Id);
          }
          else
          {
            filterdata = listCP.Where(c => ((c.OtvBuchSberDev.Id == _filter.BuchKASDev.Id) || (c.OtvBuchSberDev == null))).Select(c => c.Id);
          }
          
          if (filterdata.Any())
            filtred = filtred.Where(d => filterdata.Contains(sberdev.SBContracts.ExchangeDocumentProcessingTasks.As(d.Task).Counterparty.Id));
          else
            filtred = filtred.Where(k => k.Subject == "@#$%");
        }
      }
      
      
      return filtred;
    }
  }

  partial class FromKASDevFolderHandlers
  {

    public virtual IQueryable<sberdev.SBContracts.IExchangeDocument> FromKASDevDataQuery(IQueryable<sberdev.SBContracts.IExchangeDocument> query)
    {
      var ListDocKA = sberdev.SBContracts.ExchangeDocuments.GetAll(d => d.Counterparty != null); // Sungero.Parties.Counterparty
      var listCP = sberdev.SBContracts.Counterparties.GetAll(c => c.OtvBuchSberDev != null); // Sungero.Parties.Counterparty
      var filteredCounterparties = listCP.Where(c => c.OtvBuchSberDev.Login == Users.Current.Login).Select(c => c.Id);
      
      if (listCP.Any())
        ListDocKA = ListDocKA.Where(d => listCP.Contains(d.Counterparty));
      
      if (ListDocKA.Any())
      {
        if (_filter.MySDev == true)
          query = query.Where(d => filteredCounterparties.Contains(d.Counterparty.Id));
      }
      else
        query = query.Where(k => k.Name == "@#$%");
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