using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts
{
  partial class CompanySharedHandlers
  {

    public virtual void ControlMarkersFlags1SberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.Module.ResetBooleanGroupSelections(_obj, e, nameof(_obj.ControlMarkersFlags1SberDev));
    }

    public virtual void ControlMarkersFlags2SberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.Module.ResetBooleanGroupSelections(_obj, e, nameof(_obj.ControlMarkersFlags2SberDev));
    }

    public override void HeadCompanyChanged(Sungero.Parties.Shared.CompanyBaseHeadCompanyChangedEventArgs e)
    {
      base.HeadCompanyChanged(e);
      if (e.NewValue != null)
        _obj.HeadOrgSDev = false;
    }

    public virtual void HeadOrgSDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        var HeadOrg = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => ((c.TIN == _obj.TIN) && (c.HeadOrgSDev == true))).FirstOrDefault();
        if (HeadOrg != null)
        {
          PublicFunctions.Module.ShowInfoMessage("Для данного ИНН уже существует головная организация: " + HeadOrg.Name.ToString());
          _obj.HeadOrgSDev = false;
        }
      }
    }

    public override void NameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.NameChanged(e);
      if (e.NewValue != null)
        _obj.Name1CSberDev = Functions.Company.ModifyCompanyName(e.NewValue);
    }

    public virtual void IPSberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (_obj.IPSberDev.HasValue)
        if (!_obj.IPSberDev.Value)
          _obj.PersonSberDev = null;
    }

  }
}