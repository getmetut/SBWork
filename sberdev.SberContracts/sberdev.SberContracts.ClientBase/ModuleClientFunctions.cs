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
    /// Действие создания документа Закупки
    /// </summary>
    public virtual void CreatePurchase()
    {
      var doc = sberdev.SberContracts.Purchases.Create();
      doc.Show();
    }
    
    /// <summary>
    /// Действие создания заявки на закупку
    /// </summary>
    public virtual void CreateAppRnDPurchase()
    {
      var doc = sberdev.SberContracts.AppRnDPurchases.Create();
      doc.Show();
    }
    
    /// <summary>
    /// Действие создания заявки на производственную закупку
    /// </summary>
    public virtual void CreateAppProductPurchase()
    {
      var doc = sberdev.SberContracts.AppProductPurchases.Create();
      doc.Show();
    }
    
    /// <summary>
    /// Действие создания документа заявки на непроизводственную закупку
    /// </summary>
    public virtual void CreateAppNonProdPurchase()
    {
      var doc = sberdev.SberContracts.AppNonProdPurchases.Create();
      doc.Show();
    }
    
    /// <summary>
    /// Создать протокол выбора способа закупки
    /// </summary>
    public virtual void CreatePurchaseProtocol()
    {
      var doc = Sungero.Custom.PurchaseProtocols.Create();
      doc.Show();
    }

    /// <summary>
    /// Открыть запись справочника "Настройки калькуляции МВЗ".
    /// </summary>
    [Public]
    public void OpenMVZCalcSettings()
    {
      var emp = Sungero.Company.Employees.Current;
      if (emp != null) {
        var settings = MVZCalculationSettingses.GetAll()
          .Where(s => s.Employee != null && s.Employee == emp)
          .FirstOrDefault();
        
        // Если в справочнике не найдены настройки МВЗ для текущего пользователя.
        if (settings == null) {
          settings = MVZCalculationSettingses.Create();
          settings.Name = $"Настройки калькуляции МВЗ сотрудника {emp.Name}";
          settings.Employee = emp;
          // По умолчанию калькуляция сбрасывается.
          settings.ResetCalc = true;
        }
        
        settings.ShowModal();
      }
    }

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
      string log = PublicFunctions.Module.ReSaveServer();
      if (log != "")
        Dialogs.ShowMessage(log);
      else
        Dialogs.ShowMessage("Все записи пересохранены.");
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