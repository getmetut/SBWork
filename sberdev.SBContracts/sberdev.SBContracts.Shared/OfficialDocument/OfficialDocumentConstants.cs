using System;
using Sungero.Core;

namespace sberdev.SBContracts.Constants.Docflow
{
  public static class OfficialDocument
  {
    
    // Настройка усечения длинных значений
    public const int TRUNCATE_LEN = 120; // ограничение длины значений свойств
    public const bool LOG_PER_LINE = true; // логировать построчно, а не одной записью
    public const bool SUPPRESS_SCALARS_ON_CREATE = false; // true — не логировать скаляры при создании
    public const bool SHOW_CREATE_AS_FROM_NULL   = true; // если SUPPRESS=false: false => "Field = New", true => "null -> New"
    
    public const string LSDiadocReceived = "Получен через сервис обмена Диадок";
    public const string LSDiadocSended = "Отправлен через сервис обмена Диадок";
    public const string LSDiadocSigned = "Подписан обеими сторонами";

    public const string OldExtStatus = "OldExtStatus";
    public const string OldIntStatus = "OldIntStatus";
  }
}