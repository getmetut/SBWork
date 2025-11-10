using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts;
using Sungero.Metadata;
using Sungero.Domain.Shared;

namespace SberDev.ExchDocDistrib.Shared
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Отдает необходимого ответственного за ящик.
    /// </summary>
    /// <param name="task"></param>
    [Public]
    public Sungero.Company.IEmployee GetBoxResponsibleEmployee(sberdev.SBContracts.IExchangeDocumentProcessingTask task)
    {
      var respList = GetBoxResponsibleEmployees(task.Box);
      var resp = task.HasAccAttachSberDev == true ? respList[1] : respList[0];
      return resp;
    }
    
    /// <summary>
    /// Отдает список из двух ответсвенных за ящик нашей компании. Основной и финансовый (указан в нашей компании).
    /// </summary>
    /// <param name="box"></param>
    [Public]
    public System.Collections.Generic.List<Sungero.Company.IEmployee> GetBoxResponsibleEmployees(Sungero.ExchangeCore.IBoxBase box)
    {
      List<Sungero.Company.IEmployee> list = new List<Sungero.Company.IEmployee>();
      var unitBox = BusinessUnitBoxes.As(box);
      var depBox = Sungero.ExchangeCore.DepartmentBoxes.As(box);
      var unit = unitBox != null ? unitBox.BusinessUnit : depBox.RootBox.BusinessUnit;
      var respAcc = BusinessUnits.As(unit).AccDocsHandlerSberDev;
      list.Add(unitBox.Responsible);
      if (respAcc != null)
        list.Add(respAcc);
      return list;
    }
    
    /// <summary>
    /// Если одно из вложений формализованно под фин. документ, перенаправляет задачу фин. ответственному.
    /// </summary>
    /// <param name="assign"></param>
    [Public]
    public void DistributeFormalizedAccDocuments(sberdev.SBContracts.IExchangeDocumentProcessingTask task)
    {
      if (task.HasAccAttachSberDev != null || !task.AllAttachments.Any())
        return;
      
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
      
      var unitBox = BusinessUnitBoxes.As(task.Box);
      var depBox = Sungero.ExchangeCore.DepartmentBoxes.As(task.Box);
      var unit = unitBox != null ? unitBox.BusinessUnit : depBox.RootBox.BusinessUnit;
      var resp = BusinessUnits.As(unit).AccDocsHandlerSberDev;
      
      if (resp == null || task.Addressee == resp)
        return;

      // Проверка вложений
      if (task.AllAttachments.Any(attach =>
                                  accountingDiscriminants.Contains(attach.GetEntityMetadata().GetOriginal().NameGuid.ToString().ToLower())))
      {
        task.HasAccAttachSberDev = true;
        task.Assignee = resp;
      }
      else
        task.HasAccAttachSberDev = false;
      
    }
  }
}