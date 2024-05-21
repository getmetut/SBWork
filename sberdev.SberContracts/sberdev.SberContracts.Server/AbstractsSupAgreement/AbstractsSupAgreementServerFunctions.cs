using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AbstractsSupAgreement;

namespace sberdev.SberContracts.Server
{
  partial class AbstractsSupAgreementFunctions
  {
    /// <summary>
    /// Получить дубли гарантийных писем.
    /// </summary>
    /// <param name="aSupAgr">Доп. соглашение.</param>
    /// <param name="businessUnit">НОР.</param>
    /// <param name="registrationNumber">Рег. номер.</param>
    /// <param name="registrationDate">Дата регистрации.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="contract">Договор.</param>
    /// <returns>Дубли.</returns>
    [Remote]
    public static IQueryable<IAbstractsSupAgreement> GetDuplicates(sberdev.SberContracts.IAbstractsSupAgreement aSupAgr,
                                                             Sungero.Company.IBusinessUnit businessUnit,
                                                             string registrationNumber,
                                                             DateTime? registrationDate,
                                                             Sungero.Parties.ICounterparty counterparty,
                                                             SBContracts.IContractualDocument contract)
    {
      return AbstractsSupAgreements.GetAll()
        .Where(l => Equals(aSupAgr.DocumentKind, l.DocumentKind))
        .Where(l => Equals(businessUnit, l.BusinessUnit))
        .Where(l => registrationDate == l.RegistrationDate)
        .Where(l => registrationNumber == l.RegistrationNumber)
        .Where(l => Equals(counterparty, l.Counterparty))
        .Where(l => Equals(contract, l.LeadingDocument))
        .Where(l => !Equals(aSupAgr, l));
    }
  }
}