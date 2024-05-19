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
      return SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                             && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                 || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)).Sum(a => a.TotalAmount.Value);
    }
    [Public]
    public double CalculateTotalAmount(DateTime from, DateTime to)
    {
      return SBContracts.ContractualDocuments.GetAll().Where(c => c.Counterparty == _obj && c.TotalAmount.HasValue
                                                             && c.DocumentDate >= from && c.DocumentDate <= to 
                                                             && (c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable
                                                                 || c.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)).Sum(a => a.TotalAmount.Value);
    }
  }
}