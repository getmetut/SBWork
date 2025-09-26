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
    /// <summary>
    /// Обновить интерфейс карточки в зависимости от условий.
    /// </summary>
    public void UpdInterface()
    {
      // === Флаги состояния ===
      var isNonresident = _obj.Nonresident.HasValue && _obj.Nonresident.Value;   // true = нерезидент
      var isIPSberDev   = _obj.IPSberDev.HasValue && _obj.IPSberDev.Value;       // true = ИП СберДев
      var hasHeadCompany = _obj.HeadCompany != null;

      // === Блок ИНН/КПП (TIN/TRRC) в зависимости от резидентности и ИП СберДев ===
      // Правила:
      // - Нерезидент: TIN/TRRC не обязательны, TRRC скрыт.
      // - IPSberDev: TRRC не обязателен и скрыт.
      // - Nonresident == null трактуем как "резидент" по умолчанию (как в исходнике).
      var trrcMustBeVisible = !isNonresident && !isIPSberDev;
      _obj.State.Properties.TIN.IsRequired  = !isNonresident;   // при нерезиденте — false
      _obj.State.Properties.TRRC.IsRequired = !isNonresident && !isIPSberDev;
      _obj.State.Properties.TRRC.IsVisible  = trrcMustBeVisible;

      // === Блок "ИП СберДев" / "Физлицо" / "Головная компания" / "Нерезидент" ===
      // При IPSberDev показываем и требуем PersonSberDev, прячем HeadCompany и Nonresident.
      _obj.State.Properties.PersonSberDev.IsVisible  = isIPSberDev;
      _obj.State.Properties.PersonSberDev.IsRequired = isIPSberDev;

      _obj.State.Properties.HeadCompany.IsVisible = !isIPSberDev;
      _obj.State.Properties.Nonresident.IsVisible = !isIPSberDev;

      // === Автозаполнение Name1CSberDev при наличии Name ===
      if (_obj.Name1CSberDev == null && _obj.Name != null)
        _obj.Name1CSberDev = ModifyCompanyName(_obj.Name);

      // === Доступность HeadOrgSDev зависит от наличия HeadCompany ===
      _obj.State.Properties.HeadOrgSDev.IsEnabled = !hasHeadCompany;
    }
    
    /// <summary>
    /// Преобразует полное наименование компании в сокращённый формат.
    /// </summary>
    public static System.String ModifyCompanyName(System.String companyName)
    {
      if (companyName == null)
        return null;

      // === 1) Базовая нормализация ===
      var s = companyName.Trim();

      // Нормализуем кавычки
      s = s.Replace('«', '"').Replace('»', '"')
        .Replace('“', '"').Replace('”', '"').Replace('„', '"').Replace('‟', '"');

      // Схлопываем «О.О.О.» / «З.А.О.» и т.п. → «ООО» / «ЗАО»
      s = System.Text.RegularExpressions.Regex.Replace(
        s,
        @"\b([А-Я])\.(?:\s*([А-Я])\.){1,}\s*([А-Я])\.?\b",
        m => (m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value),
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

      // Схлопываем повторные пробелы
      s = System.Text.RegularExpressions.Regex.Replace(
        s, @"\s+", " ",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled).Trim();

      // === 2) Приводим длинные формы к аббревиатурам ===
      //   (делаем замены аккуратно, как отдельные словоформы)
      //   Порядок важен: сначала самые длинные.
      s = ReplaceWordIgnoreCase(s, "Публичное акционерное общество", "ПАО");
      s = ReplaceWordIgnoreCase(s, "Открытое акционерное общество", "ОАО");
      s = ReplaceWordIgnoreCase(s, "Закрытое акционерное общество", "ЗАО");
      s = ReplaceWordIgnoreCase(s, "Общество с ограниченной ответственностью", "ООО");
      s = ReplaceWordIgnoreCase(s, "Акционерное общество", "АО");
      s = ReplaceWordIgnoreCase(s, "Индивидуальный предприниматель", "ИП");

      // Бывает «ООО.», «ООО,» — убираем пунктуацию прямо рядом с формой.
      s = System.Text.RegularExpressions.Regex.Replace(
        s,
        @"\b(ООО|ПАО|ОАО|ЗАО|АО|ИП)[\.,]?\b",
        m => m.Groups[1].Value,
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

      // === 3) Выделяем юридическую форму где бы она ни стояла ===
      var formMatch = System.Text.RegularExpressions.Regex.Match(
        s,
        @"(?:^|[\s,""()])(?<f>ООО|ПАО|ОАО|ЗАО|АО|ИП)(?=$|[\s,""()])",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

      if (!formMatch.Success)
        return companyName; // Форму не нашли — возвращаем как есть (консервативно)

      var form = formMatch.Groups["f"].Value.ToUpperInvariant();

      // Удаляем все вхождения формы и прилипшую пунктуацию/пробелы
      var withoutForm = System.Text.RegularExpressions.Regex.Replace(
        s,
        @"(^|[\s,""()])(?:(ООО|ПАО|ОАО|ЗАО|АО|ИП))(?=($|[\s,""()]))[\s,""()]*",
        " ",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

      // Чистим хвосты: кавычки по краям, лишние пробелы, запятые
      var name = withoutForm.Trim().Trim('"', ' ', ',', '—', '-', '«', '»');
      name = System.Text.RegularExpressions.Regex.Replace(
        name, @"\s+", " ",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled).Trim();

      if (name.Length == 0)
        return companyName; // на всякий случай, чтобы не вернуться к « ПАО»

      // Убираем внешние кавычки у имени, но внутренние сохраняем как символы
      if (name.Length >= 2 && name[0] == '"' && name[name.Length - 1] == '"')
        name = name.Substring(1, name.Length - 2).Trim();

      // Верхний регистр для имени по-русски
      var ru = new System.Globalization.CultureInfo("ru-RU");
      name = name.ToUpper(ru);

      // === 4) Формируем итог ===
      var result = name + " " + form;

      return result;
    }
    
    // ===== локальная вспомогательная функция =====
    private static System.String ReplaceWordIgnoreCase(System.String source, System.String longForm, System.String shortForm)
    {
      // Меняем только как цельные фразы (границы слов/разделители), нечувствительно к регистру.
      // Разрешаем пробелы/кавычки/скобки/запятые рядом.
      var pattern = @"(?<=^|[\s,""()])" +
        System.Text.RegularExpressions.Regex.Escape(longForm) +
        @"(?=$|[\s,""()])";

      return System.Text.RegularExpressions.Regex.Replace(
        source, pattern, shortForm,
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);
    }

    #region Маркеры Фокус
    [Public]
    
    #region Геты
    
    /// <summary>
    /// Правила вычисления статуса проверки Контур-Фокус.
    /// Вынесено отдельно для единообразия.
    /// </summary>
    public Nullable<Sungero.Core.Enumeration> GetFocusStatus()
    {
      // Лёгкая ветка: была успешная проверка.
      if (_obj.FocusCheckedSberDev == true && _obj.FocusCheckedDateSberDev.HasValue)
      {
        if (_obj.FocusCheckedDateSberDev.Value.Year == Calendar.Now.Year)
          return FocusCheckStatusSberDev.Actual;
        else
          return FocusCheckStatusSberDev.DateOutdated;
      }
      
      // Сменился маркер у контрагента
      if (_obj.FocusCheckedSberDev == false)
        return FocusCheckStatusSberDev.MarkerChanged;

      // Сложная ветка: есть «бракующая» проверка/этап (CheckingCPStage).
      var hasCheckingAssign = PublicFunctions.Company.GetCheckingAssignments(_obj).Any();
      if (hasCheckingAssign)
        return FocusCheckStatusSberDev.Failed;

      // По умолчанию — оставляем null.
      return null;
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
        company.State.Properties.RelOrgAccount, company.State.Properties.ArbPractice,
        company.State.Properties.RelOrgExecCases,company.State.Properties.LowAvgHeadSberDev
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
        company.State.Properties.OrgBlocking, company.State.Properties.RelOrgToLiquid,
        company.State.Properties.RelOrgBankrupt, company.State.Properties.Unprofitable,
        company.State.Properties.TaxArrears, company.State.Properties.CaseAmountTo,
        company.State.Properties.ExecCaseAmount, company.State.Properties.Bankruptcy,
        company.State.Properties.BankruptcyTo, company.State.Properties.BankruptcyEnd,
        company.State.Properties.ArbCaseTo, company.State.Properties.BankruptcyTo,
        company.State.Properties.ToLiquidation, company.State.Properties.HeadBankrupt,
        company.State.Properties.HeadDisqual, company.State.Properties.GainDown,
        company.State.Properties.PledgedProperty, company.State.Properties.CreditPayments,
        company.State.Properties.TaxesAndFees, company.State.Properties.Seizure
      };
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
    #endregion
    
    #region Контроль
    
    /// <summary>
    /// Переключить показ маркеров Контур.Фокус.
    /// </summary>
    /// <param name="company">Карточка компании.</param>
    /// <param name="showProfitableOnly">True — показывать только доходные, False — все.</param>
    public static void ShowFocusMarkers(sberdev.SBContracts.ICompany company, bool showProfitableOnly)
    {
      if (company == null)
        return;

      var all = GetAllFocusMarkers(company);
      var profitable = GetProfitableFocusMarkers(company);

      // Сформировать объединение, чтобы точно скрыть всё, что относится к маркерам.
      var toHide = new System.Collections.Generic.List<Sungero.Domain.Shared.IPropertyState>();
      if (all != null)
        toHide.AddRange(all);
      if (profitable != null)
      {
        foreach (var ps in profitable)
        {
          var exists = false;
          foreach (var x in toHide)
          {
            if (object.ReferenceEquals(x, ps))
            { exists = true; break; }
          }
          if (!exists)
            toHide.Add(ps);
        }
      }

      // Скрыть всё.
      foreach (var st in toHide)
        st.IsVisible = false;

      // Показать нужный набор.
      var toShow = showProfitableOnly ? profitable : all;
      if (toShow != null)
      {
        foreach (var st in toShow)
          st.IsVisible = true;
      }
      
      HideEmptyFocusMarkers(company);
    }

    /// <summary>
    /// Функция скрывает белые маркеры Контур.Фокус
    /// </summary>
    private static void HideEmptyFocusMarkers(sberdev.SBContracts.ICompany company)
    {
      List<Sungero.Domain.Shared.IPropertyState> focusProps = GetAllFocusMarkers(company);
      foreach (var prop in focusProps)
      {
        if (focusProps.Contains(prop) && prop.HighlightColor == Colors.Empty)
          prop.IsVisible = false;
      }
    }
    
    #endregion
    
    #region Сеты
    
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
    #endregion
    
    #endregion
  }
}