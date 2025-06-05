using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractStatement;

namespace sberdev.SBContracts.Client
{
  partial class ContractStatementFunctions
  {

    /// <summary>
    /// Выровнять сумму по счету
    /// </summary>   
    [Public]
    public void RefreshTotalAmouInInvoice()
    {
      if (_obj.InvoiceSberDev != null)
      {
        if (_obj.InvoiceSberDev.TotalAmount != null)
        {
          if (_obj.InvoiceSberDev.TotalAmount.Value != _obj.TotalAmount.Value)
            _obj.InvoiceSberDev.TotalAmount = _obj.TotalAmount;
          if (_obj.InvoiceSberDev.Currency != _obj.Currency)
            _obj.Currency = _obj.Currency;
        }
      }
    }
  }
}