using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MatrixPOA;

namespace Sungero.Custom
{
  partial class MatrixPOAClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      PublicFunctions.MatrixPOA.UpdateCard(_obj);
    }

  }
}