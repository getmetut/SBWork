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
        var invoice = _obj.InvoiceSberDev;
        if (invoice.TotalAmount != null)
        {
            _obj.TotalAmount = invoice.TotalAmount;
            _obj.Currency = invoice.Currency;
          
          Dialogs.ShowMessage("Информация обновлена!");
        }
        else
          Dialogs.ShowMessage("Сумма в счете нулевая!");
      }
      else
        Dialogs.ShowMessage("Не указан входящий счет!");
    }
  }
}