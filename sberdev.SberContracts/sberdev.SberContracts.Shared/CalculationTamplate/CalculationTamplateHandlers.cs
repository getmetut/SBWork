using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CalculationTamplate;

namespace sberdev.SberContracts
{
  partial class CalculationTamplateSharedHandlers
  {

    public virtual void CalculationFlagChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue == CalculationFlag.Absolute)
        {
          if (_obj.ProdCollection.Any())
            foreach (var prod in _obj.ProdCollection)
          {
            prod.Percent = null;
          }
        }
        if (e.NewValue == CalculationFlag.Percent)
        {
          _obj.Amount = null;
          if (_obj.ProdCollection.Any())
            foreach (var prod in _obj.ProdCollection)
          {
            prod.Absolute = null;
          }
        }
        
        if (e.NewValue == null)
        {
          _obj.Amount = null;
          if (_obj.ProdCollection.Any())
            foreach (var prod in _obj.ProdCollection)
          {
            prod.Absolute = null;
            prod.Percent = null;
          }
        }
      }
    }

    public virtual void ProdCollectionChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      double? amount = 0;
      if (_obj.ProdCollection.Any())
      {
        if (_obj.CalculationFlag == CalculationFlag.Absolute)
        {
          foreach (var prop in _obj.ProdCollection)
            amount += prop.Absolute.HasValue ? prop.Absolute.Value : 0;
          _obj.CalculationResidualAmount = _obj.Amount - Math.Round(amount.Value, 2);
        }
        else
        {
          foreach (var prop in _obj.ProdCollection)
            amount += prop.Percent.HasValue ? prop.Percent.Value : 0;
          _obj.CalculationResidualAmount = 100 - Math.Round(amount.Value, 2);
        }
      }
    }
  }


}