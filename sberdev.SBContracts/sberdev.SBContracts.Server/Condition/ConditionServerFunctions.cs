using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Condition;

namespace sberdev.SBContracts.Server
{
  partial class ConditionFunctions
  {
    public override string GetConditionName()

    {

      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.MarketDirect)
        {
          string head = "Направление маркетинга =";
          foreach (var dir in _obj.MarketDirectSberDev)
            head += String.Format(" {0};", dir.MarketDirect.Name);
          head = head.TrimEnd(new char[] {';'});
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.FCDApprByTreasSberDev)
          return "Есть согласование на КЗД от казначея?";
        
        if (_obj.ConditionType == ConditionType.ContractSigned)
          return "Договор подписан?";
        
        if (_obj.ConditionType == ConditionType.PurchApproved)
          return "Закупка согласована?";
        
        if (_obj.ConditionType == ConditionType.ManuallyCheck)
          return "Необходима ручная проверка договорных документов?";
        
        if (_obj.ConditionType == ConditionType.DocumentChanged)
          return "Документ был изменен?";

        if (_obj.ConditionType == ConditionType.PayType)
          return ("Постоплата?");
        
        if (_obj.ConditionType == ConditionType.Framework)
          return ("Рамка/безденежный?");
        
        if (_obj.ConditionType == ConditionType.MVZ)
        { 
          string text;
          text = "МВЗ = ";
          foreach (var str in _obj.MVZ )
          {
            text = text + str.MVZ.Name + ", ";
          }
          return (text);
        }
        if (_obj.ConditionType == ConditionType.MVP)
        { 
          string text;
          text = "МВП = ";
          foreach (var str in _obj.MVP )
          {
            text = text + str.MVP.Name + "; ";
          }
          return (text);
        }
        
        if (_obj.ConditionType == ConditionType.AccArts)
        { 
          string text;
          text = "Статьи упр. учета = ";
          foreach (var str in _obj.AccountingArticles )
          {
            text = text + str.AccountingArticles.Name + "; ";
          }
          return (text);
        }
        
        if (_obj.ConditionType == ConditionType.BudgetItem)
        { 
          string text;
          text = "Статьи бух. учета = ";
          foreach (var str in _obj.BudgetItem )
          {
            text = text + str.BudgetItem.Name + "; ";
          }
          return (text);
        }
        
        if (_obj.ConditionType == ConditionType.ActExists)
          return ("Акт есть?");
        
        if (_obj.ConditionType == ConditionType.PayType)
          return ("Устройство есть?");
        
        if (_obj.ConditionType == ConditionType.PayType)
          return ("Оплачено?");
        
        if (_obj.ConditionType == ConditionType.PayType)
          return ("Цены согласовны?");
        
      }

      return base.GetConditionName();

    }
  }
}