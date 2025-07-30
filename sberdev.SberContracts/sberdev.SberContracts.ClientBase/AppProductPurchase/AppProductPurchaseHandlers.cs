using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts
{
  partial class AppProductPurchaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
    }

    public virtual void PaymentMethodValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (e.NewValue != PaymentMethod.Agent)
      {
        _obj.AgencyContract = null;
        _obj.AgencyFlagPAO = null;
        _obj.AgencyPayDate = null;
        _obj.AgencyPercent = null;
      }
    }
    
    public virtual void CpNumberValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      // вызов события обновления формы
    }

    public virtual void AgencyPercentValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue > 100)
        e.AddError(sberdev.SberContracts.AppProductPurchases.Resources.DepsitError);
    }

    public virtual void DepositValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue > 100)
        e.AddError(sberdev.SberContracts.AppProductPurchases.Resources.DepsitError);
    }

  }
}