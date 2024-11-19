using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.ATS.PowerOfAttorney;

namespace Sungero.ATS.Shared
{
  partial class PowerOfAttorneyFunctions
  {

    /// <summary>
    /// Обновление карточки ловеренности
    /// </summary>       
    [Public]
    public void UpdateCard()
    {
      var Visible = Roles.GetAll(r => r.Name == "Доступ к персональным данным").FirstOrDefault();
      _obj.State.Properties.PDInfoSDev.IsVisible = false;
      _obj.State.Properties.PDNumberSDev.IsVisible = false;
      _obj.State.Properties.PDRegistrationSDev.IsVisible = false;
      _obj.State.Properties.PDSeriesSDev.IsVisible = false;
      if (Visible != null)
      {
        if (Visible.RecipientLinks.Count > 0)
        {
          foreach (var str in Visible.RecipientLinks)
          {
            if (str.Member.Id == Users.Current.Id)
            {
              _obj.State.Properties.PDInfoSDev.IsVisible = true;
              _obj.State.Properties.PDNumberSDev.IsVisible = true;
              _obj.State.Properties.PDRegistrationSDev.IsVisible = true;
              _obj.State.Properties.PDSeriesSDev.IsVisible = true;
            }
          }
        }
      }
    }
      
  }
}