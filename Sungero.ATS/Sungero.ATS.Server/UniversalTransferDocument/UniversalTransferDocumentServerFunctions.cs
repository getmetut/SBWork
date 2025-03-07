using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.UniversalTransferDocument;

namespace Sungero.ATS.Server
{
  partial class UniversalTransferDocumentFunctions
  {

    /// <summary>
    /// Функция получения головной организации по ИНН
    /// </summary> 
    [Public, Remote]
    public sberdev.SBContracts.ICompany GetHeadCompanies(Sungero.Parties.ICompanyBase Subject)
    {
      string TINNew = sberdev.SBContracts.Companies.As(Subject).TIN;
      sberdev.SBContracts.ICompany Counterparty = sberdev.SBContracts.Companies.Null; 
      var ListOrgTrue = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => ((c.HeadOrgSDev == true) && (c != sberdev.SBContracts.Companies.As(Subject)))).ToArray();
      if ((ListOrgTrue.Count() > 0) && (ListOrgTrue.Count() < 25))
      {
        foreach (var elem in ListOrgTrue)
        {
          if (elem.TIN != null)
          {
            var OtherOrg = sberdev.SBContracts.Companies.GetAll(c => ((c.TIN == TINNew) && (c.HeadCompany != elem) && (c.Id != Subject.Id))).ToArray();
            if (OtherOrg.Count() > 0)
            {
              Counterparty = OtherOrg.FirstOrDefault();
            }
          }
        }
      }
      return Counterparty;
    }

  }
}