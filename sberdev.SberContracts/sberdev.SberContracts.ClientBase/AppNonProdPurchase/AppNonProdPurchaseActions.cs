using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppNonProdPurchase;

namespace sberdev.SberContracts.Client
{
  partial class AppNonProdPurchaseActions
  {
    public virtual void SendToBPOfficial(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      e.CloseFormAfterAction = true;
      var task = SDev.BPCustom.OfficialTasks.Create();
      task.BaseAttachments.OfficialDocuments.Add(_obj);
      if (_obj.HasRelations)
      {
        foreach (var elem in _obj.Relations.GetRelated())
        {
          task.OtherAttachmentGroup.ElectronicDocuments.Add(elem);
        }
      }
      task.Subject = "Согласование: " + _obj.Name;
      task.Show();
    }

    public virtual bool CanSendToBPOfficial(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void SendToBPTaskBase(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      e.CloseFormAfterAction = true;
      var task = SDev.BPCustom.TaskToPerformers.Create();
      task.BaseAttachments.ElectronicDocuments.Add(_obj);
      if (_obj.HasRelations)
      {
        foreach (var elem in _obj.Relations.GetRelated())
        {
          task.OtherAttachments.ElectronicDocuments.Add(elem);
        }
      }
      task.Subject = "Задача на исполнение: " + _obj.Name;
      task.Show();
    }

    public virtual bool CanSendToBPTaskBase(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        var rolesorsing = Roles.GetAll(r => r.Name == "Сорсинг").FirstOrDefault();
        if (rolesorsing != null)
        {
          if (!Users.Current.IncludedIn(rolesorsing))
          {
            if (_obj.CalculationAmountBaseSberDev > 500000)
              e.AddError("Сумма закупки превышает 500 тыс. руб. Пожалуйста, оформите заявку через портал Сорсинга https://tasks.sberdevices.ru/servicedesk/customer/portal/47");
            if (_obj.EstimatedDeliveryCost > 500000)
              e.AddError("Сумма закупки превышает 500 тыс. руб. Пожалуйста, оформите заявку через портал Сорсинга https://tasks.sberdevices.ru/servicedesk/customer/portal/47");
          }
        }
      }
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.State.Properties.Counterparty.IsRequired = false;
      base.Register(e);
    }

    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRegister(e);
    }

  }

}