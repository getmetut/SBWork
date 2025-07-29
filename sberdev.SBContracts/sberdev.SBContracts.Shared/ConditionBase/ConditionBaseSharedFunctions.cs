using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ConditionBase;

namespace sberdev.SBContracts.Shared
{
  partial class ConditionBaseFunctions
  {
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.ConditionType == ConditionType.CPartyRating)
      {
        var company = GetCompanyFromOfficialDocument(document);
        if (company != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(company.CounterpartyRating == _obj.CounterpartyRating, string.Empty);
        
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
      }
      
      return base.CheckCondition(document, task);
    }
    
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckDeliveryMethod(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (document != null)
      return Sungero.Docflow.Structures.ConditionBase.ConditionResult
        .Create(_obj.DeliveryMethods.Any(d => Equals(d.DeliveryMethod, document.DeliveryMethod)), string.Empty);
      
      return base.CheckDeliveryMethod(document, task);
    }
    
    /// <summary>
    /// Получить контрагента-организацию из OfficialDocument.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Контрагент-организация.</returns>
    private static centrvd.KFIntegration.ICompany GetCompanyFromOfficialDocument(Sungero.Docflow.IOfficialDocument document)
    {
      var company = centrvd.KFIntegration.Companies.Null;
      
      var accountingDocument = Sungero.Docflow.AccountingDocumentBases.As(document);
      if (accountingDocument != null)
        company = centrvd.KFIntegration.Companies.As(accountingDocument.Counterparty);
      
      var contractualDocument = Sungero.Docflow.ContractualDocumentBases.As(document);
      if (contractualDocument != null)
        company = centrvd.KFIntegration.Companies.As(contractualDocument.Counterparty);
      
      var outgoingDocument = Sungero.Docflow.OutgoingDocumentBases.As(document);
      if (outgoingDocument != null)
        company = centrvd.KFIntegration.Companies.As(outgoingDocument.Correspondent);
      
      var incomingDocument = Sungero.Docflow.IncomingDocumentBases.As(document);
      if (incomingDocument != null)
        company = centrvd.KFIntegration.Companies.As(incomingDocument.Correspondent);
      
      return company;
    }
  }
}