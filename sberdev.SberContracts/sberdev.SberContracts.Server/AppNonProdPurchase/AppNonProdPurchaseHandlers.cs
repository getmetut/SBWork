using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts
{
  partial class AppNonProdPurchaseServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      _obj.State.Properties.Counterparty.IsRequired = false;
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
        if (_obj.CalculationAmountBaseSberDev > 500000)
          e.AddError("Сумма закупки превышает 500 тыс. руб. Пожалуйста, оформите заявку через портал Сорсинга https://tasks.sberdevices.ru/servicedesk/customer/portal/47");
    }
  }

}