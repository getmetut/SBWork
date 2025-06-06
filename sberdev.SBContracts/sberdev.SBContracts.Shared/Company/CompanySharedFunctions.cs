using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Shared
{
  partial class CompanyFunctions
  {
    [Public]
    /// <summary>
    /// Возвращает список свойств маркеров Контур.Фокус
    /// </summary>
    public static List<Sungero.Domain.Shared.IPropertyState> GetAllFocusMarkers(centrvd.KFIntegration.ICompany company)
    {
      return new List<Sungero.Domain.Shared.IPropertyState>()
      {
        company.State.Properties.OrgLiquidated, company.State.Properties.RevokedLicense,
        company.State.Properties.OrgBlocking, company.State.Properties.Unprofitable,
        company.State.Properties.LargeLE, company.State.Properties.EPWages,
        company.State.Properties.Bankruptcy, company.State.Properties.AddressInfo,
        company.State.Properties.HeadInfo, company.State.Properties.BankruptcyTo,
        company.State.Properties.BankruptcyEnd, company.State.Properties.ToBankruptcy,
        company.State.Properties.BadSupplier, company.State.Properties.SanctionsList,
        company.State.Properties.ToLiquidation, company.State.Properties.HeadFTSList,
        company.State.Properties.HeadBankrupt, company.State.Properties.HeadDisqual,
        company.State.Properties.RelOrgToLiquid, company.State.Properties.RelOrgNegative,
        company.State.Properties.RelOrgBankrupt, company.State.Properties.RelOrgIsLiquid,
        company.State.Properties.InvalidAddress, company.State.Properties.InvalidHead,
        company.State.Properties.ArbCaseTo, company.State.Properties.RelOrgWages,
        company.State.Properties.Moratorium, company.State.Properties.HeadIndividual,
        company.State.Properties.ChangeFullName, company.State.Properties.ChangeTaxCode,
        company.State.Properties.ChangeActivity, company.State.Properties.ChangeAddAct,
        company.State.Properties.ReorgProcess, company.State.Properties.Registered,
        company.State.Properties.ChangeCap, company.State.Properties.Change2Trrc,
        company.State.Properties.Change2Name, company.State.Properties.Change2Head,
        company.State.Properties.Change3Head, company.State.Properties.Change2Founder,
        company.State.Properties.Change3Founder, company.State.Properties.Change2Address,
        company.State.Properties.Change3Address, company.State.Properties.ForeignPersons,
        company.State.Properties.MassRegistrList, company.State.Properties.TaxArrears,
        company.State.Properties.CaseAmountFrom, company.State.Properties.CaseAmountTo,
        company.State.Properties.ExecCaseAmount, company.State.Properties.HeadOldLECount,
        company.State.Properties.LECount, company.State.Properties.HeadLECount,
        company.State.Properties.LECountAddress, company.State.Properties.NoLiquidAddress,
        company.State.Properties.UpArbCaseTo, company.State.Properties.UpArbCaseFrom,
        company.State.Properties.PledgedProperty, company.State.Properties.CreditPayments,
        company.State.Properties.Premium, company.State.Properties.TaxesAndFees,
        company.State.Properties.Seizure, company.State.Properties.SpecialArbCases,
        company.State.Properties.UpCredits, company.State.Properties.ChangeData,
        company.State.Properties.UpBankruptcy, company.State.Properties.GainDown,
        company.State.Properties.TaxReports, company.State.Properties.AddValid,
        company.State.Properties.RelOrgArbCases, company.State.Properties.ExpiringLicense,
        company.State.Properties.ExpiredLicense, company.State.Properties.UpDeliveries,
        company.State.Properties.SpecialList, company.State.Properties.SpecialExecList,
        company.State.Properties.LongRegistered, company.State.Properties.CapitalM100000,
        company.State.Properties.CapitalL100000, company.State.Properties.HasBranches,
        company.State.Properties.RelOrgAddValid, company.State.Properties.RelOrgSwapHead,
        company.State.Properties.YoungRelOrg, company.State.Properties.RespondentArb,
        company.State.Properties.RelOrgGainDown, company.State.Properties.AccountingState,
        company.State.Properties.StateContracts, company.State.Properties.ActiveLicense,
        company.State.Properties.ActiveTrademark, company.State.Properties.RelOrgActiveArb,
        company.State.Properties.RelOrgActivePub, company.State.Properties.RelOrgOld,
        company.State.Properties.RelOrgAccount, company.State.Properties.MarkerQ2031ChangedDate,
        company.State.Properties.MarkerQ2032ChangedDate, company.State.Properties.MarkerQ2035ChangedDate,
        company.State.Properties.MarkerY3601B, company.State.Properties.MarkerQ2031,
        company.State.Properties.MarkerQ2035, company.State.Properties.MarkerY3601BChangedDate,
        company.State.Properties.ArbPractice, company.State.Properties.RelOrgExecCases
      };
    }
    
    [Public]
    /// <summary>
    /// Возвращает список свойств маркеров Контур.Фокус
    /// </summary>
    public static List<Sungero.Domain.Shared.IPropertyState> GetProfitableFocusMarkers(centrvd.KFIntegration.ICompany company)
    {
      return new List<Sungero.Domain.Shared.IPropertyState>()
      {
        company.State.Properties.UpBankruptcy, company.State.Properties.GainDown,
        company.State.Properties.Premium, company.State.Properties.TaxesAndFees,
        company.State.Properties.PledgedProperty, company.State.Properties.CreditPayments,
        company.State.Properties.MassRegistrList, company.State.Properties.TaxArrears,
        company.State.Properties.OrgBlocking, company.State.Properties.Unprofitable,
        company.State.Properties.HeadInfo, company.State.Properties.BankruptcyTo,
        company.State.Properties.BankruptcyEnd, company.State.Properties.ToBankruptcy,
        company.State.Properties.HeadBankrupt, company.State.Properties.HeadDisqual,
        company.State.Properties.CaseAmountFrom, company.State.Properties.CaseAmountTo
        // m1004; Исполнительные производства (наложение ареста)
        // m1006; Исполнительные производства (взыскание заложенного имущества)
        // s1008; Исп. пр-ва. Сумма (в рублях) найденных исполнительных производств,
        // предметом которых являются страховые взносы, в отношении организаций со схожими реквизитами (всего)
      };
    }
    
        [Public]
    /// <summary>
    /// Возвращает список свойств маркеров Контур.Фокус
    /// </summary>
    public static List<Sungero.Domain.Shared.IPropertyState> GetAllFocusMarkers(sberdev.SBContracts.ICompany company)
    {
      return new List<Sungero.Domain.Shared.IPropertyState>()
      {
        company.State.Properties.OrgLiquidated, company.State.Properties.RevokedLicense,
        company.State.Properties.OrgBlocking, company.State.Properties.Unprofitable,
        company.State.Properties.LargeLE, company.State.Properties.EPWages,
        company.State.Properties.Bankruptcy, company.State.Properties.AddressInfo,
        company.State.Properties.HeadInfo, company.State.Properties.BankruptcyTo,
        company.State.Properties.BankruptcyEnd, company.State.Properties.ToBankruptcy,
        company.State.Properties.BadSupplier, company.State.Properties.SanctionsList,
        company.State.Properties.ToLiquidation, company.State.Properties.HeadFTSList,
        company.State.Properties.HeadBankrupt, company.State.Properties.HeadDisqual,
        company.State.Properties.RelOrgToLiquid, company.State.Properties.RelOrgNegative,
        company.State.Properties.RelOrgBankrupt, company.State.Properties.RelOrgIsLiquid,
        company.State.Properties.InvalidAddress, company.State.Properties.InvalidHead,
        company.State.Properties.ArbCaseTo, company.State.Properties.RelOrgWages,
        company.State.Properties.Moratorium, company.State.Properties.HeadIndividual,
        company.State.Properties.ChangeFullName, company.State.Properties.ChangeTaxCode,
        company.State.Properties.ChangeActivity, company.State.Properties.ChangeAddAct,
        company.State.Properties.ReorgProcess, company.State.Properties.Registered,
        company.State.Properties.ChangeCap, company.State.Properties.Change2Trrc,
        company.State.Properties.Change2Name, company.State.Properties.Change2Head,
        company.State.Properties.Change3Head, company.State.Properties.Change2Founder,
        company.State.Properties.Change3Founder, company.State.Properties.Change2Address,
        company.State.Properties.Change3Address, company.State.Properties.ForeignPersons,
        company.State.Properties.MassRegistrList, company.State.Properties.TaxArrears,
        company.State.Properties.CaseAmountFrom, company.State.Properties.CaseAmountTo,
        company.State.Properties.ExecCaseAmount, company.State.Properties.HeadOldLECount,
        company.State.Properties.LECount, company.State.Properties.HeadLECount,
        company.State.Properties.LECountAddress, company.State.Properties.NoLiquidAddress,
        company.State.Properties.UpArbCaseTo, company.State.Properties.UpArbCaseFrom,
        company.State.Properties.PledgedProperty, company.State.Properties.CreditPayments,
        company.State.Properties.Premium, company.State.Properties.TaxesAndFees,
        company.State.Properties.Seizure, company.State.Properties.SpecialArbCases,
        company.State.Properties.UpCredits, company.State.Properties.ChangeData,
        company.State.Properties.UpBankruptcy, company.State.Properties.GainDown,
        company.State.Properties.TaxReports, company.State.Properties.AddValid,
        company.State.Properties.RelOrgArbCases, company.State.Properties.ExpiringLicense,
        company.State.Properties.ExpiredLicense, company.State.Properties.UpDeliveries,
        company.State.Properties.SpecialList, company.State.Properties.SpecialExecList,
        company.State.Properties.LongRegistered, company.State.Properties.CapitalM100000,
        company.State.Properties.CapitalL100000, company.State.Properties.HasBranches,
        company.State.Properties.RelOrgAddValid, company.State.Properties.RelOrgSwapHead,
        company.State.Properties.YoungRelOrg, company.State.Properties.RespondentArb,
        company.State.Properties.RelOrgGainDown, company.State.Properties.AccountingState,
        company.State.Properties.StateContracts, company.State.Properties.ActiveLicense,
        company.State.Properties.ActiveTrademark, company.State.Properties.RelOrgActiveArb,
        company.State.Properties.RelOrgActivePub, company.State.Properties.RelOrgOld,
        company.State.Properties.RelOrgAccount, company.State.Properties.MarkerQ2031ChangedDate,
        company.State.Properties.MarkerQ2032ChangedDate, company.State.Properties.MarkerQ2035ChangedDate,
        company.State.Properties.MarkerY3601B, company.State.Properties.MarkerQ2031,
        company.State.Properties.MarkerQ2035, company.State.Properties.MarkerY3601BChangedDate,
        company.State.Properties.ArbPractice, company.State.Properties.RelOrgExecCases,
        company.State.Properties.LowAvgHeadSberDev
      };
    }
    
    [Public]
    /// <summary>
    /// Возвращает список свойств маркеров Контур.Фокус
    /// </summary>
    public static List<Sungero.Domain.Shared.IPropertyState> GetProfitableFocusMarkers(sberdev.SBContracts.ICompany company)
    {
      return new List<Sungero.Domain.Shared.IPropertyState>()
      {
        company.State.Properties.UpBankruptcy, company.State.Properties.GainDown,
        company.State.Properties.Premium, company.State.Properties.TaxesAndFees,
        company.State.Properties.PledgedProperty, company.State.Properties.CreditPayments,
        company.State.Properties.MassRegistrList, company.State.Properties.TaxArrears,
        company.State.Properties.OrgBlocking, company.State.Properties.Unprofitable,
        company.State.Properties.HeadInfo, company.State.Properties.BankruptcyTo,
        company.State.Properties.BankruptcyEnd, company.State.Properties.ToBankruptcy,
        company.State.Properties.HeadBankrupt, company.State.Properties.HeadDisqual,
        company.State.Properties.CaseAmountFrom, company.State.Properties.CaseAmountTo
        // m1004; Исполнительные производства (наложение ареста)
        // m1006; Исполнительные производства (взыскание заложенного имущества)
        // s1008; Исп. пр-ва. Сумма (в рублях) найденных исполнительных производств,
        // предметом которых являются страховые взносы, в отношении организаций со схожими реквизитами (всего)
      };
    }

    /// <summary>
    /// Функция скрывает белые маркеры Контур.Фокус
    /// </summary>
    public void HideEmptyFocusMarkers()
    {
      List<Sungero.Domain.Shared.IPropertyState> focusProps = GetAllFocusMarkers(_obj);
      foreach (var prop in focusProps)
      {
        if (focusProps.Contains(prop) && prop.HighlightColor == Colors.Empty)
          prop.IsVisible = false;
        else
          prop.IsVisible = true;
      }
    }
    
    public override void SetColorsForKFMarkers()
    {
      var settings = centrvd.KFIntegration.Module.Company.PublicFunctions.Module.Remote.GetKFMarkersSettings();
      if (settings == null)
        return;

      var colorMap = new Dictionary<Nullable<Enumeration>, Color>
      {
        {centrvd.Integration.KFMarkersSettingMarkers.Color.Red, Colors.Common.Red},
        {centrvd.Integration.KFMarkersSettingMarkers.Color.Yellow, Colors.Common.Yellow},
        {centrvd.Integration.KFMarkersSettingMarkers.Color.Green, Colors.Common.Green}
      };
      var properties = _obj.State.Properties;
      var propertyNames = settings.Markers.Any() ? settings.Markers.FirstOrDefault().NameAllowedItems.Items.Select(z => z.ToString()) : new List<string>();
      foreach (var propertyName in propertyNames)
        if (properties[propertyName] != null)
          properties[propertyName].HighlightColor = GetHighlightColor(propertyName, colorMap, settings);
        else
          properties[propertyName + "SberDev"].HighlightColor = GetHighlightColor(propertyName + "SberDev", colorMap, settings);
    }
    
    public override void SetColorsForKFMarkers(centrvd.Integration.IKFMarkersSetting settings)
    {
      if (settings == null)
        return;

      var colorMap = new Dictionary<Nullable<Enumeration>, Color>
      {
        {centrvd.Integration.KFMarkersSettingMarkers.Color.Red, Colors.Common.Red},
        {centrvd.Integration.KFMarkersSettingMarkers.Color.Yellow, Colors.Common.Yellow},
        {centrvd.Integration.KFMarkersSettingMarkers.Color.Green, Colors.Common.Green}
      };
      
      var properties = _obj.State.Properties;
      var propertyNames = settings.Markers.Any() ? settings.Markers.FirstOrDefault().NameAllowedItems.Items.Select(z => z.ToString()) : new List<string>();

      foreach (var propertyName in propertyNames)
        if (properties[propertyName] != null)
          properties[propertyName].HighlightColor = GetHighlightColor(propertyName, colorMap, settings);
        else
          properties[propertyName + "SberDev"].HighlightColor = GetHighlightColor(propertyName + "SberDev", colorMap, settings);
      
    }
    
    /// <summary>
    /// Получить цвет маркера.
    /// </summary>
    /// <param name="propertyName">Имя маркера.</param>
    /// <param name="colorMap">Словарь соотношения значений и цветов.</param>
    /// <param name="settings">Настройки маркеров Контур.Фокус.</param>
    /// <returns>Цвет.</returns>
    private Sungero.Core.Color GetHighlightColor(string propertyName, System.Collections.Generic.Dictionary<Nullable<Enumeration>, Color> colorMap, centrvd.Integration.IKFMarkersSetting settings)
    {
      var propertyValue = (Sungero.Core.Enumeration?)_obj
        .GetType()
        .GetFinalType()
        .GetEntityMetadata()
        .Properties
        .FirstOrDefault(property => property.Name == propertyName)?
        .GetValue(_obj);
      
      var markerSetting = SBContracts.KFMarkersSettings.As(settings).Markers.FirstOrDefault(marker => (marker.Name?.ToString() == propertyName
                                                                                                       || marker.Name?.ToString() == propertyName.Replace("SberDev", ""))
                                                                                            && marker.Value == propertyValue);
      Sungero.Core.Color highlightColor;

      if (markerSetting != null && markerSetting.Color.HasValue && colorMap.TryGetValue(markerSetting.Color, out highlightColor))
        return highlightColor;

      return Colors.Empty;
    }
  }
}