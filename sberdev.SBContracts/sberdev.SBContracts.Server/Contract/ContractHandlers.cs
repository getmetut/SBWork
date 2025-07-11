using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Contract;

namespace sberdev.SBContracts
{
  partial class ContractServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (_obj.State.IsInserted)
      {
        var DOff = Sungero.Docflow.DocumentKinds.GetAll(dt => dt.Name == "Договор-оферта").FirstOrDefault();
        if (DOff != null)
          if (_obj.DocumentKind == DOff)
            if (_obj.RegistrationDate == null)
              _obj.RegistrationDate = _obj.Created;
        
        if (!_obj.ValidTill.HasValue)
        {
          if (_obj.IsAutomaticRenewal.HasValue)
          {
            if (_obj.IsAutomaticRenewal.Value == false)
            {
              _obj.State.Properties.IsAutomaticRenewal.HighlightColor = Colors.Common.Red;
              _obj.State.Properties.ValidTill.HighlightColor = Colors.Common.Red;
              e.AddError("Для сохранения документа необходимо указать признак действия договора: \"Действует до\" или \"С автопролонгацией\"!");
            }
          }
        }
      }
    }

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      Sungero.Custom.PublicFunctions.Module.CreateLiminInContract(_obj.Id);
    }
  }


  partial class ContractDocumentGroupPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DocumentGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DocumentGroupFiltering(query, e);
      if (_obj.ContrTypeBaseSberDev != null)
      {
        switch (_obj.ContrTypeBaseSberDev.Value.Value)
        {
          case "Expendable" :
            return query.Where(d => Equals( sberdev.SBContracts.ContractCategories.As(d).ContrType , sberdev.SBContracts.ContractCategory.ContrType.Expendable));
          case "Profitable" :
            return query.Where(d => Equals( sberdev.SBContracts.ContractCategories.As(d).ContrType , sberdev.SBContracts.ContractCategory.ContrType.Profitable));
          case "ExpendProfit" :
            return query.Where(d => Equals( sberdev.SBContracts.ContractCategories.As(d).ContrType , sberdev.SBContracts.ContractCategory.ContrType.ExpendProfit));
          default:
            return query;
        }
      }
      else
      {
        return query;
      }
    }
  }

}