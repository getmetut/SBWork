using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractCondition;

namespace sberdev.SBContracts.Server
{
  partial class ContractConditionFunctions
  {
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.ProductUnit)
         {
          string head = "В продуктах есть юнит:";
          foreach (var dir in _obj.ProductUnitSberDev)
            head += String.Format(" {0};", dir.ProductUnit.Name);
          head = head.TrimEnd(';');
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.IsPrepayment)
          return "Тип оплаты = предоплата?";
        
        if (_obj.ConditionType == ConditionType.InvApprBySberDev)
          return String.Format("Есть согласование на счете от {0}?", _obj.EndorserSberDev.Name);
        
        if (_obj.ConditionType == ConditionType.IsNeedCheckCp)
        {
          return "Сумма расходных договоров по данному КА больше 500 тыс. руб. (за календарный год)?";
        }
        
        if (_obj.ConditionType == ConditionType.EndorseFromSberDev)
        {
          return "Есть согласование от " + _obj.EndorserSberDev.Name + "?";
        }
        
        if (_obj.ConditionType == ConditionType.MarketDirect)
        {
          string head = "Направление маркетинга =";
          foreach (var dir in _obj.MarketDirectSberDev)
            head += String.Format(" {0};", dir.MarketDirect.Name);
          head = head.TrimEnd(';');
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.DocumentChanged)
        {
          return "Документ был изменен?";
        }
        
        if (_obj.ConditionType == ConditionType.InitiatorsDepartment)
        {
          string head = "Подразделение инициатора согласования =";
          foreach (var dep in _obj.InitiatorsDepartment)
            head += String.Format(" {0};", dep.InitiatorsDepartment.Name);
          head = head.TrimEnd(';');
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.ContrType)
          return ("Документ доходный?");
        
        if (_obj.ConditionType == ConditionType.PayType)
          return ("Постоплата?");
        
        if (_obj.ConditionType == ConditionType.Framework)
          return ("Рамка/безденежный?");
        
        if (_obj.ConditionType == ConditionType.ContrCategory)
        {
          string text;
          text = "Категория договора = ";
          foreach (var str in _obj.ContrCategorysberdev )
          {
            text = text + str.ContrCategory.Name + ", ";
          }
          text = text.Substring(0, text.Length - 1);
          text += "?";
          return text;
        }
        
        if (_obj.ConditionType == ConditionType.MVP)
        {
          string text;
          text = "МВП = ";
          foreach (var str in _obj.MVP )
          {
            text = text + str.MVP.Name + ", ";
          }
          text = text.Substring(0, text.Length - 1);
          text += "?";
          return text;
        }

        if (_obj.ConditionType == ConditionType.MVZ)
        {
          string text;
          text = "МВЗ = ";
          foreach (var str in _obj.MVZ )
          {
            text = text + str.MVZ.Name + ", ";
          }
          text = text.Substring(0, text.Length - 1);
          text += "?";
          return text;
        }

        if (_obj.ConditionType == ConditionType.AccountAticles)
        {
          string text;
          text = "Статьи упр. учета = ";
          foreach (var str in _obj.AccountingArticles )
          {
            text = text + str.AccountingArticles.Name + ", ";
          }
          text = text.TrimEnd(',');
          text += "?";
          return text;
        }

        if (_obj.ConditionType == ConditionType.BudgetItem)
        {
          string text;
          text = "Статьи бюджета = ";
          foreach (var str in _obj.BudgetItem )
          {
            text = text + str.BudgetItem.Name + ", ";
          }
          text = text.TrimEnd(',');
          text += "?";
          return text;
        }

      }

      return base.GetConditionName();

    }
  }
}