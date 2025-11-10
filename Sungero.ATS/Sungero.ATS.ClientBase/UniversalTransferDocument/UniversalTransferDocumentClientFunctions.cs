using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.UniversalTransferDocument;

namespace Sungero.ATS.Client
{
  partial class UniversalTransferDocumentFunctions
  {

    /// <summary>
    /// Обновить Контирагента на головную организацию
    /// </summary>
    public void UpdateCounterparty()
    {
      if (_obj.Counterparty != null)
      {
        var comp = sberdev.SBContracts.Companies.As(_obj.Counterparty);
        if (comp != null)
        {
          if (comp.HeadCompany != null)
            _obj.Counterparty = comp.HeadCompany;
          else
          {
            if (comp.HeadOrgSDev != true
                && !sberdev.SBContracts.PublicFunctions.Module.IsSystemUser())
            {
              var OrgTrue = PublicFunctions.UniversalTransferDocument.Remote.GetHeadCompanies(_obj, comp);
              var HeadCompany = comp.HeadCompany;
              if (HeadCompany != null)
                _obj.Counterparty = HeadCompany;
              else
              {
                if (OrgTrue != null && OrgTrue != comp)
                {
                  if (_obj.Counterparty != OrgTrue)
                    _obj.Counterparty = OrgTrue;
                }
              }
            }
          }
          Dialogs.ShowMessage("Обновление завершено.");
        }
      }
    }

  }
}