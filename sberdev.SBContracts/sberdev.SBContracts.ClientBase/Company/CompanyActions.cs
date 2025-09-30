using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts.Client
{
  partial class CompanyCollectionActions
  {

    public virtual bool CanExportFocusCheckZipManySberDev(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _objs.Any();
    }

    public virtual void ExportFocusCheckZipManySberDev(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var ids = _objs
          .Where(c => c != null)
          .Select(c => c.Id)
          .Distinct()
          .ToList();
      var zip = sberdev.SBContracts.PublicFunctions.Module.Remote.ExportFocusCheckZipByIdsRemote(ids);
      zip.Export();
    }
  }


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