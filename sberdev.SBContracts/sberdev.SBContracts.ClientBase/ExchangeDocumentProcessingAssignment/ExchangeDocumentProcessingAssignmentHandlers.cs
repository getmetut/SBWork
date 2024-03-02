using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;

namespace sberdev.SBContracts
{
  partial class ExchangeDocumentProcessingAssignmentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      string newNames = PublicFunctions.ExchangeDocumentProcessingAssignment.GetDocsNames(_obj);
      if (_obj.TaskDocsNamesSberDev != newNames)
        _obj.TaskDocsNamesSberDev = newNames;
      _obj.Save();
    }
  }

}