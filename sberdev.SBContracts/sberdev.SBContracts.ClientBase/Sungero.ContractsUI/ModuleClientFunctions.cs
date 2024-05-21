using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;

namespace sberdev.SBContracts.Module.ContractsUI.Client
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// 
    /// </summary>
    public virtual void Razregist()
      
    {
//            var contracts = sberdev.SBContracts.Contracts.GetAll(); // sberdev.SBContracts.Contracts.GetAll();
//      foreach (var contract in contracts)
//      {
//        contract.RegistrationState = sberdev.SBContracts.Contract.RegistrationState.NotRegistered;
//        contract.MVPBaseSberDev = sberdev.SberContracts.MVZs.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
//        contract.MVZBaseSberDev = sberdev.SberContracts.MVZs.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
//        //contract.BusinessUnit = Sungero.Company.BusinessUnits.GetAll(c => c.Name == "ООО «ТД ЕПК" || c.Name == "ООО \"СберДевайсы\"").FirstOrDefault();
//              contract.AccArtPrBaseSberDev = SberContracts.AccountingArticleses.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
//              contract.ProdCollectionPrBaseSberDev.AddNew().Prodsberdev = SberContracts.ProductsAndDeviceses.GetAll(c => c.Name == "Для исторических договоров").FirstOrDefault();
//        contract.Save();
//        
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void DeleteAllContracts()
    {
//      var contracts = Sungero.Contracts.Contracts.GetAll();
//      foreach (var contract in contracts)
//      {
//        var asyncDelete = sberdev.SberContracts.AsyncHandlers.DeleteContractById.Create();
//        asyncDelete.ContrId = contract.Id;
//        asyncDelete.ExecuteAsync();
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ContractMigrationAction()
    {
      var buttonDial       = Dialogs.CreateTaskDialog("Режим миграции данных", "Что необходимо загрузить?");
      var contractsBtn   = buttonDial.Buttons.AddCustom("Договоры");
      var supAgreementBtn   = buttonDial.Buttons.AddCustom("Доп соглашения");
      var attachmentBtn     = buttonDial.Buttons.AddCustom("Приложения");
      var documentsBtn       = buttonDial.Buttons.AddCustom("Документы");
      
      var result = buttonDial.Show();
      
      if (result == contractsBtn || result == supAgreementBtn || result == attachmentBtn || result == documentsBtn) {
        var dfile = Dialogs.CreateInputDialog("Выберите файл");
        var file = dfile.AddFileSelect("Файл", false);
        file.WithFilter("Эксель файлы .xlsx", "xlsx");
        
        if (dfile.Show() == DialogButtons.Ok)
        {
          // Получить содержимое файла.
          var fileContent = file.Value.Content;
          var fileName = file.Value.Name;
          // Сохранить содержимое файла на жесткий диск в папку Темп.
          var tempFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".xlsx";
          using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, System.IO.FileAccess.Write)) {
            using (var memory = new System.IO.MemoryStream(fileContent))
            {
              memory.WriteTo(fileStream);
            }
            
          }
          if (result == contractsBtn) {
            IZip zip =  SberContracts.PublicFunctions.Module.Remote.MigrationContractCard(tempFilePath); 
            zip.Export();
          }
          if (result == supAgreementBtn) {
            IZip zip = SberContracts.PublicFunctions.Module.Remote.MigrationContractCard(tempFilePath);
            zip.Export();
          }
          if (result == attachmentBtn) {
            IZip zip = SberContracts.PublicFunctions.Module.Remote.MigrationContractCard(tempFilePath);
            zip.Export();
          }
          if (result == documentsBtn) {
            IZip zip = SberContracts.PublicFunctions.Module.Remote.MigrationFile(tempFilePath);
            zip.Export();
          }
        }
      }

    }

  }
}