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
        var kind = e.NewValue.Name;
        if (kind == "Договор Xiongxin" || kind == "Дополнительное соглашение Xiongxin")
        {
          List<int> ids = SBContracts.PublicFunctions.Module.Remote.GetDevSetting("ИД сущностей для договора Xiongxin").Text.Split(',').Select(s => Int32.Parse(s)).ToList();
          Functions.Contract.Remote.SetXiongxinContractProps(_obj, ids);
        }
        bool isAcc = kind == "Счет-договор" || kind == "Договор-оферта";
        if (!isAcc)
          _obj.PayTypeBaseSungero = null;
      }
    }
  }

}