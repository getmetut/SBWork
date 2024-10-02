using System;
using Sungero.Core;

namespace sberdev.SBContracts.Constants.Docflow
{
  public static class ApprovalTask
  {
    [Sungero.Core.Public]
    public const string AccountantDZKZStage = "24de0d9d-ed7e-44cd-a09f-9696abec0487"; // этап согласования Бухгалтер ДЗ КЗ
    [Sungero.Core.Public]
    public const string ContractCheckByClerkStage = "7c1833f6-22da-460f-ab4c-6d510c5f6945"; // этап согласования Проверка договорных документов Делопроизводителем
    [Sungero.Core.Public]
    public const string CheckUCNStage = "eb288d73-0b4a-4cf8-ad17-6b250bc2392c"; // этап согласования Проверить постановку договора на валютный банковский контроль
  }
}