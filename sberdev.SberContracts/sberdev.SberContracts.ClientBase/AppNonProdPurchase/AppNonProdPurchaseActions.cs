using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts.Client
{
  partial class AppNonProdPurchaseActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
        if (_obj.CalculationAmountBaseSberDev > 500000)
          e.AddError("Сумма закупки превышает 500 тыс. руб. Пожалуйста, оформите заявку через портал Сорсинга https://tasks.sberdevices.ru/servicedesk/customer/portal/47");
          
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.State.Properties.Counterparty.IsRequired = false;
      base.Register(e);
    }

    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRegister(e);
    }

  }

}