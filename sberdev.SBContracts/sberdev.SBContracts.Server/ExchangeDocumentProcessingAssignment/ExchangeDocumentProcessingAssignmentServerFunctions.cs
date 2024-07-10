using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Server
{
  partial class ExchangeDocumentProcessingAssignmentFunctions
  {

    /// <summary>
    /// Переадресовывает формалиованый документ
    /// </summary>
    public void DistributeFormalizedDocument()
    {
      string[] accountingDiscriminants = {"a523a263-bc00-40f9-810d-f582bae2205d", "f2f5774d-5ca3-4725-b31d-ac618f6b8850",
      "58986e23-2b0a-4082-af37-bd1991bc6f7e", "f50c4d8a-56bc-43ef-bac3-856f57ca70be",
      "74c9ddd4-4bc4-42b6-8bb0-c91d5e21fb8a", "58ad01fb-6805-426b-9152-4de16d83b258", "4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"};
      var attachs = _obj.Attachments;
      foreach(var attach in attachs)
      {
        string disc = attach.GetEntityMetadata().GetOriginal().NameGuid.ToString().ToLower();
        if (accountingDiscriminants.Contains(disc))
          _obj.Forward(Sungero.Company.Employees.GetAll().Where(e => e.Id == 436).First());
        break;
      }
    }
    /// <summary>
    /// Функция возвращает имена документов из вложений без шелухи
    /// </summary>
    [Public]
    public string GetDocsNames()
    {
      string names = "";
      var attachs = _obj.Task?.Attachments;
      if (!attachs.Any())
        return names;
      foreach (var attach in attachs)
      {
        var doc = Sungero.Content.ElectronicDocuments.As(attach);
        if (doc != null)
        {
          var name = doc.Name;
          name = name.Replace("Вх. док. эл. обмена", "ВЭД");
          name = name.Replace(" от " + _obj.Counterparty.Name, "");
          string id = SberContracts.PublicFunctions.Module.GetNumberTag(name, " _ID");
          while (id != "")
          {
            name = name.Replace(" _ID" + id, "");
            id = SberContracts.PublicFunctions.Module.GetNumberTag(name, " _ID");
          }
          names += name + " // ";
        }
      }
      names = names.TrimEnd(new char[]{'/',' '});
      
      if (names.Length > 1000)
        return names.Substring(0, 1000);
      else
        return names;
    }
  }
}