using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace SberDev.ExchDocDistrib.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Показать диалог переадресации c предзаполненым адресатом.
    /// </summary>
    [Public]
    public bool ShowPrefillForwardDialog(sberdev.SBContracts.IExchangeDocumentProcessingAssignment assign)
    {
      var dialog = Dialogs.CreateInputDialog(Sungero.Docflow.Resources.ForwardAssignment);
      dialog.HelpCode = Sungero.Docflow.PublicConstants.Module.HelpCodes.ForwardDialogWithDeadline;
      var forwardButton = dialog.Buttons.AddCustom(Sungero.Docflow.Resources.Forward);
      dialog.Buttons.AddCancel();
      
      var deadline = dialog.AddDate(Sungero.Docflow.Resources.Deadline, true, Calendar.AddWorkingDays(Calendar.Now, 2)).AsDateTime();
      
      var excludedPerformers = Functions.Module.GetBoxResponsibleEmployees(assign.Box);
      var forwardTo = dialog.AddSelect(Sungero.Docflow.Resources.ForwardTo, true, Sungero.Company.Employees.Null)
        .Where(p => p.Status == Sungero.Company.Employee.Status.Active);
      
      List<Sungero.Company.IEmployee> possiblePerformers = new List<Sungero.Company.IEmployee>();
      bool hasRules = false;
      var rules = PublicFunctions.Rule.Remote.GetRules(assign.Performer, assign.Counterparty);
      if (rules.Any())
      {
        hasRules = true;
        possiblePerformers = rules.Select(r => r.Responsible).ToList();
        forwardTo.IsRequired = false;
        forwardTo.IsVisible = false;
      }
      
      var forwardToFiltred = dialog.AddSelect(Sungero.Docflow.Resources.ForwardTo, hasRules,
                                              hasRules ? possiblePerformers.FirstOrDefault() : Sungero.Company.Employees.Null)
        .Where(p => p.Status == Sungero.Company.Employee.Status.Active);
      
      if (hasRules)
        forwardToFiltred = forwardToFiltred.Where(p => possiblePerformers.Contains(p));
      
      forwardToFiltred.IsVisible = hasRules;
      
      var filterFlag = dialog.AddBoolean(SberDev.ExchDocDistrib.Resources.FilterFlagName, hasRules);
      
      filterFlag.SetOnValueChanged(
        args =>
        {
          var flag = args.NewValue.HasValue ?  args.NewValue.Value : false;
          forwardTo.IsRequired = !flag;
          forwardTo.IsVisible = !flag;
          forwardToFiltred.IsRequired = flag;
          forwardToFiltred.IsVisible = flag;
        });
      
      dialog.SetOnRefresh(
        args =>
        {
          var warnMessage = Sungero.Docflow.PublicFunctions.Module.CheckDeadlineByWorkCalendar(forwardTo.Value ?? Users.Current, deadline.Value);
          if (!string.IsNullOrEmpty(warnMessage))
            args.AddWarning(warnMessage);

          if (!Sungero.Docflow.PublicFunctions.Module.CheckDeadline(forwardTo.Value ?? Users.Current, deadline.Value, Calendar.Now))
            args.AddError(Sungero.Docflow.Resources.ImpossibleSpecifyDeadlineLessThanToday, deadline);
        });

      if (dialog.Show() == forwardButton)
      {
        assign.Addressee = forwardTo.IsVisible ? forwardTo.Value : forwardToFiltred.Value;
        assign.NewDeadline = deadline.Value;
        return true;
      }
      
      return false;
    }
  }
}