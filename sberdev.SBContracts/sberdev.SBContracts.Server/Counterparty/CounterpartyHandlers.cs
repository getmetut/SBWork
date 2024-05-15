using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Counterparty;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts
{
  partial class CounterpartyFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      var parties = query;
      if (_filter == null)
        return query;
      if (_filter.ContractsIn2024On)
      {
        var contracts = SBContracts.ContractualDocuments.GetAll().Where(c => c.ExternalApprovalState == ExternalApprovalState.Signed
                                                                        && c.InternalApprovalState == InternalApprovalState.Signed
                                                                        && (c.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Expendable
                                                                            || c.ContrTypeBaseSberDev == ContrTypeBaseSberDev.ExpendProfitSberDev)
                                                                        && c.Counterparty != null && c.Currency != null && c.Currency.Id == 1);
        var dateFrom = _filter.ContractDateRangeFrom;
        var dateTo = _filter.ContractDateRangeTo;
        
        if (dateFrom.HasValue && !dateTo.HasValue)
          contracts = contracts.Where(l => l.DocumentDate > dateFrom);
        
        if (dateTo.HasValue && !dateFrom.HasValue)
          contracts = contracts.Where(l => l.DocumentDate < dateTo);
        
        if (dateFrom.HasValue && dateTo.HasValue)
          contracts = contracts.Where(l => l.DocumentDate > dateFrom && l.DocumentDate < dateTo);
        
        contracts = contracts.Where(с =>
                                    (_filter.AmountLess100k && с.TotalAmount <= 100000) ||
                                    (_filter.AmountLess500k && с.TotalAmount > 100000 && с.TotalAmount < 500000) ||
                                    (_filter.AmountMore500k && с.TotalAmount >= 500000));
        
        var filtredParties = contracts.Select(c => c.Counterparty);
        parties = parties.Where(q => filtredParties.Contains(q));
      }
      
      if (_filter.FocusExcept)
      {
        parties = parties.Where(q => !SBContracts.Companies.As(q).ActiveLicense.HasValue);
      }
      if (_filter.FocusLeave)
      {
        parties = parties.Where(q => SBContracts.Companies.As(q).ActiveLicense.HasValue);
      }
      return parties;
    }
  }

  partial class CounterpartyServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.DiadocIsSetSberDev = false;
    }
  }

}