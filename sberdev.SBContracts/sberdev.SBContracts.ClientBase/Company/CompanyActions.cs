using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts.Client
{

  partial class CompanyActions
  {
    public virtual void ExportFocusCheckZipSberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var zip = Functions.Company.Remote.ExportFocusCheckZipRemote(_obj);
      zip.Export();
    }

    public virtual bool CanExportFocusCheckZipSberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }


    public override void OpenOnDueDiligenceWebsite(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      string link = @"https://focus.kontur.ru/entity?query=" + _obj.PSRN;
      Sungero.Core.Hyperlinks.Open(link);
      //base.OpenOnDueDiligenceWebsite(e);
    }

    public override bool CanOpenOnDueDiligenceWebsite(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.PSRN != null)
        return true;
      else
        return false;
      //base.CanOpenOnDueDiligenceWebsite(e);
    }

  }


}