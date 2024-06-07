using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts.Shared
{
  partial class CompanyFunctions
  {

    /// <summary>
    /// Функция скрывает белые маркеры Контур.Фокус
    /// </summary>
    public void HideEmptyFocusMarkers()
    {
      List<Sungero.Domain.Shared.IPropertyState> focusProps = new List<Sungero.Domain.Shared.IPropertyState>(){_obj.State.Properties.OrgLiquidated, _obj.State.Properties.RevokedLicense,
        _obj.State.Properties.OrgBlocking, _obj.State.Properties.Unprofitable, _obj.State.Properties.LargeLE, _obj.State.Properties.EPWages, _obj.State.Properties.Bankruptcy,
        _obj.State.Properties.AddressInfo, _obj.State.Properties.HeadInfo, _obj.State.Properties.BankruptcyTo, _obj.State.Properties.BankruptcyEnd, _obj.State.Properties.ToBankruptcy,
        _obj.State.Properties.BadSupplier, _obj.State.Properties.SanctionsList, _obj.State.Properties.ToLiquidation, _obj.State.Properties.HeadFTSList, _obj.State.Properties.HeadBankrupt,
        _obj.State.Properties.HeadDisqual, _obj.State.Properties.RelOrgToLiquid,_obj.State.Properties.RelOrgNegative, _obj.State.Properties.RelOrgBankrupt,
        _obj.State.Properties.RelOrgIsLiquid, _obj.State.Properties.InvalidAddress, _obj.State.Properties.InvalidHead , _obj.State.Properties.ArbCaseTo ,_obj.State.Properties.RelOrgWages ,
        _obj.State.Properties.Moratorium, _obj.State.Properties.HeadIndividual, _obj.State.Properties.ChangeFullName, _obj.State.Properties.ChangeTaxCode, _obj.State.Properties.ChangeActivity,
        _obj.State.Properties.ChangeAddAct, _obj.State.Properties.ReorgProcess, _obj.State.Properties.Registered, _obj.State.Properties.ChangeCap, _obj.State.Properties.Change2Trrc,
        _obj.State.Properties.Change2Name, _obj.State.Properties.Change2Head, _obj.State.Properties.Change3Head, _obj.State.Properties.Change2Founder, _obj.State.Properties.Change3Founder,
        _obj.State.Properties.Change2Address, _obj.State.Properties.Change3Address, _obj.State.Properties.ForeignPersons, _obj.State.Properties.MassRegistrList, _obj.State.Properties.TaxArrears,
        _obj.State.Properties.CaseAmountFrom, _obj.State.Properties.CaseAmountTo, _obj.State.Properties.ExecCaseAmount, _obj.State.Properties.HeadOldLECount, _obj.State.Properties.LECount,
        _obj.State.Properties.HeadLECount, _obj.State.Properties.LECountAddress, _obj.State.Properties.NoLiquidAddress, _obj.State.Properties.UpArbCaseTo, _obj.State.Properties.UpArbCaseFrom,
        _obj.State.Properties.PledgedProperty, _obj.State.Properties.CreditPayments, _obj.State.Properties.Premium, _obj.State.Properties.TaxesAndFees, _obj.State.Properties.Seizure,
        _obj.State.Properties.SpecialArbCases, _obj.State.Properties.UpCredits, _obj.State.Properties.ChangeData, _obj.State.Properties.UpBankruptcy, _obj.State.Properties.GainDown,
        _obj.State.Properties.TaxReports, _obj.State.Properties.AddValid, _obj.State.Properties.RelOrgArbCases, _obj.State.Properties.ExpiringLicense, _obj.State.Properties.ExpiredLicense, _obj.State.Properties.UpDeliveries,
        _obj.State.Properties.SpecialList, _obj.State.Properties.SpecialExecList, _obj.State.Properties.LongRegistered, _obj.State.Properties.CapitalM100000, _obj.State.Properties.CapitalL100000, _obj.State.Properties.HasBranches,
        _obj.State.Properties.RelOrgAddValid, _obj.State.Properties.RelOrgSwapHead, _obj.State.Properties.YoungRelOrg, _obj.State.Properties.RespondentArb, _obj.State.Properties.RelOrgGainDown,
        _obj.State.Properties.AccountingState, _obj.State.Properties.StateContracts, _obj.State.Properties.ActiveLicense, _obj.State.Properties.ActiveTrademark, _obj.State.Properties.RelOrgActiveArb,
        _obj.State.Properties.RelOrgActivePub, _obj.State.Properties.RelOrgOld, _obj.State.Properties.RelOrgAccount, _obj.State.Properties.MarkerQ2031ChangedDate,
        _obj.State.Properties.MarkerQ2032ChangedDate, _obj.State.Properties.MarkerQ2035ChangedDate, _obj.State.Properties.MarkerY3601B, _obj.State.Properties.MarkerQ2031, _obj.State.Properties.MarkerQ2035,
        _obj.State.Properties.MarkerY3601BChangedDate, _obj.State.Properties.ArbPractice, _obj.State.Properties.RelOrgExecCases};
      var props = _obj.State.Properties.ToList();
      foreach (var prop in props)
      {
        if (focusProps.Contains(prop) && prop.HighlightColor == Colors.Empty)
          prop.IsVisible = false;
        else
          prop.IsVisible = true;
      }
    }
  }
}