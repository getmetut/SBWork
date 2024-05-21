using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocumentProcessingAssignment;

namespace sberdev.SBContracts.Server
{
  partial class ExchangeDocumentProcessingAssignmentFunctions
  {
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