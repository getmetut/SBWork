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
    
    /// <summary>
    /// Удаляет исполнителя у этапа проверки ка, если он проверяется в данный момент
    /// </summary>
    /// <param name="assignment"></param>
    public void RemoveCPCheckingPerformer(Sungero.Docflow.IApprovalAssignment assignment)
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
      
      if (cp.FocusCheckingSberDev == true)
        sberAssign.Performer = null;
    }
    
    /// <summary>
    /// Изменяет статус ка на "Не проверяется в данный момент"
    /// </summary>
    /// <param name="assignment"></param>
    public void MarkCPAsChecking(Sungero.Docflow.IApprovalAssignment assignment)
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
      
      if (cp.FocusCheckingSberDev != true)
      {
        SBContracts.PublicFunctions.Module.Remote.UnblockCardByDatabase(cp);
        cp.FocusCheckingSberDev = true;
        cp.Save();
      }
    }
    
    public void SetChangeAccessRightsOnDocs(Sungero.Docflow.IApprovalAssignment assignment) {
      // Доработка в рамках задачи DRX-660.
      var task = ApprovalTasks.As(assignment.Task);
      if (task != null) {
        var author = task.Author;
        if (author != null) {
          List<IOfficialDocument> documents = new List<IOfficialDocument>();
          var leadDoc = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
          if (leadDoc != null && OfficialDocuments.Is(leadDoc))
            documents.Add(OfficialDocuments.As(leadDoc));
          
          foreach(var doc in task.AddendaGroup.OfficialDocuments) {
            if (doc != null && OfficialDocuments.Is(doc))
              documents.Add(OfficialDocuments.As(doc));
          }
          
          foreach(var doc in task.OtherGroup.All) {
            if (doc != null && OfficialDocuments.Is(doc))
              documents.Add(OfficialDocuments.As(doc));
          }
          
          foreach(var doc in documents) {
            if (!doc.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, author)) {
              doc.AccessRights.Grant(author, DefaultAccessRightsTypes.Change);
              doc.AccessRights.Save();
              Logger.Debug("SBDev DEBUG: Grant Change Access Rights for User Id {0} on Document Id {1}", author.Id, doc.Id);
            }
          }
        }
      }
    }
    
    public void ChangeNameCheckCPStage(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
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
    /// Проверяет необходимость и удаля исполнителя в доп этапе
    /// </summary>
    /// <param name="e"></param>
    public void RemoveSupStagePerformer(Sungero.Docflow.Server.ApprovalCheckingAssignmentArguments e)
    {
      if (IsNecessaryStage(PublicConstants.Docflow.ApprovalTask.SupplementalStage))
      {
        var lastAssign = GetLastTaskAssigment(_obj, null);
        var typedAssign = SBContracts.ApprovalAssignments.As(lastAssign);
        if (typedAssign != null && typedAssign?.IsNeedSupStageSberDev != true)
          e.Block.Performers.Clear();
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
  }
}