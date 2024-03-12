using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts.Shared
{
  partial class ContractualDocumentFunctions
  {
    public virtual void SetPropertiesAccess()
    {
      if (_obj.ContrTypeBaseSberDev.HasValue)
        ChangeAnaliticsAccess(_obj.ContrTypeBaseSberDev.Value.Value);
      
      bool flagCalc = _obj.MVPBaseSberDev != null ? _obj.MVPBaseSberDev.CalculationIsWorking.Value
        : (_obj.MVZBaseSberDev != null ? _obj.MVZBaseSberDev.CalculationIsWorking.Value : false);
      
      if (_obj.ProdCollectionExBaseSberDev.Any())
      {
        if ((_obj.ProdCollectionExBaseSberDev.Where(p => p.Product.Name == "General").Any()
             || _obj.ProdCollectionExBaseSberDev.Count > 1) && flagCalc)
          ChangeCalculationAccess(true);
        else
          ChangeCalculationAccess(false);
      }
      else
      {
        if (_obj.ProdCollectionPrBaseSberDev.Any())
        {
          if ((_obj.ProdCollectionPrBaseSberDev.Where(p => p.Product.Name == "General").Any()
               || _obj.ProdCollectionPrBaseSberDev.Count > 1) && flagCalc)
            ChangeCalculationAccess(true);
          else
            ChangeCalculationAccess(false);
        }
        else
          ChangeCalculationAccess(false);
      }
      
      ChangeCalculationPropertiesAccess(_obj.CalculationFlagBaseSberDev);
      
      if (_obj.TotalAmount == null || _obj.TotalAmount == 0)
      {
        ChangeCalculationAccess(false);
      }
      
      if (!flagCalc && _obj.CalculationDistributeBaseSberDev != null)
      {
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationBaseSberDev.Clear();
      }
      
      if (_obj.FrameworkBaseSberDev == true)
      {
        _obj.State.Properties.TotalAmount.IsEnabled = false ;
        _obj.State.Properties.Currency.IsEnabled = false ;
        ChangeCalculationAccess(false);
      }
      else
      {
        _obj.State.Properties.TotalAmount.IsEnabled = true ;
        _obj.State.Properties.Currency.IsEnabled = true ;
      }
      
      CancelRequiredPropeties();
   //   CancelRequiredPropetiesByType();
    }
    
    public void ChangeCalculationAccess(bool flag)
    {
      ChangeCalculationEnable(flag);
      ChangeCalculationVisible(flag);
    }
    
    public void ChangeCalculationEnable(bool flag)
    {
      _obj.State.Properties.CalculationBaseSberDev.IsEnabled = flag;
      _obj.State.Properties.CalculationBaseSberDev.IsRequired = flag;
      _obj.State.Properties.CalculationFlagBaseSberDev.IsEnabled = flag;
      _obj.State.Properties.CalculationFlagBaseSberDev.IsRequired = flag;
      _obj.State.Properties.CalculationDistributeBaseSberDev.IsEnabled = flag;
    }
    
    public void ChangeCalculationVisible(bool flag)
    {
      _obj.State.Properties.CalculationBaseSberDev.IsVisible = flag;
      _obj.State.Properties.CalculationAmountBaseSberDev.IsVisible = flag;
      _obj.State.Properties.CalculationDistributeBaseSberDev.IsVisible = flag;
      _obj.State.Properties.CalculationFlagBaseSberDev.IsVisible = flag;
      _obj.State.Properties.CalculationResidualAmountBaseSberDev.IsVisible = flag;
    }
    
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      _obj.State.Properties.DeliveryMethod.IsVisible = true;
      _obj.State.Properties.DeliveryMethod.IsEnabled = true;
      _obj.State.Properties.DeliveryMethod.IsRequired = true;
    }
    
    /// <summary>
    /// Одновременно устанавливает доступность и видимость аналитик соответвенно типам договора
    /// </summary>
    /// <param name="str">Значение перечесления Типа договора</param>
    [Public]
    public void ChangeAnaliticsAccess(string str)
    {
      ChangeAnaliticsVisible(str);
      ChangeAnaliticsEnable(str);
    }
    
    public void ChangeAnaliticsVisible(string str)
    {
      switch (str)
      {
        case ("Expendable") :
          _obj.State.Properties.MVZBaseSberDev.IsVisible = true;
          _obj.State.Properties.MVPBaseSberDev.IsVisible = false;
          _obj.State.Properties.AccArtExBaseSberDev.IsVisible = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsVisible = false;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsVisible = true;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsVisible = false;
          bool flagMarketing = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Name.IndexOf("Маркетинг") > 0;
          _obj.State.Properties.MarketDirectSberDev.IsVisible = flagMarketing;
          _obj.State.Properties.PurchComNumberSberDev.IsVisible = flagMarketing;
          break;
        case ("Profitable") :
          _obj.State.Properties.MVZBaseSberDev.IsVisible = false;
          _obj.State.Properties.MVPBaseSberDev.IsVisible = true;
          _obj.State.Properties.AccArtExBaseSberDev.IsVisible = false;
          _obj.State.Properties.AccArtPrBaseSberDev.IsVisible = true;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsVisible = false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsVisible = true;
          _obj.State.Properties.MarketDirectSberDev.IsVisible = false;
          break;
        case ("ExpendProfit") :
          _obj.State.Properties.MVZBaseSberDev.IsVisible = true;
          _obj.State.Properties.MVPBaseSberDev.IsVisible = true;
          _obj.State.Properties.AccArtExBaseSberDev.IsVisible = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsVisible = true;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsVisible = true;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsVisible = true;
          flagMarketing = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Name.IndexOf("Маркетинг") > 0;
          _obj.State.Properties.MarketDirectSberDev.IsVisible = flagMarketing;
          _obj.State.Properties.PurchComNumberSberDev.IsVisible = flagMarketing;
          break;
      }
    }
    
    [Public]
    public void ChangeAnaliticsEnable(string str)
    {
      switch (str)
      {
        case ("Expendable") :
          _obj.State.Properties.MVZBaseSberDev.IsEnabled = true;
          _obj.State.Properties.MVZBaseSberDev.IsRequired = true;
          _obj.State.Properties.MVPBaseSberDev.IsEnabled = false;
          _obj.State.Properties.MVPBaseSberDev.IsRequired = false;
          _obj.State.Properties.TotalAmount.IsRequired = _obj.FrameworkBaseSberDev.HasValue ? !_obj.FrameworkBaseSberDev.Value : true;
          _obj.State.Properties.Currency.IsRequired = _obj.FrameworkBaseSberDev.HasValue ? !_obj.FrameworkBaseSberDev.Value : true;
          _obj.State.Properties.AccArtExBaseSberDev.IsEnabled = true;
          _obj.State.Properties.AccArtExBaseSberDev.IsRequired = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsEnabled = false;
          _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = false;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsEnabled = _obj.MVZBaseSberDev != null ? true : false;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsRequired = _obj.MVZBaseSberDev != null ? true : false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsEnabled = false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsRequired = false;
          bool flagMarketing = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Name.IndexOf("Маркетинг") > 0;
          _obj.State.Properties.MarketDirectSberDev.IsEnabled = flagMarketing;
          _obj.State.Properties.MarketDirectSberDev.IsRequired = flagMarketing;
          _obj.State.Properties.PurchComNumberSberDev.IsEnabled = flagMarketing;
          _obj.State.Properties.PurchComNumberSberDev.IsRequired = flagMarketing;
          break;
        case ("Profitable") :
          _obj.State.Properties.TotalAmount.IsRequired = false;
          _obj.State.Properties.Currency.IsRequired = false;
          _obj.State.Properties.MVZBaseSberDev.IsEnabled = false;
          _obj.State.Properties.MVZBaseSberDev.IsRequired = false;
          _obj.State.Properties.MVPBaseSberDev.IsEnabled = true;
          _obj.State.Properties.MVPBaseSberDev.IsRequired = true;
          _obj.State.Properties.AccArtExBaseSberDev.IsEnabled = false;
          _obj.State.Properties.AccArtExBaseSberDev.IsRequired = false;
          _obj.State.Properties.AccArtPrBaseSberDev.IsEnabled = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = true;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsEnabled = false;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsRequired = false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsEnabled = _obj.MVPBaseSberDev != null ? true : false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsRequired = _obj.MVPBaseSberDev != null ? true : false;
          _obj.State.Properties.MarketDirectSberDev.IsEnabled = false;
          _obj.State.Properties.MarketDirectSberDev.IsRequired = false;
          break;
        case ("ExpendProfit") :
          _obj.State.Properties.TotalAmount.IsRequired = false;
          _obj.State.Properties.Currency.IsRequired = false;
          _obj.State.Properties.MVZBaseSberDev.IsEnabled = true;
          _obj.State.Properties.MVZBaseSberDev.IsRequired = true;
          _obj.State.Properties.MVPBaseSberDev.IsEnabled = true;
          _obj.State.Properties.MVPBaseSberDev.IsRequired = true;
          _obj.State.Properties.AccArtExBaseSberDev.IsEnabled = true;
          _obj.State.Properties.AccArtExBaseSberDev.IsRequired = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsEnabled = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = true;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsEnabled = _obj.MVZBaseSberDev != null ? true : false;
          _obj.State.Properties.ProdCollectionExBaseSberDev.IsRequired = _obj.MVZBaseSberDev != null ? true : false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsEnabled = _obj.MVPBaseSberDev != null ? true : false;
          _obj.State.Properties.ProdCollectionPrBaseSberDev.IsRequired = _obj.MVPBaseSberDev != null ? true : false;
          flagMarketing = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Name.IndexOf("Маркетинг") > 0;
          _obj.State.Properties.MarketDirectSberDev.IsEnabled = flagMarketing;
          _obj.State.Properties.MarketDirectSberDev.IsRequired = flagMarketing;
          _obj.State.Properties.PurchComNumberSberDev.IsEnabled = flagMarketing;
          _obj.State.Properties.PurchComNumberSberDev.IsRequired = flagMarketing;
          break;
      }
    }
    
    public void ChangeCalculationPropertiesAccess(Sungero.Core.Enumeration? flag)
    {
      if (flag == CalculationFlagBaseSberDev.Absolute)
      {
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsEnabled = true;
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsRequired = true;
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsVisible = true;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsEnabled = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsRequired = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsVisible = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.InterestCalc.IsVisible = false;
        
      }
      else if (flag == CalculationFlagBaseSberDev.Percent)
      {
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsEnabled = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsRequired = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsVisible = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsEnabled = true;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsRequired = true;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsVisible = true;
        _obj.State.Properties.CalculationBaseSberDev.Properties.InterestCalc.IsVisible = true;
      }
      else
      {
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsEnabled = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsRequired = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.AbsoluteCalc.IsVisible = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsEnabled = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsRequired = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.PercentCalc.IsVisible = false;
        _obj.State.Properties.CalculationBaseSberDev.Properties.InterestCalc.IsVisible = false;
      }
    }
    
    public void CancelRequiredPropeties()
    {
      if (SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        _obj.State.Properties.MVZBaseSberDev.IsRequired = false;
        _obj.State.Properties.MVPBaseSberDev.IsRequired = false;
        _obj.State.Properties.TotalAmount.IsRequired = false;
        _obj.State.Properties.Currency.IsRequired = false;
        _obj.State.Properties.ProdCollectionExBaseSberDev.IsRequired = false;
        _obj.State.Properties.ProdCollectionPrBaseSberDev.IsRequired = false;
        _obj.State.Properties.AccArtExBaseSberDev.IsRequired = false;
        _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = false;
        _obj.State.Properties.ContrTypeBaseSberDev.IsRequired = false;
        _obj.State.Properties.MarketDirectSberDev.IsRequired = false;
        _obj.State.Properties.PurchComNumberSberDev.IsRequired = false;
        PublicFunctions.ContractualDocument.Remote.ApplyAnaliticsStabs(_obj);
      }
      else
      {
        _obj.State.Properties.ContrTypeBaseSberDev.IsRequired = true;
        _obj.State.Properties.AccArtExBaseSberDev.IsRequired = (_obj.ContrTypeBaseSberDev.HasValue && _obj.ContrTypeBaseSberDev.Value == ContrTypeBaseSberDev.Expendable) ? true : false;
        _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = (_obj.ContrTypeBaseSberDev.HasValue && _obj.ContrTypeBaseSberDev.Value == ContrTypeBaseSberDev.Profitable) ? true : false;
        if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.ExpendProfitSberDev)
        {
          _obj.State.Properties.AccArtExBaseSberDev.IsRequired = true;
          _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = true;
        }
      }
    }
    
    public void CancelRequiredPropetiesByType()
    {
      var type = _obj.GetType().Name;
      string[] types = new string[]{"Contract", "SupAgreement", "ContractProxy", "SupAgreementProxy", "GuaranteeLetter", "GuaranteeLetterProxy"};
      if (!types.Contains(type))
      {
        _obj.State.Properties.MarketDirectSberDev.IsRequired = false;
        _obj.State.Properties.PurchComNumberSberDev.IsRequired = false;
      }
    }
    
    [Public]
    public void ApplyAnaliticSetup(SberContracts.IAnaticsSetup analiticSetup)
    {
      if ( _obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
      {
        _obj.State.Properties.MVZBaseSberDev.IsEnabled = !analiticSetup.MVZIsEnabled.GetValueOrDefault() ;
        _obj.State.Properties.MVPBaseSberDev.IsEnabled = !analiticSetup.MVPIsEnabled.GetValueOrDefault() ;
        _obj.State.Properties.MVZBaseSberDev.IsVisible = analiticSetup.MVZIsVisible.GetValueOrDefault() ;
        _obj.State.Properties.MVPBaseSberDev.IsVisible = analiticSetup.MVPIsVisible.GetValueOrDefault() ;
        _obj.State.Properties.MVZBaseSberDev.IsRequired = analiticSetup.MVZIsRequired.GetValueOrDefault() ;
        _obj.State.Properties.MVPBaseSberDev.IsRequired = analiticSetup.MVPIsRequired.GetValueOrDefault() ;
      }
      if ( _obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Expendable)
      {
        _obj.State.Properties.MVZBaseSberDev.IsEnabled = !analiticSetup.MVZIsEnabled.GetValueOrDefault() ;
        _obj.State.Properties.MVPBaseSberDev.IsEnabled = !analiticSetup.MVPIsEnabled.GetValueOrDefault() ;
        _obj.State.Properties.MVZBaseSberDev.IsVisible = analiticSetup.MVZIsVisible.GetValueOrDefault() ;
        _obj.State.Properties.MVPBaseSberDev.IsVisible = analiticSetup.MVZIsVisible.GetValueOrDefault() ;
        _obj.State.Properties.MVZBaseSberDev.IsRequired = analiticSetup.MVZIsRequired.GetValueOrDefault() ;
        _obj.State.Properties.MVPBaseSberDev.IsRequired = analiticSetup.MVPIsRequired.GetValueOrDefault() ;
      }
      _obj.State.Properties.AccArtPrBaseSberDev.IsRequired = analiticSetup.AccArtsIsRequired.GetValueOrDefault() ;
      _obj.State.Properties.AccArtPrBaseSberDev.IsEnabled = !analiticSetup.AccArtsIsEnabled.GetValueOrDefault() ;
      _obj.State.Properties.AccArtPrBaseSberDev.IsVisible = analiticSetup.AccArtsIsVisible.GetValueOrDefault() ;
      _obj.State.Properties.AccArtExBaseSberDev.IsRequired = analiticSetup.AccArtsIsRequired.GetValueOrDefault() ;
      _obj.State.Properties.AccArtExBaseSberDev.IsEnabled = !analiticSetup.AccArtsIsEnabled.GetValueOrDefault() ;
      _obj.State.Properties.AccArtExBaseSberDev.IsVisible = analiticSetup.AccArtsIsVisible.GetValueOrDefault() ;
      _obj.State.Properties.ProdCollectionPrBaseSberDev.IsVisible = analiticSetup.ProdIsVisible.GetValueOrDefault() ;
      _obj.State.Properties.ProdCollectionPrBaseSberDev.IsEnabled = !analiticSetup.ProdIsEnabled.GetValueOrDefault() ;
      _obj.State.Properties.ProdCollectionPrBaseSberDev.IsRequired = analiticSetup.ProdIsRequired.GetValueOrDefault() ;
      _obj.State.Properties.ProdCollectionExBaseSberDev.IsVisible = analiticSetup.ProdIsVisible.GetValueOrDefault() ;
      _obj.State.Properties.ProdCollectionExBaseSberDev.IsEnabled = !analiticSetup.ProdIsEnabled.GetValueOrDefault() ;
      _obj.State.Properties.ProdCollectionExBaseSberDev.IsRequired = analiticSetup.ProdIsRequired.GetValueOrDefault() ;
      
      CancelRequiredPropeties();
    }
  }
}