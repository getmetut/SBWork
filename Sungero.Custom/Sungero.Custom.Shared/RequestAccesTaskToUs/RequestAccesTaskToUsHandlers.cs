using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.RequestAccesTaskToUs;

namespace Sungero.Custom
{
  partial class RequestAccesTaskToUsSharedHandlers
  {

    public virtual void DepartmentChanged(Sungero.Custom.Shared.RequestAccesTaskToUsDepartmentChangedEventArgs e)
    {
      string name = "";
      if (_obj.Employee != null)
        name += _obj.Employee.Name;
      
      if (_obj.Department != null)
        name += " => " + _obj.Department.Name;
      
      if (_obj.Name != name)
        _obj.Name = name;
    }

    public virtual void EmployeeChanged(Sungero.Custom.Shared.RequestAccesTaskToUsEmployeeChangedEventArgs e)
    {
      string name = "";
      if (_obj.Employee != null)
        name += _obj.Employee.Name;
      
      if (_obj.Department != null)
        name += " => " + _obj.Department.Name;
      
      if (_obj.Name != name)
        _obj.Name = name;
    }

  }
}