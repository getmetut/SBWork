using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Вызывает диалоговое окно для выбора Продукт-юнита и возвращает выбранный элемент
    /// </summary>
    [Public]
    public System.Collections.Generic.IEnumerable<SberContracts.IProductUnit> CreateProductUnitDialog()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SberContracts.Resources.CreateProductUnitDialogTittle);
      var unit = dialog.AddSelectMany(sberdev.SberContracts.Resources.CreateProductUnitDialogProp, true, ProductUnits.Null);
      if (dialog.Show() == DialogButtons.Ok)
        return unit.Value;
      else
        return null;
    }
    
    /// <summary>
    /// Вызывает диалоговое окно для выбора Шаблона калькуляции и возвращает выбранный элемент
    /// </summary>
    [Public]
    public SberContracts.ICalculationTamplate CreateCalcTamplateDialog(string str)
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SberContracts.Resources.CreateProductUnitDialogTittle);
      if (str == "Percent")
      {
        var tamplate = dialog.AddSelect(sberdev.SberContracts.Resources.CreateCalcTamplateDialogProp, true, CalculationTamplates.Null)
          .Where(t => t.CalculationFlag == SberContracts.CalculationTamplate.CalculationFlag.Percent);
        if (dialog.Show() == DialogButtons.Ok)
          return tamplate.Value;
        else
          return null;
      }
      else
      {
        var tamplate = dialog.AddSelect(sberdev.SberContracts.Resources.CreateCalcTamplateDialogProp, true, CalculationTamplates.Null)
          .Where(t => t.CalculationFlag == SberContracts.CalculationTamplate.CalculationFlag.Absolute);
        if (dialog.Show() == DialogButtons.Ok)
          return tamplate.Value;
        else
          return null;
      }
    }

    /// <summary>
    /// Функция пересохранения справочника Компаний
    /// </summary>
    public virtual void ReSaveCompany()
    {
      var CompanyList = sberdev.SBContracts.Companies.GetAll(c => c.Status == sberdev.SBContracts.Company.Status.Active);
      string log = "";
      int ind = 0;
      foreach (var comp in CompanyList)
      {
        try
        {
          comp.Name = comp.Name;
          comp.Save();
        }
        catch (Exception e)
        {
          log += comp.Name.ToString() + "| Ошибка: " + e.Message.ToString();
        }
        ind += 1;
      }
      if (log != "")
        Dialogs.ShowMessage(log);
      else
        Dialogs.ShowMessage("Все записи пересохранены. Всего: " + ind.ToString());
    }

    /// <summary>
    /// Функция для корректного отображения отчета по оплаченным счетам
    /// </summary>
    [Public]
    public static string PaidInvoiceReportFunction(string s)
    {
      return s == "Profitable" ? "Доходный" : (s == "Extandable" ? "Расходный" : "Не указан");
    }
    /// <summary>
    /// Вызывать окно с предупреждением при изменении суммы
    /// </summary>
    [Public]
    public void ShowTotalAmountChangedDialog(bool flag)
    {
      if (flag)
        Dialogs.ShowMessage(sberdev.SberContracts.Resources.AmountChangedAbsolute, MessageType.Information);
      else
        Dialogs.ShowMessage(sberdev.SberContracts.Resources.AmountChangedPercent, MessageType.Information);
    }
    
  }
}