using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.BusinessUnitBox;
using System.Net;
using Sungero.Parties;

namespace sberdev.SBContracts.Server
{
  partial class BusinessUnitBoxFunctions
  {
    public override Sungero.Parties.ICounterparty CreateCounterparty(NpoComputer.DCX.Common.IContact contact)
    {
      var cp = CompanyBases.As(base.CreateCounterparty(contact));
      
      if (cp == null)
      {
        return null;
      }
      
      if (string.IsNullOrWhiteSpace(cp.PSRN) && string.IsNullOrWhiteSpace(cp.TIN) && string.IsNullOrWhiteSpace(cp.Name))
        return cp;
      
      if (!string.IsNullOrEmpty(cp.PSRN))
        cp.PSRN = cp.PSRN.Trim();
      
      if (!string.IsNullOrEmpty(cp.TIN))
        cp.TIN = cp.TIN.Trim();
      
      var flag = GetCounterpartyInfoFromService(cp);
      
      if (flag)
        Logger.Debug(CompanyBases.Resources.FillFromServiceSuccess.StringFormat);
      else
        Logger.Debug("В сервисе нашлось больше 2ух или ни одной КА с указанными ОГРН и ИНН, поэтому КА будет заполнен из ЭДО");
      
      return cp;
    }
    
    /// <summary>
    /// Получить адрес нашего сервиса заполнения контрагентов.
    /// </summary>
    /// <returns>Адрес сервера, или пустую строку, если его нет.</returns>
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
    
    /// <summary>
    /// Сформировать строку с ОКВЭД.
    /// </summary>
    /// <param name="company">Компания с сервиса.</param>
    /// <returns>Список ОКВЭД организации.</returns>
    public static string GetFormatOkved(Sungero.CompanyData.CompaniesDTO.CompanyDTO company)
    {
      if (company.AdditionalOkveds == null || !company.AdditionalOkveds.Any())
        return company.MainOkved.Code;
      
      var separator = "; ";
      var okveds = new List<string>() { company.MainOkved.Code };
      okveds.AddRange(company.AdditionalOkveds.Select(x => x.Code).Take(6));
      return string.Join(separator, okveds.ToArray());
    }
    
    /// <summary>
    /// Получить реквизиты контрагента из сервиса.
    /// </summary>
    /// <param name="specifiedPSRN">ОГРН выбранной организации.</param>
    /// <returns>Статус запроса.</returns>
    public bool GetCounterpartyInfoFromService(Sungero.Parties.ICompanyBase cp)
    {
      var url = GetCompanyDataServiceURL();
      if (string.IsNullOrEmpty(url))
      {
        Logger.Debug(CompanyBases.Resources.ErrorNotFound.StringFormat);
        return false;
      }
      
      // Запрос в сервис.
      var searchResult = Sungero.CompanyData.Client.Search(cp.PSRN, cp.TIN, cp.Name, url);
      switch (searchResult.StatusCode)
      {
        case HttpStatusCode.OK:
          break;
        case HttpStatusCode.Unauthorized:
          Logger.Debug(CompanyBases.Resources.ErrorUnauthorized);
          return false;
        case HttpStatusCode.Forbidden:
          Logger.Debug(CompanyBases.Resources.ErrorForbidden);
          return false;
        case (HttpStatusCode)429:
          Logger.Debug(CompanyBases.Resources.ErrorTooManyRequests);
          return false;
        case HttpStatusCode.ServiceUnavailable:
        case HttpStatusCode.BadGateway:
        case HttpStatusCode.NotFound:
          Logger.Debug(CompanyBases.Resources.ErrorNotFound);
          return false;
        case HttpStatusCode.PaymentRequired:
          Logger.Debug(CompanyBases.Resources.ErrorNoLicense);
          return false;
        default:
          Logger.Debug(CompanyBases.Resources.ErrorInService);
          return false;
      }
      
      Logger.DebugFormat("{0} counterparties found in service", searchResult.Count.ToString());
      
      // Нашли ровно один. Сразу заполняем реквизиты.
      if (searchResult.Count == 1)
      {
        var company = searchResult.Companies.First();
        cp.Name = company.ShortName;
        cp.LegalName = company.LegalName;
        cp.PSRN = company.Ogrn;
        cp.TIN = company.Inn;
        cp.TRRC = company.Kpp;
        cp.NCEO = company.Okpo;
        cp.NCEA = GetFormatOkved(company);
        cp.LegalAddress = company.Address;
        cp.Region = Sungero.Commons.PublicFunctions.Region.GetRegionFromAddress(company.Address);
        cp.City = Sungero.Commons.PublicFunctions.City.GetCityFromAddress(company.Address);
        if (!Equals(company.State, "Действующее"))
        {
          if (string.IsNullOrEmpty(cp.Note))
            cp.Note = company.State;
          else if (!cp.Note.Contains(company.State))
            cp.Note = string.Format("{0}\r\n{1}", cp.Note, company.State);
        }
        return true;
      }
      else
        return false;
    }
  }
}
