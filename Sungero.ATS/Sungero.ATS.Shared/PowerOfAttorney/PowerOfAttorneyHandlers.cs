using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.PowerOfAttorney;

namespace Sungero.ATS
{
  partial class PowerOfAttorneySharedHandlers
  {

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      _obj.Subject = e.NewValue.ToString();
    }

    public override void IssuedToChanged(Sungero.Docflow.Shared.PowerOfAttorneyBaseIssuedToChangedEventArgs e)
    {
      base.IssuedToChanged(e);
      if (e.NewValue != null)
      {
        if (e.NewValue.JobTitle != null)
          _obj.JobTitleSDev = e.NewValue.JobTitle.Name.ToString();
      }
      else
        _obj.JobTitleSDev = null;
    }

  }
}