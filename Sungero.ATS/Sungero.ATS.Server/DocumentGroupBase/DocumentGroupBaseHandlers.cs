using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.DocumentGroupBase;

namespace Sungero.ATS
{
  partial class DocumentGroupBaseServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (_obj.DocumentKinds.Count > 0)
      {
        string list = "";
        foreach (var elem in _obj.DocumentKinds)
        {
          list += elem.DocumentKind.Name.ToString() + "; ";
        }
        if (_obj.DocKindsStringSDev != list)
          _obj.DocKindsStringSDev = list;
      }
    }
  }

}