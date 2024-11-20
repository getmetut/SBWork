using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractualDocument;

namespace sberdev.SBContracts.Client
{
  partial class ContractualDocumentActions
  {
    public virtual void CreateBodyByPropertiesSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      SBContracts.PublicFunctions.Module.Remote.CreateBodyByProperties(_obj);
      _obj.Save();
    }

    public virtual bool CanCreateBodyByPropertiesSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool flag = false;
      if (_obj.DocumentKind == null)
        return !_obj.State.IsInserted && flag;
      var name = _obj.DocumentKind.Name;
      if (name == "Договор Xiongxin" || name == "Заказ Xiongxin")
        flag = true;
      return !_obj.State.IsInserted && flag;
    }

  }


}