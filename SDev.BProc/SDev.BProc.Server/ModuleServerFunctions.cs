using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Company;
using Sungero.Content;
using SDev.BProc.Structures.Module;

namespace SDev.BProc.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать архив с результатами проверок Контур Фокус и скачать его.
    /// Обрабатываются только компании, по которым есть проверки.
    /// </summary>
    /// <param name="companyChecks">Данные проверок по компаниям.</param>
    /// <returns>ZIP-архив.</returns>
    [Public, Remote]
    public static Sungero.Core.IZip ExportKonturFocusChecks(Dictionary<ICompany, KonturFocusCheck> companyChecks)
    {
      var zip = Zip.Create();

      foreach (var pair in companyChecks.Where(p => p.Value != null && p.Value.Checks != null && p.Value.Checks.Any()))
      {
        var companyFolder = SanitizeFileName(pair.Key.Name);

        // Таблица с проверками и статусом.
        var builder = new StringBuilder();
        builder.AppendLine($"Статус;{pair.Value.Status}");
        builder.AppendLine("Дата;Результат");
        foreach (var check in pair.Value.Checks)
          builder.AppendLine($"{check.Date:yyyy-MM-dd};{check.Result}");

        zip.Add(Encoding.UTF8.GetBytes(builder.ToString()), companyFolder + "/Проверки", ".csv");

        // Сопроводительные файлы.
        if (pair.Value.Files != null)
        {
          foreach (var document in pair.Value.Files)
          {
            if (!document.HasVersions)
              continue;

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var extension = "." + document.LastVersion.AssociatedApplication.Extension;
            document.LastVersion.Export(tempFile);
            var bytes = File.ReadAllBytes(tempFile);
            File.Delete(tempFile);

            var fileName = SanitizeFileName(Path.GetFileNameWithoutExtension(document.Name));
            zip.Add(bytes, companyFolder + "/Файлы/" + fileName, extension);
          }
        }
      }

      var zipFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");
      zip.Save(zipFilePath);
      File.Delete(zipFilePath);

      return zip;
    }

    private static string SanitizeFileName(string name)
    {
      foreach (var ch in Path.GetInvalidFileNameChars())
        name = name.Replace(ch, '_');
      return name;
    }
  }
}