using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;

namespace sberdev.SBContracts
{
  partial class AccountingDocumentBaseClientHandlers
  {

    public virtual void ContrTypeBaseSberDevValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
    if (e.NewValue == ContrTypeBaseSberDev.Expendable)
      _obj.MVPBaseSberDev = null;
    if (e.NewValue == ContrTypeBaseSberDev.Profitable)
      _obj.MVZBaseSberDev = null;
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.AccountingDocumentBase.SetPropertiesAccess(_obj);
    }

  }
}