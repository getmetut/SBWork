using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Structures.ExchangeCore.BusinessUnitBox
{
  partial class FoundCompanies
  {
    public string Message { get; set; }
    
    public List<sberdev.SBContracts.Structures.ExchangeCore.BusinessUnitBox.CompanyDisplayValue> CompanyDisplayValues { get; set; }
    
    public List<sberdev.SBContracts.Structures.ExchangeCore.BusinessUnitBox.FoundContact> FoundContacts { get; set; }
    
    public int Amount { get; set; }
  }
  
  partial class CompanyDisplayValue
  {
    public string DisplayValue { get; set; }
    
    public string PSRN { get; set; }
  }
  
  partial class FoundContact
  {
    public string FullName { get; set; }
    
    public string JobTitle { get; set; }
    
    public string Phone { get; set; }
  }
}