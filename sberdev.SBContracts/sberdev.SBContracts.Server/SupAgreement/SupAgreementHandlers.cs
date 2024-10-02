using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.SupAgreement;

namespace sberdev.SBContracts
{
  partial class SupAgreementServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
    }

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      if (_obj.LeadingDocument != null)
        Sungero.Custom.PublicFunctions.Module.CreateSupInLiminInContract(_obj.LeadingDocument.Id, _obj.Id);
    }
  }

  partial class SupAgreementLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      return query.Where(q => SBContracts.Contracts.Is(q) || SBContracts.SupAgreements.Is(q));
    }
  }
}