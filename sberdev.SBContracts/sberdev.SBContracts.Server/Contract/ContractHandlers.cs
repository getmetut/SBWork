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