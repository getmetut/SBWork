using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ExchangeDocument;

namespace sberdev.SBContracts.Client
{
  partial class ExchangeDocumentFunctions
  {

    /// <summary>
    /// Перекрытие доступных к выбору типов документов для смены типа
    /// </summary>
    public override List<Sungero.Domain.Shared.IEntityInfo> GetTypesAvailableForChange()
    {
      var types = base.GetTypesAvailableForChange();
      types.Add(Sungero.Custom.NDAs.Info);
      types.Add(sberdev.SberContracts.OtherContractDocuments.Info);
      return types;
    }

  }
}