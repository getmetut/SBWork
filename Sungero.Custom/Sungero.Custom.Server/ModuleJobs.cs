using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Custom.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Фоновый процесс отбора документов и заполнение полей о продуктах и калькуляциях
    /// </summary>
    public virtual void JobSearchDocumProdCalc()
    {
      int Acc = 0;
      int errAcc = 0;
      int Con = 0;
      int errCon = 0;
      string ErrorString = "";
      
      var listDogDocs = sberdev.SBContracts.ContractualDocuments.GetAll().ToList();
      foreach (var contrdoc in listDogDocs)
      {
        try
        {
          contrdoc.Note += " "; 
          if ((contrdoc.State.Properties.PurchComNumberSberDev.IsRequired == true) && (contrdoc.PurchComNumberSberDev == null))
            contrdoc.PurchComNumberSberDev = "000.0";
          contrdoc.Save();
          Con += 1;
        }
        catch
        {
          errCon += 1;
          ErrorString += contrdoc.Id.ToString() + ";";
        }
      }
      var listAccoutingDocs = sberdev.SBContracts.AccountingDocumentBases.GetAll().ToList();
      foreach (var accdoc in listAccoutingDocs)
      {
        try
        {
          accdoc.Note += " ";  
          accdoc.Save();
          Acc += 1;
        }
        catch
        {
          errAcc += 1;
          ErrorString += accdoc.Id.ToString() + ";";
        }
      }
      Logger.Debug("Отработан фоновый процесс заполнения продуктов и калькуляций. Отработано Финансовых докуменнтов: " + Acc.ToString() + "| " + "Отработано Финансовых докуменнтов: " + Acc.ToString() + "| " + 
                   "Отработано Договорных докуменнтов: " + Con.ToString() + "| Получено ошибок: " + errAcc.ToString() + "/" + errCon.ToString());
      Logger.Debug("Ошибки в документах: " + ErrorString);
    }

  }
}