using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using SberDev.ExchDocDistrib.LogEntry;

namespace SberDev.ExchDocDistrib.Shared
{
  partial class LogEntryFunctions
  {

    /// <summary>
    /// Автозаполнение имени
    /// </summary>
    public virtual void FillName()
    {
      var boxResp = Functions.Module.GetBoxResponsibleEmployee(_obj.Task);
      if (_obj.IsRedistribution == true)
        _obj.Action = String.Format("{0} -> {1}",
                                  Sungero.Company.PublicFunctions.Employee.GetShortName(_obj.DistributedTo, true),
                                  Sungero.Company.PublicFunctions.Employee.GetShortName(boxResp, true));
      else
        _obj.Action = String.Format("{0} -> {1}",
                                  sberdev.SBContracts.Companies.As(_obj.Counterparty).Name1CSberDev,
                                  Sungero.Company.PublicFunctions.Employee.GetShortName(_obj.DistributedTo, true));
      
      if (_obj.IsAccounting == true)
        _obj.Action = _obj.Action + "; финансовый";
      
      if (_obj.IsAuto == true)
        _obj.Action = _obj.Action + "; авто";
    }

  }
}