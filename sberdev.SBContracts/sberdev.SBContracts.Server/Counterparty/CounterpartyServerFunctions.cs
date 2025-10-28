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

    /// <summary>
    /// Получить первую запись в истории: Создание
    /// </summary>
    [Public]
    public Sungero.Domain.Shared.IHistory GetFirstHistory()
    {
      return _obj.History.GetAll().Where(h => h.Action == Sungero.CoreEntities.History.Action.Create).FirstOrDefault();
    }
    
    #region Подсчет сумм договоров по КА
    [Public]
    /// <summary>
    /// Считает сумму всех расходных договоров с контрагентом
    /// </summary>
    /// <returns></returns>
    public double CalculateExpandableTotalAmount()
    {
      var contracts =  SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                       && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                           || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
                                                                      && (c.LifeCycleState != SBContracts.Contract.LifeCycleState.Terminated && c.LifeCycleState != SBContracts.Contract.LifeCycleState.Obsolete)).ToList();
      
      return contracts.Sum(a => a.TotalAmount.GetValueOrDefault());
    }
    
    /// <summary>
    /// Считает сумму расходных договоров за указанный период
    /// </summary>
    /// <param name="dateFrom"></param>
    /// <param name="dateTo"></param>
    /// <returns></returns>
    [Public]
    public double CalculateExpendableTotalAmount(Nullable<DateTime> dateFrom, Nullable<DateTime> dateTo)
    {
      var contracts = SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                                      && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                          || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
                                                                      && (c.LifeCycleState != SBContracts.Contract.LifeCycleState.Terminated && c.LifeCycleState != SBContracts.Contract.LifeCycleState.Obsolete)).ToList();
                                   
      if (dateFrom.HasValue && !dateTo.HasValue)
        contracts = contracts.Where(l => l.Created > dateFrom).ToList();
      
      if (dateTo.HasValue && !dateFrom.HasValue)
        contracts = contracts.Where(l => l.Created < dateTo).ToList();
      
      if (dateFrom.HasValue && dateTo.HasValue)
        contracts = contracts.Where(l => l.Created > dateFrom && l.DocumentDate < dateTo).ToList();
      
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
                                                                           || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
                                                                      && (c.LifeCycleState != SBContracts.Contract.LifeCycleState.Terminated && c.LifeCycleState != SBContracts.Contract.LifeCycleState.Obsolete)).ToList();
      
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
                                                                          || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
                                                                      && (c.LifeCycleState != SBContracts.Contract.LifeCycleState.Terminated && c.LifeCycleState != SBContracts.Contract.LifeCycleState.Obsolete)).ToList();
      if (dateFrom.HasValue && !dateTo.HasValue)
        contracts = contracts.Where(l => l.Created > dateFrom).ToList();
      
      if (dateTo.HasValue && !dateFrom.HasValue)
        contracts = contracts.Where(l => l.Created < dateTo).ToList();
      
      if (dateFrom.HasValue && dateTo.HasValue)
        contracts = contracts.Where(l => l.Created > dateFrom && l.DocumentDate < dateTo).ToList();
      
      return contracts.Sum(a => a.TotalAmount.GetValueOrDefault());
    }
    #endregion

    [Public]
    /// <summary>
    /// Устанавливает значение в поле ManualChecking
    /// </summary>       
    public void SetManualCheckingValue(bool value)
    {
      if (_obj.ManualCheckingSberDev != value)
      {
        SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(_obj);
        _obj.ManualCheckingSberDev = value;
        _obj.Save();
      }
    }
  }
}