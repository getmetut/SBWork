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

    public virtual void FlagVTBValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      _obj.FlagAgencyScheme = !(e.NewValue ?? true);
      if (e.NewValue == true)
      {
        _obj.AgencyContract = null;
        _obj.AgencyFlagPAO = null;
        _obj.AgencyPayDate = null;
        _obj.AgencyPercent = null;
      }
    }

    public virtual void FlagAgencySchemeValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      _obj.FlagVTB = !(e.NewValue ?? true);
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