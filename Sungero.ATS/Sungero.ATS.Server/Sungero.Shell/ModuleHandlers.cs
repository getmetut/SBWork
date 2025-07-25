using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ATS.Module.Shell.Server
{
  partial class OnApprovalFolderHandlers
  {

    public override IQueryable<Sungero.Workflow.IAssignmentBase> OnApprovalDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      return base.OnApprovalDataQuery(query);;
    }
  }

  partial class FilterApprovalSDevFolderHandlers
  {

    public virtual bool IsFilterApprovalSDevVisible()
    {
      if (Users.Current.IncludedIn(Roles.Administrators))
        return true;
      else
        return false;
    }

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> FilterApprovalSDevDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      if (_filter != null)
      {
        if (_filter.GoupAuthorSDev != null)
        {
          query = query.Where(a => a.Author.IncludedIn(_filter.GoupAuthorSDev));
        }
        if ((_filter.DohSDev) && (_filter.RashSDev))
        {
          //
        }
        else
        {
          if (_filter.DohSDev)
          {
            query = query.Where(a => sberdev.SBContracts.ApprovalAssignments.Is(a)).
              Where(a => (sberdev.SBContracts.AccountingDocumentBases.Is(sberdev.SBContracts.ApprovalAssignments.As(a).DocumentGroup.OfficialDocuments.FirstOrDefault())))
              .Where(a => sberdev.SBContracts.AccountingDocumentBases.As(sberdev.SBContracts.ApprovalAssignments.As(a).DocumentGroup.OfficialDocuments.FirstOrDefault())
                                                                          .ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable);
          }
          if (_filter.RashSDev)
          {
            query = query.Where(a => sberdev.SBContracts.ApprovalAssignments.Is(a)).
              Where(a => (sberdev.SBContracts.AccountingDocumentBases.Is(sberdev.SBContracts.ApprovalAssignments.As(a).DocumentGroup.OfficialDocuments.FirstOrDefault())))
              .Where(a => sberdev.SBContracts.AccountingDocumentBases.As(sberdev.SBContracts.ApprovalAssignments.As(a).DocumentGroup.OfficialDocuments.FirstOrDefault())
                                                                          .ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable);
          }
        }
      }
      
      return query;
    }
  }


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
      if (_filter != null && (_filter.IncomingSberDev || _filter.OutgoingSberDev))
      {
        bool isIncoming = _filter.IncomingSberDev;
        
        filtred = query
          .Where(q => sberdev.SBContracts.ExchangeDocumentProcessingAssignments.Is(q))
          .Select(p => sberdev.SBContracts.ExchangeDocumentProcessingAssignments.As(p))
          .Where(e => sberdev.SBContracts.ExchangeDocumentProcessingTasks.As(e.Task).IsIncoming == isIncoming);
      }
      if (_filter != null)
      {
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

    public virtual IQueryable<Sungero.CoreEntities.IGroup> ApprovalAuthorGroupATSDevFiltering(IQueryable<Sungero.CoreEntities.IGroup> query)
    {
      query = query.Where(r => r.Status == Sungero.CoreEntities.Group.Status.Active);
      query = query.Where(g => g.RecipientLinks.Any());
      return query;
    }

    public override IQueryable<Sungero.Workflow.ITask> ApprovalDataQuery(IQueryable<Sungero.Workflow.ITask> query)
    {
      query = base.ApprovalDataQuery(query);
      if (_filter != null)
      {
        if (_filter.AuthorGroupATSDev != null)
          query = query.Where(au => au.Author.IncludedIn(_filter.AuthorGroupATSDev));
        
        if (!_filter.AllATSDev)
        {
          //var DocProfit = Sungero.Docflow.ApprovalAssignments.As(cast).DocumentGroup.OfficialDocuments.FirstOrDefault().
//          if (_filter.ProfitATSDev)       // Доходный
//          {
//            List<Sungero.Docflow.IOfficialDocument> DocsProfits = new List<Sungero.Docflow.IOfficialDocument>();
//            foreach (var doc in sberdev.SBContracts.AccountingDocumentBases.GetAll(d => d.ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable).ToList())
//            {
//              if (Sungero.Docflow.OfficialDocuments.Is(doc))
//                DocsProfits.Add(Sungero.Docflow.OfficialDocuments.As(doc));
//            }
//            foreach (var doc in sberdev.SBContracts.ContractualDocuments.GetAll(d => d.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable).ToList())
//            {
//              if (Sungero.Docflow.OfficialDocuments.Is(doc))
//                DocsProfits.Add(Sungero.Docflow.OfficialDocuments.As(doc));
//            }            
//            query = query.Where(j => sberdev.SBContracts.ApprovalTasks.Is(j)).Select(j => sberdev.SBContracts.ApprovalTasks.As(j)).
//              Where(j => DocsProfits.Contains(sberdev.SBContracts.ApprovalTasks.As(j).DocumentGroup.OfficialDocuments.FirstOrDefault()));
//          }
//            
//          if (_filter.ConsumableDoATSDev) // До 5 млн. Расходный
//          {
//            List<Sungero.Docflow.IOfficialDocument> DocsConsumable = new List<Sungero.Docflow.IOfficialDocument>();
//            foreach (var doc in sberdev.SBContracts.AccountingDocumentBases.GetAll(d => d.ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable).
//                     Where(d => d.TotalAmount < 5000000).ToList())
//            {
//              if (Sungero.Docflow.OfficialDocuments.Is(doc))
//                DocsConsumable.Add(Sungero.Docflow.OfficialDocuments.As(doc));
//            }
//            foreach (var doc in sberdev.SBContracts.ContractualDocuments.GetAll(d => d.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable).
//                     Where(d => d.TotalAmount < 5000000).ToList())
//            {
//              if (Sungero.Docflow.OfficialDocuments.Is(doc))
//                DocsConsumable.Add(Sungero.Docflow.OfficialDocuments.As(doc));
//            }            
//            query = query.Where(j => sberdev.SBContracts.ApprovalTasks.Is(j)).
//              Select(j => sberdev.SBContracts.ApprovalTasks.As(j)).
//              Where(j => DocsConsumable.Contains(sberdev.SBContracts.ApprovalTasks.As(j).DocumentGroup.OfficialDocuments.FirstOrDefault()));
//          }
//          if (_filter.ConsumablAfterATSDev) // Расходный более 5 млн.
//          {
//            List<Sungero.Docflow.IOfficialDocument> DocsConsumable = new List<Sungero.Docflow.IOfficialDocument>();
//            foreach (var doc in sberdev.SBContracts.AccountingDocumentBases.GetAll(d => d.ContrTypeBaseSberDev == sberdev.SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Expendable).
//                     Where(d => d.TotalAmount >= 5000000).ToList())
//            {
//              if (Sungero.Docflow.OfficialDocuments.Is(doc))
//                DocsConsumable.Add(Sungero.Docflow.OfficialDocuments.As(doc));
//            }
//            foreach (var doc in sberdev.SBContracts.ContractualDocuments.GetAll(d => d.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable).
//                     Where(d => d.TotalAmount >= 5000000).ToList())
//            {
//              if (Sungero.Docflow.OfficialDocuments.Is(doc))
//                DocsConsumable.Add(Sungero.Docflow.OfficialDocuments.As(doc));
//            }            
//            query = query.Where(j => sberdev.SBContracts.ApprovalTasks.Is(j)).
//              Select(j => sberdev.SBContracts.ApprovalTasks.As(j)).
//              Where(j => DocsConsumable.Contains(sberdev.SBContracts.ApprovalTasks.As(j).DocumentGroup.OfficialDocuments.FirstOrDefault()));
//          }
        }
      }
      return query;
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
      return true;
    }
  }


  partial class ShellHandlers
  {
  }
}