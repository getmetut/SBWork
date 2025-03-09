using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Client
{
  partial class ApprovalAnalyticsWidgetWidgetHandlers
  {

    public virtual void ExecuteApprovalAnalyticsWidgetAssignApprByDeparReportAction()
    {/*
      var performerParameterIsAll = Equals(_parameters.Performer, Widgets.AssignmentCompletionDepartmentGraph.Performer.All);
      
      // Проверить наличие прав на отчёт и наличие данных для его формирования.
      if (!Docflow.Reports.GetDepartmentsAssignmentCompletionReport().CanExecute() ||
          (!performerParameterIsAll && Users.Current.IsSystem == true) ||
          (performerParameterIsAll && !Docflow.PublicFunctions.Module.Remote.IsAdministratorOrAdvisor()))
      {
        Dialogs.ShowMessage(Sungero.Shell.Resources.NoDataOrNoRightsToReport, MessageType.Error);
        return;
      }
      
      var departmentIds = Functions.Module.Remote.GetWidgetDepartmentIds(_parameters.Performer);
      var businessUnitIds = Functions.Module.Remote.GetWidgetBusinessUnitIds(_parameters.Performer);

      var periodBegin = Functions.Module.GetWidgetBeginPeriod(_parameters.Period);
      var unwrap = Functions.Module.NeedUnwrapWidgetDepartments(_parameters.Performer);
      
      if (!Docflow.PublicFunctions.Module.Remote.BusinessUnitAssignmentCompletionWidgetDataExist(businessUnitIds, departmentIds, periodBegin, Calendar.UserToday.EndOfDay(), unwrap, false, !performerParameterIsAll))
      {
        Dialogs.ShowMessage(Sungero.Shell.Resources.NoDataOrNoRightsToReport, MessageType.Error);
        return;
      }
      
      var widgetParameter = Functions.Module.GetWidgetParameterLocalizedName(_parameters.Performer);
      Docflow.PublicFunctions.Module.OpenDepartmentsAssignmentCompletionReport(departmentIds, periodBegin, Calendar.UserToday, widgetParameter, unwrap, false);*/
    }

    public virtual void ExecuteApprovalAnalyticsWidgetAssignApprByDepartChartAction(Sungero.Domain.Client.ExecuteWidgetBarChartActionEventArgs e)
    {/*
      long departmentId;
      if (!long.TryParse(e.SeriesId, out departmentId))
      {
        Logger.ErrorFormat("ExecuteAssignmentCompletionDepartmentGraphChartAction. Failed parse department id {0}", e.SeriesId);
        return;
      }
      
      PublicFunctions.Module.OpenReportFromDepartmentWidgets(departmentId, new List<long>() { departmentId }, _parameters.Performer, _parameters.Period, true);*/
    }
  }


}