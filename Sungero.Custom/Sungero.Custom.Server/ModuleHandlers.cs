using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom.Server
{
  partial class AccountingDocumentsFolderHandlers
  {

    public virtual IQueryable<sberdev.SBContracts.IAccountingDocumentBase> AccountingDocumentsDataQuery(IQueryable<sberdev.SBContracts.IAccountingDocumentBase> query)
    {
      //query = query.Where(d => (d.AccessRights.IsGranted(Sungero.Core.DefaultAccessRightsTypes.Read, Users.Current)) ||
      //                    (d.AccessRights.IsGranted(Sungero.Core.DefaultAccessRightsTypes.Change, Users.Current)) ||
      //                    (d.AccessRights.IsGranted(Sungero.Core.DefaultAccessRightsTypes.FullAccess, Users.Current)));
      query = query.Where(d => (d.CalcListSDev.Length > 1)); // ((d.ProductListSDev.Length > 1) || 
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