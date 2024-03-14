using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Module.Integration.Server
{
  partial class ModuleFunctions
  {
    public override IQueryable<centrvd.KFIntegration.ICompany> GetCompaniesToFillInMarkers()
    {
      return base.GetCompaniesToFillInMarkers().Where(c => c.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
    }
  }
}