using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AnaticsSetup;

namespace sberdev.SberContracts
{
  partial class AnaticsSetupServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.AccArtsIsEnabled = false;
      _obj.AccArtsIsRequired  = false;
      _obj.AccArtsIsVisible = false;
      _obj.BrandNameIsEnable = false;
      _obj.BrandNameIsRequired = false;
      _obj.BrandNameIsVisible = false;
      _obj.MVZIsEnabled = false;
      _obj.MVZIsRequired = false;
      _obj.MVZIsVisible = false;
      _obj.ProdIsEnabled = false;
      _obj.ProdIsRequired = false;
      _obj.ProdIsVisible = false;
      _obj.MVPIsEnabled = false;
      _obj.MVPIsRequired = false;
      _obj.MVPIsVisible = false;
      _obj.ContractIsEnabled =false;
      _obj.ContractIsRequired = false;
      _obj.ContractIsVisible = false;
      _obj.ActExistsEnabled = false;
      _obj.ActExistsRequired = false;
      _obj.ActExistsVisible = false;
      _obj.DeviceExistsEnabled = false;
      _obj.DeviceExistsRequired = false;
      _obj.DeviceExistsVisible = false;
      _obj.FactOfPaymentEnabled = false;
      _obj.FactOfPaymentRequired = false;
      _obj.FactOfPaymentVisible = false;
      _obj.PricesAgreedEnabled = false;
      _obj.PricesAgreedRequired= false;
      _obj.PricesAgreedVisible = false;
      _obj.FrameworkEnabled = false;
      _obj.FrameworkRequired = false;
      _obj.FrameworkVisible = false;
     
    }
  }

}