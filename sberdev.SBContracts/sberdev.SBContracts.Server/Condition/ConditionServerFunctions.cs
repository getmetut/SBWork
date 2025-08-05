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
    /// <summary>
    /// 
    /// </summary>   
    [Remote]
    public bool CheckSameAttorney(Sungero.ATS.IPowerOfAttorney power)
    {
      return Sungero.ATS.PowerOfAttorneys.GetAll().Any(p => p.IssuedTo == power.IssuedTo &&
                                                       p.MatrixSDev == power.MatrixSDev &&
                                                       p.AgentType == power.AgentType);
    }
    
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.Product)
        {
          string head = "Документ по любому из продуктов:";
          foreach (var prod in _obj.ProductsSberDev)
            head += String.Format(" {0};", prod.Product.Name);
          head = head.TrimEnd(';');
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.InitDepart)
        {
          string head = "Подразделение инициатора согласования =";
          foreach (var dep in _obj.InitiatorsDepartmentSberDev)
            head += String.Format(" {0};", dep.InitiatorsDepartment.Name);
          head = head.TrimEnd(';');
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.PurchAmount)
        {
          switch (_obj.AmountOperator.Value.Value)
          {
            case "GreaterOrEqual":
              return "Сумма закупки >= " + _obj.PurchaseAmountSberDev.Value.ToString();
            case "GreaterThan":
              return "Сумма закупки > " + _obj.PurchaseAmountSberDev.Value.ToString();
            case "LessOrEqual":
              return "Сумма закупки <= " + _obj.PurchaseAmountSberDev.Value.ToString();
            case "LessThan":
              return "Сумма закупки < " + _obj.PurchaseAmountSberDev.Value.ToString();
          }
        }
        if (_obj.ConditionType == ConditionType.IsProdPurchase)
        {
          return "Производственная закупка?";
        }
        
        if (_obj.ConditionType == ConditionType.SameAttorney)
        {
          return "Подобная доверенность выдавалась раньше?";
        }
        
        if (_obj.ConditionType == ConditionType.InitIsAttorney)
        {
          return "Инициатор и поверенный одно лицо?";
        }
        
        if (_obj.ConditionType == ConditionType.MRP)
        {
          return "Матрица - МЧД?";
        }
        
        if (_obj.ConditionType == ConditionType.EarlyProxy)
        {
          return "Продление ранней доверенности";
        }
        
        if (_obj.ConditionType == ConditionType.Contrtype)
        {
          return "Документ доходный?";
        }
        
        if (_obj.ConditionType == ConditionType.ProductUnit)
        {
          string head = "В продуктах есть юнит:";
          foreach (var dir in _obj.ProductUnitSberDev)
            head += String.Format(" {0};", dir.ProductUnit.Name);
          head = head.TrimEnd(';');
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.MarketDirect)
        {
          string head = "Направление маркетинга =";
          foreach (var dir in _obj.MarketDirectSberDev)
            head += String.Format(" {0};", dir.MarketDirect.Name);
          head = head.TrimEnd(new char[] {';'});
          head += "?";
          return head;
        }
        
        if (_obj.ConditionType == ConditionType.EndorseFromSberDev)
        {
          return "Есть согласование от " + _obj.EndorserSberDev.Name + "?";
        }
        
        if (_obj.ConditionType == ConditionType.FCDApprBySberDev)
          return String.Format("Есть согласование на КЗД от {0}?", _obj.EndorserSberDev.Name);
        
        if (_obj.ConditionType == ConditionType.ContractSigned)
          return "Договор подписан?";
        
        if (_obj.ConditionType == ConditionType.PurchApproved)
          return "Закупка согласована?";
        
        if (_obj.ConditionType == ConditionType.ManuallyCheck)
          return "Необходима ручная проверка договорных документов?";
        
        if (_obj.ConditionType == ConditionType.DocChangedApprSberDev)
          return "Документ изменен после первого согласования?";

        if (_obj.ConditionType == ConditionType.PayType)
          return ("Постоплата?");
        
        if (_obj.ConditionType == ConditionType.Framework)
          return ("Рамка/безденежный?");
        
        if (_obj.ConditionType == ConditionType.PlusMinus)
        {
          string itg = "Сумма документа ";
          if (_obj.PlusMinusSDev == sberdev.SBContracts.Condition.PlusMinusSDev.Profitable)
            itg += "Доходный?";
          
          if (_obj.PlusMinusSDev == sberdev.SBContracts.Condition.PlusMinusSDev.Expendable)
            itg += "Расходный?";
          
          if (_obj.PlusMinusSDev == sberdev.SBContracts.Condition.PlusMinusSDev.ExpendProfit)
            itg += "Доходно-расходный?";
          
          return (itg);
        }
                
        if (_obj.ConditionType == ConditionType.INNCollection)
          return ("Контроль по ИНН КА");

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