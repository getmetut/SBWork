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
    /// Процесс поиска и расстановки уровней подчиненности среди организаций
    /// </summary>
    public virtual void JobHeadOrg()
    {
      var ListOrgTrue = sberdev.SBContracts.Companies.GetAll(c => c.HeadOrgSDev.HasValue).Where(c => c.HeadOrgSDev == true).ToArray();
      if (ListOrgTrue.Count() > 0)
      {
        foreach (var elem in ListOrgTrue)
        {
          if (elem.TIN != null)
          {
            var OtherOrg = sberdev.SBContracts.Companies.GetAll(c => ((c.TIN == elem.TIN) && (c.HeadCompany != elem) && (c.Id != elem.Id))).ToArray();
            if (OtherOrg.Count() > 0)
            {
              foreach (var othorg in OtherOrg)
              {
                othorg.HeadOrgSDev = false;
                othorg.HeadCompany = elem;
                othorg.Save();
              }
            }
          }
        }
      }
      
      var ListCompany = sberdev.SBContracts.Companies.GetAll(c => ((c.HeadCompany == null) && (!c.HeadOrgSDev.HasValue))).ToArray();
      if (ListCompany.Count() > 0)
      {
        foreach (var org in ListCompany)
        {
          if (org.TIN != null)
          {
            var OtherOrg2 = sberdev.SBContracts.Companies.GetAll(c => ((c.TIN == org.TIN) && (c.HeadOrgSDev.HasValue) && (c.Id != org.Id))).ToArray();
            if (OtherOrg2.Count() > 0)
            {
              foreach (var othorg in OtherOrg2)
              {
                if (othorg.HeadOrgSDev.Value)
                {
                  org.HeadCompany = othorg;
                  org.Save();
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Процесс редоставления прав на документы по признаку нахождения МВЗ
    /// </summary>
    public virtual void JobAccesMVZ()
    {
      var MVZs = sberdev.SberContracts.MVZs.GetAll(m => m.Status == sberdev.SberContracts.MVZ.Status.Active).ToList();
      if (MVZs.Count > 0)
      {
        foreach (var mvz in MVZs)
        {
          if (mvz.CollectionEmplAcc.Count > 0)
          {
            foreach (var empl in mvz.CollectionEmplAcc)
            {
              var usr = empl.Employee;
              
              var DocsContractual = sberdev.SBContracts.ContractualDocuments.GetAll(c => ((c.MVZBaseSberDev == mvz) && (!c.AccessRights.CanRead(usr)))).ToList();
              var DocsAccounting = sberdev.SBContracts.AccountingDocumentBases.GetAll(a => ((a.MVZBaseSberDev == mvz) && (!a.AccessRights.CanRead(usr)))).ToList();
              
              if (DocsContractual.Count > 0)
              {
                foreach (var contr in DocsContractual)
                {
                  if (!contr.AccessRights.CanRead(usr))
                  {
                    contr.AccessRights.Grant(usr, DefaultAccessRightsTypes.Read);
                    contr.Save();
                  }
                  
                  var Tasks = Sungero.Docflow.ApprovalTasks.GetAll(t => t.DocumentGroup.OfficialDocuments.FirstOrDefault() != null).Where(t => ((t.DocumentGroup.OfficialDocuments.FirstOrDefault() == contr) && (!t.AccessRights.CanRead(usr)))).ToList();
                  if (Tasks.Count > 0)
                  {
                    foreach (var tsk in Tasks)
                    {
                      if (!tsk.AccessRights.CanRead(usr))
                      {
                        tsk.AccessRights.Grant(usr, DefaultAccessRightsTypes.Read);
                        tsk.Save();
                      }
                    }
                  }
                }
              }
              if (DocsAccounting.Count > 0)
              {
                foreach (var Accoun in DocsAccounting)
                {
                  if (!Accoun.AccessRights.CanRead(usr))
                  {
                    Accoun.AccessRights.Grant(usr, DefaultAccessRightsTypes.Read);
                    Accoun.Save();
                  }
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Фоновый процесс рассылки уведомлений по договорам с истекающим сроком действия
    /// </summary>
    public virtual void ControlSkorContracts()
    {
      DateTime currentDate = Calendar.Today;
      var targetDates = new List<DateTime>
      {
        currentDate.AddDays(14),
        currentDate.AddDays(7),
        currentDate.AddDays(3),
        currentDate.AddDays(1)
      };
      
      foreach (var targetDate in targetDates)
      {
        if (targetDate.DayOfWeek == DayOfWeek.Saturday)
          targetDate.AddDays(-1);
        else if (targetDate.DayOfWeek == DayOfWeek.Sunday)
          targetDate.AddDays(-2);
      }
      var Contractuals = sberdev.SBContracts.Contracts.GetAll(d => d.ValidTill.HasValue).Where(c => ((targetDates.Contains(c.ValidTill.Value)) && (c.LifeCycleState == sberdev.SBContracts.Contract.LifeCycleState.Active)));
      if (Contractuals.Count() > 0)
      {
        foreach (var cons in Contractuals)
        {
          var Empl = Sungero.Company.Employees.GetAll(r => r.Login == cons.Author.Login).FirstOrDefault();
          if (Empl != null)
          {
            var task = FreedomicTasks.Create();
            task.Employee = Empl;
            task.Subject = "Заканчивается срок действия договора: " + cons.Name;
            task.OtherAttachment.ElectronicDocuments.Add(cons);
            task.Start();
          }
        }
      }
    }

    /// <summary>
    /// Обработка заданий для занесения их в список заданий в справочнике
    /// </summary>
    public virtual void JobsinReference()
    {
      var ActualList = DatabookJobses.GetAll();
      List<long> ListIds = new List<long>();
      foreach (var elem in ActualList)
      {
        ListIds.Add(long.Parse(elem.IDJob.Value.ToString()));
      }
      var Jobs = Sungero.Workflow.Assignments.GetAll(a => ((a.Status == Sungero.Workflow.Assignment.Status.InProcess) && (!ListIds.Contains(a.Id)))).ToList();
      foreach (var job in Jobs)
      {
        var DJ = DatabookJobses.Create();
        try
        {
          DJ.IDJob = int.Parse(job.Id.ToString());
          DJ.Save();
        }
        catch (Exception e)
        {
          Logger.Debug("Фоновый процесс сбора заданий завершился ошибкой: " + e.Message.ToString());
        }
      }
    }

  }
}