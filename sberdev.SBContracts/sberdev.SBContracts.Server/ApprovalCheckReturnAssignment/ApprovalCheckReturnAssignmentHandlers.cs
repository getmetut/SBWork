using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalCheckReturnAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalCheckReturnAssignmentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      /*
      var doc = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (doc != null)
      {
        if ((sberdev.SBContracts.SupAgreements.Is(doc)) || (sberdev.SBContracts.IncomingInvoices.Is(doc)) ||
            (sberdev.SberContracts.Purchases.Is(doc)) || (sberdev.SberContracts.Specifications.Is(doc)))
        {
          List<Sungero.Company.IEmployee> users = new List<Sungero.Company.IEmployee>();
          if (sberdev.SBContracts.SupAgreements.Is(doc))
          {
            var docsup = sberdev.SBContracts.SupAgreements.As(doc);
            if (docsup.ResponsibleObserversATSDevATSDev.Count > 0)
            {
              foreach (var elem in docsup.ResponsibleObserversATSDevATSDev)
                users.Add(elem.Observer);
            }
          }
          
          if (sberdev.SBContracts.IncomingInvoices.Is(doc))
          {
            var docinc = sberdev.SBContracts.IncomingInvoices.As(doc);
            if (docinc.ResponsibleObserversATSDev.Count > 0)
            {
              foreach (var elem in docinc.ResponsibleObserversATSDev)
                users.Add(elem.Observer);
            }
          }
          
          if (sberdev.SberContracts.Purchases.Is(doc))
          {
            var docpurch = sberdev.SberContracts.Purchases.As(doc);
            if (docpurch.ResponsibleObserversATSDev.Count > 0)
            {
              foreach (var elem in docpurch.ResponsibleObserversATSDev)
                users.Add(elem.Observer);
            }
          }
          
          if (sberdev.SberContracts.Specifications.Is(doc))
          {
            var docsspec = sberdev.SberContracts.Specifications.As(doc);
            if (docsspec.ResponsibleObserversATSDev.Count > 0)
            {
              foreach (var elem in docsspec.ResponsibleObserversATSDev)
                users.Add(elem.Observer);
            }
          }
          
          if (users.Count > 0)
          {
            var notif = Sungero.Workflow.SimpleTasks.CreateWithNotices("Подписан документ: " + doc.Name, users.ToArray());
            notif.ActiveText = "Подписан документ: " + Hyperlinks.Get(doc).ToString() + '\n' +
              "Исходная задача согласования: " + Hyperlinks.Get(_obj.Task).ToString();
            notif.Start();
          }
        }
      }*/
    }

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      base.BeforeComplete(e);
      PublicFunctions.ApprovalTask.ClearPublicComment(_obj);
    }
  }


}