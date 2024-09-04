using System;
using Sungero.Core;

namespace sberdev.SBContracts.Constants
{
  public static class Module
  {
    [Public]
    public const string PurchaseShortTemplateDocxName = "//PurchaseShortTemplate.docx";
    [Public]
    public const string PurchaseTemplateDocxName = "//PurchaseTemplate.docx";
    [Public]
    public const string ContractXiongxinTemplateDocxName = "//ContractXioxingTemplate.docx";
    [Public]
    public const string OrderXiongxinTemplateDocxName = "//OrderXioxingTemplate.docx";
    
    public static readonly Guid KZTypeGuid = Guid.Parse("271898c8-18ca-4192-9892-e27b273ce5fc");
    /// <summary>
    /// Список идентификаторов объектов.
    /// </summary>
    public static class CertificateOid
    {
      [Sungero.Core.Public]
      public const string CommonName = "2.5.4.3";
      [Sungero.Core.Public]
      public const string Country = "2.5.4.6";
      [Sungero.Core.Public]
      public const string State = "2.5.4.8";
      [Sungero.Core.Public]
      public const string Locality = "2.5.4.7";
      [Sungero.Core.Public]
      public const string Street = "2.5.4.9";
      [Sungero.Core.Public]
      public const string Department = "2.5.4.11";
      [Sungero.Core.Public]
      public const string Surname = "2.5.4.4";
      [Sungero.Core.Public]
      public const string GivenName = "2.5.4.42";
      [Sungero.Core.Public]
      public const string JobTitle = "2.5.4.12";
      [Sungero.Core.Public]
      public const string OrganizationName = "2.5.4.10";
      [Sungero.Core.Public]
      public const string Email = "1.2.840.113549.1.9.1";
      [Sungero.Core.Public]
      public const string TIN = "1.2.643.3.131.1.1";
      [Sungero.Core.Public]
      public const string OrganizationTIN = "1.2.643.100.4";
    }
  }
}