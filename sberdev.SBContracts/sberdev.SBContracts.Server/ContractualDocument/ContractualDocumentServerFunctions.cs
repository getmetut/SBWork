using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;
using sberdev.SberContracts;
using System.Reflection;
using Sungero.Domain.Shared;  // чтобы видеть IPropertyState


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
      var company = SBContracts.Companies.As(cp);
      if (company == null)
        return info;

      // Выбираем список маркеров в зависимости от типа договора
      var focusMarkers = (_obj.ContrTypeBaseSberDev == ContrTypeBaseSberDev.Profitable)
        ? PublicFunctions.Company.GetProfitableFocusMarkers(company)
        : PublicFunctions.Company.GetAllFocusMarkers(company);

      // Списки для отображения маркеров по цвету
      var redMarkers = new List<string>();
      var yellowMarkers = new List<string>();
      var noValueMarkers = new List<string>();

      var settings = centrvd.KFIntegration.Module.Company.PublicFunctions.Module.Remote.GetKFMarkersSettings();
      var colorMap = new Dictionary<Nullable<Enumeration>, Color>
      {
        { centrvd.Integration.KFMarkersSettingMarkers.Color.Red, Colors.Common.Red },
        { centrvd.Integration.KFMarkersSettingMarkers.Color.Yellow, Colors.Common.Yellow },
        { centrvd.Integration.KFMarkersSettingMarkers.Color.Green, Colors.Common.Green }
      };

      // Получаем тип контейнера свойств
      var propsContainerType = company.State.Properties.GetType();
      var propsContainer = company.State.Properties;

      // Получаем ВСЕ свойства, включая из интерфейсов
      var companyType = company.GetType();
      var allCompanyProps = new List<PropertyInfo>();
      
      // Добавляем свойства самого типа
      allCompanyProps.AddRange(companyType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
      
      // Добавляем свойства из всех интерфейсов
      foreach (var interfaceType in companyType.GetInterfaces())
      {
        allCompanyProps.AddRange(interfaceType.GetProperties());
      }
      
      // Убираем дубликаты по имени
      var uniqueProps = allCompanyProps
        .GroupBy(p => p.Name)
        .Select(g => g.First())
        .ToList();

      foreach (var propState in focusMarkers)
      {
        try
        {
          if (propState == null)
            continue;

          // Находим соответствующее свойство состояния
          var matchingStateProp = propsContainerType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(pi => ReferenceEquals(pi.GetValue(propsContainer), propState));

          if (matchingStateProp == null)
            continue;

          var propertyName = matchingStateProp.Name;

          // Ищем в расширенном списке свойств
          var companyValueProp = uniqueProps
            .FirstOrDefault(pi => pi.Name.Equals(propertyName, StringComparison.Ordinal));

          if (companyValueProp == null)
            continue;

          // Получаем значение
          var markerValue = companyValueProp.GetValue(company);
          var displayName = GetFocusPropertyDisplayName(propertyName);

          if (markerValue == null)
          {
            noValueMarkers.Add(displayName);
            continue;
          }

          var displayValue = markerValue.ToString() == "Yes" ? "Да" : markerValue.ToString() == "No" ? "Нет" : markerValue.ToString();
          var color = GetMarkerColor(propertyName, (Enumeration?)markerValue, colorMap, settings);

          if (color == Colors.Common.Red)
            redMarkers.Add($"{displayName}: {displayValue}");
          else if (color == Colors.Common.Yellow)
            yellowMarkers.Add($"{displayName}: {displayValue}");
        }
        catch (Exception ex)
        {
          Logger.Error($"Ошибка при обработке маркера '{propState?.ToString() ?? "[null]"}'", ex);
        }
      }

      // Выводим результаты
      
      // Настраеваем стиль заголовока блока
      var block = info.AddBlock();
      var blockTitleStyle = StateBlockLabelStyle.Create();
      blockTitleStyle.FontWeight = FontWeight.Bold;
      blockTitleStyle.Color = Colors.Common.Red;
      block.AddLabel(sberdev.SBContracts.ContractualDocuments.Resources.TitleCriticalMarkers, blockTitleStyle);
      block.AddLineBreak();
      
      foreach (var marker in redMarkers)
      {
        block.AddLabel(marker);
        block.AddLineBreak();
      }
      
      block = info.AddBlock();
      blockTitleStyle = StateBlockLabelStyle.Create();
      blockTitleStyle.FontWeight = FontWeight.Bold;
      blockTitleStyle.Color = Colors.Common.Yellow;
      block.AddLabel(sberdev.SBContracts.ContractualDocuments.Resources.TitleWarningMarkers, blockTitleStyle);
      block.AddLineBreak();

      foreach (var marker in yellowMarkers)
      {
        block.AddLabel(marker);
        block.AddLineBreak();
      }
      
      block = info.AddBlock();
      blockTitleStyle = StateBlockLabelStyle.Create();
      blockTitleStyle.FontWeight = FontWeight.Bold;
      blockTitleStyle.Color = Colors.Common.LightGray;
      block.AddLabel(sberdev.SBContracts.ContractualDocuments.Resources.TitleBlankMarkers, blockTitleStyle);
      block.AddLineBreak();

      if (noValueMarkers.Any())
      {
        block.AddLabel(string.Join(", ", noValueMarkers));
        block.AddLineBreak();
      }

      return info;
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

    // Получить цвет маркера из настроек Контур.Фокус
    private Color GetMarkerColor(string propertyName, Enumeration? propertyValue,
                                 Dictionary<Nullable<Enumeration>, Color> colorMap,
                                 centrvd.Integration.IKFMarkersSetting settings)
    {
      if (settings == null)
        return Colors.Empty;

      var markerSetting = SBContracts.KFMarkersSettings.As(settings).Markers
        .FirstOrDefault(marker =>
                        (marker.Name?.ToString() == propertyName ||
                         marker.Name?.ToString() == propertyName.Replace("SberDev", "")) &&
                        marker.Value == propertyValue);
      Color highlightColor;
      if (markerSetting != null && markerSetting.Color.HasValue && colorMap.TryGetValue(markerSetting.Color, out highlightColor))
        return highlightColor;

      return Colors.Empty;
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