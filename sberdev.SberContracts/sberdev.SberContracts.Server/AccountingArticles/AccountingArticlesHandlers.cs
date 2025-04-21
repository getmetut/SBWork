using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AccountingArticles;

namespace sberdev.SberContracts
{
  partial class AccountingArticlesServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.SalesChannel = false;
    }
  }

}