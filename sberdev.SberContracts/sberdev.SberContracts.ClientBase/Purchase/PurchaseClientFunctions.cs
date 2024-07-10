using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.Purchase;

namespace sberdev.SberContracts.Client
{
  partial class PurchaseFunctions
  {

    public void ChooseImage()
    {
      var dfile = Dialogs.CreateInputDialog("Выберите файл");
      var file = dfile.AddFileSelect("Файл", true).WithFilter("Расширения изображений доступные к выбору", ".jpg", ".png");
      
      if (dfile.Show() == DialogButtons.Ok)
      {
        _obj.ScreenBusinessPlan = file.Value.Content;
        
      }
    }
    
    public void ShowJustifCooseCpFAQ()
    {
      Dialogs.ShowMessage(sberdev.SberContracts.Purchases.Resources.JustifChooseCpFAQName, sberdev.SberContracts.Purchases.Resources.JustifChooseCpFAQ);
    }
    
    public void ShowSpeqificationFAQ()
    {
      Dialogs.ShowMessage(sberdev.SberContracts.Purchases.Resources.SpecificationFAQname, sberdev.SberContracts.Purchases.Resources.SpecificcationFAQ);
    }
    
    
    public override bool NeedViewDocumentSummary()
    {
      return true;
    }
  }
}