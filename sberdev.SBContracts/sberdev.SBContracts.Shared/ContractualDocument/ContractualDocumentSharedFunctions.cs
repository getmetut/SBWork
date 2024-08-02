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
    
    #region Общая
    
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
        _obj.State.Properties.TotalAmount.IsEnabled = false;
        _obj.State.Properties.Currency.IsEnabled = false ;
        ChangeCalculationAccess(false);
      }
      else
      {
        _obj.State.Properties.TotalAmount.IsEnabled = true;
        _obj.State.Properties.Currency.IsEnabled = true;
      }
      
      ChangePropertiesAccessByKind();
      CancelRequiredPropeties();
    }
    #endregion
    
    #region Контроля калькуляции
    
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
    #endregion
    
    #region Контроль аналитик
    
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
          _obj.State.Properties.PurchComNumberSberDev.IsVisible = false;
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
          _obj.State.Properties.PurchComNumberSberDev.IsEnabled = false;
          _obj.State.Properties.PurchComNumberSberDev.IsRequired = false;
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
    
    #endregion
    
    #region Разные по аналитикам
    
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
    
    public void HighlightClosedAnalitics()
    {
      _obj.State.Properties.MVPBaseSberDev.HighlightColor = _obj.MVPBaseSberDev != null && _obj.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      _obj.State.Properties.MVZBaseSberDev.HighlightColor = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      _obj.State.Properties.AccArtExBaseSberDev.HighlightColor = _obj.AccArtExBaseSberDev != null && _obj.AccArtExBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      _obj.State.Properties.AccArtPrBaseSberDev.HighlightColor = _obj.AccArtPrBaseSberDev != null && _obj.AccArtPrBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      
      bool prodFlag = true;
      foreach (var product in _obj.ProdCollectionExBaseSberDev)
      {
        if (product.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
          prodFlag = false;
      }
      _obj.State.Properties.ProdCollectionExBaseSberDev.HighlightColor = prodFlag ? Colors.Empty : Colors.Common.Red;
      
      prodFlag = true;
      foreach (var product in _obj.ProdCollectionPrBaseSberDev)
      {
        if (product.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
          prodFlag = false;
      }
      _obj.State.Properties.ProdCollectionPrBaseSberDev.HighlightColor = prodFlag ? Colors.Empty : Colors.Common.Red;
    }
    
    public bool CheckOldAnalicitics()
    {
      bool flag = true;
      flag = _obj.MVPBaseSberDev != null && _obj.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed;
      flag = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed;
      flag = _obj.AccArtExBaseSberDev != null && _obj.AccArtExBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed;
      flag = _obj.AccArtPrBaseSberDev != null && _obj.AccArtPrBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed;
      foreach (var product in _obj.ProdCollectionExBaseSberDev)
      {
        if (product.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
          flag = false;
      }
      foreach (var product in _obj.ProdCollectionPrBaseSberDev)
      {
        if (product.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
          flag = false;
      }
      return flag;
    }
    
    #endregion
    
    #region Прочие
    
    
    /// <summary>
    /// Формирует строку с продуктами
    /// </summary>
    [Public]
    public string GetCalculationString()
    {
      string spisokCalc = "";
      if (_obj.CalculationBaseSberDev.Count > 0)
      {
        if (_obj.CalculationFlagBaseSberDev == CalculationFlagBaseSberDev.Absolute)
          foreach (var elem in _obj.CalculationBaseSberDev)
            spisokCalc += elem.ProductCalc.Name + " " + Math.Round((decimal)elem.AbsoluteCalc.Value, 2) + "; ";
          else
            foreach (var elem in _obj.CalculationBaseSberDev)
              spisokCalc += elem.ProductCalc.Name + " " + Math.Round((decimal)elem.InterestCalc.Value, 2) + "; ";
      }
      else
      {
        spisokCalc += _obj.ProdCollectionExBaseSberDev.FirstOrDefault()?.Product.Name + " ";
        if (_obj.ProdCollectionPrBaseSberDev.Count > 0)
          spisokCalc += _obj.ProdCollectionPrBaseSberDev.FirstOrDefault()?.Product.Name + " ";
        spisokCalc += _obj.TotalAmount;
      }
      spisokCalc = spisokCalc.Substring(0, Math.Min(spisokCalc.Length, 999)).TrimEnd(';');
      return spisokCalc;
    }
    
    [Public]
    public Sungero.CoreEntities.IUser GetMVZBudgetOwner()
    {
      if (_obj.MVZBaseSberDev != null)
        if (_obj.MVZBaseSberDev.MainMVZ != null)
          return _obj.MVZBaseSberDev.MainMVZ.BudgetOwner;
        else
          return _obj.MVZBaseSberDev.BudgetOwner;
      if (_obj.MVPBaseSberDev != null)
        if (_obj.MVPBaseSberDev.MainMVZ != null)
          return _obj.MVPBaseSberDev.MainMVZ.BudgetOwner;
        else
          return _obj.MVPBaseSberDev.BudgetOwner;
      return null;
    }
    
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      _obj.State.Properties.DeliveryMethod.IsVisible = true;
      _obj.State.Properties.DeliveryMethod.IsEnabled = true;
      _obj.State.Properties.DeliveryMethod.IsRequired = true;
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
    
    public void ChangePropertiesAccessByKind()
    {
      if (_obj.DocumentKind == null)
        return;
      var name = _obj.DocumentKind.Name;
      if (name == "Договор Xiongxin" || name == "Дополнительное соглашение Xiongxin")
      {
        _obj.State.Properties.AgentSaluteSberDev.IsVisible = true;
        _obj.State.Properties.AgentSaluteSberDev.IsRequired = true;
        _obj.State.Properties.DelPeriodSberDev.IsVisible = true;
        _obj.State.Properties.DelPeriodSberDev.IsRequired = true;
        _obj.State.Properties.AmountPrepaySberDev.IsVisible = true;
        _obj.State.Properties.AmountPrepaySberDev.IsRequired = true;
        _obj.State.Properties.AmountPostpaySberDev.IsVisible = true;
        _obj.State.Properties.AmountPostpaySberDev.IsRequired = true;
        _obj.State.Properties.DeadlinePrepaySberDev.IsVisible = true;
        _obj.State.Properties.DeadlinePrepaySberDev.IsRequired = true;
        _obj.State.Properties.AdressSberDev.IsVisible = true;
        _obj.State.Properties.PhoneNumberSberDev.IsVisible = true;
        _obj.State.Properties.EmailSberDev.IsVisible = true;
        _obj.State.Properties.FrameworkBaseSberDev.IsEnabled = false;
        _obj.State.Properties.ValidFrom.IsRequired = true;
        _obj.State.Properties.ValidTill.IsRequired = true;
      }
      else
      {
        _obj.State.Properties.AgentSaluteSberDev.IsVisible = false;
        _obj.State.Properties.AgentSaluteSberDev.IsRequired = false;
        _obj.State.Properties.DelPeriodSberDev.IsVisible = false;
        _obj.State.Properties.DelPeriodSberDev.IsRequired = false;
        _obj.State.Properties.AmountPrepaySberDev.IsVisible = false;
        _obj.State.Properties.AmountPrepaySberDev.IsRequired = false;
        _obj.State.Properties.AmountPostpaySberDev.IsVisible = false;
        _obj.State.Properties.AmountPostpaySberDev.IsRequired = false;
        _obj.State.Properties.DeadlinePrepaySberDev.IsVisible = false;
        _obj.State.Properties.DeadlinePrepaySberDev.IsRequired = false;
        _obj.State.Properties.AdressSberDev.IsVisible = false;
        _obj.State.Properties.PhoneNumberSberDev.IsVisible = false;
        _obj.State.Properties.EmailSberDev.IsVisible = false;
        _obj.State.Properties.FrameworkBaseSberDev.IsEnabled = true;
        _obj.State.Properties.ValidFrom.IsRequired = false;
        _obj.State.Properties.ValidTill.IsRequired = false;
      }
      if (name == "Счет-договор" || name == "Договор-оферта")
      {
        _obj.State.Properties.FrameworkBaseSberDev.IsVisible = false;
      }
      else
      {
        _obj.State.Properties.FrameworkBaseSberDev.IsVisible = true;
      }
    }
    
    /// <summary>
    /// Функция возвращает текст ошибки, если в каком либо поле выбраны заглушки
    /// </summary>
    [Public]
    public string BanToSaveForStabs()
    {
      var error = "";
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        if (_obj.MVPBaseSberDev != null && _obj.MVPBaseSberDev.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")
          error += ", МВП";
        if (_obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")
          error += ", МВЗ";
        if (_obj.AccArtExBaseSberDev != null && _obj.AccArtExBaseSberDev.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")
          error += ", Статья упр. учета (рас.)";
        if (_obj.AccArtPrBaseSberDev != null && _obj.AccArtPrBaseSberDev.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")
          error += ", Статья упр. учета (дох.)";
        if (_obj.ProdCollectionExBaseSberDev.FirstOrDefault() != null
            && _obj.ProdCollectionExBaseSberDev.Select(p => p.Product.Name).Any(p => p == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"))
          error += ", Продукт (рас.)";
        if (_obj.ProdCollectionPrBaseSberDev.FirstOrDefault() != null
            && _obj.ProdCollectionPrBaseSberDev.Select(p => p.Product.Name).Any(p => p == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"))
          error += ", Продукт (дох.)";
        
        if (error != "")
        {
          error = "Выберите нужные значения вместо заглушек в полях:" + error.TrimStart(',') + ". Документ: " + _obj.Name;
        }
      }
      return error;
    }
    
    #endregion
    
  }
}