using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.MVZ;

namespace sberdev.SberContracts.Client
{

  partial class MVZCollectionActions
  {

    public virtual bool CanAddUserInMVZ(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void AddUserInMVZ(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var Dial = Dialogs.CreateInputDialog();
      var Empl = Dial.AddSelect("Укажите сотрудника",true,Sungero.Company.Employees.Null);
      if (Dial.Show() == DialogButtons.Ok)
      {
        var emp = Sungero.Company.Employees.GetAll(r => r.Id == Empl.Value.Id).FirstOrDefault();
        foreach (var elem in _objs)
        {
          elem.CollectionEmplAcc.AddNew().Employee = emp;
          elem.Save();
        }
      }
    }

    public virtual bool CanDelUserInMVZ(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void DelUserInMVZ(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dial = Dialogs.CreateInputDialog();
      var empl = dial.AddSelect("Укажите сотрудника", true, Sungero.Company.Employees.Null);
      
      if (dial.Show() != DialogButtons.Ok)
        return;

      var empId = empl.Value?.Id ?? -1;
      var employee = Sungero.Company.Employees.GetAll().FirstOrDefault(e => e.Id == empId);
      
      if (employee == null)
        return;

      foreach (var element in _objs)
      {
        var employeeAcc = element.CollectionEmplAcc.FirstOrDefault(ea => ea.Employee == employee);
        if (employeeAcc != null)
        {
          element.CollectionEmplAcc.Remove(employeeAcc);
          break;
        }
      }
    }
  }

}