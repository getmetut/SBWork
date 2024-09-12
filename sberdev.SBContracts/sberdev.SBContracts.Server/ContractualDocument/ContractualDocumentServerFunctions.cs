using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;
using sberdev.SberContracts;

namespace sberdev.SBContracts.Server
{
  partial class ContractualDocumentFunctions
  {
    public void BeforeSaveFunction()
    {
      SendNotice();
      CreateOrUpdateAnaliticsCashe();
      ReplaceProducts();
      CreateOrUpdateAnaliticsCasheGeneral();
      _obj.CalcListSDev = PublicFunctions.ContractualDocument.GetCalculationString(_obj);
    }
    
    #region Прочие функции
    
    [Remote]
    public void SetXiongxinProps(List<int>  ids)
    {
      _obj.Counterparty = Counterparties.GetAll().Where(c => c.Id == ids[1]).FirstOrDefault();
      _obj.Currency = Sungero.Commons.Currencies.GetAll().Where(c => c.Id == ids[2]).FirstOrDefault();
      _obj.DeliveryMethod = Sungero.Docflow.MailDeliveryMethods.GetAll().Where(c => c.Id == ids[3]).FirstOrDefault();
      _obj.MVZBaseSberDev = MVZs.GetAll().Where(c => c.Id == ids[4]).FirstOrDefault();
      _obj.AccArtExBaseSberDev = AccountingArticleses.GetAll().Where(c => c.Id == ids[5]).FirstOrDefault();
    }
    
    public override IQueryable<Sungero.Docflow.ISignatureSetting> GetSignatureSettingsQuery()
    {
      var query = base.GetSignatureSettingsQuery();
      var sbQuery = query.Select(q => SBContracts.SignatureSettings.As(q));
      var prodEx = _obj.ProdCollectionExBaseSberDev.FirstOrDefault();
      var prodPr = _obj.ProdCollectionPrBaseSberDev.FirstOrDefault();
      
      if (prodEx != null && prodEx.Product.Name != "General")
        sbQuery = sbQuery.Where(q => q.ProductsSberDev.Select(qq => qq.Product).Contains(prodEx.Product));
      if (prodPr != null && prodPr.Product.Name != "General")
        sbQuery = sbQuery.Where(q => q.ProductsSberDev.Select(qq => qq.Product).Contains(prodPr.Product));
      if (!sbQuery.Any())
        sbQuery = query.Select(q => SBContracts.SignatureSettings.As(q));
      
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Expendable)
        return sbQuery.Where(q => q.ExpendableSberDev.Value);
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
        return sbQuery.Where(q => q.ProfitableSberDev.Value);
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.ExpendProfitSberDev)
        return sbQuery.Where(q => q.ExpendProfitSberDev.Value);
      return sbQuery;
    }
    
    /// <summary>
    /// Функция устанавливает значение закупочной комиссии
    /// </summary>
    /// <param name="valArr"></param>
    [Remote]
    public void SetPurchComNumber(string str)
    {
      var valArr = str.ToArray();
      _obj.PurchComNumberSberDev = valArr[0].ToString() + valArr[1].ToString() + valArr[2].ToString() + "." + valArr[3].ToString();
    }
    
    /// <summary>
    /// Если выбрано много продуктов заменяет их одним - "General"
    /// </summary>
    public void ReplaceProducts()
    {
      if (_obj.ProdCollectionExBaseSberDev.Count > 1)
      {
        _obj.ProdCollectionExBaseSberDev.Clear();
        var genProd = _obj.ProdCollectionExBaseSberDev.AddNew();
        genProd.Product = SberContracts.PublicFunctions.Module.GetOrCreateGeneralProduct(_obj);
      }
      if (_obj.ProdCollectionPrBaseSberDev.Count > 1)
      {
        _obj.ProdCollectionPrBaseSberDev.Clear();
        var genProd = _obj.ProdCollectionPrBaseSberDev.AddNew();
        genProd.Product = SberContracts.PublicFunctions.Module.GetOrCreateGeneralProduct(_obj);
      }
    }
    
    /// <summary>
    /// Функция отправляет уведомление о подписании документа с двух сторон
    /// </summary>
    public void SendNotice()
    {
      if (_obj.LocationState == "Получен через сервис обмена Диадок. Подписан обеими сторонами" && !_obj.NoticeSendBaseSberDev.GetValueOrDefault())
      {
        var subject = string.Format(Sungero.Exchange.Resources.TaskSubjectTemplate, "Подписан обеими сторонами:", Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(_obj.Name));
        var task = Sungero.Workflow.SimpleTasks.Create();
        task.Subject = subject.Length > 248 ? subject.Substring(0, 247) : subject;
        task.NeedsReview = false;
        task.Attachments.Add(_obj);
        var routeStep = task.RouteSteps.AddNew();
        routeStep.AssignmentType = Sungero.Workflow.SimpleTaskRouteSteps.AssignmentType.Notice;
        routeStep.Performer = Roles.GetAll(l => l.Sid == sberdev.SberContracts.PublicConstants.Module.NoticeBySing ).FirstOrDefault(); //_obj.Author;
        routeStep.Deadline = null;
        task.Start();
        _obj.NoticeSendBaseSberDev = true;
      }
    }
    
    #endregion
    
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
        cashe.ContrType = _obj.ContrTypeBaseSberDev;
        cashe.DocumentKind = _obj.DocumentKind;
        cashe.DocumentGroup = _obj.DocumentGroup;
        cashe.IsStandard = _obj.IsStandard;
        cashe.AccArt = _obj.AccArtPrBaseSberDev;
        cashe.AccArtMVZ = _obj.AccArtExBaseSberDev;
        cashe.MVZ = _obj.MVZBaseSberDev;
        cashe.MVP = _obj.MVPBaseSberDev;
        cashe.MarkDirection = _obj.MarketDirectSberDev;
        cashe.ExitComment = _obj.ExitCommentBaseSberDev;
        cashe.Counterparty = _obj.Counterparty;
        cashe.ProdCollection.Clear();
        var collection = _obj.ProdCollectionPrBaseSberDev;
        if (collection.Count > 0)
        {
          foreach (var str in collection)
          {
            var p = cashe.ProdCollection.AddNew();
            p.Prod = str.Product;
          }
        }
        cashe.ProdMVZCollection.Clear();
        var collectionMVZ = _obj.ProdCollectionExBaseSberDev;
        if (collectionMVZ.Count > 0)
        {
          foreach (var str in collectionMVZ)
          {
            var p = cashe.ProdMVZCollection.AddNew();
            p.ProdMVZProp = str.Product;
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
        {
          cashe = sberdev.SberContracts.AnaliticsCasheGenerals.Create();
        }
        cashe.ContrType = _obj.ContrTypeBaseSberDev;
        cashe.AccArtPr = _obj.AccArtPrBaseSberDev;
        cashe.AccArtEx = _obj.AccArtExBaseSberDev;
        cashe.MVZ = _obj.MVZBaseSberDev;
        cashe.MVP = _obj.MVPBaseSberDev;
        cashe.MarkDirection = _obj.MarketDirectSberDev;
        cashe.Counterparty = _obj.Counterparty;
        cashe.ProdCollectionPr.Clear();
        var collection = _obj.ProdCollectionPrBaseSberDev;
        if (collection.Count > 0)
        {
          foreach (var str in collection)
          {
            var p = cashe.ProdCollectionPr.AddNew();
            p.Product = str.Product;
          }
        }
        cashe.ProdCollectionEx.Clear();
        var collectionMVZ = _obj.ProdCollectionExBaseSberDev;
        if (collectionMVZ.Count > 0)
        {
          foreach (var str in collectionMVZ)
          {
            var p = cashe.ProdCollectionEx.AddNew();
            p.Product = str.Product;
          }
        }
        
        cashe.Save();
      }
    }
    
    #endregion
    
    #region Функии заглушек
    
    [Public, Remote]
    public void ApplyAnaliticsStabs()
    {
      if (_obj.ContrTypeBaseSberDev.HasValue)
        switch (_obj.ContrTypeBaseSberDev.Value.Value)
      {
        case "Expendable" :
          if (_obj.MVZBaseSberDev == null)
            _obj.MVZBaseSberDev = MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                               && m.ContrType.ToString() == "Expendable");
          if (_obj.AccArtExBaseSberDev == null)
            _obj.AccArtExBaseSberDev = AccountingArticleses.GetAll().FirstOrDefault(a => a.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                                    && a.ContrType.ToString() == "Expendable");
          if (!_obj.ProdCollectionExBaseSberDev.Any())
          {
            var prod = _obj.ProdCollectionExBaseSberDev.AddNew();
            prod.Product = ProductsAndDeviceses.GetAll().FirstOrDefault(p => p.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)");
          }
          break;
        case "Profitable" :
          if (_obj.MVPBaseSberDev == null)
            _obj.MVPBaseSberDev = MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                               && m.ContrType.ToString() == "Profitable");
          if (_obj.AccArtPrBaseSberDev == null)
            _obj.AccArtPrBaseSberDev = AccountingArticleses.GetAll().FirstOrDefault(a => a.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                                    && a.ContrType.ToString() == "Profitable");
          if (!_obj.ProdCollectionPrBaseSberDev.Any())
          {
            var prod = _obj.ProdCollectionPrBaseSberDev.AddNew();
            prod.Product = ProductsAndDeviceses.GetAll().FirstOrDefault(p => p.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)");
          }
          break;
        case "ExpendProfit" :
          if (_obj.MVZBaseSberDev == null)
            _obj.MVZBaseSberDev = MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                               && m.ContrType.ToString() == "Expendable");
          if (_obj.AccArtExBaseSberDev == null)
            _obj.AccArtExBaseSberDev = AccountingArticleses.GetAll().FirstOrDefault(a => a.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                                    && a.ContrType.ToString() == "Expendable");
          if (!_obj.ProdCollectionExBaseSberDev.Any())
          {
            var prod = _obj.ProdCollectionExBaseSberDev.AddNew();
            prod.Product = ProductsAndDeviceses.GetAll().FirstOrDefault(p => p.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)");
          }
          if (_obj.MVPBaseSberDev == null)
            _obj.MVPBaseSberDev = MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                               && m.ContrType.ToString() == "Profitable");
          if (_obj.AccArtPrBaseSberDev == null)
            _obj.AccArtPrBaseSberDev = AccountingArticleses.GetAll().FirstOrDefault(a => a.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                                    && a.ContrType.ToString() == "Profitable");
          if (!_obj.ProdCollectionPrBaseSberDev.Any())
          {
            var prod = _obj.ProdCollectionPrBaseSberDev.AddNew();
            prod.Product = ProductsAndDeviceses.GetAll().FirstOrDefault(p => p.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)");
          }
          break;
      }
      else
      {
        _obj.ContrTypeBaseSberDev = ContrTypeBaseSberDev.Profitable;
        if (_obj.MVPBaseSberDev == null)
          _obj.MVPBaseSberDev = MVZs.GetAll().FirstOrDefault(m => m.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                             && m.ContrType.ToString() == "Profitable");
        if (_obj.AccArtExBaseSberDev == null)
          _obj.AccArtExBaseSberDev = AccountingArticleses.GetAll().FirstOrDefault(a => a.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                                  && a.ContrType.ToString() == "Expendable");
        if (!_obj.ProdCollectionPrBaseSberDev.Any())
        {
          var prod = _obj.ProdCollectionExBaseSberDev.AddNew();
          prod.Product = ProductsAndDeviceses.GetAll().FirstOrDefault(p => p.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)");
        }
      }
      
      _obj.DeliveryMethod = Sungero.Docflow.MailDeliveryMethods.GetAll().FirstOrDefault(d => d.Id == 1);
    }
    
    #endregion
  }
}