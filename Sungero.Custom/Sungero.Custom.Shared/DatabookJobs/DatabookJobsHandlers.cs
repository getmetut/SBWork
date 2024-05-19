using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Custom.DatabookJobs;

namespace Sungero.Custom
{
  partial class DatabookJobsSharedHandlers
  {
    public virtual void IDJobChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        var job = Sungero.Workflow.Assignments.Get(long.Parse(e.NewValue.ToString()));
        if (job != null)
        {
          if (job.Author != null)
          {
            var AuthorJob = Sungero.Company.Employees.GetAll(r => r.Login == job.Author.Login).FirstOrDefault();
            if (AuthorJob != null)
              if (_obj.AuthorJob != AuthorJob)
                _obj.AuthorJob = AuthorJob;
          }
          
          if (_obj.Assignment != job)
            _obj.Assignment = job;
          
          var EmployeeJob = Sungero.Company.Employees.GetAll(r => r.Login == job.Performer.Login).FirstOrDefault();
          if (EmployeeJob != null)
            if (_obj.EmployeeJob != EmployeeJob)
              _obj.EmployeeJob = EmployeeJob;
          
          if (_obj.Name != job.Subject)
            _obj.Name = job.Subject;
          
          if (job.Deadline.HasValue)
            _obj.Deadline = job.Deadline.Value;
          
          if (job.IsRead.HasValue)
            _obj.IsRead = job.IsRead.Value;
          
            _obj.IsExpired = job.IsExpired;
          
          if (_obj.StateJob != job.Status.Value.ToString())
            _obj.StateJob = job.Status.Value.ToString();
          
          if (job.AllAttachments.Count() > 0)
          {
            foreach (var doc in job.AllAttachments)
            {
              if (sberdev.SBContracts.OfficialDocuments.GetAll(od => od.Id == doc.Id).FirstOrDefault() != null)
              {
                var ofdoc = sberdev.SBContracts.OfficialDocuments.GetAll(od => od.Id == doc.Id).FirstOrDefault();
                if ((_obj.BusinessUnit != ofdoc.BusinessUnit) && (ofdoc.BusinessUnit != null))
                  _obj.BusinessUnit = ofdoc.BusinessUnit;
              }
              if (sberdev.SBContracts.ContractualDocuments.GetAll(cd => cd.Id == doc.Id).FirstOrDefault() != null)
              {
                var cddoc = sberdev.SBContracts.ContractualDocuments.GetAll(cd => cd.Id == doc.Id).FirstOrDefault();
                if ((_obj.BusinessUnit != cddoc.BusinessUnit) && (cddoc.BusinessUnit != null))
                  _obj.BusinessUnit = cddoc.BusinessUnit;
                
                if ((_obj.Company != cddoc.Counterparty) && (cddoc.Counterparty != null))
                  _obj.Company = cddoc.Counterparty;
              }
            }
          }
        }
      }
    }
  }
}