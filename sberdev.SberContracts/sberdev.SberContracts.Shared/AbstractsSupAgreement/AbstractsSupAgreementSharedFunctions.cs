using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AbstractsSupAgreement;

namespace sberdev.SberContracts.Shared
{
  partial class AbstractsSupAgreementFunctions
  {
    /// <summary>
    /// Проверить доп. соглашение на дубли.
    /// </summary>
    /// <param name="guaLetter">Доп. соглашение.</param>
    /// <param name="businessUnit">НОР.</param>
    /// <param name="registrationNumber">Рег. номер.</param>
    /// <param name="registrationDate">Дата регистрации.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="contract">Договор.</param>
    /// <returns>Признак дублей.</returns>
    public static bool HaveDuplicates(IAbstractsSupAgreement aSupAgr,
                                      Sungero.Company.IBusinessUnit businessUnit,
                                      string registrationNumber,
                                      DateTime? registrationDate,
                                      Sungero.Parties.ICounterparty counterparty,
                                      SBContracts.IContractualDocument contract)
    {
      if (aSupAgr == null ||
          businessUnit == null ||
          string.IsNullOrWhiteSpace(registrationNumber) ||
          registrationDate == null ||
          counterparty == null ||
          contract == null)
        return false;
      
      return Functions.AbstractsSupAgreement.Remote.GetDuplicates(aSupAgr,
                                                                  businessUnit,
                                                                  registrationNumber,
                                                                  registrationDate,
                                                                  counterparty,
                                                                  contract)
        .Any();
    }
    
    public override void SetPropertiesAccess()
    {
      base.SetRequiredProperties();
      
      _obj.State.Properties.PurchComNumberSberDev.IsRequired = false;
    }
  }
}