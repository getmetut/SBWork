using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts.Client
{
  partial class CompanyFunctions
  {
    /// <summary>
    /// Обновление интерфейса карточки в зависисмости от условий
    /// </summary>
    public void UpdInterface()
    {
      if (_obj.Nonresident != null)
      {
        _obj.State.Properties.TIN.IsRequired = !_obj.Nonresident.Value;
        _obj.State.Properties.TRRC.IsRequired = !_obj.Nonresident.Value;
        _obj.State.Properties.TRRC.IsVisible = !_obj.Nonresident.Value;
      }
      else
      {
        _obj.State.Properties.TIN.IsRequired = true;
        _obj.State.Properties.TRRC.IsRequired = true;
        _obj.State.Properties.TRRC.IsVisible = true;
      }
      if (_obj.IPSberDev != null)
      {
        _obj.State.Properties.PersonSberDev.IsVisible = _obj.IPSberDev.Value;
        _obj.State.Properties.PersonSberDev.IsRequired = _obj.IPSberDev.Value;
        _obj.State.Properties.HeadCompany.IsVisible = !_obj.IPSberDev.Value;
        _obj.State.Properties.Nonresident.IsVisible = !_obj.IPSberDev.Value;
        if (_obj.IPSberDev == true)
        {
          _obj.State.Properties.TRRC.IsRequired = false;
          _obj.State.Properties.TRRC.IsVisible = false;
        }
      }
      else
      {
        _obj.State.Properties.PersonSberDev.IsVisible = false;
        _obj.State.Properties.PersonSberDev.IsRequired = false;
      }
      
      if ((_obj.Name1CSberDev == null) && (_obj.Name != null))
      {
        _obj.Name1CSberDev = ModifyCompanyName(_obj.Name);
      }
    }
    
    static string ModifyCompanyName(string companyName)
    {
        //companyName = companyName.Trim('"'); // убираем кавычки в начале и конце строки
        string CompNameNew = companyName.Replace("Общество с ограниченной ответственностью","ООО");
        CompNameNew = CompNameNew.Replace("Публичное акционерное общество","ПАО");
        CompNameNew = CompNameNew.Replace("Открытое акционерное общество","ОАО");
        CompNameNew = CompNameNew.Replace("Закрытое акционерное общество","ЗАО");
        CompNameNew = CompNameNew.Replace("Индивидуальный предприниматель","ИП");
        CompNameNew = CompNameNew.Replace("Акционерное общество","АО");
        CompNameNew = CompNameNew.Replace('«','"');
        CompNameNew = CompNameNew.Replace('»','"');
        // разделяем наименование и правовую форму        
        string[] parts = CompNameNew.Split(' ');
        string legalForm = parts[0]; // первая часть - правовая форма (ООО, ЗАО, etc.)
        string name = string.Join(" ", parts, 1, parts.Length - 1).Trim('"'); // остальная часть - наименование
        name = name.ToUpper();
        // формируем измененное название
        string modifiedName = $"{name} {legalForm}";
        if ((legalForm == "ООО") || (legalForm == "ЗАО") || (legalForm == "ОАО") || (legalForm == "АО") || (legalForm == "ПАО") || (legalForm == "ИП"))
          return modifiedName;
        else
          return companyName; 
    }
  }
}