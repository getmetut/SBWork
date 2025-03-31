using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Shared
{
  partial class ApprovalTaskFunctions
  {
    public void FillDocumentTypeOnTaskStart()
    {
      try
      {
        // Пропускаем, если тип документа уже заполнен
        if (!string.IsNullOrEmpty(_obj.DocumentTypeSungero))
          return;
        
        // Определяем документ из группы вложений
        if (_obj.DocumentGroup != null && _obj.DocumentGroup.OfficialDocuments.Any())
        {
          var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
          if (document != null)
          {
            // Последовательно проверяем типы
            foreach (var docType in new[] { "Contractual", "IncInvoce", "Accounting", "AbstractContr", "Another" })
            {
              if (SberContracts.PublicFunctions.Module.SafeMatchesDocumentType(document, docType))
              {
                _obj.DocumentTypeSungero = docType;
                return;
              }
            }
            
            // Если ни один тип не подошел, устанавливаем "Another"
            _obj.DocumentTypeSungero = "Another";
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error($"Ошибка при заполнении типа документа для задачи {_obj.Id}", ex);
      }
    }

    public override void SetVisibleProperties(Sungero.Docflow.Structures.ApprovalTask.RefreshParameters refreshParameters)
    {
      base.SetVisibleProperties(refreshParameters);
      var attach = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (attach != null && SBContracts.IncomingInvoices.Is(attach)
          && SBContracts.IncomingInvoices.As(attach).NoNeedLeadingDocs == false)
        _obj.State.Properties.IsNeedManuallyCheckSberDev.IsVisible = true;
      else
        _obj.State.Properties.IsNeedManuallyCheckSberDev.IsVisible = false;
    }
  }
}
