using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Docflow.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// 
    /// </summary>
    public void SendCheckDocumentSignTask(Sungero.Workflow.IAssignmentBase assign)
    {
      var subtask = SberContracts.CheckDocumentSignTasks.CreateAsSubtask(assign);
      subtask.Author = assign.Performer;
      subtask.Subject = "Напомнить о подписании";
      subtask.ActiveText = "Просим напомнить к/а о подписании документа. В случае отсутствия подписанного с двух сторон документа," +
        " через 1 неделю подзадача переадресуется обратно Делопроизводителю";
      subtask.Performer = assign.Task.Author;
      subtask.Start();
    }
    
    public void SendCheckDocumentSignNotification(SberContracts.ICheckDocumentSignTask task)
    {
      var notice = SberContracts.CheckDocumentSignNotifications.Create(task);
      notice.Performer = task.Performer;
      notice.Subject = "Не забудьте напомнить к/а о подписании документа";
      notice.Save();
    }
    
    public override bool GeneratePublicBodyForExchangeDocument(Sungero.Docflow.IOfficialDocument document, long versionId, Nullable<Enumeration> exchangeState, Nullable<DateTime> startTime)
    {
      SBContracts.OfficialDocuments.As(document).BodyExtSberDev = document.LastVersion.AssociatedApplication.Extension;
      return base.GeneratePublicBodyForExchangeDocument(document, versionId, exchangeState, startTime);
    }
    
    public override void GeneratePublicBodyForExchangeDocument(Sungero.Docflow.IOfficialDocument document, long versionId, Nullable<Enumeration> exchangeState)
    {
      SBContracts.OfficialDocuments.As(document).BodyExtSberDev = document.LastVersion.AssociatedApplication.Extension;
      base.GeneratePublicBodyForExchangeDocument(document, versionId, exchangeState);
    }
    
    public override Sungero.Docflow.Structures.OfficialDocument.ConversionToPdfResult GeneratePublicBodyWithSignatureMark(Sungero.Docflow.IOfficialDocument document, long versionId, string signatureMark)
    {
      SBContracts.OfficialDocuments.As(document).BodyExtSberDev = document.LastVersion.AssociatedApplication.Extension;
      return base.GeneratePublicBodyWithSignatureMark(document, versionId, signatureMark);
    }
  }
}