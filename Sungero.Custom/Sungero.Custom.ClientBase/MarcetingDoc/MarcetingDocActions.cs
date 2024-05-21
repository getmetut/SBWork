using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MarcetingDoc;

namespace Sungero.Custom.Client
{
  partial class MarcetingDocActions
  {
    public virtual void SendToTM(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      e.CloseFormAfterAction = true;
      var Task = Custom.Marketings.Create();
      Task.Baseattachment.MarcetingDocs.Add(_obj);
      Task.Subject = "Согласование: " + _obj.Name.ToString();
      if (_obj.DevicesAction.Count > 0)
      {
        foreach (var str in _obj.DevicesAction)
        {
          if (str.ProductsAndDevices.ProductUnit.Responsible != null)
            Task.ProductCollection.AddNew().Employee = str.ProductsAndDevices.ProductUnit.Responsible;
        }
      }
      if (_obj.BudjetList.Count > 0)
      {
        foreach (var elem in _obj.BudjetList)
        {
          if (elem.MVZ.BudgetOwner != null)
            Task.BudjetList.AddNew().Employee = elem.MVZ.BudgetOwner;
        }
      }
      Task.Show();
    }

    public virtual bool CanSendToTM(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}