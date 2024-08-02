using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Contract;

namespace sberdev.SBContracts
{
  partial class ContractSharedHandlers
  {

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      if (e.NewValue != null)
      {
        var name = e.NewValue.Name;
        if (name == "Договор Xiongxin" || name == "Дополнительное соглашение Xiongxin")
        {
          List<int> ids = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("ИД сущностей для договора Xiongxin").Text.Split(',').Select(s => Int32.Parse(s)).ToList();
          Functions.Contract.Remote.SetXiongxinContractProps(_obj, ids);
        }
      }
    }
  }

}