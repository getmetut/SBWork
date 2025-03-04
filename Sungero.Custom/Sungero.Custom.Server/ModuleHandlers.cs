using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom.Server
{
  partial class ReestrNDAFolderHandlers
  {

    public virtual IQueryable<Sungero.Custom.INDA> ReestrNDADataQuery(IQueryable<Sungero.Custom.INDA> query)
    {
      if (_filter != null)
      {
        if (_filter.Counterparty != null)
          query = query.Where(q => q.Counterparty == _filter.Counterparty);
        
        if (_filter.KindDoc != null)
          query = query.Where(q => q.DocumentKind == _filter.KindDoc);
        
        if (_filter.AuthorDoc != null)
          query = query.Where(q => q.Author.Login == _filter.AuthorDoc.Login);
        
        if (_filter.Department != null)
          query = query.Where(q => q.Department == _filter.Department);
        
        if (_filter.DateRangeTo != null)
          query = query.Where(q => q.RegistrationDate <= _filter.DateRangeTo);
        
        if (_filter.DateRangeFrom != null)
          query = query.Where(q => q.RegistrationDate >= _filter.DateRangeFrom);
      }
      return query;
    }

    public virtual IQueryable<Sungero.Docflow.IDocumentKind> ReestrNDAKindDocFiltering(IQueryable<Sungero.Docflow.IDocumentKind> query)
    {
      query = query.Where(q => q.DocumentType.Name == "NDA / Соглашение об ЭДО");
      return query;
    }
  }


  partial class ProdCalcFolderFolderHandlers
  {

    public virtual IQueryable<Sungero.Custom.IAuthorJob> FolderDataQuery(IQueryable<Sungero.Custom.IAuthorJob> query)
    {
      return query;
    }
  }

  partial class CustomHandlers
  {
  }
}