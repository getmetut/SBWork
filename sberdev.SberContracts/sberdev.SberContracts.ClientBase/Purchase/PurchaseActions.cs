using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts.Client
{
  partial class PurchaseActions
  {
    public virtual void CreateBodyByPropertiesPurch(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      SBContracts.PublicFunctions.Module.Remote.CreateBodyByProperties(_obj);
      _obj.Save();
    }

    public virtual bool CanCreateBodyByPropertiesPurch(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool flag = false;
      var name = _obj.DocumentKind.Name;
      if (name == "Договор Xiongxin" || name == "Дополнительное соглашение Xiongxin")
        flag = true;
      return !_obj.State.IsInserted && flag;
    }

    public virtual void SendToTM(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.CloseFormAfterAction = true;
      var Task = SDev.BPCustom.PurchaseTasks.Create();
      Task.BaseAttachments.Purchases.Add(sberdev.SberContracts.Purchases.Get(_obj.Id));
      Task.Subject = "Согласование: " + _obj.Name.ToString();
      Task.Save();
      Task.Show();
    }

    public virtual bool CanSendToTM(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var User = Users.Current;
      var ctr = false;
      var Rol = Roles.GetAll(r => r.Name == "Тестировщики").FirstOrDefault();
      if (Rol != null)
      {
        foreach (var us in Rol.RecipientLinks)
        {
          if (us.Member.Name == User.Name)
            ctr = true;
        }
      }
      return ctr;
    }

  }



}