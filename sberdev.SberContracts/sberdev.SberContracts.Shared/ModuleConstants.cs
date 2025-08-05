using System;
using Sungero.Core;

namespace sberdev.SberContracts.Constants
{
  public static class Module
  {
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AppRnDPurchaseGuid =  Guid.Parse("5363e3b0-3af0-4788-b341-b769d45daaa4");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AppNonProdPurchaseGuid =  Guid.Parse("675e2890-7dba-4d1a-a3bb-8a829bdabd66");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AppProductPurchaseGuid =  Guid.Parse("5f15d0c0-c0e3-42df-b869-7aa26ae26a5e");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid OrderXiongxin =  Guid.Parse("f22bbef8-9754-4963-8d60-21368432bf4b");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid ContractXiongxin =  Guid.Parse("c0e69cec-ce0d-41d5-8a87-92a648cb9f89");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid FakeSignContractRoleGuid =  Guid.Parse("65bb3be1-cfbf-4d99-a88f-8fd0d4884a06");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AccDocsHandlerGFRoleGuid =  Guid.Parse("b592491a-057c-4568-9d80-83e23e8b9edf");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AccDocsHandlerRoleGuid =  Guid.Parse("c054a467-183b-44d5-87b1-c3e4e03c93ec");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid CpSyncConflictRoleGuid =  Guid.Parse("d0fb1092-ed47-46b1-bb6c-536fb01068f6");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AdminButtonsUserRoleGuid = Guid.Parse("c48e6195-7f93-4e1b-98cf-293d2a2c862d");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid BusinessControlerRoleGuid = Guid.Parse("4002a798-770a-4f55-8d42-4c8855ed3d2e");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid KZTypeGuid = Guid.Parse( "271898c8-18ca-4192-9892-e27b273ce5fc");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid GBTypeGuid = Guid.Parse( "ACAEB5B3-C56B-457B-873D-E2FEE6C1B608");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid FDTypeGuid = Guid.Parse( "6931AD3D-0618-465D-BEFE-1043ABC7A4CF");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid FACTypeGuid = Guid.Parse( "0893C40D-0977-4D7D-AFBC-001AD20FD734");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid OldMVZGuid = Guid.Parse( "908D8CDA-B8DD-4838-8336-61759B2090CD");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid OldMVPGuid = Guid.Parse( "06ABE28F-A144-4B8D-89FA-79A48599AEEB");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid NoticeBySing = Guid.Parse( "41A3A02A-5D9D-43AB-A7C0-81AEE79C7B5D");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid PurchaserBySing = Guid.Parse( "caadab73-6e42-4d66-984a-126225ddc101");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid TochWorkGUIDRole = Guid.Parse( "22eeb7b3-0362-4af2-abc7-ef815fa4b5a9");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid CounterpartiesResponsibleRole = Guid.Parse( "C719C823-C4BD-4434-A34B-D7E83E524414");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid MVPStabGuid = Guid.Parse("49e415a7-10e3-4046-af50-10f1cd3818b8");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid MVZStabGuid = Guid.Parse("ffc3d62a-b438-4dcf-9bfe-99bddbd479e7");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid AccArtStabGuid = Guid.Parse("465bf65a-3e4c-4430-8bf5-557626a62c76");
    [Sungero.Core.PublicAttribute]
    public static readonly Guid ProductStabGuid = Guid.Parse("fee545b2-0f1e-40da-8c9d-f7abd565c6cf");
    
    public static readonly Guid IncomeExpenditureContract = Guid.Parse("adfeffa9-c9ed-4f87-9002-630996a71333");
    // Уникальный идентификатор для вида «Отчет».

    public static readonly Guid Otchet = Guid.Parse("B26DBEC0-537B-42D8-AEBF-1D3746EAD77B");

    // Уникальный идентификатор для вида «Иные документы».

    public static readonly Guid OtherDoc = Guid.Parse("E1F1621B-0FA7-42F7-B68F-7F0B82725154");
    
    // Уникальный идентификатор для вида «Закупка».
    public static readonly Guid Purchase = Guid.Parse("eafc0659-4e4e-4b01-a814-f6ad9ae400a3");
    
    // Уникальный идентификатор для вида «Гарантийное письмо».
    public static readonly Guid GuaranteeLetter = Guid.Parse("90aeab95-25c8-42bf-aab4-c3104b0d170f");
    
    // Уникальный идентификатор для «Абстрактное доп соглашение».
    public static readonly Guid AbstractSupAreement = Guid.Parse("0c496820-add9-451d-ab6e-a61d04545d2d");
    
    // Уникальный идентификатор для вида «Спецификация ПАО».
    public static readonly Guid SpecificationPAO = Guid.Parse("f7d27ec4-3429-4387-bd21-bfa6f9c38aa8");
    
    // Уникальный идентификатор для вида «Техническое задание».
    public static readonly Guid Specification = Guid.Parse("68a41ea6-e83e-4fe7-a62e-a0df6a79ec94");
    
    #region Идентификаторы видов документов для типа "Учредительный документ" (доработка в рамках задачи DRX-669).
    /// <summary>
    /// Вид "Протокол ВОСУ".
    /// </summary>
    public static readonly Guid FoundDocVOSUProtocol = Guid.Parse("42AF1470-0636-4E4D-B9CA-7994D1CD5971");
    
    /// <summary>
    /// Вид "Протокол Совета директоров".
    /// </summary>
    public static readonly Guid FoundDocDirectProtocol = Guid.Parse("3C51133B-063E-4945-B996-DA6AFCA62C98");
    
    /// <summary>
    /// Вид "Решение единственного участника".
    /// </summary>
    public static readonly Guid FoundDocSingleSolution = Guid.Parse("1404C6C6-3DA8-43D1-8461-FDE4096F6A65");
    
    /// <summary>
    /// Вид "Выписка".
    /// </summary>
    public static readonly Guid FoundDocOrdering = Guid.Parse("FC95B821-97F3-40C1-B12B-D23CA38B8673");
    
    /// <summary>
    /// Вид "Лист записи".
    /// </summary>
    public static readonly Guid FoundDocOrderPage = Guid.Parse("879E9A8C-F12A-4D8E-A125-149607DA8CFA");
    
    /// <summary>
    /// Вид "Свидетельство".
    /// </summary>
    public static readonly Guid FoundDocCertificate = Guid.Parse("44BA35FC-C0BC-4838-AF8E-C59773DA6B94");
    
    /// <summary>
    /// Вид "Письмо".
    /// </summary>
    public static readonly Guid FoundDocLetter = Guid.Parse("EC80587A-9B61-4B38-9155-64AC6A9AD16F");
    
    /// <summary>
    /// Вид "Справка".
    /// </summary>
    public static readonly Guid FoundDocInquiry = Guid.Parse("49ABA7A3-0F58-46DD-B735-6E696903276C");
    
    /// <summary>
    /// Вид "Уведомление".
    /// </summary>
    public static readonly Guid FoundDocNotification = Guid.Parse("030D76D1-B2EC-49B2-B549-045CEA7DCBB8");
    
    /// <summary>
    /// Вид "Карточка компании".
    /// </summary>
    public static readonly Guid FoundDocCompanyCard = Guid.Parse("A7A99161-ED63-476F-A789-162EA8AE4909");
    
    /// <summary>
    /// Вид "Иные".
    /// </summary>
    public static readonly Guid FoundDocOther = Guid.Parse("B8DC4F10-1A8D-4945-9927-60409EFB32DC");
    #endregion
    
    // Уникальный идентификатор для этапа «Проверка договорных документов Делопроизводителем».
    public static readonly Guid ApprovalStageManuallyChecking = Guid.Parse("61589cbf-7f0f-4773-9c15-be99a726a6be");
    // Уникальный идентификатор для этапа «Запуск согласования ГП по маршруту "Договор"».
    public static readonly Guid ApprovalStageGLStartedContractApproval = Guid.Parse("a48a3820-93b6-4128-9e97-fbcd20c3b8e5");
    // Уникальный идентификатор для этапа «Согласование закупки».
    public static readonly Guid ApprovalStagePurchseApproval = Guid.Parse("8ce26ec5-f2d7-4d50-b3f6-e97d958a5fef");
  }
}