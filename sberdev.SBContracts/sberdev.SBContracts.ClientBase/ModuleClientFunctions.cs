using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Функция выводит информационное сообщение в диалоговом окне
    /// </summary>
    /// <param name="message">Текс сообщения</param>
    [Public]
    public void ShowInfoMessage(string message)
    {
      Dialogs.ShowMessage(message, MessageType.Information);
    }
    
    /// <summary>
    /// Функция выводит сообщение об ошибке в диалоговом окне
    /// </summary>
    /// <param name="message">Текс сообщения</param>
    [Public]
    public void ShowErrorMessage(string message)
    {
      Dialogs.ShowMessage(message, MessageType.Error);
    }
    
    /// <summary>
    /// Сформировать и скачать один ZIP для выбранных контрагентов (массово).
    /// </summary>
    [Public]
    public static void DownloadFocusCheckArchive()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.Resources.SelectCounterpartiesDialog);
      var сompanies = dialog.AddSelectMany(sberdev.SBContracts.Resources.SelectCounterpartiesDialogProp, true, Companies.Null);
      if (dialog.Show() == DialogButtons.Ok)
      {
        var ids = сompanies.Value
          .Where(c => c != null)
          .Select(c => c.Id)
          .Distinct()
          .ToList();

        if (!ids.Any())
        {
          Dialogs.NotifyMessage("Не выбрано ни одного контрагента.");
          return;
        }

        var zip = sberdev.SBContracts.PublicFunctions.Module.Remote.ExportFocusCheckZipByIdsRemote(ids);
        if (zip == null)
        {
          Dialogs.NotifyMessage("Нет данных для выгрузки по выбранным контрагентам.");
          return;
        }
        zip.Export();
      }
    }
  }
}