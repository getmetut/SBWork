using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Shared
{
  public class ModuleFunctions
  {
    
    #region Текстовые
    
    [Public]
    // Вспомогательная функция для получения отображаемого имени свойства
    public string GetFocusPropertyDisplayName(string propertyName)
    {
      var displayNames = new Dictionary<string, string>
      {
        { "OrgLiquidated", "Организация ликвидирована или ликвидируется" },
        { "RevokedLicense", "Наличие отозванных лицензий" },
        { "OrgBlocking", "Блокировка банковского счета организации" },
        { "Unprofitable", "Деятельность предприятия убыточна" },
        { "LargeLE", "Значительное количество юридических лиц руководителя (с учетом ИННФЛ)" },
        { "EPWages", "Исполнительные производства по заработной плате" },
        { "Bankruptcy", "Намерение подать иск о банкротстве" },
        { "AddressInfo", "Недостоверность сведений об адресе" },
        { "HeadInfo", "Недостоверные сведения о руководителе или об учредителе" },
        { "BankruptcyTo", "Обнаружены арбитражные дела о банкротстве в качестве ответчика" },
        { "BankruptcyEnd", "Обнаружены признаки завершенной процедуры банкротства" },
        { "ToBankruptcy", "Обнаружены сообщения о текущей процедуре банкротства" },
        { "BadSupplier", "Организация в реестре недобросовестных поставщиков" },
        { "SanctionsList", "Организация в санкционных списках" },
        { "ToLiquidation", "Подано заявление на ликвидацию" },
        { "HeadFTSList", "Руководитель и/или учредитель найден (-ы) в Особых реестрах ФНС" },
        { "HeadBankrupt", "Руководитель либо учредитель организации является банкротом" },
        { "HeadDisqual", "Руководство в реестре дисквалифицированных лиц" },
        { "RelOrgToLiquid", "Более половины связанных организаций в стадии ликвидации" },
        { "RelOrgNegative", "Более половины связанных организаций имеют негативный статус" },
        { "RelOrgBankrupt", "Более половины связанных организаций имеют признаки банкротства" },
        { "RelOrgIsLiquid", "Значительное количество ликвидированных связанных организаций" },
        { "InvalidAddress", "По связанным организациям в ЕГРЮЛ указан признак недостоверности сведений в отношении адреса" },
        { "InvalidHead", "По связанным организациям в ЕГРЮЛ указан признак недостоверности сведений в отношении руководителя или учредителей" },
        { "ArbCaseTo", "По связанным организациям найдены арбитражные дела о банкротстве в качестве ответчика" },
        { "RelOrgWages", "По связанным организациям найдены исполнительные производства, предметом которых является заработная плата" },
        { "Moratorium", "Входит в перечень лиц, не подлежащих банкротству согласно мораторию в связи с пандемией COVID-19" },
        { "HeadIndividual", "Директор и учредитель одно физическое лицо" },
        { "ChangeFullName", "Изменилось ФИО ИП" },
        { "ChangeTaxCode", "Изменился код налогового органа" },
        { "ChangeActivity", "Изменился основной вид деятельности" },
        { "ChangeAddAct", "Изменился список дополнительных видов деятельности" },
        { "ReorgProcess", "Организация в процессе реорганизации" },
        { "Registered", "Организация зарегистрирована менее 12 месяцев назад" },
        { "ChangeCap", "Организация меняла уставный капитал дважды и более за последние 12 месяцев" },
        { "Change2Trrc", "Организация сменила КПП дважды или более за последние 12 месяцев" },
        { "Change2Name", "Организация сменила название дважды и более за последние 12 месяцев" },
        { "Change2Head", "Организация сменила руководителя или управляющую компанию дважды за последние 12 месяцев" },
        { "Change3Head", "Организация сменила руководителя или управляющую компанию трижды и более за последние 12 месяцев" },
        { "Change2Founder", "Организация сменила учредителя дважды за последние 12 месяцев" },
        { "Change3Founder", "Организация сменила учредителя трижды и более за последние 12 месяцев" },
        { "Change2Address", "Организация сменила юридический адрес дважды за последние 12 месяцев" },
        { "Change3Address", "Организация сменила юридический адрес трижды и более за последние 12 месяцев" },
        { "ForeignPersons", "Среди учредителей найдены иностранные лица" },
        { "MassRegistrList", "Адрес организации был найден в списке \"адресов массовой регистрации\" (ФНС)" },
        { "TaxArrears", "Задолженность по уплате налогов" },
        { "CaseAmountFrom", "Значительная сумма арбитражных дел в качестве истца" },
        { "CaseAmountTo", "Значительная сумма арбитражных дел в качестве ответчика" },
        { "ExecCaseAmount", "Значительная сумма исполнительных производств" },
        { "HeadOldLECount", "Значительное количество бывших юридических лиц руководителя (с учетом ИННФЛ)" },
        { "LECount", "Значительное количество учрежденных юридических лиц" },
        { "HeadLECount", "Значительное количество учрежденных юридических лиц руководителя (с учетом ФИО)" },
        { "LECountAddress", "Значительное количество юридических лиц, когда-либо зарегистрированных по тому же адресу" },
        { "NoLiquidAddress", "Значительное число не ликвидированных юридических лиц по тому же адресу" },
        { "UpArbCaseTo", "Значительный рост суммы арбитражных дел в качестве ответчика за последние 12 месяцев" },
        { "UpArbCaseFrom", "Значительный рост суммы арбитражных дел в качестве истца за последние 12 месяцев" },
        { "PledgedProperty", "Исполнительные производства (взыскание заложенного имущества)" },
        { "CreditPayments", "Исполнительные производства (кредитные платежи)" },
        { "Premium", "Исполнительные производства (страховые взносы)" },
        { "TaxesAndFees", "Исполнительные производства (налоги и сборы)" },
        { "Seizure", "Исполнительные производства (наложение ареста)" },
        { "SpecialArbCases", "Обнаружены арбитражные дела, требующие особого внимания" },
        { "UpCredits", "Рост количества арбитражных дел по договорам займа, кредита, лизинга" },
        { "ChangeData", "Подано заявление об изменении учетных данных" },
        { "UpBankruptcy", "Рост количества арбитражных дел по банкротству" },
        { "GainDown", "Существенное снижение выручки" },
        { "TaxReports", "Организация не предоставляет налоговую отчетность более года" },
        { "AddValid", "Рекомендована дополнительная проверка" },
        { "RelOrgArbCases", "Значительная сумма арбитражных дел по связанным организациям" },
        { "ExpiringLicense", "Наличие лицензий с истекающим сроком действия" },
        { "ExpiredLicense", "Наличие истекших лицензий" },
        { "UpDeliveries", "Рост количества арбитражных дел по договорам поставки" },
        { "SpecialList", "Значительное количество связанных организаций найдены в особых реестрах ФНС" },
        { "SpecialExecList", "Значительное количество связанных организаций с особыми исполнительными производствами" },
        { "LongRegistered", "Организация зарегистрирована более 5 лет тому назад" },
        { "CapitalM100000", "Уставный капитал более 100 000 руб" },
        { "CapitalL100000", "Уставный капитал меньше или равен 100 000 руб" },
        { "HasBranches", "Наличие филиалов и представительств" },
        { "RelOrgAddValid", "Значительное количество связанных организаций, по которым требуется дополнительная проверка" },
        { "RelOrgSwapHead", "Значительное количество связанных организаций, у которых за последние 12 месяцев хотя бы раз менялся директор или учредитель" },
        { "YoungRelOrg", "Значительное количество связанных организаций, зарегистрированных менее 12 месяцев назад" },
        { "RespondentArb", "Ответчик по арбитражным делам" },
        { "RelOrgGainDown", "Снижение выручки по связанным организациям более 30%" },
        { "AccountingState", "Наличие бухгалтерской отчетности за предыдущий отчетный период" },
        { "StateContracts", "Наличие государственных контрактов" },
        { "ActiveLicense", "Наличие действующих лицензий надзорных органов" },
        { "ActiveTrademark", "Наличие товарных знаков" },
        { "RelOrgActiveArb", "Активность в арбитражных делах у связанных организаций" },
        { "RelOrgActivePub", "Активность в государственных торгах у связанных организаций" },
        { "RelOrgOld", "Значительная часть связанных организаций существует более 5 лет" },
        { "RelOrgAccount", "Наличие бухгалтерской отчетности у связанных организаций" },
        { "MarkerQ2031ChangedDate", "" },
        { "MarkerQ2032ChangedDate", "" },
        { "MarkerQ2035ChangedDate", "" },
        { "MarkerY3601B", "" },
        { "MarkerQ2031", "" },
        { "MarkerQ2035", "" },
        { "MarkerY3601BChangedDate", "" },
        { "ArbPractice", "Наличие арбитражной практики" },
        { "RelOrgExecCases", "Значительная сумма исполнительных производств по связанным организациям" },
        { "LowAvgHeadSberDev", "Низкая среднесписочная численность работников" }
      };
      
      return displayNames.ContainsKey(propertyName)
        ? displayNames[propertyName]
        : propertyName;
    }
    
    [Public]
    public string TranslateContrTypeEnum(string enumValue)
    {
      switch (enumValue)
      {
        case "Expendable":
          return "Расходный";
        case "Profitable":
          return "Доходный";
        case "ExpendProfit":
          return "Доходно-расходный";
        default:
          return enumValue; // Возвращает оригинальное значение, если сопоставление не найдено
      }
    }
    
    /// <summary>
    /// Функция возваращает русское название месяца по его номеру
    /// </summary>
    [Public]
    public string GetMonthGenetiveName(int monthNumber)
    {
      switch (monthNumber)
      {
        case 1:
          return "января";
        case 2:
          return "февраля";
        case 3:
          return "марта";
        case 4:
          return "апреля";
        case 5:
          return "мая";
        case 6:
          return "июня";
        case 7:
          return "июля";
        case 8:
          return "августа";
        case 9:
          return "сентября";
        case 10:
          return "октября";
        case 11:
          return "ноября";
        case 12:
          return "декабря";
        default:
          throw new ArgumentOutOfRangeException("Номер месяца должен быть от 1 до 12");
      }
    }
    
    public static string Transliterate(string russianText)
    {
      Dictionary<char, string> translitMap = new Dictionary<char, string>
      {
        {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
        {'е', "e"}, {'ё', "yo"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
        {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
        {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
        {'у', "u"}, {'ф', "f"}, {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"},
        {'ш', "sh"}, {'щ', "shch"}, {'ы', "y"}, {'э', "e"}, {'ю', "yu"},
        {'я', "ya"}, {'ь', ""}, {'ъ', ""},
        {'А', "A"}, {'Б', "B"}, {'В', "V"}, {'Г', "G"}, {'Д', "D"},
        {'Е', "E"}, {'Ё', "Yo"}, {'Ж', "Zh"}, {'З', "Z"}, {'И', "I"},
        {'Й', "Y"}, {'К', "K"}, {'Л', "L"}, {'М', "M"}, {'Н', "N"},
        {'О', "O"}, {'П', "P"}, {'Р', "R"}, {'С', "S"}, {'Т', "T"},
        {'У', "U"}, {'Ф', "F"}, {'Х', "Kh"}, {'Ц', "Ts"}, {'Ч', "Ch"},
        {'Ш', "Sh"}, {'Щ', "Shch"}, {'Ы', "Y"}, {'Э', "E"}, {'Ю', "Yu"},
        {'Я', "Ya"}, {'Ь', ""}, {'Ъ', ""}
      };
      System.Text.StringBuilder translitText = new System.Text.StringBuilder();
      string translitChar = null;

      foreach (char c in russianText)
      {
        if (translitMap.TryGetValue(c, out translitChar))
        {
          translitText.Append(translitChar);
        }
        else
        {
          translitText.Append(c);
        }
      }

      return translitText.ToString();
    }
    
    /// <summary>
    /// Функция переводит цифры в текст (до миллиона)
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string NumberToWords(int number)
    {
      string[] Units =
      {
        "ноль", "один", "два", "три", "четыре", "пять",
        "шесть", "семь", "восемь", "девять", "десять",
        "одиннадцать", "двенадцать", "тринадцать",
        "четырнадцать", "пятнадцать", "шестнадцать",
        "семнадцать", "восемнадцать", "девятнадцать"
      };
      string[] Tens =
      {
        "", "", "двадцать", "тридцать", "сорок",
        "пятьдесят", "шестьдесят", "семьдесят",
        "восемьдесят", "девяносто"
      };
      string[] Hundreds =
      {
        "", "сто", "двести", "триста", "четыреста",
        "пятьсот", "шестьсот", "семьсот",
        "восемьсот", "девятьсот"
      };
      string[] Thousands =
      {
        "", "тысяча", "две тысячи", "три тысячи", "четыре тысячи",
        "пять тысяч", "шесть тысяч", "семь тысяч",
        "восемь тысяч", "девять тысяч"
      };
      if (number == 0)
        return Units[0];

      if (number < 0)
        return "минус " + NumberToWords(Math.Abs(number));

      var words = new List<string>();

      if (number / 1000 > 0)
      {
        words.Add(NumberToWords(number / 1000));
        words.Add(GetThousandsWord(number / 1000));
        number %= 1000;
      }

      if (number / 100 > 0)
      {
        words.Add(Hundreds[number / 100]);
        number %= 100;
      }

      if (number / 10 > 1)
      {
        words.Add(Tens[number / 10]);
        number %= 10;
      }

      if (number > 0)
      {
        words.Add(Units[number]);
      }

      return string.Join(" ", words);
    }

    private static string GetThousandsWord(int number)
    {
      
      string[] Thousands =
      {
        "", "тысяча", "две тысячи", "три тысячи", "четыре тысячи",
        "пять тысяч", "шесть тысяч", "семь тысяч",
        "восемь тысяч", "девять тысяч"
      };
      
      number %= 10;
      switch (number)
      {
          case 1: return "тысяча";
          case 4: return Thousands[number];
          default: return "тысяч";
      }
    }
    #endregion
    
    /// <summary>
    /// Вычисляет количество рабочих дней между двумя датами.
    /// </summary>
    [Public]
    public virtual int CalculateBusinessDays(DateTime? startDate, DateTime? endDate)
    {
      if (!startDate.HasValue || !endDate.HasValue)
        return 0;
      
      // Проверяем корректность дат
      if (startDate.Value > endDate.Value)
      {
        Logger.Debug($"CalculateBusinessDays: Некорректные даты - начальная дата ({startDate.Value:yyyy-MM-dd HH:mm:ss}) больше конечной ({endDate.Value:yyyy-MM-dd HH:mm:ss})");
        // Вместо выброса исключения возвращаем 0
        return 0;
      }
      
      // Приводим к началу дня для корректного расчета
      var start = startDate.Value.Date;
      var end = endDate.Value.Date;
      
      // Если даты совпадают, возвращаем 0 или 1 в зависимости от того, считаем ли текущий день
      if (start == end)
        return 1; // Считаем текущий день как 1 рабочий день
      
      // Количество календарных дней
      int calendarDays = (end - start).Days + 1;
      
      // Количество выходных дней
      int weekendDays = 0;
      for (var date = start; date <= end; date = date.AddDays(1))
      {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
          weekendDays++;
      }
      
      // Учитываем праздничные дни (если есть такая функциональность)
      int holidayDays = GetHolidayDays(start, end);
      
      // Рабочие дни = календарные дни - выходные - праздники (не совпадающие с выходными)
      return calendarDays - weekendDays - holidayDays;
    }

    /// <summary>
    /// Получает количество праздничных дней в диапазоне дат.
    /// </summary>
    private static int GetHolidayDays(DateTime start, DateTime end)
    {
      try
      {
        // Здесь должен быть код для определения праздничных дней
        // на основе настроек системы или данных из базы
        
        // Это заглушка, которую следует реализовать
        return 0;
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка при определении праздничных дней: {ex.Message}", ex);
        return 0;
      }
    }
    
    /// <summary>
    /// Функция проверяет является ли пользователь системным
    /// </summary>
    [Public]
    public bool IsSystemUser()
    {
      return Users.Current.Name == "Service User" || Users.Current.Name == "Administrator"
        || (Users.Current.IsSystem.HasValue && Users.Current.IsSystem.Value);
    }
    
  }
}