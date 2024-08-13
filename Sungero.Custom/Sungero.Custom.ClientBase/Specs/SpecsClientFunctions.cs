using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.Specs;

namespace Sungero.Custom.Client
{
  partial class SpecsFunctions
  {

    /// <summary>
    /// Отчищает таблицу калькуляции
    /// </summary>
    public void ClearCalculation()
    {
      _obj.CalculationBaseSberDev.Clear();
    }
    
    /// <summary>
    /// Фуннкция для кнопки распеределения
    /// </summary>
    public void DistributeTo()
    {
      if (_obj.CalculationDistributeBaseSberDev != null)
        sberdev.SberContracts.PublicFunctions.Module.Remote.DistributeToCalculation(_obj);
      else
        Dialogs.ShowMessage(sberdev.SBContracts.Resources.NotSelectedDistribution, MessageType.Error);
    }

  }
}