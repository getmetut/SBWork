using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.PurchaseProtocol;

namespace Sungero.Custom.Client
{
  partial class PurchaseProtocolFunctions
  {

    /// <summary>
    /// Инструкция
    /// </summary>
    public void ShowJustifCooseCpFAQ()
    {
      string PublicText = "";
      if (_obj.PurchaseMethod != null)
      {
        if (_obj.PurchaseMethod.Instruction != null)
          PublicText = _obj.PurchaseMethod.Instruction;
      }
      
      Dialogs.ShowMessage(PublicText);
    }

  }
}