using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts
{
  partial class AppProductPurchaseNDAPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> NDAFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparty != null)
      {
        query = query.Where(g => g.Counterparty == _obj.Counterparty);
        _obj.State.Properties.Counterparty.HighlightColor = Colors.Empty;
      }
      else
      {
        query = query.Where(g => g.Name == "@#$%^&^%");
        _obj.State.Properties.Counterparty.HighlightColor = Colors.Common.LightOrange;
      }
      return query;
    }
  }

  partial class AppProductPurchaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FlagNDA = false;
      _obj.FlagVAT = false;
      _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Expendable;
      _obj.SelectedCounterparty1 = false;
      _obj.SelectedCounterparty2 = false;
      _obj.SelectedCounterparty3 = false;
      _obj.SelectedCounterparty4 = false;
      _obj.SelectedCounterparty5 = false;
      _obj.SelectedCounterparty6 = false;
      _obj.SelectedCounterparty7 = false;
    }
  }

}