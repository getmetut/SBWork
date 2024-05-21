using System;
using Sungero.Core;

namespace Sungero.Custom.Constants
{
  public static class Module
  {
    // Уникальный идентификатор для типа «Неформализованный отчет».
    public static readonly Guid FacelessTochet = Guid.Parse("c61b0478-a42d-4138-88da-bf33e487a62c");
    
    // Уникальный идентификатор для типа «Маркетинговые акции».
    public static readonly Guid Marketing = Guid.Parse("17b4a41d-2ea8-43d3-b2c2-39dc966189db");
    
    // Уникальный идентификатор для типа «Sup. Contractual Document Plus».
    public static readonly Guid SupPlusContractual = Guid.Parse("1b71c97c-a6d5-435a-8baf-095d5b3264f7");

    // Уникальный идентификатор для типа «NDA».
    public static readonly Guid NDA = Guid.Parse("d62141c7-715c-47bb-82b0-cf2ffbe8b6e7");
  }
}