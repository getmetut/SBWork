using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts.Client
{
  partial class CompanyFunctions
  {
    public void DownloadFocusCheckZip()
    {
      var zip = PublicFunctions.Company.Remote.ExportFocusCheckZipRemote(_obj);
      if (zip != null)
        zip.Export();
    }
  }
}