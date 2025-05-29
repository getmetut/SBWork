using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Parties.CompanyBase;
using Sungero.ATS.CompanyBase;

namespace Sungero.ATS.Server
{
  partial class CompanyBaseFunctions
  {
    /// <summary>
    /// Получить адрес нашего сервиса заполнения контрагентов.
    /// </summary>
    /// <returns>Адрес сервера, или пустую строку, если его нет.</returns>
    [Public]
    public string GetCompanyDataServiceURL()
    {
      var key = Sungero.Docflow.PublicConstants.Module.CompanyDataServiceKey;
      var command = string.Format(Sungero.Parties.Queries.Module.SelectCompanyDataService, key);
      var commandExecutionResult = Sungero.Docflow.PublicFunctions.Module.ExecuteScalarSQLCommand(command);
      var serviceUrl = string.Empty;
      if (!(commandExecutionResult is DBNull) && commandExecutionResult != null)
        serviceUrl = commandExecutionResult.ToString();
      
      return serviceUrl;
    }
    
  public override void FillCompanyNames(Sungero.CompanyData.CompaniesDTO.CompanyDTO company)
  {
    //base.FillCompanyNames(company);
    var decapitalizationModeOn = Sungero.Docflow.PublicFunctions.Module.Remote.GetDocflowParamsBooleanValue(Sungero.Parties.Constants.Module.DecapitalizeCompanyNameParamName);
      if (decapitalizationModeOn)
      {
        _obj.Name = company.ShortName;
        _obj.LegalName = company.LegalName;
      }
      else
      {
        _obj.Name = company.ShortName;
        _obj.LegalName = company.LegalName;
      }
  }
    
    /// <summary>
    /// Заполнить реквизиты контрагента из сервиса.
    /// </summary>
    /// <param name="specifiedPSRN">ОГРН выбранной организации.</param>
    /// <returns>Статус запроса, список представлений организаций, список контактов.</returns>
    public Sungero.Parties.Structures.CompanyBase.FoundCompanies FillFromService(string specifiedPSRN)
    {
      var url = GetCompanyDataServiceURL();
      if (string.IsNullOrEmpty(url))
        return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorNotFound, null, null, 0);
      
      // Указать ОГРН выбранного контрагента.
      var psrn = _obj.PSRN;
      if (!string.IsNullOrWhiteSpace(specifiedPSRN))
        psrn = specifiedPSRN;

      // Запрос в сервис.
      var searchResult = Sungero.CompanyData.Client.Search(psrn, _obj.TIN, _obj.Name, url);
      switch (searchResult.StatusCode)
      {
        case HttpStatusCode.OK:
          break;
        case HttpStatusCode.Unauthorized:
          return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorUnauthorized, null, null, 0);
        case HttpStatusCode.Forbidden:
          return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorForbidden, null, null, 0);
        case (HttpStatusCode)429:
          return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorTooManyRequests, null, null, 0);
        case HttpStatusCode.ServiceUnavailable:
        case HttpStatusCode.BadGateway:
        case HttpStatusCode.NotFound:
          return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorNotFound, null, null, 0);
        case HttpStatusCode.PaymentRequired:
          return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorNoLicense, null, null, 0);
        default:
          return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(Sungero.Parties.CompanyBases.Resources.ErrorInService, null, null, 0);
      }
      
      Logger.DebugFormat("{0} counterparties found in service", searchResult.Count.ToString());

      // Ничего не нашли.
      if (searchResult.Count < 1)
      {
        // Пустая структура, чтобы можно было отделить результат, когда ничего не найдено, от случая, когда сервис вернул ошибку.
        var emptyListOfCompanies = new List<Sungero.Parties.Structures.CompanyBase.CompanyDisplayValue>();
        return Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create(CompanyBases.Resources.ErrorCompanyNotFoundInService, emptyListOfCompanies, null, 0);
      }
      
      // Подготовить ответ.
      var result = Sungero.Parties.Structures.CompanyBase.FoundCompanies.Create();
      result.CompanyDisplayValues = searchResult.Companies
        .Select(r =>
                {
                  var decapitalizationModeOn = true;
                  var shortName = decapitalizationModeOn ? r.ShortName : r.ShortName;
                  var dialogText = string.IsNullOrWhiteSpace(r.Kpp)
                    ? CompanyBases.Resources.CompanySelectDialogTextFormat(shortName, r.Inn)
                    : CompanyBases.Resources.CompanySelectDialogTextWithTRRCFormat(shortName, r.Inn, r.Kpp);
                  return Sungero.Parties.Structures.CompanyBase.CompanyDisplayValue.Create(dialogText, r.Ogrn);
                })
        .ToList();
      
      result.Amount = searchResult.Total;
      
      // Нашли ровно один. Сразу заполняем реквизиты.
      if (searchResult.Count == 1)
      {
        var company = searchResult.Companies.First();
        this.FillCompanyNames(company);
        _obj.PSRN = company.Ogrn;
        _obj.TIN = company.Inn;
        _obj.TRRC = company.Kpp;
        _obj.NCEO = company.Okpo;
        _obj.NCEA = Sungero.Parties.Functions.CompanyBase.GetFormatOkved(company);
        _obj.LegalAddress = company.Address;
        _obj.Region = Commons.PublicFunctions.Region.GetRegionFromAddress(company.Address);
        _obj.City = Commons.PublicFunctions.City.GetCityFromAddress(company.Address);
        if (!Equals(company.State, Sungero.Parties.Constants.CompanyBase.ActiveCounterpartyStateInService))
        {
          if (string.IsNullOrEmpty(_obj.Note))
            _obj.Note = company.State;
          else if (!_obj.Note.Contains(company.State))
            _obj.Note = string.Format("{0}\r\n{1}", _obj.Note, company.State);
        }
        result.FoundContacts = company.Managers
          .Select(t => Sungero.Parties.Structures.CompanyBase.FoundContact.Create(t.FullName, t.JobTitle, t.Phone))
          .ToList();
      }

      return result;
    }
  }
}