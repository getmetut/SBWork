using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MarcetingDoc;

namespace Sungero.Custom
{
  partial class MarcetingDocChannelsProductsAndDevicesPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ChannelsProductsAndDevicesFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.MarcetingDoc.BusinessUnit != null)
        query = query.Where(d => d.BusinessUnit == _obj.MarcetingDoc.BusinessUnit);
      
      if (_obj.MarcetingDoc.DevicesAction.Count > 0)
      {
        var listdevice = sberdev.SberContracts.ProductsAndDeviceses.GetAll(d => d.Name == "#$").ToList();
        foreach (var elem in _obj.MarcetingDoc.DevicesAction)
        {
          listdevice.Add(elem.ProductsAndDevices);
        }
        query = query.Where(dev => listdevice.Contains(dev));
      }
      
      return query;
    }
  }

  partial class MarcetingDocDevicesActionProductsAndDevicesPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DevicesActionProductsAndDevicesFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.MarcetingDoc.BusinessUnit != null)
        query = query.Where(d => d.BusinessUnit == _obj.MarcetingDoc.BusinessUnit);
      
      return query;
    }
  }

  partial class MarcetingDocDocumentKindPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //query = base.DocumentKindFiltering(query, e);
      query = query.Where(k => k.DocumentType.Name == "Документ маркетинговых операций");
      return query;
    }
  }

  partial class MarcetingDocServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (_obj.ActionName != null)
      {
        string name = _obj.ActionName + " ID" + _obj.Id.ToString();
        if (_obj.Name != name)
          _obj.Name = name;
      }
      
      if (_obj.CtrlKolvo != 0.0)
        e.AddError("Нарушен контроль кол-ва позиций в Продуктах и Каналах! Индекс: " + _obj.CtrlKolvo.ToString());
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FacktRashCurrency = Sungero.Commons.Currencies.Get(1);
      _obj.FacktSummCurrency = Sungero.Commons.Currencies.Get(1);
      _obj.PlanRashCurrency = Sungero.Commons.Currencies.Get(1);
      _obj.PlanSummCurrency = Sungero.Commons.Currencies.Get(1);
      var emp = Sungero.Company.Employees.GetAll(empl => empl.Login == Users.Current.Login).FirstOrDefault();
      if (emp != null)
        _obj.BusinessUnit = emp.Department.BusinessUnit;
      _obj.DocumentKind = Sungero.Docflow.DocumentKinds.GetAll().FirstOrDefault();
      _obj.ActionCode = int.Parse(_obj.Id.ToString());
      _obj.StagesOfApproval = Custom.MarcetingDoc.StagesOfApproval.NewObj;
    }
  }

  partial class MarcetingDocMarketingSubKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MarketingSubKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.MarketingKind != null)
        query = query.Where(q => q.MarketingKind == _obj.MarketingKind);
      
      return query;
    }
  }

}