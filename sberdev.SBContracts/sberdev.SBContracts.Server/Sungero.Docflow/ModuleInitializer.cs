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
    }
    public static void CreateRelationTypes()
    {/*
            // Закупка к документу.
      var addendum = CreateRelationType(Constants.Module.PurchaseRelationName, "Закупки Название источника",
                                        "Закупки Название назначения", "Закупки Описание источника",
                                        "Закупки Описание назначения", true, false, false, true);
      addendum.Mapping.Clear();
      var addendumRow = addendum.Mapping.AddNew();
      addendumRow.Source = SberContracts.Purchases.Info;
      addendumRow.Target = SungeroContent.ElectronicDocuments.Info;
      addendumRow = addendum.Mapping.AddNew();
      addendumRow.Source = Content.ElectronicDocuments.Info;
      addendumRow.Target = Docflow.Addendums.Info;
      addendumRow.RelatedProperty = SberContracts.Addendums.Info.Properties.LeadingDocument;
      addendum.Save();*/
    }
  }
}
