using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalTaskFunctions
  {

    /// <summary>
    /// Установка срочности для задачи
    /// </summary> 
    [Public]
    public void HighImportanceIns()
    {
      if (_obj.Importance != Importance.High)
      {
        var Dial = Dialogs.CreateInputDialog("Введите обоснование срочности задачи");
        var obosn = Dial.AddMultilineString("Обоснование",true);
        var text = Dial.AddMultilineString("Справка",false,@"Укажите в обосновании: Опишите необходимость срочного согласования.
        Почему заявка стала срочной?
        Кто инициатор срочности?
        Какие негативные последствия и риски для компании, если вовремя не будет согласована?");
        
        if (Dial.Show() == DialogButtons.Ok)
        {
          _obj.Importance = Importance.High;
          _obj.StrHighImportanceATSDev = obosn.Value.ToString();
          _obj.HighImportanceUserSberDev = Users.Current.Name;
        }
      }
      else
        Dialogs.ShowMessage("У задачи уже установлена высокая важность.");
    }
    
    [Public]
    public bool ShowConfirmationAmountDialog()
    {
      var dialog = Dialogs.CreateConfirmDialog(sberdev.SBContracts.ApprovalTasks.Resources.GuranteeAmountConfirm);
      return dialog.Show();
    }
    
    /// <summary>
    /// Вывести сообщение с результатом проверки подписей договорных документов при отправке входящего счета на согласование
    /// </summary>
    [Public]
    public string ShowCheckSignaturesResult(bool flagContractOwn, bool flagContractCounter, bool flagActOwn, bool flagActCounter)
    {
      string result = "";
        result += "Нельзя запустить согласование.\n";
        if (!flagContractOwn || !flagContractCounter)
          result += "Договор не имеет необходимой подписи: \n";
        if (!flagContractOwn)
          result += "— от нашей организации\n";
        if (!flagContractCounter)
          result += "— от контрагента\n";
        if (!flagActOwn || !flagActCounter)
          result += "Дополнительный финансовый документ не имеет необходимой подписи: \n";
        if (!flagActOwn)
          result += "— от нашей организации\n";
        if (!flagActCounter)
          result += "— от контрагента\n";
      
      return result;
    }

    [Public]
    public string ShowCheckSignaturesResult(bool flagContractOwn, bool flagContractCounter)
    {
      string result = "";
        result += "Нельзя запустить согласование.\n";
        if (!flagContractOwn || !flagContractCounter)
          result += "Договор не имеет необходимой подписи: \n";
        if (!flagContractOwn)
          result += "— от нашей организации\n";
        if (!flagContractCounter)
          result += "— от контрагента\n";
      
      return result;
    }
  }
}