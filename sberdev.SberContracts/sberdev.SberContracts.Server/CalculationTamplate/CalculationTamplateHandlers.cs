using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CalculationTamplate;

namespace sberdev.SberContracts
{
  partial class CalculationTamplateUiFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.UiFilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      return query.Where(q => q.User == Users.Current);
    }
  }

  partial class CalculationTamplateProdCollectionProductPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ProdCollectionProductFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      string[] exception = new string[]{"General", "ЗАГЛУШКА ДЛЯ ВХОДЯЩИХ ДОКУМЕНТОВ (нужно проставить аналитики)", "Для исторических договоров"};
      return query.Where(q => q.BusinessUnit == _obj.CalculationTamplate.BusinessUnit && !exception.Contains(q.Name));
    }
  }

  partial class CalculationTamplateServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.ProdCollection.Any())
      {
        double sum = 0;
        if (_obj.CalculationFlag == SberContracts.CalculationTamplate.CalculationFlag.Percent)
        {
          foreach(var prod in _obj.ProdCollection)
            sum = sum + prod.Percent.Value;
          if (sum > 100.01)
            e.AddError(sberdev.SberContracts.CalculationTamplates.Resources.CalcTamplateError);
        }
        else
        {
          foreach(var prod in _obj.ProdCollection)
            sum = sum + prod.Absolute.Value + 0.01;
          if (sum > _obj.Amount)
            e.AddError(sberdev.SberContracts.CalculationTamplates.Resources.CalcTamplateError3);
        }
      }
      else
        e.AddError(sberdev.SberContracts.CalculationTamplates.Resources.CalcTamplateError2);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.BusinessUnit = Sungero.Company.Employees.As(Users.Current).Department.BusinessUnit;
      _obj.User = Users.Current;
      _obj.CalculationFlag = CalculationFlag.Percent;
    }
  }

}