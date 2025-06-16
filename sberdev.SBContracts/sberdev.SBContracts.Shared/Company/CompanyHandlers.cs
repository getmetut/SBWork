using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;

namespace sberdev.SBContracts
{
  partial class CompanySharedHandlers
  {

    public override void OrgLiquidatedChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      base.OrgLiquidatedChanged(e);
    }

    public override void HeadCompanyChanged(Sungero.Parties.Shared.CompanyBaseHeadCompanyChangedEventArgs e)
    {
      base.HeadCompanyChanged(e);
      if (e.NewValue != null)
        _obj.HeadOrgSDev = false;
    }

    public virtual void HeadOrgSDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (e.NewValue == true)
        {
          var HeadOrg = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => ((c.TIN == _obj.TIN) && (c.HeadOrgSDev == true))).FirstOrDefault();
          if (HeadOrg != null)
          {
            PublicFunctions.Module.ShowInfoMessage("Для данного ИНН уже существует головная организация: " + HeadOrg.Name.ToString());
            _obj.HeadOrgSDev = false;
          }
        }
      }
    }

    public override void NameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.NameChanged(e);
      if (e.NewValue != null)
      {
        
        string Name1C = ModifyCompanyName(e.NewValue);
        if (_obj.Name1CSberDev != Name1C)
          _obj.Name1CSberDev = Name1C;
      }
    }

    public virtual void IPSberDevChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (_obj.IPSberDev.HasValue)
        if (!_obj.IPSberDev.Value)
          _obj.PersonSberDev = null;
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