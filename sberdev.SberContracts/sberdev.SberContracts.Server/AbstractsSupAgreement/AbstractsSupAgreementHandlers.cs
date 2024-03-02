using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AbstractsSupAgreement;

namespace sberdev.SberContracts
{
  partial class AbstractsSupAgreementLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      
      if (_obj.Counterparty != null)
        query = query.Where(c => Equals(c.Counterparty, _obj.Counterparty));
      
      query = query.Where(c => !Equals(c.LifeCycleState, Sungero.Contracts.ContractBase.LifeCycleState.Obsolete) &&
                          !Equals(c.LifeCycleState, Sungero.Contracts.ContractBase.LifeCycleState.Closed) &&
                          !Equals(c.LifeCycleState, Sungero.Contracts.ContractBase.LifeCycleState.Terminated) &&
                          Equals(c.ContrTypeBaseSberDev, ContrTypeBaseSberDev.Expendable));

      // В процессе верификации при смене типа с договора на доп. соглашение
      // сущность может быть еще договором (до первого сохранения после смены),
      // и в этом случае не нужно предоставлять ее для выбора в качестве ведущего документа.
      query = query.Where(c => c.Id != _obj.Id);
      
      return query;
    }
  }

}