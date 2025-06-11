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
    /// <summary>
    /// Считает сумму всех расходных договоров с контрагентом
    /// </summary>
    /// <returns></returns>
    public double CalculateExpandableTotalAmount()
    {
      var contracts =  SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                       && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                           || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)).ToList();
      
      return contracts.Sum(a => a.TotalAmount.GetValueOrDefault());
    }
    
    /// <summary>
    /// Счтает сумму расходных договоров за указанный период
    /// </summary>
    /// <param name="dateFrom"></param>
    /// <param name="dateTo"></param>
    /// <returns></returns>
    [Public]
    public double CalculateExpendableTotalAmount(Nullable<DateTime> dateFrom, Nullable<DateTime> dateTo)
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
    
    [Public]
    /// <summary>
    /// Считает сумму всех доходных договоров с контрагентом
    /// </summary>
    /// <returns></returns>
    public double CalculateProfitableTotalAmount()
    {
      var contracts =  SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                       && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable
                                                                           || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)).ToList();
      
      return contracts.Sum(a => a.TotalAmount.GetValueOrDefault());
    }
    
    /// <summary>
    /// Счтает сумму доходных договоров за указанный период
    /// </summary>
    /// <param name="dateFrom"></param>
    /// <param name="dateTo"></param>
    /// <returns></returns>
    [Public]
    public double CalculateProfitableTotalAmount(Nullable<DateTime> dateFrom, Nullable<DateTime> dateTo)
    {
      var contracts = SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                      && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable
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