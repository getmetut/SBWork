using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Company;
using Sungero.Domain.Shared;

namespace sberdev.SBContracts.Server
{
  partial class CompanyFunctions
  {
    public override void FillInRequisites(centrvd.KFIntegration.Structures.Parties.Company.CompanyInfo counterpartyInfo, bool isLegalEntity)
    {
      
      base.FillInRequisites(counterpartyInfo, isLegalEntity);
      
      var yes = new Enumeration("Yes");
      var no = new Enumeration("No");
      
      var req = counterpartyInfo.Req;
      var egrDetails = counterpartyInfo.EgrDetails;
      var license = counterpartyInfo.License;
      var analitycs = counterpartyInfo.Analytics;
      var reqCompanyAffiliates = counterpartyInfo.ReqCompanyAffiliates;
      var egrDetailsCompanyAffiliates = counterpartyInfo.EgrDetailsCompanyAffiliates;
      var analyticsCompanyAffiliates = counterpartyInfo.AnalyticsCompanyAffiliates;
      var petitionersOfArbitration = counterpartyInfo.PetitionersOfArbitration;
      var moratorium = counterpartyInfo.Moratorium;
      
      // Значительная сумма арбитражных дел в качестве ответчика.
      var caseAmountTo =  isLegalEntity ? (analitycs?.Analytics?.S2001 > 50000000 ? yes : no) : no;
      if (_obj.CaseAmountTo != caseAmountTo)
        _obj.CaseAmountTo = caseAmountTo;
      // Значительная сумма исполнительных производств.
      var execCaseAmount = isLegalEntity ? (analitycs?.Analytics?.S1001 > 50000000 ? yes : no) : no;
      if (_obj.ExecCaseAmount != execCaseAmount)
        _obj.ExecCaseAmount = execCaseAmount;
      // Значительное количество связанных организаций найдены в особых реестрах ФНС.
      var specialList = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M5008 == true || x?.Analytics?.M5009 == true || x?.Analytics?.M5010 == true ||
                                         x?.Analytics?.M5003 == true || x?.Analytics?.M5004 == true || x?.Analytics?.M5005 == true).
        Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.SpecialList != specialList)
        _obj.SpecialList = specialList;
      // Значительное количество связанных организаций с особыми исполнительными производствами.
      var specialExecList = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M1003 == true || x?.Analytics?.M1004 == true || x?.Analytics?.M1005 == true ||
                                         x?.Analytics?.M1006 == true || x?.Analytics?.S1007 > 0 || x?.Analytics?.S1008 > 0 ).
        Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.SpecialExecList != specialExecList)
        _obj.SpecialExecList = specialExecList;
      // Значительное количество связанных организаций, зарегистрированных менее 12 месяцев назад.
      var youngRelOrg = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M7004 == true ).Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.YoungRelOrg != youngRelOrg)
        _obj.YoungRelOrg = youngRelOrg;
      // Значительное количество связанных организаций, по которым требуется дополнительная проверка.
      var relOrgAddValid = analyticsCompanyAffiliates != null &&
        analyticsCompanyAffiliates.Where(x => x?.Analytics?.M7001 == true).Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.RelOrgAddValid != relOrgAddValid)
        _obj.RelOrgAddValid = relOrgAddValid;
      // Значительное количество связанных организаций, у которых за последние 12 месяцев хотя бы раз менялся директор или учредитель.
      var relOrgSwapHead = analyticsCompanyAffiliates.Where(x =>
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.Heads != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.Heads.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.ManagementCompanies != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.ManagementCompanies.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.FoundersFL != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.FoundersFL.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.FoundersUL != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.FoundersUL.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1))) ||
                                                            (egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault()?.UL?.History?.FoundersForeign != null &&
                                                             egrDetailsCompanyAffiliates.Where(e => (e.Inn == x.Inn) || (e.Ogrn == x.Ogrn)).FirstOrDefault().UL.History.FoundersForeign.
                                                             Any(d => d.Date != null && DateTime.Parse(d.Date) > Calendar.Today.AddYears(-1)))).
        Count() > analyticsCompanyAffiliates.Count() * 0.5 ? yes : no;
      if (_obj.RelOrgSwapHead != relOrgSwapHead)
        _obj.RelOrgSwapHead = relOrgSwapHead;
      // Значительное число не ликвидированных юридических лиц по тому же адресу.
      var noLiquidAddress = analitycs?.Analytics?.Q7006 > 2 ? yes : no;
      if (_obj.NoLiquidAddress != noLiquidAddress)
        _obj.NoLiquidAddress = noLiquidAddress;
      // Маленькая среднесписочная численность работников.
      var avgEmpl = analitycs?.Analytics?.Q7047 < 5 ? yes : no;
      if (_obj.LowAvgHeadSberDev != avgEmpl)
        _obj.LowAvgHeadSberDev = avgEmpl;
    }
    
    /// <summary>
    /// Заполнение таблицы изменения значений маркеров.
    /// </summary>
    public void FillMarkersChangesTable()
    {
      foreach (var item in ChangeRequisites(_obj))
      {
        var values = item.Split(new char[] { ':' });
        
        var property = new Enumeration(values[0]);
        var newValue = new Enumeration(values[1]);
        var originalValue = new Enumeration(values[2]);
        
        var markersTable = _obj.MarkersChanges;
        var propertyRow = markersTable.FirstOrDefault(z => z.Name == property);
        if (propertyRow == null)
        {
          propertyRow = markersTable.AddNew();
          propertyRow.Name = property;
        }
        propertyRow.NewValue = newValue;
        propertyRow.OldValue = originalValue;
        propertyRow.ChangeDateTime = Calendar.Now;
      }
    }
    
    /// <summary>
    /// Получить список измененных маркеров вместе со значениями.
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns>Список измененных свойств</returns>
    private static List<string> ChangeRequisites(Sungero.Domain.Shared.IEntity entity)
    {      
      var changeList = new List<string>();
      
      var objType = entity.GetType().GetFinalType();
      var objMetadata = objType.GetEntityMetadata();      
      var requisitesList = new List<string>(Enumeration.GetDirectItems(typeof(centrvd.Integration.KFMarkersSettingMarkers.Name)).Select(z => z.Value));
      var properties = objMetadata.Properties.Where(p => requisitesList.Contains(p.Name));
      foreach (var propertyMetadata in properties)
      {
        if (propertyMetadata.PropertyType == Sungero.Metadata.PropertyType.Enumeration)
        {
          var checkResult = CheckRequisite(entity, propertyMetadata);
          if (!string.IsNullOrEmpty(checkResult))
            changeList.Add(checkResult);
        }
      }
      
      return changeList;
    }

    /// <summary>
    /// Проверка изменения свойства
    /// </summary>
    /// <param name="entity">Сущность или строка коллекции</param>
    /// <param name="propertyMetadata">Метаданные свойства</param>
    /// <returns>Сообщение об изменении свойства, иначе string.Empty</returns>
    private static string CheckRequisite(Sungero.Domain.Shared.IEntity entity, Sungero.Metadata.PropertyMetadata propertyMetadata)
    {
      var stateProperties = entity.State.Properties;
      var propertyState = stateProperties.GetType().GetProperty(propertyMetadata.Name).GetValue(stateProperties);
      
      var newValue = propertyMetadata.GetValue(entity);
      var originalValue = propertyState.GetType().GetProperty("OriginalValue").GetValue(propertyState);
      
      if (newValue == null || originalValue == null || newValue.ToString() == originalValue.ToString())
        return string.Empty;
      
      return string.Format("{0}:{1}:{2}", propertyMetadata.Name, newValue.ToString(), originalValue.ToString());
    }
  }
}