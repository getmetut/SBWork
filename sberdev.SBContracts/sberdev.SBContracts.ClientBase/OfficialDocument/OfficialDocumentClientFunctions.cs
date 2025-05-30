using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts.Client
{
  partial class OfficialDocumentFunctions
  { 
    /// <summary>
    /// Вызывает диалоговое оконо для публикации коментария к задаче
    /// </summary>       
    public string PublicCommentDialog()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ApprovalTasks.Resources.PublicCommentDialogTitle);
      var text = dialog.AddMultilineString(sberdev.SBContracts.ApprovalTasks.Resources.PublicCommentDialogProp, true);
      if (dialog.Show() == DialogButtons.Ok)
        return text.Value;
      else
        return "";
      
    }
    
    public override Sungero.Docflow.IOfficialDocument ChangeDocumentType(List<Sungero.Domain.Shared.IEntityInfo> types)
    {
      var CDT = base.ChangeDocumentType(types);
      if (CDT != null)
        CDT.RegistrationDate = CDT.Created;
      
      return CDT;
    }
  }
}