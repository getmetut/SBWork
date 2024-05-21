using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.MarcetingDoc;

namespace Sungero.Custom.Shared
{
  partial class MarcetingDocFunctions
  {
    public void UpdateControlCard()
    {
      var Stat = _obj.StagesOfApproval;
      
      _obj.State.Properties.FacktTotalSumm.IsVisible = (Stat == Custom.MarcetingDoc.StagesOfApproval.SummingResult); // Фактическая сумма продаж
      _obj.State.Properties.FacktSummCurrency.IsVisible = (Stat == Custom.MarcetingDoc.StagesOfApproval.SummingResult); // Валюта (Фактическая сумма продаж)
      _obj.State.Properties.FacktRashod.IsVisible = (Stat == Custom.MarcetingDoc.StagesOfApproval.SummingResult); // Фактические затраты
      _obj.State.Properties.FacktRashCurrency.IsVisible = (Stat == Custom.MarcetingDoc.StagesOfApproval.SummingResult); // Валюта (Фактические затраты)
      _obj.State.Properties.FacktKolInt.IsVisible = (Stat == Custom.MarcetingDoc.StagesOfApproval.SummingResult); // Фактическое кол-во интересов
      
      
      if ((Stat == Custom.MarcetingDoc.StagesOfApproval.Draft) || (Stat == Custom.MarcetingDoc.StagesOfApproval.NewObj) ||
          (Stat == Custom.MarcetingDoc.StagesOfApproval.OnApproval) || (Stat == null))
      {
        _obj.State.Properties.PlannedSummRub.IsEnabled = true;// Плановая сумма продаж
        _obj.State.Properties.PlanSummCurrency.IsEnabled = true;// Валюта (Плановая сумма продаж)
        _obj.State.Properties.PlannedRashod.IsEnabled = true;// Плановые затраты
        _obj.State.Properties.PlanRashCurrency.IsEnabled = true;// Валюта (Плановые затраты)
        _obj.State.Properties.PlanKolInt.IsEnabled = true;// Плановое кол-во интересов 
        _obj.State.Properties.Channels.CanAdd = true;  
        _obj.State.Properties.Channels.Properties.Komission.IsEnabled = true;
        _obj.State.Properties.DevicesAction.Properties.PricePromo.IsEnabled = true;
        _obj.State.Properties.DevicesAction.Properties.PriceNoPromo.IsEnabled = true;
      }
      else
      {
        _obj.State.Properties.PlannedSummRub.IsEnabled = false;// Плановая сумма продаж
        _obj.State.Properties.PlanSummCurrency.IsEnabled = false;// Валюта (Плановая сумма продаж)
        _obj.State.Properties.PlannedRashod.IsEnabled = false;// Плановые затраты
        _obj.State.Properties.PlanRashCurrency.IsEnabled = false;// Валюта (Плановые затраты)
        _obj.State.Properties.PlanKolInt.IsEnabled = false;// Плановое кол-во интересов
        _obj.State.Properties.Channels.CanAdd = false;
        _obj.State.Properties.Channels.Properties.Komission.IsEnabled = false;
        _obj.State.Properties.DevicesAction.Properties.PricePromo.IsEnabled = false;
        _obj.State.Properties.DevicesAction.Properties.PriceNoPromo.IsEnabled = false;
      }
            
    }
  }
}