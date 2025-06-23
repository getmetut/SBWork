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
      _obj.HeadOrgSDev = false;
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      #region Тарас
      if (!SBContracts.PublicFunctions.Module.IsSystemUser())
      {
        // Проверяем только резидентов
        bool isResident = !(_obj.Nonresident.HasValue && _obj.Nonresident.Value);
        
        if (isResident)
        {
          // Проверка банковского счета
          if (string.IsNullOrEmpty(_obj.Account) || _obj.Account.Length < 5)
          {
            _obj.State.Properties.Account.HighlightColor = Colors.Common.Red;
            e.AddError("Необходимо заполнить Счет в банковских реквизитах!");
          }
          
          // Проверка головной организации для филиалов
          if (_obj.HeadCompany == null &&
              (!_obj.HeadOrgSDev.HasValue || !_obj.HeadOrgSDev.Value))
          {
            e.AddError("Добавление филиала без указания головной организации - запрещено.");
          }
        }
      }
      
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
      #endregion
      
      var changedMarkers = _obj.MarkersChanges.Where(m => m.ChangeDateTime.Value.Date == Calendar.Now.Date);
      if (changedMarkers.Any())
      {
        foreach (var marker in changedMarkers)
        {
          var operation = new Enumeration("ChangeMarkers");
          string comment = String.Format("{0}: {1} -> {2}", marker.Name.Value.Value, marker.OldValue.Value.Value, marker.NewValue.Value.Value);
          _obj.History.Write(operation, operation, comment);
        }
        _obj.FocusCheckedSberDev = false;
      }
    }
  }

}