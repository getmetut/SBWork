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
                                                                        && c.Counterparty != null && c.Currency != null && c.Currency.Id == 1);
        var dateFrom = _filter.ContractDateRangeFrom;
        var dateTo = _filter.ContractDateRangeTo;
        
        var allCounterparties = contracts.Select(c => c.Counterparty).Distinct().ToList();
        var filteredParties = allCounterparties.Where(cp =>
                                                      {
                                                        var totalAmount = PublicFunctions.Counterparty.CalculateTotalAmount(SBContracts.Counterparties.As(cp), dateFrom, dateTo);
                                                        return (_filter.AmountLess100k && totalAmount <= 100000) ||
                                                          (_filter.AmountLess500k && totalAmount > 100000 && totalAmount < 500000) ||
                                                          (_filter.AmountMore500k && totalAmount >= 500000);
                                                      }).ToList();

        parties = parties.Where(q => filteredParties.Contains(q));
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