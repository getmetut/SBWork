using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Integration.Server
{
  partial class ModuleFunctions
  {
    public override IQueryable<centrvd.KFIntegration.ICompany> GetCompaniesToFillInMarkers()
    {
      var parties = base.GetCompaniesToFillInMarkers();
      var contracts = SBContracts.ContractualDocuments.GetAll().Where(c => c.ExternalApprovalState == SBContracts.OfficialDocument.ExternalApprovalState.Signed
                                                                      && c.InternalApprovalState == SBContracts.OfficialDocument.InternalApprovalState.Signed
                                                                      && c.Counterparty != null && c.Currency != null && c.Currency.Id == 1);
      var dateFrom = Calendar.Now.BeginningOfYear();
      var dateTo = Calendar.Now.EndOfYear();
      var allCounterparties = contracts.Select(c => c.Counterparty).Distinct().ToList();
      var filteredParties = allCounterparties.Where(cp =>
                                                    {
                                                      var totalAmount = PublicFunctions.Counterparty.CalculateTotalAmount(SBContracts.Counterparties.As(cp), dateFrom, dateTo);
                                                      return totalAmount >= 500000;
                                                    }).ToList();
      var partiesChecked = parties.Where(q => SBContracts.Companies.As(q).ActiveLicense.HasValue).ToList();
      parties = parties.Where(q => q.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && filteredParties.Contains(q));
      var partiesList = parties.ToList();
      
      HashSet<centrvd.KFIntegration.ICompany> totalPartiesHash = new HashSet<centrvd.KFIntegration.ICompany>(partiesList);
      totalPartiesHash.UnionWith(partiesChecked);
      List<centrvd.KFIntegration.ICompany> totalParties = new List<centrvd.KFIntegration.ICompany>(totalPartiesHash);
      
      return totalParties.AsQueryable();
    }
    
    public override void SendCounterpartyReport()
    {
      var settings = centrvd.Integration.CounterpartyReportSettings.GetAll()
        .Where(z => z.Status == centrvd.Integration.CounterpartyReportSetting.Status.Active);

      foreach (var setting in settings)
      {
        var period = setting.Period == centrvd.Integration.CounterpartyReportSetting.Period.Daily ? 24 :
          setting.Period == centrvd.Integration.CounterpartyReportSetting.Period.Monthly ? 168 : 720;
        var markers = setting.Markers.Select(z => new Nullable<Enumeration>(new Enumeration(z.Marker.PropertyName)));
        var companies = SBContracts.Companies.GetAll()
          .Where(z => z.MarkersChanges.Any(x => markers.Contains(x.Name) &&
                                           x.OldValue == setting.OldValue && x.NewValue == setting.NewValue)).ToList().
          Where(z => z.MarkersChanges.Any(x => markers.Contains(x.Name) &&
                                          x.OldValue == setting.OldValue && x.NewValue == setting.NewValue &&
                                          x.ChangeDateTime.Value.AddHours(period) > Calendar.Now));
        foreach (var company in companies)
          company.FocusCheckedSberDev = false;
      }
      base.SendCounterpartyReport();
    }
  }
}