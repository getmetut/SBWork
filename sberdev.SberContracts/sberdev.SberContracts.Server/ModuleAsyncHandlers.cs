using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void DeleteContractById(sberdev.SberContracts.Server.AsyncHandlerInvokeArgs.DeleteContractByIdInvokeArgs args)
    {int contractId = args.ContrId;
      try
      {sberdev.SBContracts.Contracts.Delete(sberdev.SBContracts.Contracts.Get(contractId));}
      catch (Exception e)
      {Logger.Error(e.Message + contractId);
      }
      args.Retry = false;
    }
  }
}