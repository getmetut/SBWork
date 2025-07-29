using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PurchaseProtocol;

namespace Sungero.Custom
{
  partial class PurchaseProtocolSharedHandlers
  {

    public virtual void RouteTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      PublicFunctions.PurchaseProtocol.UpdateCard(_obj);
    }

    public virtual void TotalSummChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      PublicFunctions.PurchaseProtocol.UpdateCard(_obj);
    }

  }
}