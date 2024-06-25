using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.ControlContract;

namespace Sungero.Custom
{
  partial class ControlContractCollectionDocsSharedHandlers
  {

    public virtual void CollectionDocsSummChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      var DDS = _obj.ControlContract.CollectionDocs;
      double limit = _obj.ControlContract.Limit.Value;
      foreach (var str in DDS)
      {
        limit -= str.Summ.Value;
      }
      _obj.ControlContract.TotalLimit = limit;
    }

    public virtual void CollectionDocsIDDocChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        var doc = sberdev.SBContracts.SupAgreements.GetAll(d => d.Id == long.Parse(e.NewValue.ToString())).FirstOrDefault();
        if (doc != null)
        {
          if (doc.TotalAmount > 0.0)
            _obj.Summ = doc.TotalAmount;
          _obj.NameDoc = doc.Name;
        }
        else
          PublicFunctions.ControlContract.ShowMessage(_obj.ControlContract, "По данному ИД не найдено подходящего документа!");
      }
      else
      {
        _obj.Summ = 0.0;
      }
    }
  }

  partial class ControlContractSharedHandlers
  {

    public virtual void LeadingDocIDChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        var doc = sberdev.SBContracts.Contracts.GetAll(d => d.Id == long.Parse(e.NewValue.ToString())).FirstOrDefault();
        if (doc != null)
        {
          if (doc.TotalAmount > 0.0)
            _obj.Limit = doc.TotalAmount;
          _obj.Name = doc.Name;
        }
        else
          PublicFunctions.ControlContract.ShowMessage(_obj, "По данному ИД не найдено подходящего документа!");
      }
      else
      {
        _obj.Limit = 0.0;
      }
    }

  }
}