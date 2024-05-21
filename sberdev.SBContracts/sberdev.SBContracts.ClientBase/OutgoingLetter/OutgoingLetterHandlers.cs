using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.OutgoingLetter;

namespace sberdev.SBContracts
{
  partial class OutgoingLetterClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      var analiticSetup = SberContracts.AnaticsSetups.GetAll(r => (r.DocumentType == SberContracts.AnaticsSetup.DocumentType.OutgoingLetter) && (r.DocumentKind == _obj.DocumentKind)).FirstOrDefault();
        if (analiticSetup != null)
          { _obj.State.Properties.MVZ.IsEnabled = !analiticSetup.MVZIsEnabled.GetValueOrDefault() ;
            _obj.State.Properties.MVP.IsEnabled = !analiticSetup.MVPIsEnabled.GetValueOrDefault() ;
            _obj.State.Properties.MVZ.IsVisible = analiticSetup.MVZIsVisible.GetValueOrDefault() ;
            _obj.State.Properties.MVP.IsVisible = analiticSetup.MVPIsVisible.GetValueOrDefault() ;
            _obj.State.Properties.MVZ.IsRequired = analiticSetup.MVZIsRequired.GetValueOrDefault() ;
            _obj.State.Properties.MVP.IsRequired = analiticSetup.MVPIsRequired.GetValueOrDefault() ;
            _obj.State.Properties.ExBudg.IsVisible = analiticSetup.MVZIsVisible.GetValueOrDefault() ;            
            _obj.State.Properties.AccArt.IsRequired = analiticSetup.AccArtsIsRequired.GetValueOrDefault() ;
            _obj.State.Properties.AccArt.IsEnabled = !analiticSetup.AccArtsIsEnabled.GetValueOrDefault() ;
            _obj.State.Properties.AccArt.IsVisible = analiticSetup.AccArtsIsVisible.GetValueOrDefault() ;
            _obj.State.Properties.BrandName.IsVisible = analiticSetup.BrandNameIsVisible.GetValueOrDefault() ;
            _obj.State.Properties.BrandName.IsEnabled = !analiticSetup.BrandNameIsEnable.GetValueOrDefault() ;
            _obj.State.Properties.BrandName.IsRequired = analiticSetup.BrandNameIsRequired.GetValueOrDefault() ;
            _obj.State.Properties.DirectionCollection.IsVisible = analiticSetup.ProdIsVisible.GetValueOrDefault() ;
            _obj.State.Properties.DirectionCollection.IsEnabled = !analiticSetup.ProdIsEnabled.GetValueOrDefault() ;
            _obj.State.Properties.DirectionCollection.IsRequired = analiticSetup.ProdIsRequired.GetValueOrDefault() ;
            _obj.State.Properties.LeadingDocument.IsVisible =  analiticSetup.ContractIsVisible.GetValueOrDefault();
            _obj.State.Properties.LeadingDocument.IsEnabled =  !analiticSetup.ContractIsEnabled.GetValueOrDefault();
            _obj.State.Properties.LeadingDocument.IsRequired =  analiticSetup.ContractIsRequired.GetValueOrDefault();
            _obj.State.Properties.ActExists.IsVisible = analiticSetup.ActExistsVisible.GetValueOrDefault();
            _obj.State.Properties.ActExists.IsEnabled = !analiticSetup.ActExistsEnabled.GetValueOrDefault();
            _obj.State.Properties.ActExists.IsRequired = analiticSetup.ActExistsRequired.GetValueOrDefault();
            _obj.State.Properties.DeviceExists.IsVisible = analiticSetup.DeviceExistsVisible.GetValueOrDefault();
            _obj.State.Properties.DeviceExists.IsEnabled = !analiticSetup.DeviceExistsEnabled.GetValueOrDefault();
            _obj.State.Properties.DeviceExists.IsRequired = analiticSetup.DeviceExistsRequired.GetValueOrDefault();
            _obj.State.Properties.FactOfPayment.IsVisible = analiticSetup.FactOfPaymentVisible.GetValueOrDefault();
            _obj.State.Properties.FactOfPayment.IsEnabled = !analiticSetup.FactOfPaymentEnabled.GetValueOrDefault();
            _obj.State.Properties.FactOfPayment.IsRequired = analiticSetup.FactOfPaymentRequired.GetValueOrDefault();
            _obj.State.Properties.PricesAgreed.IsVisible = analiticSetup.PricesAgreedVisible.GetValueOrDefault();
            _obj.State.Properties.PricesAgreed.IsEnabled = !analiticSetup.PricesAgreedEnabled.GetValueOrDefault();
            _obj.State.Properties.PricesAgreed.IsRequired = analiticSetup.PricesAgreedRequired.GetValueOrDefault();
            _obj.State.Properties.Framework.IsVisible = analiticSetup.FrameworkVisible.GetValueOrDefault();
            _obj.State.Properties.Framework.IsEnabled = !analiticSetup.FrameworkEnabled.GetValueOrDefault();
            _obj.State.Properties.Framework.IsRequired = analiticSetup.FrameworkRequired.GetValueOrDefault();
        }
        else
        {
            _obj.State.Properties.MVZ.IsEnabled = true ;
            _obj.State.Properties.MVP.IsEnabled = true ;
            _obj.State.Properties.MVZ.IsVisible = true ;
            _obj.State.Properties.MVP.IsVisible = true ;
            _obj.State.Properties.MVZ.IsRequired = false ;
            _obj.State.Properties.MVP.IsRequired = false ;
            _obj.State.Properties.ExBudg.IsVisible = true ;            
            _obj.State.Properties.AccArt.IsRequired = false ;
            _obj.State.Properties.AccArt.IsEnabled = true ;
            _obj.State.Properties.AccArt.IsVisible = true ;
            _obj.State.Properties.BrandName.IsVisible = true ;
            _obj.State.Properties.BrandName.IsEnabled = true ;
            _obj.State.Properties.BrandName.IsRequired = false ;
            _obj.State.Properties.DirectionCollection.IsVisible = true ;
            _obj.State.Properties.DirectionCollection.IsEnabled = true ;
            _obj.State.Properties.DirectionCollection.IsRequired = false ;
            _obj.State.Properties.LeadingDocument.IsVisible =  true;
            _obj.State.Properties.LeadingDocument.IsEnabled =  true;
            _obj.State.Properties.LeadingDocument.IsRequired =  false;
            _obj.State.Properties.ActExists.IsVisible = true;
            _obj.State.Properties.ActExists.IsEnabled = true;
            _obj.State.Properties.ActExists.IsRequired = false;
            _obj.State.Properties.DeviceExists.IsVisible = true;
            _obj.State.Properties.DeviceExists.IsEnabled = true;
            _obj.State.Properties.DeviceExists.IsRequired = false;
            _obj.State.Properties.FactOfPayment.IsVisible = true;
            _obj.State.Properties.FactOfPayment.IsEnabled = true;
            _obj.State.Properties.FactOfPayment.IsRequired = false;
            _obj.State.Properties.PricesAgreed.IsVisible = true;
            _obj.State.Properties.PricesAgreed.IsEnabled = true;
            _obj.State.Properties.PricesAgreed.IsRequired = false;
            _obj.State.Properties.Framework.IsVisible = false;
            _obj.State.Properties.Framework.IsEnabled = false;
            _obj.State.Properties.Framework.IsRequired = false;
            
        }
        
        
    }

  }
}