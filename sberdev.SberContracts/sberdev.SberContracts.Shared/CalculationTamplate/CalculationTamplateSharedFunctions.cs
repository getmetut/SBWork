using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CalculationTamplate;

namespace sberdev.SberContracts.Shared
{
  partial class CalculationTamplateFunctions
  {
    public void TableAccess()
    {
      if (_obj.CalculationFlag.HasValue)
      {
        if (_obj.CalculationFlag.Value == CalculationFlag.Percent)
        {
          _obj.State.Properties.ProdCollection.Properties.Percent.IsRequired = true;
          _obj.State.Properties.ProdCollection.Properties.Percent.IsVisible = true;
          _obj.State.Properties.ProdCollection.Properties.Absolute.IsRequired = false;
          _obj.State.Properties.ProdCollection.Properties.Absolute.IsVisible = false;
          _obj.State.Properties.Amount.IsRequired = false;
          _obj.State.Properties.Amount.IsVisible = false;
        }
        else
        {
          _obj.State.Properties.ProdCollection.Properties.Percent.IsRequired = false;
          _obj.State.Properties.ProdCollection.Properties.Percent.IsVisible = false;
          _obj.State.Properties.ProdCollection.Properties.Absolute.IsRequired = true;
          _obj.State.Properties.ProdCollection.Properties.Absolute.IsVisible = true;
          _obj.State.Properties.Amount.IsRequired = true;
          _obj.State.Properties.Amount.IsVisible = true;
        }
      }
      else
      {
        _obj.State.Properties.ProdCollection.Properties.Percent.IsRequired = false;
        _obj.State.Properties.ProdCollection.Properties.Percent.IsVisible = false;
        _obj.State.Properties.ProdCollection.Properties.Absolute.IsRequired = false;
        _obj.State.Properties.ProdCollection.Properties.Absolute.IsVisible = false;
        _obj.State.Properties.Amount.IsRequired = false;
        _obj.State.Properties.Amount.IsVisible = false;
      }
    }
  }
}