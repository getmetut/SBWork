using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;

namespace sberdev.SBContracts.Shared
{
  partial class AccountingDocumentBaseFunctions
  {
    
    /// <summary>
    /// Изменяет доступность и обязательность полей доставки в зависимости от метода доставки и типа документа.
    /// </summary>
    public void ChangeDeliveryInfoAccess()
    {
      var props = _obj.State.Properties;
      
      // Определяем тип текущего документа
      var documentType = _obj.GetType().Name;
      string[] excludedTypes = new string[] { "IncomingInvoice", "IncomingInvoiceProxy", "OutgoingInvoice", "OutgoingInvoiceProxy" };
      var isApplicableDocumentType = !excludedTypes.Contains(documentType);
      
      // Если документ не входит в список исключаемых типов, проверяем метод доставки
      if (isApplicableDocumentType)
      {
        // Определяем способ доставки
        var deliveryMethod = _obj.DeliveryMethod;
        var isMail = deliveryMethod != null && deliveryMethod.Id == 2;
        var isCourier = deliveryMethod != null && deliveryMethod.Id == 3;
        var showDeliveryFields = isMail || isCourier;
        
        // Настраиваем видимость и обязательность полей в зависимости от способа доставки
        // Email
        props.EmailSberDevSungero.IsVisible = showDeliveryFields;
        props.EmailSberDevSungero.IsEnabled = showDeliveryFields;
        props.EmailSberDevSungero.IsRequired = isMail;
        
        // Адрес
        props.AdressSberDevSungero.IsVisible = showDeliveryFields;
        props.AdressSberDevSungero.IsEnabled = showDeliveryFields;
        props.AdressSberDevSungero.IsRequired = showDeliveryFields;
        
        // Телефон
        props.PhoneNumberSberDevSungero.IsVisible = showDeliveryFields;
        props.PhoneNumberSberDevSungero.IsEnabled = showDeliveryFields;
        props.PhoneNumberSberDevSungero.IsRequired = isCourier;
      }
      else
      {
        // Скрываем все поля доставки для исключенных типов документов
        SetDeliveryFieldsVisibility(props, false);
      }
    }

    /// <summary>
    /// Устанавливает видимость, доступность и обязательность всех полей доставки.
    /// </summary>
    /// <param name="props">Свойства объекта.</param>
    /// <param name="value">Значение для установки (true/false).</param>
    private void SetDeliveryFieldsVisibility(SBContracts.IAccountingDocumentBasePropertyStates props, bool value)
    {
      props.EmailSberDevSungero.IsVisible = value;
      props.EmailSberDevSungero.IsEnabled = value;
      props.EmailSberDevSungero.IsRequired = value;
      
      props.AdressSberDevSungero.IsVisible = value;
      props.AdressSberDevSungero.IsEnabled = value;
      props.AdressSberDevSungero.IsRequired = value;
      
      props.PhoneNumberSberDevSungero.IsVisible = value;
      props.PhoneNumberSberDevSungero.IsEnabled = value;
      props.PhoneNumberSberDevSungero.IsRequired = value;
    }
    
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      // Определяем тип текущего документа
      var documentType = _obj.GetType().Name;
      string[] excludedTypes = new string[] { "IncomingInvoice", "IncomingInvoiceProxy", "OutgoingInvoice", "OutgoingInvoiceProxy" };
      var isApplicableDocumentType = !excludedTypes.Contains(documentType);
      // Если документ не входит в список исключаемых типов, проверяем метод доставки
      if (isApplicableDocumentType)
      {
        _obj.State.Properties.DeliveryMethod.IsVisible = true;
        _obj.State.Properties.DeliveryMethod.IsEnabled = true;
        _obj.State.Properties.DeliveryMethod.IsRequired = true;
      }
    }
    
    /// <summary>
    /// Переносит информацию по доставке из таргета в объект
    /// </summary>
    public void FillDeliveryInfo(SBContracts.IContractualDocument target)
    {
      if (target != null && (target.DeliveryMethod?.Id == 3 || target.DeliveryMethod?.Id == 2))
      {
        _obj.DeliveryMethod = target.DeliveryMethod;
        _obj.AdressSberDevSungero = target.AdressSberDev;
        _obj.EmailSberDevSungero = target.EmailSberDev;
        _obj.PhoneNumberSberDevSungero = target.PhoneNumberSberDev;
      }
    }

    /// <summary>
    /// Обновление карточки докуента и открытие реквизитов по логике запроса и условий
    /// </summary>
    public void UpdateCard()
    {
      bool NMVisible = false;
      if (_obj.AccArtBaseSberDev != null)
      {
        if (_obj.AccArtBaseSberDev.NMA != null)
          NMVisible = _obj.AccArtBaseSberDev.NMA.Value;
      }
      _obj.State.Properties.NMASDev.IsVisible = NMVisible;
    }

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
        spisokCalc += _obj.ProdCollectionBaseSberDev.FirstOrDefault()?.Product.Name + " " + _obj.TotalAmount;
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
    
    public virtual void SetPropertiesAccess()
    {
      ChangeAnaliticsEnable(_obj.ContrTypeBaseSberDev);
      
      bool flagCalc = _obj.MVPBaseSberDev != null ? _obj.MVPBaseSberDev.CalculationIsWorking.Value
        : (_obj.MVZBaseSberDev != null ? _obj.MVZBaseSberDev.CalculationIsWorking.Value : false);
      
      if (_obj.ProdCollectionBaseSberDev.Any())
      {
        if ((_obj.ProdCollectionBaseSberDev.Where(p => p.Product.Name == "General").Any()
             || _obj.ProdCollectionBaseSberDev.Count > 1) && flagCalc)
          ChangeCalculationAccess(true);
        else
          ChangeCalculationAccess(false);
      }
      else
        ChangeCalculationAccess(false);
      
      ChangeCalculationPropertiesAccess(_obj.CalculationFlagBaseSberDev);
      
      if (_obj.TotalAmount == null || _obj.TotalAmount == 0)
        ChangeCalculationAccess(false);
      
      if (!flagCalc && _obj.CalculationDistributeBaseSberDev != null)
      {
        _obj.CalculationDistributeBaseSberDev = null;
        _obj.CalculationFlagBaseSberDev = null;
        _obj.CalculationBaseSberDev.Clear();
      }
      
      if (_obj.FrameworkBaseSberDev == true)
      {
        _obj.State.Properties.TotalAmount.IsEnabled = false;
        _obj.State.Properties.Currency.IsEnabled = false;
        ChangeCalculationAccess(false);
      }
      else
      {
        _obj.State.Properties.TotalAmount.IsEnabled = true;
        _obj.State.Properties.Currency.IsEnabled = true;
      }
      
      bool prodFlag = (_obj.MVZBaseSberDev != null || _obj.MVPBaseSberDev != null) ? true : false;
      _obj.State.Properties.ProdCollectionBaseSberDev.IsEnabled = prodFlag;
      _obj.State.Properties.ProdCollectionBaseSberDev.IsRequired = prodFlag;
      
      bool markFlag = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Name.IndexOf("Маркетинг") > 0;
      _obj.State.Properties.MarketDirectSberDev.IsEnabled = markFlag;
      _obj.State.Properties.MarketDirectSberDev.IsVisible = markFlag;
      _obj.State.Properties.MarketDirectSberDev.IsRequired = markFlag;
      
      bool invoiceFlag = _obj.PayTypeBaseSberDev == PayTypeBaseSberDev.Prepayment ? true : false;
      _obj.State.Properties.InvoiceSberDev.IsRequired = SBContracts.IncomingInvoices.Is(_obj) ? false : invoiceFlag;
      
      _obj.State.Properties.LeadingDocument.IsEnabled = _obj.PayTypeBaseSberDev.HasValue;
      _obj.State.Properties.InvoiceSberDev.IsEnabled = _obj.PayTypeBaseSberDev.HasValue;
      _obj.State.Properties.AccDocSberDev.IsEnabled = _obj.PayTypeBaseSberDev.HasValue;
      _obj.State.Properties.MarketingSberDev.IsEnabled = _obj.PayTypeBaseSberDev.HasValue;
      
      ChangeDeliveryInfoAccess();
      CancelRequiredPropeties();
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
    
    public void ChangeAnaliticsEnable(Sungero.Core.Enumeration? type)
    {
      if (type == ContrTypeBaseSberDev.Profitable)
      {
        _obj.State.Properties.MVZBaseSberDev.IsEnabled = false;
        _obj.State.Properties.MVZBaseSberDev.IsRequired = false;
        _obj.State.Properties.MVPBaseSberDev.IsEnabled = true;
        _obj.State.Properties.MVPBaseSberDev.IsRequired = true;
        _obj.State.Properties.TotalAmount.IsRequired = _obj.FrameworkBaseSberDev.HasValue ? !_obj.FrameworkBaseSberDev.Value : true;
        _obj.State.Properties.Currency.IsRequired = _obj.FrameworkBaseSberDev.HasValue ? !_obj.FrameworkBaseSberDev.Value : true;
      }
      if (type == ContrTypeBaseSberDev.Expendable)
      {
        _obj.State.Properties.MVZBaseSberDev.IsEnabled = true;
        _obj.State.Properties.MVZBaseSberDev.IsRequired = true;
        _obj.State.Properties.MVPBaseSberDev.IsEnabled = false;
        _obj.State.Properties.MVPBaseSberDev.IsRequired = false;
        _obj.State.Properties.TotalAmount.IsRequired = false;
        _obj.State.Properties.Currency.IsRequired = false;
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
        _obj.State.Properties.ProdCollectionBaseSberDev.IsRequired = false;
        _obj.State.Properties.AccArtBaseSberDev.IsRequired = false;
        _obj.State.Properties.ContrTypeBaseSberDev.IsRequired = false;
        _obj.State.Properties.TotalAmount.IsRequired = false;
        _obj.State.Properties.Currency.IsRequired = false;
        _obj.State.Properties.PayTypeBaseSberDev.IsRequired = false;
        _obj.State.Properties.InvoiceSberDev.IsRequired = false;
        _obj.State.Properties.EstPaymentDateSberDev.IsRequired = false;
        PublicFunctions.AccountingDocumentBase.Remote.ApplyAnaliticsStabs(_obj);
      }
      else
      {
        _obj.State.Properties.EstPaymentDateSberDev.IsRequired = SBContracts.IncomingInvoices.Is(_obj) ? false : true;
        _obj.State.Properties.PayTypeBaseSberDev.IsRequired = true;
        _obj.State.Properties.ContrTypeBaseSberDev.IsRequired = true;
        _obj.State.Properties.AccArtBaseSberDev.IsRequired = true;
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
    
    public void HighlightClosedAnalitics()
    {
      _obj.State.Properties.MVPBaseSberDev.HighlightColor = _obj.MVPBaseSberDev != null && _obj.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      _obj.State.Properties.MVZBaseSberDev.HighlightColor = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      _obj.State.Properties.AccArtBaseSberDev.HighlightColor = _obj.AccArtBaseSberDev != null &&_obj.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed ?
        Colors.Common.Red : Colors.Empty;
      var products = _obj.ProdCollectionBaseSberDev;
      bool prodFlag = true;
      foreach (var product in products)
      {
        if (product.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
          prodFlag = false;
      }
      _obj.State.Properties.ProdCollectionBaseSberDev.HighlightColor = prodFlag ? Colors.Empty : Colors.Common.Red;
    }
    
    /// <summary>
    /// Возвращает результат проверки аналитик на закрытые записи
    /// </summary>
    /// <returns>True если старых аналитик нет</returns>
    public bool CheckOldAnalicitics()
    {
      bool flag = true;
      flag = _obj.MVPBaseSberDev != null && _obj.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Active;
      flag = _obj.MVZBaseSberDev != null && _obj.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Active;
      flag = _obj.AccArtBaseSberDev != null &&_obj.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Active;
      foreach (var product in _obj.ProdCollectionBaseSberDev)
      {
        if (product.Product.Status == SberContracts.ProductsAndDevices.Status.Active)
          flag = false;
      }
      return flag;
    }
    
    [Public]
    public string ShowOldProductsCalcWarning()
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser() && _obj.State.IsInserted)
      {
        List<SberContracts.IProductsAndDevices> old = new List<SberContracts.IProductsAndDevices>();
        foreach (var prod in _obj.CalculationBaseSberDev)
          if (prod.ProductCalc.Status == SberContracts.ProductsAndDevices.Status.Closed)
            old.Add(prod.ProductCalc);
        if (old.Any())
        {
          string err = "";
          foreach (var prod in old)
            err += prod.Name + ", ";
          return "Необходимо заменить устаревшие продукты в калькуляции: " + err.Substring(0, err.Length - 2);
        }
        else
          return "";
      }
      else
        return "";
    }
  }
}