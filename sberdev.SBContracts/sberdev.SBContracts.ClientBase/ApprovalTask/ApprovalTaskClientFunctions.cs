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