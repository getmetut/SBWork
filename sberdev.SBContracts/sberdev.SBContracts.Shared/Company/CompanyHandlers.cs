using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts
{
  partial class CompanySharedHandlers
  {

    public override void HeadCompanyChanged(Sungero.Parties.Shared.CompanyBaseHeadCompanyChangedEventArgs e)
    {
      base.HeadCompanyChanged(e);
      if (e.NewValue != null)
        _obj.HeadOrgSDev = false;
    }

    public virtual void HeadOrgSDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (e.NewValue == true)
        {
          var HeadOrg = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => ((c.TIN == _obj.TIN) && (c.HeadOrgSDev == true))).FirstOrDefault();
          if (HeadOrg != null)
          {
            PublicFunctions.Module.ShowInfoMessage("Для данного ИНН уже существует головная организация: " + HeadOrg.Name.ToString());
            _obj.HeadOrgSDev = false;
          }
        }
      }
    }

    public override void NameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.NameChanged(e);
      if (e.NewValue != null)
      {
        
        string Name1C = ModifyCompanyName(e.NewValue);
        if (_obj.Name1CSberDev != Name1C)
          _obj.Name1CSberDev = Name1C;
      }
    }

    public virtual void IPSberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (_obj.IPSberDev.HasValue)
        if (!_obj.IPSberDev.Value)
          _obj.PersonSberDev = null;
    }

    private void WriteMarkerHistory(string markerName, Enumeration? value)
    {
      var operation = new Enumeration("ChangeMarkers");
      var comment = string.Format("{0}: {1}", markerName, value != null ? value.ToString() : string.Empty);
      _obj.History.Write(operation, operation, comment);
    }

    public virtual void OrgBlockingChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.OrgBlocking.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgToLiquidChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgToLiquid.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgBankruptChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgBankrupt.LocalizedName, e.NewValue);
    }

    public virtual void UnprofitableChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Unprofitable.LocalizedName, e.NewValue);
    }

    public virtual void TaxArrearsChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.TaxArrears.LocalizedName, e.NewValue);
    }

    public virtual void CaseAmountToChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.CaseAmountTo.LocalizedName, e.NewValue);
    }

    public virtual void ExecCaseAmountChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ExecCaseAmount.LocalizedName, e.NewValue);
    }

    public virtual void BankruptcyChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Bankruptcy.LocalizedName, e.NewValue);
    }

    public virtual void BankruptcyToChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.BankruptcyTo.LocalizedName, e.NewValue);
    }

    public virtual void BankruptcyEndChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.BankruptcyEnd.LocalizedName, e.NewValue);
    }

    public virtual void ArbCaseToChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ArbCaseTo.LocalizedName, e.NewValue);
    }

    public virtual void ToLiquidationChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ToLiquidation.LocalizedName, e.NewValue);
    }

    public virtual void HeadBankruptChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadBankrupt.LocalizedName, e.NewValue);
    }

    public virtual void HeadDisqualChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadDisqual.LocalizedName, e.NewValue);
    }

    public virtual void GainDownChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.GainDown.LocalizedName, e.NewValue);
    }

    public virtual void PledgedPropertyChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.PledgedProperty.LocalizedName, e.NewValue);
    }

    public virtual void CreditPaymentsChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.CreditPayments.LocalizedName, e.NewValue);
    }

    public virtual void TaxesAndFeesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.TaxesAndFees.LocalizedName, e.NewValue);
    }

    public virtual void SeizureChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Seizure.LocalizedName, e.NewValue);
    }
    
    static string ModifyCompanyName(string companyName)
    {
        //companyName = companyName.Trim('"'); // убираем кавычки в начале и конце строки
        string CompNameNew = companyName.Replace("Общество с ограниченной ответственностью","ООО");
        CompNameNew = CompNameNew.Replace("Публичное акционерное общество","ПАО");
        CompNameNew = CompNameNew.Replace("Открытое акционерное общество","ОАО");
        CompNameNew = CompNameNew.Replace("Закрытое акционерное общество","ЗАО");
        CompNameNew = CompNameNew.Replace("Индивидуальный предприниматель","ИП");
        CompNameNew = CompNameNew.Replace("Акционерное общество","АО");
        CompNameNew = CompNameNew.Replace('«','"');
        CompNameNew = CompNameNew.Replace('»','"');
        // разделяем наименование и правовую форму        
        string[] parts = CompNameNew.Split(' ');
        string legalForm = parts[0]; // первая часть - правовая форма (ООО, ЗАО, etc.)
        string name = string.Join(" ", parts, 1, parts.Length - 1).Trim('"'); // остальная часть - наименование
        name = name.ToUpper();
        // формируем измененное название
        string modifiedName = $"{name} {legalForm}";
        if ((legalForm == "ООО") || (legalForm == "ЗАО") || (legalForm == "ОАО") || (legalForm == "АО") || (legalForm == "ПАО") || (legalForm == "ИП"))
          return modifiedName;
        else
          return companyName;
    }
  }
    public virtual void OrgLiquidatedChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.OrgLiquidated.LocalizedName, e.NewValue);
    }

    public virtual void RevokedLicenseChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RevokedLicense.LocalizedName, e.NewValue);
    }

    public virtual void LargeLEChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.LargeLE.LocalizedName, e.NewValue);
    }

    public virtual void EPWagesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.EPWages.LocalizedName, e.NewValue);
    }

    public virtual void AddressInfoChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.AddressInfo.LocalizedName, e.NewValue);
    }

    public virtual void HeadInfoChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadInfo.LocalizedName, e.NewValue);
    }

    public virtual void ToBankruptcyChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ToBankruptcy.LocalizedName, e.NewValue);
    }

    public virtual void BadSupplierChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.BadSupplier.LocalizedName, e.NewValue);
    }

    public virtual void SanctionsListChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.SanctionsList.LocalizedName, e.NewValue);
    }

    public virtual void HeadFTSListChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadFTSList.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgNegativeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgNegative.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgIsLiquidChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgIsLiquid.LocalizedName, e.NewValue);
    }

    public virtual void InvalidAddressChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.InvalidAddress.LocalizedName, e.NewValue);
    }

    public virtual void InvalidHeadChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.InvalidHead.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgWagesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgWages.LocalizedName, e.NewValue);
    }

    public virtual void MoratoriumChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Moratorium.LocalizedName, e.NewValue);
    }

    public virtual void HeadIndividualChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadIndividual.LocalizedName, e.NewValue);
    }

    public virtual void ChangeFullNameChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ChangeFullName.LocalizedName, e.NewValue);
    }

    public virtual void ChangeTaxCodeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ChangeTaxCode.LocalizedName, e.NewValue);
    }

    public virtual void ChangeActivityChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ChangeActivity.LocalizedName, e.NewValue);
    }

    public virtual void ChangeAddActChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ChangeAddAct.LocalizedName, e.NewValue);
    }

    public virtual void ReorgProcessChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ReorgProcess.LocalizedName, e.NewValue);
    }

    public virtual void RegisteredChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Registered.LocalizedName, e.NewValue);
    }

    public virtual void ChangeCapChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ChangeCap.LocalizedName, e.NewValue);
    }

    public virtual void Change2TrrcChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change2Trrc.LocalizedName, e.NewValue);
    }

    public virtual void Change2NameChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change2Name.LocalizedName, e.NewValue);
    }

    public virtual void Change2HeadChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change2Head.LocalizedName, e.NewValue);
    }

    public virtual void Change3HeadChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change3Head.LocalizedName, e.NewValue);
    }

    public virtual void Change2FounderChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change2Founder.LocalizedName, e.NewValue);
    }

    public virtual void Change3FounderChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change3Founder.LocalizedName, e.NewValue);
    }

    public virtual void Change2AddressChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change2Address.LocalizedName, e.NewValue);
    }

    public virtual void Change3AddressChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Change3Address.LocalizedName, e.NewValue);
    }

    public virtual void ForeignPersonsChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ForeignPersons.LocalizedName, e.NewValue);
    }

    public virtual void MassRegistrListChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.MassRegistrList.LocalizedName, e.NewValue);
    }

    public virtual void CaseAmountFromChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.CaseAmountFrom.LocalizedName, e.NewValue);
    }

    public virtual void HeadOldLECountChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadOldLECount.LocalizedName, e.NewValue);
    }

    public virtual void LECountChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.LECount.LocalizedName, e.NewValue);
    }

    public virtual void HeadLECountChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HeadLECount.LocalizedName, e.NewValue);
    }

    public virtual void LECountAddressChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.LECountAddress.LocalizedName, e.NewValue);
    }

    public virtual void NoLiquidAddressChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.NoLiquidAddress.LocalizedName, e.NewValue);
    }

    public virtual void UpArbCaseToChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.UpArbCaseTo.LocalizedName, e.NewValue);
    }

    public virtual void UpArbCaseFromChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.UpArbCaseFrom.LocalizedName, e.NewValue);
    }

    public virtual void PremiumChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.Premium.LocalizedName, e.NewValue);
    }

    public virtual void SpecialArbCasesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.SpecialArbCases.LocalizedName, e.NewValue);
    }

    public virtual void UpCreditsChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.UpCredits.LocalizedName, e.NewValue);
    }

    public virtual void ChangeDataChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ChangeData.LocalizedName, e.NewValue);
    }

    public virtual void UpBankruptcyChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.UpBankruptcy.LocalizedName, e.NewValue);
    }

    public virtual void TaxReportsChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.TaxReports.LocalizedName, e.NewValue);
    }

    public virtual void AddValidChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.AddValid.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgArbCasesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgArbCases.LocalizedName, e.NewValue);
    }

    public virtual void ExpiringLicenseChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ExpiringLicense.LocalizedName, e.NewValue);
    }

    public virtual void ExpiredLicenseChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ExpiredLicense.LocalizedName, e.NewValue);
    }

    public virtual void UpDeliveriesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.UpDeliveries.LocalizedName, e.NewValue);
    }

    public virtual void SpecialListChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.SpecialList.LocalizedName, e.NewValue);
    }

    public virtual void SpecialExecListChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.SpecialExecList.LocalizedName, e.NewValue);
    }

    public virtual void LongRegisteredChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.LongRegistered.LocalizedName, e.NewValue);
    }

    public virtual void CapitalM100000Changed(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.CapitalM100000.LocalizedName, e.NewValue);
    }

    public virtual void CapitalL100000Changed(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.CapitalL100000.LocalizedName, e.NewValue);
    }

    public virtual void HasBranchesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.HasBranches.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgAddValidChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgAddValid.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgSwapHeadChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgSwapHead.LocalizedName, e.NewValue);
    }

    public virtual void YoungRelOrgChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.YoungRelOrg.LocalizedName, e.NewValue);
    }

    public virtual void RespondentArbChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RespondentArb.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgGainDownChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgGainDown.LocalizedName, e.NewValue);
    }

    public virtual void AccountingStateChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.AccountingState.LocalizedName, e.NewValue);
    }

    public virtual void StateContractsChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.StateContracts.LocalizedName, e.NewValue);
    }

    public virtual void ActiveLicenseChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ActiveLicense.LocalizedName, e.NewValue);
    }

    public virtual void ActiveTrademarkChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ActiveTrademark.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgActiveArbChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgActiveArb.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgActivePubChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgActivePub.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgOldChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgOld.LocalizedName, e.NewValue);
    }

    public virtual void ArbPracticeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.ArbPractice.LocalizedName, e.NewValue);
    }

    public virtual void RelOrgExecCasesChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      WriteMarkerHistory(_obj.Info.Properties.RelOrgExecCases.LocalizedName, e.NewValue);
    }
}