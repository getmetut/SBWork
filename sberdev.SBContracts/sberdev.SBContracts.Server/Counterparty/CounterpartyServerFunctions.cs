using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Counterparty;

namespace sberdev.SBContracts.Server
{
  partial class CounterpartyFunctions
  {
    [Public]
    public double CalculateTotalAmount()
    {
      var contracts =  SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                       && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                           || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)).ToList();
      
      return contracts.Sum(a => a.TotalAmount.GetValueOrDefault());
    }
    [Public]
    public double CalculateTotalAmount(Nullable<DateTime> dateFrom, Nullable<DateTime> dateTo)
    {
      var contracts = SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                      && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                          || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)).ToList();
      if (dateFrom.HasValue && !dateTo.HasValue)
        contracts = contracts.Where(l => l.DocumentDate > dateFrom).ToList();
      
      if (dateTo.HasValue && !dateFrom.HasValue)
        contracts = contracts.Where(l => l.DocumentDate < dateTo).ToList();
      
      if (dateFrom.HasValue && dateTo.HasValue)
        contracts = contracts.Where(l => l.DocumentDate > dateFrom && l.DocumentDate < dateTo).ToList();
      
      return contracts.Sum(a => a.TotalAmount.GetValueOrDefault());
    }
  }
}