using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingTask;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Server
{
  partial class ExchangeDocumentProcessingTaskFunctions
  {
    public void DistributeFormalizedDocument()
    {
      if (_obj.ReadressedSberDev == false)
      {
        // Массив уникальных идентификаторов
        var accountingDiscriminants = new HashSet<string>
        {
          "a523a263-bc00-40f9-810d-f582bae2205d",
          "f2f5774d-5ca3-4725-b31d-ac618f6b8850",
          "58986e23-2b0a-4082-af37-bd1991bc6f7e",
          "f50c4d8a-56bc-43ef-bac3-856f57ca70be",
          "74c9ddd4-4bc4-42b6-8bb0-c91d5e21fb8a",
          "58ad01fb-6805-426b-9152-4de16d83b258",
          "4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"
        };
        
        var unitBox = SBContracts.BusinessUnitBoxes.As(_obj.Box);
        var depBox = Sungero.ExchangeCore.DepartmentBoxes.As(_obj.Box);
        var unit = unitBox != null ? unitBox.BusinessUnit : depBox.RootBox.BusinessUnit;
        var resp = SBContracts.BusinessUnits.As(unit).AccDocsHandlerSberDev;
        
        if (resp == null || _obj.Addressee == resp)
          return;

        // Проверка вложений
        if (_obj.AllAttachments.Any(attach =>
                                    accountingDiscriminants.Contains(attach.GetEntityMetadata().GetOriginal().NameGuid.ToString().ToLower())))
        {
          _obj.ReadressedSberDev = true;
          _obj.Addressee = resp;
        }
      }
    }
  }
}