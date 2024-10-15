using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.AccountingDocumentBase;
using sberdev.SberContracts;

namespace sberdev.SBContracts.Server
{
  partial class AccountingDocumentBaseFunctions
  {
    #region Функции кнопок автозаполнения
    
    /// <summary>
    /// Функция создает или обновляет элемент справочника AnaliticsCashe
    /// </summary>
    public void CreateOrUpdateAnaliticsCashe()
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        var cashe = sberdev.SberContracts.AnaliticsCashes.GetAll(j => j.User == Users.Current && j.Counterparty == _obj.Counterparty).FirstOrDefault();
        if (cashe == null)
        {
          cashe = sberdev.SberContracts.AnaliticsCashes.Create();
        }
        cashe.PayTypeBaseSberDev = _obj.PayTypeBaseSberDev;
        cashe.ContrType = _obj.ContrTypeBaseSberDev;
        cashe.AccArt = _obj.AccArtBaseSberDev;
        cashe.MVZ = _obj.MVZBaseSberDev;
        cashe.MVP = _obj.MVPBaseSberDev;
        cashe.MarkDirection = _obj.MarketDirectSberDev;
        cashe.ProdCollection.Clear();
        cashe.Counterparty = _obj.Counterparty;
        var collection = _obj.ProdCollectionBaseSberDev;
        if ( collection.Count > 0)
        {
          foreach ( var str in collection )
          {
            var p = cashe.ProdCollection.AddNew();
            p.Prod = str.Product;
          }
        }
        cashe.Save();
      }
    }
    
    public void CreateOrUpdateAnaliticsCasheGeneral()
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        var cashe = sberdev.SberContracts.AnaliticsCasheGenerals.GetAll(j => j.User == Users.Current).FirstOrDefault();
        
        if (cashe == null)
          cashe = sberdev.SberContracts.AnaliticsCasheGenerals.Create();
        else
          SberContracts.PublicFunctions.AnaliticsCasheGeneral.ClearByUser(cashe, Users.Current);
        
        
        cashe.ContrType = _obj.ContrTypeBaseSberDev;
        cashe.PayTypeBaseSberDev = _obj.PayTypeBaseSberDev;
        if (cashe.ContrType == SberContracts.AnaliticsCasheGeneral.ContrType.Profitable)
        {
          cashe.MVP = _obj.MVPBaseSberDev;
          cashe.AccArtPr = _obj.AccArtBaseSberDev;
          cashe.ProdCollectionPr.Clear();
          var collection = _obj.ProdCollectionBaseSberDev;
          if (collection.Count > 0)
          {
            foreach (var str in collection)
            {
              var p = cashe.ProdCollectionPr.AddNew();
              p.Product = str.Product;
            }
          }
        }
        
        if (cashe.ContrType == SberContracts.AnaliticsCasheGeneral.ContrType.Expendable)
        {
          cashe.MVZ = _obj.MVZBaseSberDev;
          cashe.AccArtEx = _obj.AccArtBaseSberDev;
          cashe.MarkDirection = _obj.MarketDirectSberDev;
          cashe.ProdCollectionEx.Clear();
          var collection = _obj.ProdCollectionBaseSberDev;
          if (collection.Count > 0)
          {
            foreach (var str in collection)
            {
              var p = cashe.ProdCollectionEx.AddNew();
              p.Product = str.Product;
            }
          }
        }
        
        cashe.Counterparty = _obj.Counterparty;
        cashe.Save();
      }
    }
    
    #endregion
    
    /// <summary>
    /// Если выбрано много продуктов заменяет их одним - "General"
    /// </summary>
    public void ReplaceProducts()
    {
      if (_obj.ProdCollectionBaseSberDev.Count > 1)
      {
        _obj.ProdCollectionBaseSberDev.Clear();
        var genProd = _obj.ProdCollectionBaseSberDev.AddNew();
        genProd.Product = SberContracts.PublicFunctions.Module.GetOrCreateGeneralProduct(_obj);
      }
    }
    
    #region Функии заглушек
    
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
        if (_obj.AccArtBaseSberDev != null && _obj.AccArtBaseSberDev.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")
          error += ", Статья упр. учета";
        if (_obj.ProdCollectionBaseSberDev.FirstOrDefault() != null
            && _obj.ProdCollectionBaseSberDev.FirstOrDefault().Product.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)")
          error += ", Продукт";
        
        if (error != "")
        {
          error = "Выберите нужные значения вместо заглушек в полях:" + error.TrimStart(',') + ". Документ: " + _obj.Name;
        }
      }
      return error;
    }
    
    [Public, Remote]
    public void ApplyAnaliticsStabs()
    {
      if (_obj.ContrTypeBaseSberDev.HasValue)
        switch (_obj.ContrTypeBaseSberDev.Value.Value)
      {
        case "Expendable" :
          if (_obj.MVZBaseSberDev == null)
            _obj.MVZBaseSberDev = PublicFunctions.Module.GetMVZStab("Expendable");
          break;
        case "Profitable" :
          if (_obj.MVPBaseSberDev == null)
            _obj.MVPBaseSberDev = PublicFunctions.Module.GetMVZStab("Profitable");
          break;
        case "ExpendProfit" :
          if (_obj.MVZBaseSberDev == null)
            _obj.MVZBaseSberDev = PublicFunctions.Module.GetMVZStab("Expendable");
          if (_obj.MVPBaseSberDev == null)
            _obj.MVPBaseSberDev = PublicFunctions.Module.GetMVZStab("Profitable");
          break;
      }
      else
      {
        _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Profitable;
        if (_obj.MVPBaseSberDev == null)
          _obj.MVPBaseSberDev = PublicFunctions.Module.GetMVZStab("Profitable");
      }
      
      if (_obj.AccArtBaseSberDev == null)
        _obj.AccArtBaseSberDev = PublicFunctions.Module.GetAccountingArticleStab("Expendable");
      if (!_obj.ProdCollectionBaseSberDev.Any())
      {
        var prod = _obj.ProdCollectionBaseSberDev.AddNew();
        prod.Product = PublicFunctions.Module.GetProductStab();
      }
    }
    
    #endregion
    
    public override IQueryable<Sungero.Docflow.ISignatureSetting> GetSignatureSettingsQuery()
    {
      var query = base.GetSignatureSettingsQuery();
      var sbQuery = query.Select(q => SBContracts.SignatureSettings.As(q));
      var prod = _obj.ProdCollectionBaseSberDev.FirstOrDefault();
      
      if (prod != null && prod.Product.Name != "General")
        sbQuery = sbQuery.Where(q => q.ProductsSberDev.Select(qq => qq.Product).Contains(prod.Product));
      if (!sbQuery.Any())
        sbQuery = query.Select(q => SBContracts.SignatureSettings.As(q));
      
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Expendable)
        return sbQuery.Where(q => q.ExpendableSberDev.Value);
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
        return sbQuery.Where(q => q.ProfitableSberDev.Value);
      return sbQuery;
    }
  }
}