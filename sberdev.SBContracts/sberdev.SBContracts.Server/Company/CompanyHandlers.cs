using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts
{
  partial class CompanyServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IPSberDev = false;
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var checkDuplicatesErrorText = Sungero.Parties.PublicFunctions.Counterparty.GetCounterpartyDuplicatesErrorText(_obj);
      if (!string.IsNullOrWhiteSpace(checkDuplicatesErrorText))
        e.AddError(checkDuplicatesErrorText, _obj.Info.Actions.ShowDuplicates);
      base.BeforeSave(e);
      _obj.ChangeDate = Calendar.Now;
      string NewName = "";
      if (_obj.IPSberDev != null)
      {
        if (_obj.IPSberDev.Value)
        {
          if (_obj.PersonSberDev != null)
            NewName = "ИП " + _obj.PersonSberDev.Name.ToString();
          
          if (_obj.Name != NewName)
          {
            _obj.Name = NewName;
            _obj.LegalName = NewName;
          }
        }
      }
    }
  }

}