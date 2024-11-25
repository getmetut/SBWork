using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace sberdev.SBContracts.Module.Docflow.Server
{
  public partial class ModuleInitializer
  {
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      CreateRelationTypes();
    }
    public static void CreateRelationTypes()
    {
      InitializationLogger.Debug("Init: Create custom relation types.");
      // Закупка к документу.
      var purchase = CreateRelationType(Constants.Module.PurchaseRelationName, sberdev.SBContracts.Module.Docflow.Resources.PurchaseRelationNameSource,
                                        sberdev.SBContracts.Module.Docflow.Resources.PurchaseRelationNameTarget, sberdev.SBContracts.Module.Docflow.Resources.PurchaseRelationDicripSource,
                                        sberdev.SBContracts.Module.Docflow.Resources.PurchaseRelationDiscrp, true, false, false, true);
      purchase.Mapping.Clear();
      var purchaseRow = purchase.Mapping.AddNew();
      purchaseRow.Target = SberContracts.AbstractsSupAgreements.Info;
      purchaseRow.Source = SBContracts.ContractualDocuments.Info;
      purchaseRow.RelatedProperty = SberContracts.Purchases.Info.Properties.LeadingDocument;
      purchase.Save();
    }
  }
}