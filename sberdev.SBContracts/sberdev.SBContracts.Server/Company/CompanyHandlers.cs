using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts
{
  partial class CompanyOtvBuchSDevSearchPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OtvBuchSDevSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
    {
      var OBrole = Roles.GetAll(r => r.Name == "Ответственный бухгалтер за Контрагентов").FirstOrDefault();
      List<Sungero.CoreEntities.IRecipient> Userlist = new List<Sungero.CoreEntities.IRecipient>();
      if (OBrole != null)
      {
        if (OBrole.RecipientLinks.Count > 0)
        {
          foreach (var elem in OBrole.RecipientLinks)
          {
              Userlist.Add(elem.Member);
          }
          query = query.Where(q => Userlist.Contains(q));
        }
      }
      return query;
    }
  }

  partial class CompanyOtvBuchSDevPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OtvBuchSDevFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var OBrole = Roles.GetAll(r => r.Name == "Ответственный бухгалтер за Контрагентов").FirstOrDefault();
      List<Sungero.CoreEntities.IRecipient> Userlist = new List<Sungero.CoreEntities.IRecipient>();
      if (OBrole != null)
      {
        if (OBrole.RecipientLinks.Count > 0)
        {
          foreach (var elem in OBrole.RecipientLinks)
          {
              Userlist.Add(elem.Member);
          }
          query = query.Where(q => Userlist.Contains(q));
        }
      }
      return query;
    }
  }

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