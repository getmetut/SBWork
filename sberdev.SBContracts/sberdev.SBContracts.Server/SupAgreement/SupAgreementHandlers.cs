using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts
{
  partial class SupAgreementLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      return query.Where(q => SBContracts.Contracts.Is(q) || SBContracts.SupAgreements.Is(q));
    }
  }
}