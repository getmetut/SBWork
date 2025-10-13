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
    /// Сформировать и скачать один ZIP для всех контрагентов (массово).
    /// </summary>
    [Public]
    public static void DownloadFocusCheckArchive()
    {
      var dialog = Dialogs.CreateConfirmDialog("Вы хотите экспортировать архив проверок всех контрганетов?");
      if (dialog.Show())
      {
        var ids = PublicFunctions.Module.Remote.GetFocusCompaniesIDs();
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