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
    [Public, Remote]
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
    
    [Remote]
    public void SetXiongxinProps(List<int>  ids)
    {
      _obj.Counterparty = Counterparties.GetAll().Where(c => c.Id == ids[1]).FirstOrDefault();
      _obj.Currency = Sungero.Commons.Currencies.GetAll().Where(c => c.Id == ids[2]).FirstOrDefault();
      _obj.DeliveryMethod = Sungero.Docflow.MailDeliveryMethods.GetAll().Where(c => c.Id == ids[3]).FirstOrDefault();
      _obj.MVZBaseSberDev = MVZs.GetAll().Where(c => c.Id == ids[4]).FirstOrDefault();
      _obj.AccArtExBaseSberDev = AccountingArticleses.GetAll().Where(c => c.Id == ids[5]).FirstOrDefault();
    }
    public void BeforeSaveFunction()
    {
      SendNotice();
      CreateOrUpdateAnaliticsCashe();
      ReplaceProducts();
      CreateOrUpdateAnaliticsCasheGeneral();
      _obj.CalcListSDev = PublicFunctions.ContractualDocument.GetCalculationString(_obj);
    }
    
    #region Прочие функции
      
    [Public]
    public StateView ShowLegalInfo(StateView info)
    {
      var cp = _obj.Counterparty;
      
      // Определяем, какой список маркеров использовать
      var focusMarkers = (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
        ? PublicFunctions.Company.GetProfitableFocusMarkers(Companies.As(cp))
        : PublicFunctions.Company.GetAllFocusMarkers(Companies.As(cp));
      
      var coloredMarkers = new List<string>(); // Для красных и желтых маркеров
      var emptyColorMarkers = new List<string>(); // Для бесцветных маркеров
      
      // Получаем состояние свойств контрагента
      var cpPropsType = cp.State.Properties.GetType();
      
      foreach (var marker in focusMarkers)
      {
        // Получаем IPropertyState для маркера
        var propState = cpPropsType.GetProperty(marker.ToString())?.GetValue(cp.State.Properties) as Sungero.Domain.Shared.IPropertyState;
        
        if (propState != null)
        {
          // Получаем значение маркера  
          var markerValue = cp.GetType().GetProperty(marker.ToString())?.GetValue(cp);
          var displayValue = markerValue?.ToString() == "Yes" ? "Да" : "Нет";
          var displayName = GetPropertyDisplayName(marker.ToString());
          
          // Проверяем цвет
          if (propState.HighlightColor == Colors.Empty)
          {
            emptyColorMarkers.Add($"{displayName}: {displayValue}");
          }
          else
          {
            // Красный или желтый цвет
            coloredMarkers.Add($"{displayName}: {displayValue}");
          }
        }
      }
      
      // Выводим красные и желтые маркеры по отдельности
      foreach (var marker in coloredMarkers)
      {
        var block = info.AddBlock();
        block.AddLabel(marker);
        block.AddLineBreak();
      }
      
      // Выводим бесцветные маркеры одной строкой в конце
      if (emptyColorMarkers.Any())
      {
        var block = info.AddBlock();
        block.AddLabel(string.Join(", ", emptyColorMarkers));
        block.AddLineBreak();
      }
      
      return info;
    }

    // Вспомогательная функция для получения отображаемого имени свойства
    private string GetPropertyDisplayName(string propertyName)
    {
      var displayNames = new Dictionary<string, string>
      {
        { "IsResident", "Резидент РФ" },
        { "IsRoaming", "Роуминговая компания" },
        { "HasEDS", "Наличие ЭЦП" },
        // Добавьте другие маркеры для обычных и Profitable документов
      };
      
      return displayNames.ContainsKey(propertyName)
        ? displayNames[propertyName]
        : propertyName;
    }

    // Вспомогательная функция для получения отображаемого имени свойства
    private string GetFocusPropertyDisplayName(string propertyName)
    {
      var displayNames = new Dictionary<string, string>
      {
        { "IsResident", "Резидент РФ" },
        { "IsRoaming", "Роуминговая компания" },
        { "HasEDS", "Наличие ЭЦП" },
        // Добавьте другие маркеры
      };
      
      return displayNames.ContainsKey(propertyName)
        ? displayNames[propertyName]
        : propertyName;
    }
    
    public override List<Sungero.Docflow.IApprovalRuleBase> GetApprovalRules()
    {
      var query = base.GetApprovalRules();
      if (SBContracts.SupAgreements.Is(_obj) || SBContracts.Contracts.Is(_obj))
      {
        var sbQuery = query.Select(q => SBContracts.ContractsApprovalRules.As(q));
        
        if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Expendable)
          sbQuery = sbQuery.Where(q => q.ExpendableSberDev == true);
        if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
          sbQuery = sbQuery.Where(q => q.ProfitableSberDev == true);
        if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.ExpendProfitSberDev)
          sbQuery = sbQuery.Where(q => q.ExpendProfitSberDev == true);
        
        return sbQuery.Where(q => q.TypicalSberDev == _obj.IsStandard).Select(q => Sungero.Docflow.ApprovalRuleBases.As(q)).ToList();
      }
      return query;
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
        return sbQuery.Where(q => q.ExpendableSberDev == true);
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
        return sbQuery.Where(q => q.ProfitableSberDev == true);
      if (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.ExpendProfitSberDev)
        return sbQuery.Where(q => q.ExpendProfitSberDev == true);
      return sbQuery;
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
  }
}