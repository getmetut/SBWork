using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ExchangeDocumentProcessingAssignmentFunctions
  {
    public override bool ShowForwardingDialog()
    {
      var responsibles = SberDev.ExchDocDistrib.PublicFunctions.Module.GetBoxResponsibleEmployees(_obj.Box);
      var currentEmp = Sungero.Company.Employees.As(_obj.Performer);
      if (responsibles.Contains(currentEmp))
        return SberDev.ExchDocDistrib.PublicFunctions.Module.ShowPrefillForwardDialog(_obj);
      else
        return base.ShowForwardingDialog();
    }
    
  }
}