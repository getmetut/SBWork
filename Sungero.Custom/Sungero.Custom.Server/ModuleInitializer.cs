using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Custom.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      var Role = Roles.GetAll().Where(r => r.Name == "Доступ к обложке Custom").FirstOrDefault();
      bool marker = false;
      if (Role != null)
      {
        foreach (var Sot in Role.RecipientLinks)
        {
          if (Users.Current.Id == Sot.Member.Id)
            marker = true;
        }
      }
      return marker;
    }

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateDocumentTypes();
      CreateDocumentKinds();
      GrantCreateRightsOnInfoDocument();
      CreateSystemRole();
    }
    
    public void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Документ маркетинговых операций", Marketing.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Расширенное дополнительное соглашение", SupAgreementPlus.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("NDA", NDA.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Неформализованный отчет", FacelessTochet.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType("Спецификация", Specs.ClassTypeGuid, Sungero.Docflow.DocumentType.DocumentFlow.Incoming, true);
    }
    
    public void CreateDocumentKinds()
    {
      // Создание вида документа «Маркетинговые акции».
      // Чтобы документы можно было регистрировать, задается свойство Registrable.
      // В качестве ИД вида документа используется константа ExplanLettersKind.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Маркетинговые акции", "Маркетинговые акции",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,      
                                                                              Marketing.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.Marketing);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Спецификация", "Спецификация",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Incoming, true, false,      
                                                                              Specs.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.Specs);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Рекламная заявка", "Рекламная заявка",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,      
                                                                              SupAgreementPlus.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.SupPlusContractual);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("NDA ООО 'СалютДевайсы'", "NDA ООО 'СалютДевайсы'",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, false,      
                                                                              NDA.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.NDA);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("NDA ООО 'СалютДевайсы' с правками контрагента ", "NDA ООО 'СалютДевайсы' с правками контрагента ",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, false,      
                                                                              NDA.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.NDA);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("NDA контрагента", "NDA контрагента",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, false,      
                                                                              NDA.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.NDA);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind("Неформализованный отчет", "Неформализованный отчет",      
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable ,      
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,      
                                                                              FacelessTochet.ClassTypeGuid,      
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },      
                                                                              Constants.Module.FacelessTochet);
    }

    public static void GrantCreateRightsOnInfoDocument()
    {
      var allUsers = Roles.AllUsers;
      Sungero.Custom.Marketings.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      Sungero.Custom.Specses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      Sungero.Custom.SupAgreementPluses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      Custom.NDAs.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      Custom.FacelessTochets.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
    }
    
    public static void CreateSystemRole()
    {
      if (Roles.GetAll(r => (r.Name == "Доступ к обложке Custom")).Count() == 0)
      {
        var Role = Roles.Create();
        Role.Name = "Доступ к обложке Custom";
        Role.Status = Sungero.CoreEntities.Role.Status.Active;
        Role.Description = "Программная роль для доступа к обложке Custom";
        Role.IsSingleUser = false;
        Role.Save();
      }
      
      
      if (Roles.GetAll(r => (r.Name == "Доступ к персональным данным")).Count() == 0)
      {
        var Role = Roles.Create();
        Role.Name = "Доступ к персональным данным";
        Role.Status = Sungero.CoreEntities.Role.Status.Active;
        Role.Description = "Программная роль для доступа к персональным данным";
        Role.IsSingleUser = false;
        Role.Save();
      }  
        
      if (Roles.GetAll(r => (r.Name == "Доступ к обложке Отчеты")).Count() == 0)
      {
        var Role = Roles.Create();
        Role.Name = "Доступ к обложке Отчеты";
        Role.Status = Sungero.CoreEntities.Role.Status.Active;
        Role.Description = "Программная роль для доступа к обложке Отчеты";
        Role.IsSingleUser = false;
        Role.Save();
      }
      if (Roles.GetAll(r => (r.Name == "Ответственный бухгалтер за Контрагентов")).Count() == 0)
      {
        var Role = Roles.Create();
        Role.Name = "Ответственный бухгалтер за Контрагентов";
        Role.Status = Sungero.CoreEntities.Role.Status.Active;
        Role.Description = "Программная роль для фильтрации сотрудников по ответственным бухгалтерам в карточке Контрагента.";
        Role.IsSingleUser = false;
        Role.Save();
      }
      
      if (Roles.GetAll(r => (r.Name == "Допуск к маркетинговым документам")).Count() == 0)
      {
        var Role = Roles.Create();
        Role.Name = "Допуск к маркетинговым документам";
        Role.Status = Sungero.CoreEntities.Role.Status.Active;
        Role.Description = "Программная роль для допуска к маркетинговым документам и дополнительным элементам.";
        Role.IsSingleUser = false;
        Role.Save();
      }
    }
  }
}
