using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalTask;

namespace sberdev.SBContracts.Server
{
  partial class ApprovalTaskFunctions
  {
    #region Сеты*
    
    [Public]
    public static void ClearPublicComment(Sungero.Workflow.IAssignment assignment)
    {
      var sberAssign = SBContracts.ApprovalAssignments.As(assignment);
      var task = sberdev.SBContracts.ApprovalTasks.As(assignment.Task);
      if (task == null)
        return;
      
      var official = SBContracts.OfficialDocuments.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
      if (official == null)
        return;
      
      PublicFunctions.OfficialDocument.ClearPublicComment(official);
    }
    
    /// <summary>
    /// Ставит статус ка "проверяется в данный момент"
    /// </summary>
    /// <param name="assignment"></param>
    public void SetCPCheckingProperty(Sungero.Docflow.IApprovalAssignment assignment)
    {
      var sberAssign = SBContracts.ApprovalAssignments.As(assignment);
      var stage = GetStage();
      if (stage == null)
        return;
      
      bool flag = IsNecessaryStage(PublicConstants.Docflow.ApprovalTask.CheckingCPStage);
      if (!flag)
        return;
      
      var contractual = SBContracts.ContractualDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault());
      if (contractual == null)
        return;
      
      var cp = SBContracts.Counterparties.As(contractual.Counterparty);
      if (cp == null)
        return;
      
      PublicFunctions.Counterparty.SetManualCheckingValue(cp, true);
    }
    
    public void SetChangeAccessRightsOnDocs(Sungero.Docflow.IApprovalAssignment assignment)
    {
      var task = Sungero.Docflow.ApprovalTasks.As(assignment.Task);
      if (task == null || task.Author == null)
        return;

      var author = task.Author;

      var docs = new System.Collections.Generic.List<Sungero.Docflow.IOfficialDocument>();
      var lead = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (lead != null && Sungero.Docflow.OfficialDocuments.Is(lead))
        docs.Add(Sungero.Docflow.OfficialDocuments.As(lead));

      foreach (var d in task.AddendaGroup.OfficialDocuments)
        if (d != null && Sungero.Docflow.OfficialDocuments.Is(d))
          docs.Add(Sungero.Docflow.OfficialDocuments.As(d));

      foreach (var any in task.OtherGroup.All)
        if (any != null && Sungero.Docflow.OfficialDocuments.Is(any))
          docs.Add(Sungero.Docflow.OfficialDocuments.As(any));

      foreach (var doc in docs)
        GrantChangeRightSafe(doc, author);
    }

    private static void GrantChangeRightSafe(Sungero.Docflow.IOfficialDocument doc, Sungero.CoreEntities.IUser user)
    {
      // уже есть право — выходим
      if (doc.AccessRights.IsGranted(Sungero.Core.DefaultAccessRightsTypes.Change, user))
        return;

      const int maxAttempts = 3;

      for (var attempt = 1; attempt <= maxAttempts; attempt++)
      {
        // аккуратно пробуем захватить
        if (!Locks.TryLock(doc))
        {
          System.Threading.Thread.Sleep(300 * attempt);
          continue;
        }

        try
        {
          doc.AccessRights.Grant(user, Sungero.Core.DefaultAccessRightsTypes.Change);
          doc.AccessRights.Save();
          return;
        }
        catch (Sungero.Domain.Shared.Exceptions.OperationRetryTimeoutException)
        {
          if (attempt == maxAttempts) throw;
          System.Threading.Thread.Sleep(500 * attempt);
        }
        catch (Sungero.Domain.Shared.Exceptions.LockManagementException)
        {
          if (attempt == maxAttempts) throw;
          System.Threading.Thread.Sleep(500 * attempt);
        }
        finally
        {
          Locks.Unlock(doc);
        }
      }
    }

    public void SetCheckCPStageName(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      bool isCheckingCPStage = PublicFunctions.ApprovalTask.IsNecessaryStage(_obj, PublicConstants.Docflow.ApprovalTask.CheckingCPStage);
      if (isCheckingCPStage)
      {
        var contract = SBContracts.ContractualDocuments.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault());
        if (contract.Counterparty == null)
          return;
        e.Block.Subject = $"Проверьте \"{contract.Counterparty.Name}\" на благонадежность";
        e.Block.ThreadSubject = $"Проверьте \"{contract.Counterparty.Name}\" на благонадежность";
      }
    }
    
    public void AddMVZApprovers(SBContracts.IApprovalAssignment blok)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      var contract = SBContracts.Contracts.As(document);
      if (contract!= null)
      {
        sberdev.SberContracts.IMVZ mvz = null;
        
        if (contract.MVZBaseSberDev != null)
          mvz = sberdev.SberContracts.MVZs.GetAll().Where(l => l.Id == contract.MVZBaseSberDev.Id ).First();
        else
          mvz = sberdev.SberContracts.MVZs.GetAll().Where(l => l.Id == contract.MVPBaseSberDev.Id ).First();
        
        if (mvz != null)
        {
          var operation = new Enumeration("AddApprover");
          blok.Forward(mvz.BudgetOwner, ForwardingLocation.Next, Calendar.Today.AddWorkingDays(2));
          blok.History.Write(operation, operation, Sungero.Company.PublicFunctions.Employee.GetShortName(mvz.BudgetOwner, false));
          /*
          var task = ApprovalTasks.As(_obj);
          var approvalAsg = ApprovalAssignments.As(_obj); //
          if (task != null && approvalAsg != null)
          {
            var approver = task.AddApproversExpanded.AddNew();
            approver.Approver = mvz.BudgetOwner;
            task.Save();
          }*/
        }
      }
    }
    
    /// <summary>
    /// Проверяет необходимость и удаляет исполнителя в доп этапе
    /// </summary>
    /// <param name="e"></param>
    public void RemoveSupStagePerformer(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      if (IsNecessaryStage(PublicConstants.Docflow.ApprovalTask.SupplementalStage))
      {
        var performers = e.Block.Performers.ToList();
        e.Block.Performers.Clear();
        var lastAssign = GetLastTaskAssigment(_obj, null);
        var typedAssign = SBContracts.ApprovalAssignments.As(lastAssign);
        if (typedAssign?.IsNeedSupStageSberDev == true)
          foreach (var performer in performers)
            e.Block.Performers.Add(performer);
      }
    }
    
    /// <summary>
    /// Функция для того чтобы убарть скип согласования
    /// </summary>
    /// <param name="e"></param>
    public void CancelApproveSkip(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      var stage = GetStage();
      if (stage == null)
        return;
      if (IsNecessaryStage(PublicConstants.Docflow.ApprovalTask.CancelApproveSkipStage))
      {
        var approvers = Sungero.Docflow.PublicFunctions.ApprovalStage.Remote.GetStagePerformers(_obj, GetStage());
        foreach (var approver in approvers)
          e.Block.Performers.Add(approver);
      }
    }
    
    public override void UpdateDocumentApprovalState(Sungero.Docflow.IOfficialDocument document, Nullable<Enumeration> state)
    {
      var invoice = SBContracts.IncomingInvoices.As(document);
      if (invoice == null)
        base.UpdateDocumentApprovalState(document, state);
    }
    
    #endregion
    
    #region Переадресация задания на замещающего
    /// <summary>
    /// Часть механизма переадресации задания на замещающего что отправил на доработку
    /// Записывает в задачу пользователя что выполнил задание по замещению и замещающего
    /// </summary>
    /// <param name="assigment"></param>
    public void SaveSubstitutePerformer(Sungero.Workflow.IAssignment assignment)
    {
      if (assignment.CompletedBy != null)
      {
        var elem = _obj.SubstitutePerformersSentToReworkSberDev.AddNew();
        elem.Performer = Sungero.Company.Employees.As(assignment.Performer);
        elem.Substitute = Sungero.Company.Employees.As(assignment.CompletedBy);
        elem.IsProcessed = false;
      }
    }
    
    /// <summary>
    /// Часть механизма переадресации задания на замещающего что отправил на доработку
    /// Если найден иполнитель найден в списке, ставит вместо него замещающего
    /// </summary>
    /// <param name="assigment"></param>
    public void SetSubstitutePerformer(Sungero.Workflow.IAssignment assignment)
    {
      var collection = _obj.SubstitutePerformersSentToReworkSberDev;
      var match = collection.FirstOrDefault(elem => elem.Performer == assignment.Performer);
      if (match != null)
        assignment.Performer = match.Substitute;
    }
    
    /// <summary>
    /// Часть механизма переадресации задания на замещающего что отправил на доработку
    /// Удаляет запись о замещающем из списка
    /// </summary>
    /// <param name="assigment"></param>
    public bool MarkSubstitutePerformerAsProcessed(Sungero.Workflow.IAssignment assignment)
    {
      var collection = _obj.SubstitutePerformersSentToReworkSberDev;
      var match = collection.FirstOrDefault(elem => elem.Substitute == assignment.Performer);
      if (match != null)
      {
        var doneStage = _obj.DoneStage.AddNew();
        doneStage.Performer = match.Performer;
        doneStage.Stage = GetStage();
        match.IsProcessed = true;
        return false;
      }
      else
        return true;
    }
    
    /// <summary>
    /// Часть механизма переадресации задания на замещающего что отправил на доработку
    /// Удаляет все записи со статусом IsProcessed = true.
    /// </summary>
    public void CleanupProcessedSubstitutePerformers()
    {
      var collection = _obj.SubstitutePerformersSentToReworkSberDev;
      var processedRecords = collection.Where(elem => elem.IsProcessed == true).ToList();
      foreach (var record in processedRecords)
      {
        collection.Remove(record);
      }
    }
    
    #endregion
    
    #region Выполнять один раз
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// Сохраняем информацию о том что нужно пропустить
    /// </summary>
    public void SaveOneTimeCompletePerformer(Sungero.Workflow.IAssignment assignment)
    {
      var stage = GetStage();

      if (stage == null) return;

      if (stage.OneTime == true)
      {
        var newStage = _obj.DoneStage.AddNew();
        newStage.Stage = stage;
        newStage.Performer = assignment.Performer;
      }
    }
    
    /// <summary>
    /// Механика пропуска этапа если выбран флажок "Выполнять один раз" в этапе согласования
    /// Удаляем исполнителей этапа
    /// </summary>
    public void RemoveOneTimeCompletePerformers(object arguments)
    {
      var stage = GetStage();
      if (stage == null)
        return;

      if (_obj.DoneStage.Any(s => s.Stage == stage))
      {
        // Получаем коллекцию исполнителей через рефлексию
        var blockProperty = arguments.GetType().GetProperty("Block");
        if (blockProperty == null)
          throw new InvalidOperationException("Свойство Block не найдено.");

        var block = blockProperty.GetValue(arguments);
        var performersProperty = block?.GetType().GetProperty("Performers");
        if (performersProperty == null)
          throw new InvalidOperationException("Свойство Performers не найдено.");

        var performersCollection = performersProperty.GetValue(block) as ICollection<IRecipient>;
        if (performersCollection == null)
          throw new InvalidOperationException("Свойство Performers имеет некорректный формат.");

        // Получаем DoneStage через прямой доступ
        var needRemovePerformers = _obj.DoneStage
          .Where(r => r.Stage == stage)
          .Select(s => s.Performer)
          .ToList();

        // Удаляем исполнителей через рефлексию
        foreach (var performer in needRemovePerformers)
        {
          if (performer != null)
            performersCollection.Remove(performer);
        }
      }
    }
    
    #endregion
    
    #region Геты*
    
    /// <summary>
    /// Получить последнее задание по задаче. Созданное не позже указанной даты.
    /// </summary>
    /// <param name="task">Задача согласования.</param>
    /// <param name="lastDate">Крайняя дата.</param>
    /// <returns>Последнее задание по задаче.</returns>
    [Public, Remote]
    public static Sungero.Workflow.IAssignment GetLastTaskAssigment(Sungero.Workflow.ITask task, DateTime? lastDate)
    {
      var taskAssignments = Sungero.Workflow.Assignments.GetAll().Where(o => Equals(o.Task, task));

      if (lastDate.HasValue)
        taskAssignments = taskAssignments.Where(o => o.Created < lastDate);
      
      var lastAssignment = taskAssignments.OrderByDescending(o => o.Created).FirstOrDefault();
      
      return lastAssignment;
    }
    
    /// <summary>
    /// Получить сортированный по дате (сначала старые) список заданий по задаче.
    /// </summary>
    /// <param name="task">Задача.</param>
    [Public, Remote]
    public static System.Collections.Generic.List<Sungero.Workflow.IAssignment> GetAllTaskAssigment(Sungero.Workflow.ITask task)
    {
      var taskAssignments = Sungero.Workflow.Assignments.GetAll()
        .Where(o => Equals(o.Task, task)).OrderBy(o => o.Created).ToList();
      return taskAssignments;
    }
    
    [Public, Remote]
    public System.Collections.Generic.List<sberdev.SBContracts.IApprovalTask> GetTasksByCounterparty(sberdev.SBContracts.ICompany company)
    {
      return ApprovalTasks.GetAll().Where(t => t.CounterpartySberDev == company).ToList();
    }
    
    /// <summary>
    /// Определить тот ли ето этап что нужен
    /// </summary>
    /// <param name="sid">Sid</param>
    /// <returns></returns>
    [Public]
    public bool IsNecessaryStage(string sid)
    {
      return _obj.ApprovalRule.Stages
        .Where(s => s.Stage != null)
        .Any(s => s.Number == _obj.StageNumber && SBContracts.ApprovalStages.As(s.Stage).SidSberDev == sid);
    }
    
    /// <summary>
    /// Получить текущий этап
    /// </summary>
    /// <returns></returns>
    [Public]
    public SBContracts.IApprovalStage GetStage()
    {
      return SBContracts.ApprovalStages.As(_obj.ApprovalRule.Stages.FirstOrDefault(s => s.Number == _obj.StageNumber)?.Stage);
    }
    
    #endregion
    
    #region Создающие функции
    
    /// <summary>
    /// Функция создания и отправки задачи по настройке Диадока
    /// </summary>
    /// <param name="doc"></param>
    public void SendDiadocSettingsTask(Sungero.Docflow.IOfficialDocument doc)
    {
      var contr = SBContracts.ContractualDocuments.As(doc);
      SBContracts.ICounterparty cp = null;
      if (contr != null)
        cp = SBContracts.Counterparties.As(contr.Counterparty);
      else
        return;
      if (cp == null)
        return;
      if (doc.DeliveryMethod != null && doc.DeliveryMethod.Id != 1)
        return;
      if (cp.Nonresident == true)
        return;
      if (cp.CanExchange == true)
        return;
      var task = SberContracts.DiadocSettingsTasks.Create();
      task.Counterparty = cp;
      task.Subject = sberdev.SBContracts.ApprovalTasks.Resources.SendDiadocSettingsTaskSubject + task.Counterparty.Name + sberdev.SBContracts.ApprovalTasks.Resources.SendDiadocSettingsTaskSubject2;
      task.NeedsReview = false;
      task.Start();
    }
    #endregion
    
    #region Валидация
    
    /// <summary>
    /// Функция возвращает true если в согласуемом документе указаны закрытые аналитики
    /// </summary>
    [Public]
    public bool OldAnaliticsInDocument(Sungero.Docflow.IOfficialDocument doc)
    {
      var contractual = SBContracts.ContractualDocuments.As(doc);
      var accounting = SBContracts.AccountingDocumentBases.As(doc);
      bool flag = false;
      if (contractual != null)
      {
        if (contractual.MVPBaseSberDev != null && contractual.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (contractual.MVZBaseSberDev != null && contractual.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (contractual.AccArtExBaseSberDev != null && contractual.AccArtExBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed)
          flag = true;
        if (contractual.AccArtPrBaseSberDev != null && contractual.AccArtPrBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed)
          flag = true;
        if (contractual.MarketDirectSberDev != null && contractual.MarketDirectSberDev.Status == SberContracts.MarketingDirection.Status.Closed)
          flag = true;
        foreach (var prod in contractual.ProdCollectionExBaseSberDev)
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
            flag = true;
        foreach (var prod in contractual.ProdCollectionPrBaseSberDev)
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
            flag = true;
      }
      if (accounting != null)
      {
        if (accounting.MVPBaseSberDev != null && accounting.MVPBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (accounting.MVZBaseSberDev != null && accounting.MVZBaseSberDev.Status == SberContracts.MVZ.Status.Closed)
          flag = true;
        if (accounting.AccArtBaseSberDev != null && accounting.AccArtBaseSberDev.Status == SberContracts.AccountingArticles.Status.Closed)
          flag = true;
        foreach (var prod in accounting.ProdCollectionBaseSberDev)
          if (prod.Product.Status == SberContracts.ProductsAndDevices.Status.Closed)
            flag = true;
      }
      return flag;
    }
    #endregion
    
    #region Переопределенные функции
    
    public override void SendApprovalAbortNotice()
    {
      if (_obj.NeedAbort.GetValueOrDefault() )
      {
        var document = _obj.DocumentGroup.OfficialDocuments.First();
        var subject = string.Format(Sungero.Exchange.Resources.TaskSubjectTemplate, "Прекращено согласование", Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(document.Name));
        var task = Sungero.Workflow.SimpleTasks.CreateAsSubtask(_obj);
        task.Subject = subject ;
        task.ThreadSubject = "Прекращено согласование";
        task.ActiveText = _obj.AbortingReason;
        task.NeedsReview = false;
        task.Author = _obj.Author;
        var routeStep = task.RouteSteps.AddNew();
        routeStep.AssignmentType = Sungero.Workflow.SimpleTaskRouteSteps.AssignmentType.Notice;
        routeStep.Performer = _obj.Author;
        routeStep.Deadline = null;
        task.Start();
      }
      else
        base.SendApprovalAbortNotice();
    }
    #endregion
  }
}