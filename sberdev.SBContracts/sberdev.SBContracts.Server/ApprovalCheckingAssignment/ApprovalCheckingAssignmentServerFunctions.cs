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
    /// Получение пользователя по сотруднику
    /// </summary>   
    [Public]    
    public static IUser GetUserToEmployeer(Sungero.Company.IEmployee empl)
    {
      return Users.GetAll(u => u.Login == empl.Login).FirstOrDefault();
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
  }
}