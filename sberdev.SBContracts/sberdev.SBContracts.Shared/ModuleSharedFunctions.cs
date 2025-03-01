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
    /// Полчуить количество рабочих дней меду двумя датами
    /// </summary>
    [Public]
    public int CalculateBusinessDays(DateTime? startDate, DateTime? endDate)
    {
      if (startDate == null || endDate == null)
        throw new ArgumentException("Параметры должны содержать значения.");

      if (startDate > endDate)
        throw new ArgumentException("Начальная дата должна быть меньше или равна конечной дате.");

      // Общее количество дней включая начальную и конечную дату
      int totalDays = (endDate.Value - startDate.Value).Days + 1;

      // Количество полных недель
      int fullWeeks = totalDays / 7;

      // Количество рабочих дней из полных недель
      int businessDays = fullWeeks * 5;

      // Остаточные дни
      int remainingDays = totalDays % 7;

      // День недели для начальной даты
      DayOfWeek startDayOfWeek = startDate.Value.DayOfWeek;

      for (int i = 0; i < remainingDays; i++)
      {
        DayOfWeek currentDay = (DayOfWeek)(((int)startDayOfWeek + i) % 7);
        if (currentDay != DayOfWeek.Saturday && currentDay != DayOfWeek.Sunday)
        {
          businessDays++;
        }
      }

      return businessDays;
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