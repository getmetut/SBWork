using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AbstractsSupAgreement;

namespace sberdev.SberContracts
{
  partial class AbstractsSupAgreementSharedHandlers
  {

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      if (Equals(e.NewValue, e.OldValue))
        return;
      FillName();
    }

    public virtual void AddendumDocumentChanged(sberdev.SberContracts.Shared.AbstractsSupAgreementAddendumDocumentChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

  }
}