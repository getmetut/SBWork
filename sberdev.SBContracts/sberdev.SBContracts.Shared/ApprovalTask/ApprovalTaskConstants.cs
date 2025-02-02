using System;
using Sungero.Core;

namespace sberdev.SBContracts.Constants.Docflow
{
  public static class ApprovalTask
  {
    [Sungero.Core.Public]
    public const string SupplementalStage = "55d13a81-9812-42e9-ab13-08f849fed4bd"; // "Этап дополнительный (сейчас если IsNeedSupStageSberDev == false то этап пропускается)
    [Sungero.Core.Public]
    public const string CheckingCPStage = "e08081d4-ec20-4f56-bf43-534e3f01cdff"; // "Этап проверки ка на благонадежность (показывает поле NeedFinance в карточке)
    [Sungero.Core.Public]
    public const string AccountantDZKZStage = "24de0d9d-ed7e-44cd-a09f-9696abec0487"; // этап согласования Бухгалтер ДЗ КЗ (вызывает диалоговое окно)
    [Sungero.Core.Public]
    public const string ContractCheckByClerkStage = "7c1833f6-22da-460f-ab4c-6d510c5f6945"; // этап согласования Проверка договорных документов Делопроизводителем
    [Sungero.Core.Public]
    public const string CheckUCNStage = "eb288d73-0b4a-4cf8-ad17-6b250bc2392c"; // этап согласования Проверить постановку договора на валютный банковский контроль (вызывает диалоговое окно)
    [Sungero.Core.Public]
    public const string CancelApproveSkipStage = "3236816c-cb3c-467a-bd50-12fd0accab6a"; // убирает скип согласования
    [Sungero.Core.Public]
    public const string TreasuryStage = "9326c1f6-1a79-452c-9e13-63b0ba7d978a"; // этап Казначей ПП (показывает поля по внедоговрными счетами и согласованию счета казначеем)
  }
}