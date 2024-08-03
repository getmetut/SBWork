using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.SimpleDocument;

namespace Sungero.ATS.Client
{
  partial class SimpleDocumentFunctions
  {

    /// <summary>
    /// 
    /// </summary>       
    public override List<Domain.Shared.IEntityInfo> GetTypesAvailableForChange()
    {
      var types = base.GetTypesAvailableForChange();
      types.Add(Custom.NDAs.Info);
      return types;
    }

  }
}