using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;
using System.Text.RegularExpressions;

namespace sberdev.SberContracts
{
  partial class PurchaseSharedHandlers
  {

    public virtual void JiraLinkChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewValue))
      {
        string resstring = EnsureLinkFormat(e.NewValue);
        if (e.NewValue != resstring)
          _obj.JiraLink = resstring;          
      }
    }
    
    private string EnsureLinkFormat(string input)
    {
      string pattern = @"^https:\/\/tasks\.sberdevices\.ru\/browse\/([A-Z]+\-\d+)$";
      if (Regex.IsMatch(input, pattern))
      {
        return input; 
      }
      string codePattern = @"^([A-Z]+\-\d+)$";
      if (Regex.IsMatch(input, codePattern))
      {
        return $"https://tasks.sberdevices.ru/browse/{input}";
      }
      return "";
    }

    public override void AccArtExBaseSberDevChanged(sberdev.SBContracts.Shared.ContractualDocumentAccArtExBaseSberDevChangedEventArgs e)
    {
      PublicFunctions.Purchase.UpdateCard(_obj);
      base.AccArtExBaseSberDevChanged(e);
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
     
      _obj.Relations.AddFromOrUpdate("Purchase", e.OldValue, e.NewValue);
    }

    public virtual void SpecificationChanged(sberdev.SberContracts.Shared.PurchaseSpecificationChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
     
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public virtual void CommercialOfferChanged(sberdev.SberContracts.Shared.PurchaseCommercialOfferChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
     
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public virtual void UpgradedCommercialOfferChanged(sberdev.SberContracts.Shared.PurchaseUpgradedCommercialOfferChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
     
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

  }
}