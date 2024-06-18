using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OfficialDocument;

namespace sberdev.SBContracts.Shared
{
  partial class OfficialDocumentFunctions
  {

    public Nullable<Enumeration> GetIntApprEnum(string value)
    {
      switch (value)
      {
        case "OnApproval":
          return InternalApprovalState.OnApproval;
        case "OnRework":
          return InternalApprovalState.OnRework;
        case "PendingSign":
          return InternalApprovalState.PendingSign;
        case "Signed":
          return InternalApprovalState.Signed;
        case "Aborted":
          return InternalApprovalState.Aborted;
        default:
          return null;
      }
    }
    public Nullable<Enumeration> GetExtApprEnum(string value)
    {
      switch (value)
      {
        case "OnApproval":
          return ExternalApprovalState.OnApproval;
        case "UnSigned":
          return ExternalApprovalState.Unsigned;
        case "Signed":
          return ExternalApprovalState.Signed;
        default:
          return null;
      }
    }
  }
}