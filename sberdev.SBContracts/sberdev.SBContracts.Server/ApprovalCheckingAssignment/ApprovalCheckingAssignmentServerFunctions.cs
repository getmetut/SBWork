using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckingAssignment;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalCheckingAssignmentFunctions
  {

    /// <summary>
    /// Переадресовать задание без его выполнения
    /// </summary>
    [Public]
    public static void ReSendToUser(IUser us, IApprovalCheckingAssignment job)
    {
      var taska = ApprovalCheckingAssignments.As(job);
      taska.Performer = us;
      
      if (!taska.AccessRights.CanUpdate(us))
        taska.AccessRights.Grant(us, DefaultAccessRightsTypes.Change);
      
      foreach (var att in taska.AllAttachments)
      {
        try
        {
          if (!att.AccessRights.CanUpdate(us))
            att.AccessRights.Grant(us, DefaultAccessRightsTypes.Change);
        }
        catch (Exception err)
        {
          Logger.Debug(string.Format("Ошибка выдачи прав доступа в задании: {0} => {1}", taska.Id, err.Message));
        }
      }
      var folderApproval = PublicFunctions.ApprovalCheckingAssignment.GetUserFolderApproval(us);
      if (folderApproval != null)
        folderApproval.Items.Add(taska);
      
      var newJob = Sungero.Workflow.SimpleTasks.CreateWithNotices("На вас назначено задание: " + taska.Subject, us);
      newJob.ActiveText = "На вас назначено задание: " + Hyperlinks.Get(taska);
      newJob.Attachments.Add(taska);
      newJob.Save();
      
      taska.Save();
    }
    /// <summary>
    /// Получение пользователя по сотруднику
    /// </summary>
    [Public]
    public static IFolder GetUserFolderApproval(IUser empl)
    {
      return Folders.GetAll(f => f.Name == "Approval").Where(f => f.Author == empl).FirstOrDefault();
    }
    
    /// <summary>
    /// Возвращает согласуемый документ
    /// </summary>
    /// <returns></returns>
    [Remote]
    public SBContracts.IOfficialDocument GetAttachment()
    {
      return SBContracts.OfficialDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault()/*Task?.Attachments.FirstOrDefault()*/);
    }
    /// <summary>
    /// Получение пользователя по сотруднику
    /// </summary>
    [Public]
    public static IUser GetUserToEmployeer(Sungero.Company.IEmployee empl)
    {
      return Users.GetAll(u => u.Login == empl.Login).FirstOrDefault();
    }
  }
}
