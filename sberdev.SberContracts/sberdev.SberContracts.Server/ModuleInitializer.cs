using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace sberdev.SberContracts.Server
{
  public partial class ModuleInitializer
  {
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateApprovalRoles();
      CreateRoles();
      CreateDocumentTypes();
      CreateDocumentKinds();
      GrantCreateRightsOnInfoDocument();
      GrantRightsOnFolder();
      CreateMVZ();
      CreateStages();
      CreateGroupsForJobChangeFakePersonsNames();
      GrantRightsOnReports();
      GrantRightsOnDatabooks();
      CreateStabs();
      CreateDevSettings();
      CreateTempDocsDir();
    }
    
    #region Роли
    
    public static void CreateRoles()
    {
      var roleSC = Roles.GetAll(r => r.Sid == Constants.Module.CpSyncConflictRoleGuid).FirstOrDefault();
      
      if (roleSC == null)
      {
        roleSC = Roles.Create();
        roleSC.Name = "Ответсвенный за конфликты синхронизации контрагентов";
        roleSC.Description = "Участнику роли приходят задания на решение конфликтов синхронизации контрагентов";
        roleSC.Sid = Constants.Module.CpSyncConflictRoleGuid;
        roleSC.IsSystem = false;
        roleSC.RecipientLinks.AddNew().Member = Users.GetAll().Where(r => r.Id == 10).FirstOrDefault();
        roleSC.IsSingleUser = true;
        roleSC.Save();
        InitializationLogger.Debug("Ответсвенный за фин. вх. документы");
      }
      else
      {
        roleSC.Name = "Ответсвенный за конфликты синхронизации контрагентов";
        roleSC.Description = "Участнику роли приходят задания на решение конфликтов синхронизации контрагентов.";
        roleSC.Sid = Constants.Module.CpSyncConflictRoleGuid;
        roleSC.IsSystem = false;
        roleSC.IsSingleUser = true;
        roleSC.Save();
        InitializationLogger.Debug("Ответсвенный за конфликты синхронизации контрагентов");
      }
      
      var roleAH = Roles.GetAll(r => r.Sid == Constants.Module.AccDocsHandlerRoleGuid).FirstOrDefault();
      
      if (roleAH == null)
      {
        roleAH = Roles.Create();
        roleAH.Name = "Ответсвенный за фин. вх. документы";
        roleAH.Description = "Участнику роли приходят задания на обработку формализованых финансовых вх. документов.";
        roleAH.Sid = Constants.Module.AccDocsHandlerRoleGuid;
        roleAH.IsSystem = false;
        roleAH.RecipientLinks.AddNew().Member = Users.GetAll().Where(r => r.Id == 10).FirstOrDefault();
        roleAH.IsSingleUser = true;
        roleAH.Save();
        InitializationLogger.Debug("Ответсвенный за фин. вх. документы");
      }
      else
      {
        roleAH.Name = "Ответсвенный за фин. вх. документы";
        roleAH.Description = "Участнику роли приходят задания на обработку формализованых финансовых вх. документов.";
        roleAH.Sid = Constants.Module.AccDocsHandlerRoleGuid;
        roleAH.IsSystem = false;
        roleAH.IsSingleUser = true;
        roleAH.Save();
        InitializationLogger.Debug("Ответсвенный за фин. вх. документы");
      }
      
      var roleAB = Roles.GetAll(r => r.Sid == Constants.Module.AdminButtonsUserRoleGuid).FirstOrDefault();
      
      if (roleAB == null)
      {
        roleAB = Roles.Create();
        roleAB.Name = "Пользователи админ. кнопок";
        roleAB.Description = "Участники роли могут использовать кнопки группы \"Администрирование\" в карточках сущностей";
        roleAB.Sid = Constants.Module.AdminButtonsUserRoleGuid;
        roleAB.IsSystem = false;
        roleAB.Save();
        InitializationLogger.Debug("Пользователи админ. кнопок");
      }
      else
      {
        roleAB.Name = "Пользователи админ. кнопок";
        roleAB.Description = "Участники роли могут использовать кнопки группы \"Администрирование\" в карточках сущностей";
        roleAB.Sid = Constants.Module.AdminButtonsUserRoleGuid;
        roleAB.IsSystem = false;
        roleAB.Save();
        InitializationLogger.Debug("Пользователи админ. кнопок");
      }
      
      var roleBC = Roles.GetAll(r => r.Sid == Constants.Module.BusinessControlerRoleGuid).FirstOrDefault();
      
      if (roleBC == null)
      {
        roleBC = Roles.Create();
        roleBC.Name = sberdev.SberContracts.Resources.BCRoleName;
        roleBC.Description = sberdev.SberContracts.Resources.BCRoleDisc;
        roleBC.Sid = Constants.Module.BusinessControlerRoleGuid;
        roleBC.IsSystem = false;
        roleBC.Save();
        InitializationLogger.Debug("Создана роль Бизнес-контролер");
      }
      else
      {
        roleBC.Name = sberdev.SberContracts.Resources.BCRoleName;
        roleBC.Description = sberdev.SberContracts.Resources.BCRoleDisc;
        roleBC.Sid = Constants.Module.BusinessControlerRoleGuid;
        roleBC.IsSystem = false;
        roleBC.Save();
        InitializationLogger.Debug("Обновлена роль Бизнес-контролер");
      }
      
      var role = Roles.GetAll(r => r.Sid == Constants.Module.KZTypeGuid).FirstOrDefault();
      
      if (role == null)
      {
        role = Roles.Create();
        role.Name = sberdev.SberContracts.Resources.KZgroup;
        role.Description = sberdev.SberContracts.Resources.KZgroupDesc;
        role.Sid = Constants.Module.KZTypeGuid;
        role.IsSystem = false;
        role.Save();
        InitializationLogger.Debug("Создана роль Центр снабжения");
      }
      else
      {
        role.Name = sberdev.SberContracts.Resources.KZgroup;
        role.Description = sberdev.SberContracts.Resources.KZgroupDesc;
        role.Sid =  Constants.Module.KZTypeGuid;
        role.IsSystem = false;
        role.Save();
        InitializationLogger.Debug("Обновлена роль Центр снабжения");
      }
      var roleGB = Roles.GetAll(r => r.Sid == Constants.Module.GBTypeGuid).FirstOrDefault();
      
      if (roleGB == null)
      {
        roleGB = Roles.Create();
        roleGB.Name = "Главный бухгалтер";
        roleGB.Description = "Главный бухгалтер";
        roleGB.Sid = Constants.Module.GBTypeGuid;
        roleGB.IsSystem = false;
        roleGB.Save();
        InitializationLogger.Debug("Создана роль Главный бухгалтер");
      }
      else
      {
        roleGB.Name = "Главный бухгалтер";
        roleGB.Description = "Главный бухгалтер";
        roleGB.Sid = Constants.Module.GBTypeGuid;
        roleGB.IsSystem = false;
        roleGB.Save();
        InitializationLogger.Debug("Создана роль Главный бухгалтер");
      }
      var roleFD = Roles.GetAll(r => r.Sid == Constants.Module.FDTypeGuid).FirstOrDefault();
      
      if (roleFD == null)
      {
        roleFD = Roles.Create();
        roleFD.Name = "Финансовый директор";
        roleFD.Description = "Финансовый директор";
        roleFD.Sid = Constants.Module.FDTypeGuid;
        roleFD.IsSystem = false;
        roleFD.Save();
        InitializationLogger.Debug("Создана роль Финансовый директор");
      }
      else
      {
        roleFD.Name = "Финансовый директор";
        roleFD.Description = "Финансовый директор";
        roleFD.Sid = Constants.Module.FDTypeGuid;
        roleFD.IsSystem = false;
        roleFD.Save();
        InitializationLogger.Debug("Создана роль Финансовый директор");
      }
      
      var roleFAC = Roles.GetAll(r => r.Sid == Constants.Module.FACTypeGuid).FirstOrDefault();
      
      if (roleFAC == null)
      {
        roleFAC = Roles.Create();
        roleFAC.Name = "Полный доступ к договорам";
        roleFAC.Description = "Полный доступ к договорам";
        roleFAC.Sid = Constants.Module.FACTypeGuid;
        roleFAC.IsSystem = false;
        roleFAC.Save();
        InitializationLogger.Debug("Создана роль Полный доступ к договорам");
      }
      else
      {
        roleFAC.Name = "Полный доступ к договорам";
        roleFAC.Description = "Полный доступ к договорам";
        roleFAC.Sid = Constants.Module.FACTypeGuid;
        roleFAC.IsSystem = false;
        roleFAC.Save();
        InitializationLogger.Debug("Создана роль Полный доступ к договорам");
      }
      var roleNotice = Roles.GetAll(r => r.Sid == Constants.Module.NoticeBySing).FirstOrDefault();
      
      if (roleNotice == null)
      {
        roleNotice = Roles.Create();
        roleNotice.Name = sberdev.SberContracts.Resources.NoticeBySingGroup;
        roleNotice.Description = sberdev.SberContracts.Resources.NoticeBySingDesc;
        roleNotice.Sid = Constants.Module.NoticeBySing;
        roleNotice.IsSystem = false;
        roleNotice.Save();
        InitializationLogger.Debug("Создана роль Контроль подписания Диадок");
      }
      else
      {
        roleNotice.Name = sberdev.SberContracts.Resources.NoticeBySingGroup;
        roleNotice.Description = sberdev.SberContracts.Resources.NoticeBySingDesc;
        roleNotice.Sid = Constants.Module.NoticeBySing;
        roleNotice.IsSystem = false;
        roleNotice.Save();
        InitializationLogger.Debug("Обновлена роль Контроль подписания Диадок");
      }
      
    }
    
    // <summary>
    // Создание роли.
    // </summary>
    public static void CreateApprovalRoles()
    {
      var roles = CustomAppovalRoles.GetAll();
      List<Enumeration> customRoles = new List<Enumeration>{
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwner,
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwnerMVP,
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwnerMVZ,
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwnerMark,
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwnerProd,
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwnerPrGe,
        sberdev.SberContracts.CustomAppovalRole.Type.BudgetOwnerUnit,
        sberdev.SberContracts.CustomAppovalRole.Type.Attorney,
        sberdev.SberContracts.CustomAppovalRole.Type.AttorneyManager};
      foreach (var customRole in customRoles)
      {
        var role = roles.FirstOrDefault(r => r.Type.Value == customRole);
        if (role == null)
        {
          role = CustomAppovalRoles.Create();
          role.Type = customRole;
          role.Description = customRole.Value;
          role.Save();
          InitializationLogger.Debug("Создана роль " + customRole.Value);
        }
      }
    }
    
    #endregion
    
    #region Типы
    
    public void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Закупка", Purchase.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Заявка на производ. закупку", AppProductPurchase.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Иные договорные документы", OtherContractDocument.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Гарантийное письмо", GuaranteeLetter.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Outgoing, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Документы на основе доп. соглашения", AbstractsSupAgreement.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Техническое задание", Specification.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
    }
    
    public void CreateDocumentKinds()
    {
      // Создание вида документа «Заявка на производ. закупку».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Заявка на производ. закупку", "Заявка на производ. закупку",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              AppProductPurchase.ClassTypeGuid, new Sungero.Domain.Shared.IActionInfo[]
                                                                              {Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval},
                                                                              Constants.Module.AppProductPurchaseGuid);
      // Создание вида документа «Договор Xiongxin».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Договор Xiongxin", "Договор Xiongxin",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, true,
                                                                              Sungero.Contracts.Server.Contract.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.ContractXiongxin);
      // Создание вида документа «Заказ Xiongxin».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Заказ Xiongxin", "Заказ Xiongxin",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, true,
                                                                              Sungero.Contracts.Server.SupAgreement.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.OrderXiongxin);
      // Создание вида документа «Спецификация ПАО».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Спецификация ПАО", "Спецификация ПАО",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, true,
                                                                              Sungero.Contracts.Server.SupAgreement.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SpecificationPAO);
      // Создание вида документа «Гарантийное письмо».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Гарантийное письмо", "Гарантийное письмо",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Outgoing, true, true,
                                                                              GuaranteeLetter.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.GuaranteeLetter);
      
      // Создание вида документа «Объяснительная записка».
      // Чтобы документы можно было регистрировать, задается свойство Registrable.
      // В качестве ИД вида документа используется константа ExplanLettersKind.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Отчет", "Отчет",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, true,
                                                                              OtherContractDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.Otchet);
      

      // Создание вида документа «Закупка».
      // Чтобы документы можно было регистрировать, задается свойство Registrable.
      // В качестве ИД вида документа используется константа ExplanLettersKind.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Закупка", "Закупка", Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true, Purchase.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval}, Constants.Module.Purchase);

      // Создание вида документа «Заявление».
      // Чтобы документы можно было регистрировать, задается свойство Registrable.
      // В качестве ИД вида документа используется константа StatementsKind.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Иные документы", "Иные документы",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts,
                                                                              true, true, OtherContractDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.OtherDoc);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Техническое задание", "Техническое задание",
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true, true, Specification.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.Specification);
    }
    
    #endregion
    
    #region Права
    
    public static void GrantCreateRightsOnInfoDocument()
    {
      var allUsers = Roles.AllUsers;

      SberContracts.OtherContractDocuments.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      SberContracts.Purchases.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      SberContracts.GuaranteeLetters.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      SberContracts.OtherContractDocuments.AccessRights.Save();
      SberContracts.Purchases.AccessRights.Save();
      SberContracts.GuaranteeLetters.AccessRights.Save();
    }

    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Выдача прав на Справочники");
      var allUsers = Roles.AllUsers;

      SberContracts.AccountingArticleses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.BudgetItems.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.CustomAppovalRoles.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.MVZs.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.ProductsAndDeviceses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.AnaliticsCashes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.FullAccess);
      SberContracts.ProductUnits.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.BusinessUnitFilteringCashes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.FullAccess);
      SberContracts.AnaliticsCashes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.FullAccess);
      SberContracts.AnaliticsCasheGenerals.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.FullAccess);
      SberContracts.CalculationTamplates.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.FullAccess);
      SberContracts.MarketingDirections.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.NonContractInvoiceCounters.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.NonContractInvoiceCounters.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      SberContracts.NonContractInvoiceCounters.AccessRights.Save();
      SberContracts.MarketingDirections.AccessRights.Save();
      SberContracts.AccountingArticleses.AccessRights.Save();
      SberContracts.BudgetItems.AccessRights.Save();
      SberContracts.CustomAppovalRoles.AccessRights.Save();
      SberContracts.MVZs.AccessRights.Save();
      SberContracts.ProductsAndDeviceses.AccessRights.Save();
      SberContracts.AnaliticsCashes.AccessRights.Save();
      SberContracts.ProductUnits.AccessRights.Save();
      SberContracts.CalculationTamplates.AccessRights.Save();
      SberContracts.BusinessUnitFilteringCashes.AccessRights.Save();
      SberContracts.AnaliticsCashes.AccessRights.Save();
      SberContracts.AnaliticsCasheGenerals.AccessRights.Save();
    }

    public void GrantRightsOnFolder()
    {
      var allUsers = Roles.AllUsers;
      SberContracts.SpecialFolders.Notice.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      SberContracts.SpecialFolders.Notice.AccessRights.Save();

      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Уведомления'");
    }
    
    public void GrantRightsOnReports()
    {
      InitializationLogger.Debug("Init: Выдача прав на Отчёты");
      var allUsers = Roles.AllUsers;
      Reports.AccessRights.Grant(Reports.GetAvgTimeCompletedTaskReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetCompleteAssignsReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetPaidInvoiceReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetBudgetOwnerReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetStartedTasksReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetAccDocsProductReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetContrDocsProductReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetTasksByDocReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
    }
    
    public void GrantRightsOnTasks()
    {
      InitializationLogger.Debug("Init: Выдача прав на Задачи");
      var allUsers = Roles.AllUsers;
      
      SberContracts.DiadocSettingsTasks.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.FullAccess);
      SberContracts.DiadocSettingsTasks.AccessRights.Save();
    }

    #endregion
    
    #region Этапы
    
    /// <summary>
    /// Общая функция созлдания этапов согласования
    /// </summary>
    public void CreateStages()
    {
      CreateAutoSetLifeCycleStateActiveStage();
      CreateAutoSetLifeCycleStateInRegisterStage();
      CreateApprovalStageManuallyChecking();
      CreateApprovalStagePurchseApproval();
      CreateApprovalStageGLStartedContractApproval();
      CreateApprovalStageProvidePostalInformation();
      CreateAutoSetMetaIDStage();
      CreateAutoCreateInvoiceContractTaskStage();
      CreateAutoSetFocusFlagStage();
    }
    
    /// <summary>
    /// Создание сценария создания и запуска согласования счета-оферты
    /// </summary>
    public void CreateAutoCreateInvoiceContractTaskStage()
    {
      InitializationLogger.DebugFormat("Init: Create stage for automatic create incoming invoice from invoice-contract and start approval task by new document.");
      if (SberContracts.AutoStartInvoiceContractTasks.GetAll().Any())
        return;
      var stage = SberContracts.AutoStartInvoiceContractTasks.Create();
      stage.Name = "Создание карточки входящего счета из договора-оферты и старт его согласования";
      stage.TimeoutInHours = 2;
      stage.Save();
    }
    
    /// <summary>
    /// Создание сценария установки флага проверки маркеров Фокус
    /// </summary>
    public void CreateAutoSetFocusFlagStage()
    {
      InitializationLogger.DebugFormat("Init: Create stage for automatic set Focus flag.");
      if (SberContracts.AutoSetFocusFlags.GetAll().Any())
        return;
      var stage = SberContracts.AutoSetFocusFlags.Create();
      stage.Name = "Установка состояния \"Проверен\" для КА";
      stage.TimeoutInHours = 2;
      stage.Save();
    }
    
    /// <summary>
    /// Создании записи сценария "Установка состояния "Принят к оплате" для счета"
    /// </summary>
    public void CreateAutoSetLifeCycleStateActiveStage()
    {
      InitializationLogger.DebugFormat("Init: Create stage for automatic setting life cycle state \"Active\" for incoming invoices.");
      if (SberContracts.AutoSetLifeCycleStateActives.GetAll().Any())
        return;
      var stage = SberContracts.AutoSetLifeCycleStateActives.Create();
      stage.Name = "Установка состояния \"Принят к оплате\" для счета";
      stage.TimeoutInHours = 2;
      stage.Save();
    }
    
    /// <summary>
    /// Создание сценария записи ID из Директума в метаданные документа
    /// </summary>
    public void CreateAutoSetMetaIDStage()
    {
      InitializationLogger.DebugFormat("Init: Create stage for automatic create meta-property \"DirectumID\" with document card ID.");
      if (SberContracts.AutoSetMetaIDs.GetAll().Any())
        return;
      var stage = SberContracts.AutoSetMetaIDs.Create();
      stage.Name = "Запись ИД карточки документа в метаданные его версии(тела)";
      stage.TimeoutInHours = 2;
      stage.Save();
    }
    
    
    /// <summary>
    /// Создании записи сценария "Установка состояния "В реестре платежей" для счета"
    /// </summary>
    public void CreateAutoSetLifeCycleStateInRegisterStage()
    {
      InitializationLogger.DebugFormat("Init: Create stage for automatic setting life cycle state \"In payment register\" for incoming invoices.");
      if (SberContracts.AutoSetLifeCycleStateInRegisters.GetAll().Any())
        return;
      var stage = SberContracts.AutoSetLifeCycleStateInRegisters.Create();
      stage.Name = "Установка состояния \"В реестре платежей\" для счета";
      stage.TimeoutInHours = 2;
      stage.Save();
    }

    /// <summary>
    /// Создание этапа согласования "Проверка договорных документов Делопроизводителем"
    /// </summary>
    public void CreateApprovalStageManuallyChecking()
    {
      InitializationLogger.DebugFormat("Init: Создание этапа соглаосвания \"Проверка договорных документов Делопроизводителем\".");
      if (SBContracts.ApprovalStages.GetAll(s => Equals(s.Subject, "Проверка договорных документов Делопроизводителем")).Any())
        return;
      var stage = SBContracts.ApprovalStages.Create();
      stage.Subject = "Проверка договорных документов Делопроизводителем";
      stage.Name = "Проверка договорных документов Делопроизводителем";
      stage.DeadlineInDays = 2;
      stage.StageType = Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      stage.AllowSendToRework = true;
      stage.Status = Sungero.Docflow.ApprovalStage.Status.Active;
      stage.RightType = Sungero.Docflow.ApprovalStage.RightType.Edit;
      var recip = stage.Recipients.AddNew();
      recip.Recipient = Groups.GetAll().Where(r => Equals(r.Name, "Делопроизводитель")).First();
      stage.AssignmentInstruction = sberdev.SberContracts.Resources.MCTaskInstr;
      stage.Save();
    }
    
    /// <summary>
    /// Создание этапа согласования "Согласование закупки"
    /// </summary>
    public void CreateApprovalStagePurchseApproval()
    {
      InitializationLogger.DebugFormat("Init: Создание этапа соглаосвания \"Согласование закупки\".");
      if (SBContracts.ApprovalStages.GetAll(s => Equals(s.Subject, "Согласование закупки")).Any())
        return;
      var stage = SBContracts.ApprovalStages.Create();
      stage.Subject = "Согласование закупки";
      stage.Name = "Согласование закупки";
      stage.DeadlineInDays = 4;
      stage.StageType = Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      stage.Status = Sungero.Docflow.ApprovalStage.Status.Active;
      stage.RightType = Sungero.Docflow.ApprovalStage.RightType.Edit;
      var recip = stage.Recipients.AddNew();
      recip.Recipient = Groups.GetAll().Where(r => Equals(r.Sid, Constants.Module.BusinessControlerRoleGuid)).First();
      stage.AssignmentInstruction = sberdev.SberContracts.Resources.PATaskInstr;
      stage.Save();
    }
    
    /// <summary>
    /// Создание этапа согласования "Запустить согласование ГП (договор)"
    /// </summary>
    public void CreateApprovalStageGLStartedContractApproval()
    {
      InitializationLogger.DebugFormat("Init: Создание этапа соглаосвания \"Запустить согласование ГП (договор)\".");
      if (SBContracts.ApprovalStages.GetAll(s => Equals(s.Subject, "Запустить согласование ГП (договор)")).Any())
        return;
      var stage = SBContracts.ApprovalStages.Create();
      stage.Subject = "Запустить согласование ГП (договор)";
      stage.Name = "Запустить согласование ГП (договор)";
      stage.DeadlineInDays = 2;
      stage.StageType = Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      stage.Status = Sungero.Docflow.ApprovalStage.Status.Active;
      stage.RightType = Sungero.Docflow.ApprovalStage.RightType.Edit;
      var recip = stage.Recipients.AddNew();
      recip.Recipient = Groups.GetAll().Where(r => Equals(r.Sid, Constants.Module.BusinessControlerRoleGuid)).First();
      stage.AssignmentInstruction = sberdev.SberContracts.Resources.PATaskInstr;
      stage.Save();
    }
    
    /// <summary>
    /// Создание этапа согласования "Предоставление почтовой информации"
    /// </summary>
    public void CreateApprovalStageProvidePostalInformation()
    {
      InitializationLogger.DebugFormat("Init: Создание этапа соглаосвания \"Предоставление почтовой информации\".");
      if (SBContracts.ApprovalStages.GetAll(s => Equals(s.Subject, "Предоставление почтовой информации")).Any())
        return;
      var stage = SBContracts.ApprovalStages.Create();
      stage.Subject = "Предоставление почтовой информации";
      stage.Name = "Предоставление почтовой информации";
      stage.DeadlineInHours = 4;
      stage.StageType = Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      stage.Status = Sungero.Docflow.ApprovalStage.Status.Active;
      stage.RightType = Sungero.Docflow.ApprovalStage.RightType.Edit;
      var role = stage.ApprovalRoles.AddNew();
      role.ApprovalRole = Sungero.Docflow.ApprovalRoleBases.GetAll(r => r.Type == Sungero.Docflow.ApprovalRoleBase.Type.Initiator).First();
      stage.AssignmentInstruction = sberdev.SberContracts.Resources.ProvidePostalInformationInstruction;
      stage.Save();
    }

    #endregion

    #region Прочее
    
    public static void CreateMVZ()
    {
      var OldMVZ = SberContracts.MVZs.GetAll(r => r.GUID == Constants.Module.OldMVZGuid.ToString()).FirstOrDefault();
      if (OldMVZ == null)
      {OldMVZ = SberContracts.MVZs.Create();
        OldMVZ.Name = "Для исторических договоров";
        OldMVZ.ContrType = SberContracts.MVZ.ContrType.Expendable;
        OldMVZ.GUID =  Constants.Module.OldMVZGuid.ToString();
        OldMVZ.BudgetOwner = Sungero.Company.Employees.GetAll().FirstOrDefault();
        OldMVZ.Save();
        InitializationLogger.Debug("Создан МВЗ");
      }
      var OldMVP = SberContracts.MVZs.GetAll(r => r.GUID == Constants.Module.OldMVPGuid.ToString()).FirstOrDefault();
      if (OldMVP == null)
      {OldMVP = SberContracts.MVZs.Create();
        OldMVP.Name = "Для исторических договоров";
        OldMVP.ContrType = SberContracts.MVZ.ContrType.Profitable;
        OldMVP.GUID =  Constants.Module.OldMVPGuid.ToString();
        OldMVP.BudgetOwner = Sungero.Company.Employees.GetAll().FirstOrDefault();
        OldMVP.Save();
        InitializationLogger.Debug("Создан МВП");
      }
    }
    
    /// <summary>
    /// Создание группы "Псевдо-пользователи" и "Исключения для процесса раскрытия полных имен"
    /// </summary>
    public void CreateGroupsForJobChangeFakePersonsNames()
    {
      InitializationLogger.DebugFormat("Init: Создание группы \"Псевдо-пользователи\" и \"Исключения для процесса раскрытия полных имен\"");
      if (Sungero.Company.Departments.GetAll().Where(d => Equals(d.Name, "Псевдо-пользователи")
                                                     || Equals(d.Name, "Исключения для процесса раскрытия полных имен")).Count() == 2)
        return;
      
      var depFU = Sungero.Company.Departments.Create();
      depFU.Name = "Псевдо-пользователи";
      depFU.Save();
      var depEx = Sungero.Company.Departments.Create();
      depEx.Name = "Исключения для процесса раскрытия полных имен";
      depEx.Save();
    }
    
    public void CreateStabs()
    {
      var MVPStab = SberContracts.MVZs.GetAll(r => r.GUID == Constants.Module.MVPStabGuid.ToString()).FirstOrDefault();
      if (MVPStab == null)
      {MVPStab = SberContracts.MVZs.Create();
        MVPStab.Name = "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)";
        MVPStab.ContrType = SberContracts.MVZ.ContrType.Profitable;
        MVPStab.GUID =  Constants.Module.MVPStabGuid.ToString();
        MVPStab.BudgetOwner = Sungero.Company.Employees.GetAll().FirstOrDefault();
        MVPStab.BusinessUnit = Sungero.Company.BusinessUnits.GetAll().FirstOrDefault();
        MVPStab.CalculationIsWorking = false;
        MVPStab.Save();
        InitializationLogger.Debug("Создан МВП - заглушка для вх. документов");
      }
      var MVZStab = SberContracts.MVZs.GetAll(r => r.GUID == Constants.Module.MVZStabGuid.ToString()).FirstOrDefault();
      if (MVZStab == null)
      {MVZStab = SberContracts.MVZs.Create();
        MVZStab.Name = "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)";
        MVZStab.ContrType = SberContracts.MVZ.ContrType.Expendable;
        MVZStab.GUID =  Constants.Module.MVZStabGuid.ToString();
        MVZStab.BudgetOwner = Sungero.Company.Employees.GetAll().FirstOrDefault();
        MVZStab.BusinessUnit = Sungero.Company.BusinessUnits.GetAll().FirstOrDefault();
        MVZStab.CalculationIsWorking = false;
        MVZStab.Save();
        InitializationLogger.Debug("Создан МВЗ - заглушка для вх. документов");
      }
      var AccArtExStab = SberContracts.AccountingArticleses.GetAll(r => r.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                   && r.ContrType == SberContracts.AccountingArticles.ContrType.Expendable).FirstOrDefault();
      if (AccArtExStab == null)
      {AccArtExStab = SberContracts.AccountingArticleses.Create();
        AccArtExStab.Name = "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)";
        AccArtExStab.ContrType = SberContracts.AccountingArticles.ContrType.Expendable;
        AccArtExStab.Save();
        InitializationLogger.Debug("Создана Статья упр. учета - заглушка для вх. документов");
      }
      var AccArtPrStab = SberContracts.AccountingArticleses.GetAll(r => r.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)"
                                                                   && r.ContrType == SberContracts.AccountingArticles.ContrType.Profitable).FirstOrDefault();
      if (AccArtPrStab == null)
      {AccArtPrStab = SberContracts.AccountingArticleses.Create();
        AccArtPrStab.Name = "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)";
        AccArtPrStab.ContrType = SberContracts.AccountingArticles.ContrType.Profitable;
        AccArtPrStab.Save();
        InitializationLogger.Debug("Создана Статья упр. учета - заглушка для вх. документов");
      }
      var ProductStab = SberContracts.ProductsAndDeviceses.GetAll(r => r.Name == "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)").FirstOrDefault();
      if (ProductStab == null)
      {ProductStab = SberContracts.ProductsAndDeviceses.Create();
        ProductStab.Name = "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)";
        ProductStab.BusinessUnit = Sungero.Company.BusinessUnits.GetAll().FirstOrDefault();
        ProductStab.Save();
        InitializationLogger.Debug("Создан Продукт - заглушка для вх. документов");
      }
    }
    
    public void CreateDevSettings()
    {
      Dictionary<string, string> settingsNames = new  Dictionary<string, string>();
      settingsNames.Add("Путь к папке с логами", "В текстовом параметре нужно указать путь к папке с логами.");
      settingsNames.Add("Путь к папке с договорами", "В текстовом параметре нужно указать путь к папке в которой будут храниться выгружаемые договора.");
      settingsNames.Add("Путь к папке с шаблонами", "В текстовом параметре нужно указать путь к папке в которой будут храниться шаблоны для автосоздоваемых типов документов.");
      settingsNames.Add("ИД сущностей для договора Xiongxin", "1) Категория - С поставщиком 2) Контрагент - Xiongxin 3) Валюта - Юань 4) Способ доставки - По ел. почте 5) МВЗ - PROD 6) Статья упр. учета - Оплата товаров и услуг в составе себестоимости. Перечилслить ИД необходимых сущностей в текстовом параметре");
      foreach(var settingName in settingsNames)
      {
        var devSet = SBContracts.PublicFunctions.Module.Remote.GetDevSetting(settingName.Key);
        if (devSet == null)
        {
          devSet = DevSettingses.Create();
          devSet.Name = settingName.Key;
          devSet.Discription = settingName.Value;
          devSet.Save();
        }
      }
    }
    
    public void CreateTempDocsDir()
    {
      DirectoryInfo dir = new DirectoryInfo("C:\\TempDocs");
      if (!dir.Exists)
        dir.Create();
    }
    #endregion
    
  }
}
