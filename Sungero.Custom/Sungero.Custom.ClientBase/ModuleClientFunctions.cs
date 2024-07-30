using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.Text;

namespace Sungero.Custom.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Пересохранение Сотрудников
    /// </summary>
    public virtual void ReSaveEmpl()
    {
      var EmplList = Sungero.Company.Employees.GetAll().ToList();
      string log = "";
      foreach (var empl in EmplList)
      {
        try
        {
          var us = empl.Login;
          if (us != null)
          {
            us.LoginName = us.LoginName + "&";
            us.LoginName = us.LoginName.Replace("&","");
            us.Save();
          }
          empl.Save();
        }
        catch (Exception e)
        {
          log += e.Message.ToString() + '\n';
        }
      }
      if (log != "")
        Dialogs.ShowMessage(log);
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void SearchDocDiadoc()
    {
      var Exc = sberdev.SBContracts.ExchangeDocuments.GetAll(d => ((d.Created > Calendar.AddWorkingDays(Calendar.Today, -10)) && (d.LastVersionApproved.Value))).ToList();
      string Res = "";
      foreach (var elem in Exc)	
      {
        string cou = elem.Counterparty != null ? elem.Counterparty.Name.ToString() : "";
        string num = elem.RegistrationNumber != null ? elem.RegistrationNumber.ToString() : "";
        string dat = elem.RegistrationDate != null ? elem.RegistrationDate.ToString() : "";
        string ddat = elem.DocumentDate != null ? elem.DocumentDate.ToString() : "";
        Res += cou + ";" + elem.Name.ToString() + ";" + num + ";" + dat + ";" + ddat + '\n';
      }
      string path = @"C:\temp\" + Calendar.Now.ToString().Replace(".","").Replace(":","").ToString() + ".csv";
      
      Encoding encoding = Encoding.GetEncoding("windows-1251");
      byte[] encodedBytes = encoding.GetBytes(Res);
      string ansiString = encoding.GetString(encodedBytes);
      
      File.AppendAllText(path,ansiString);
      var Doc = Custom.FacelessTochets.Create();
      
      Doc.Name = "Diadoc list - " + Calendar.Now.ToString();
      Doc.Author = Users.Current;
      Doc.Save();
      Doc.CreateVersionFrom(path);
      Doc.Save();
      //File.Delete(path);
      Doc.Show();
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ParseCsv()
    {
        var dialog = Dialogs.CreateInputDialog("Выберите csv-файл для обработки");
        var filePath = dialog.AddFileSelect(@"Выберите файл в папке C:\temp\",true);
        var soglashenie = dialog.AddBoolean("Шаблон файла соответствует требованиям");
        
        if (dialog.Show() == DialogButtons.Ok)
        {
          if (soglashenie.Value != true)
            Dialogs.ShowMessage("Шаблон csv должен содержать колонки: ID, ИНН, КПП, Признак 0/1 на автозаполнение без заголовка!");
          else
          {
            try
            {
              string PathFile = filePath.Value.Name;
              string log = "";
              using (var sr = new StreamReader(@"C:\temp\" + PathFile))
              {
                while (!sr.EndOfStream)
                {
                  var line = sr.ReadLine();
                  var values = line.Split(';'); // Используем разделитель ";"
                  
                  // Если количество столбцов не соответствует ожидаемому, пропускаем строку
                  if (values.Length != 4)
                  {
                    log+= "Ошибка: количество столбцов не соответствует ожидаемому.";
                    continue;
                  }
                  else
                  {
                    // Извлекаем значения столбцов
                    var id = values[0];
                    var inn = values[1];
                    var kpp = values[2];
                    var number = values[3];
                    try
                    {
                      var KA = sberdev.SBContracts.Companies.GetAll(k => k.Id == int.Parse(id)).FirstOrDefault();
                      if (KA != null)
                      {
                        KA.TIN = inn;
                        if (kpp != null)
                          KA.TRRC = kpp;
                        
                        KA.Save();
                        log+= "Успешно обновлен: " + KA.Name.ToString() + '\n';
                      }
                      else
                        log+= "Не найден КА с ИД: " + id + '\n';
                    }
                    catch (Exception err)
                    {
                      log+= "Возникла проблема: " + err.Message.ToString() + '\n';
                    }
                  }
                }
                Dialogs.ShowMessage("Готово. Logtext:" + '\n' + log);
              }
            }
            catch (FileNotFoundException)
            {
                Dialogs.ShowMessage("Файл не найден.");
            }
            catch (Exception e)
            {
                Dialogs.ShowMessage("Ошибка при разборе файла: " + e.Message);
            }
          }
        }   
    }

    /// <summary>
    /// вывод сообщения для пользователя (информационный диалог)
    /// </summary>
    [Public]
    public void MSG(string msg)
    {
      Dialogs.ShowMessage(msg);
    }

    /// <summary>
    /// Функция пересохранения всех записей справочника Контрагентов
    /// </summary>
    public virtual void ReSaveKA()
    {
      var lst = sberdev.SBContracts.Companies.GetAll(k => k.Status == sberdev.SBContracts.Company.Status.Active);
      int oks = 0;
      int def = 0;
      string log = "";
      foreach (var elem in lst)
      {
        try
        {
          elem.Name = elem.Name;
          elem.Save();
          oks += 1;
        }
        catch (Exception e)
        {
          def += 1;
          log += elem.Id.ToString() + ": " + e.Message.ToString() + '\n';
        }
      }
      Dialogs.ShowMessage("Успешно отработано: " + oks.ToString() + '\n' + "Завершились ошиибкой: " + def.ToString() + '\n' + "ЛОГ: " + log);
    }
  }
}