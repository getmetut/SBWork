using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Metadata;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sungero.POST.Server
{
  
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Функция передачи актов выполненных работ
    /// </summary>
    [Public(WebApiRequestType = RequestType.Get)]
    public List<Sungero.POST.Structures.Module.IStructContractStatement> ContractStatements(int iddoc)
    {
      var DocList = sberdev.SBContracts.ContractStatements.GetAll();
      List<Sungero.POST.Structures.Module.IStructContractStatement> RequestElem = new List<Sungero.POST.Structures.Module.IStructContractStatement>(); //Structures.Module.StructContractList.Create();
      if (iddoc == 0)
      {
        foreach (var Doc in DocList)
        {
          var ContractSt = Structures.Module.StructContractStatement.Create();
          ContractSt.Id = Doc.Id != null ? Doc.Id.ToString() : "";
          ContractSt.Name = Doc.Name != null ? Doc.Name.ToString() : "";
          ContractSt.DocumentKind = Doc.DocumentKind != null ? Doc.DocumentKind.Name.ToString() : "";
          ContractSt.DocumentGroup = Doc.DocumentGroup != null ? Doc.DocumentGroup.Name.ToString() : "";
          ContractSt.Number = Doc.Number != null ? Doc.Number.ToString() : "";
          ContractSt.Date = Doc.Date != null ? Doc.Date.Value.ToString() : "";
          ContractSt.Counterparty = Doc.Counterparty != null ? Doc.Counterparty.ToString() : "";
          ContractSt.Contact = Doc.Contact != null ? Doc.Contact.Name.ToString() : "";
          ContractSt.CounterpartySignatory = Doc.CounterpartySignatory != null ? Doc.CounterpartySignatory.Name.ToString() : "";
          ContractSt.LeadingDocument = Doc.LeadingDocument != null ? Doc.LeadingDocument.Name.ToString() : "";
          ContractSt.BusinessUnit = Doc.BusinessUnit != null ? Doc.BusinessUnit.Name.ToString() : "";
          ContractSt.Department = Doc.Department != null ? Doc.Department.Name.ToString() : "";
          ContractSt.ResponsibleEmployee = Doc.ResponsibleEmployee != null ? Doc.ResponsibleEmployee.Name.ToString() : "";
          ContractSt.OurSignatory = Doc.OurSignatory != null ? Doc.OurSignatory.Name.ToString() : "";
          ContractSt.ExchangeState = Doc.ExchangeState != null ? Doc.ExchangeState.Value.ToString() : "";
          ContractSt.Subject = Doc.Subject != null ? Doc.Subject.ToString() : "";
          ContractSt.IsFormalized = Doc.IsFormalized != null ? Doc.IsFormalized.Value.ToString() : "";
          ContractSt.IsAdjustment = Doc.IsAdjustment != null ? Doc.IsAdjustment.Value.ToString() : "";
          ContractSt.Corrected = Doc.Corrected != null ? Doc.Corrected.Name.ToString() : "";
          ContractSt.VerificationState = Doc.VerificationState != null ? Doc.VerificationState.Value.ToString() : "";
          ContractSt.DocumentDate = Doc.DocumentDate != null ? Doc.DocumentDate.Value.ToString() : "";
          ContractSt.OurSigningReason = Doc.OurSigningReason != null ? Doc.OurSigningReason.Name.ToString() : "";
          ContractSt.PaperCount = Doc.PaperCount != null ? Doc.PaperCount.Value.ToString() : "";
          ContractSt.StoredIn = Doc.StoredIn != null ? Doc.StoredIn.ToString() : "";
          ContractSt.Topic = Doc.Topic != null ? Doc.Topic.Name.ToString() : "";
          ContractSt.Subtopic = Doc.Subtopic != null ? Doc.Subtopic.Name.ToString() : "";
          ContractSt.ManuallyCheckedSberDev = Doc.ManuallyCheckedSberDev != null ? Doc.ManuallyCheckedSberDev.Value.ToString() : "";
          ContractSt.FormalizedServiceType = Doc.FormalizedServiceType != null ? Doc.FormalizedServiceType.Value.ToString() : "";
          ContractSt.FormalizedFunction = Doc.FormalizedFunction != null ? Doc.FormalizedFunction.Value.ToString() : "";
          ContractSt.IsRevision = Doc.IsRevision != null ? Doc.IsRevision.Value.ToString() : "";
          ContractSt.CounterpartySigningReason = Doc.CounterpartySigningReason != null ? Doc.CounterpartySigningReason.ToString() : "";
          ContractSt.PurchaseOrderNumber = Doc.PurchaseOrderNumber != null ? Doc.PurchaseOrderNumber.ToString() : "";
          ContractSt.ContrTypeBaseSberDev = Doc.ContrTypeBaseSberDev != null ? Doc.ContrTypeBaseSberDev.Value.ToString() : "";
          ContractSt.FrameworkBaseSberDev = Doc.FrameworkBaseSberDev != null ? Doc.FrameworkBaseSberDev.Value.ToString() : "";
          ContractSt.AccDocSberDev = Doc.AccDocSberDev != null ? Doc.AccDocSberDev.Name.ToString() : "";
          ContractSt.MVZBaseSberDev = Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Name.ToString() : "";
          ContractSt.MVPBaseSberDev = Doc.MVPBaseSberDev != null ? Doc.MVPBaseSberDev.Name.ToString() : "";
          ContractSt.AccArtBaseSberDev = Doc.AccArtBaseSberDev != null ? Doc.AccArtBaseSberDev.Name.ToString() : "";
          ContractSt.InvoiceSberDev = Doc.InvoiceSberDev != null ? Doc.InvoiceSberDev.Name.ToString() : "";
          ContractSt.PayTypeBaseSberDev = Doc.PayTypeBaseSberDev != null ? Doc.PayTypeBaseSberDev.Value.ToString() : "";

          List<Sungero.POST.Structures.Module.IStructProdCollection> ProdCollection = new List<Sungero.POST.Structures.Module.IStructProdCollection>();
          if (Doc.ProdCollectionBaseSberDev.Count > 0)
          {
            foreach (var elem in Doc.ProdCollectionBaseSberDev)
            {
              var newstr = Sungero.POST.Structures.Module.StructProdCollection.Create();
              newstr.Product = elem.Product.Name.ToString();
              ProdCollection.Add(newstr);
            } 
          }
          ContractSt.ProdCollectionBaseSberDev = ProdCollection;
                
          ContractSt.MarketDirectSberDev = Doc.MarketDirectSberDev != null ? Doc.MarketDirectSberDev.Name.ToString() : "";
        
          List<Sungero.POST.Structures.Module.IStructCalculation> Calculation = new List<Sungero.POST.Structures.Module.IStructCalculation>();
          if (Doc.CalculationBaseSberDev.Count > 0)
          {
            foreach (var elem in Doc.CalculationBaseSberDev)
            {
              var newstr2 = Sungero.POST.Structures.Module.StructCalculation.Create();
              newstr2.AbsoluteCalc = elem.AbsoluteCalc.Value.ToString();
              newstr2.AggregationCalc = elem.AggregationCalc.ToString();
              newstr2.InterestCalc = elem.InterestCalc.Value.ToString();
              newstr2.PercentCalc = elem.PercentCalc.Value.ToString();
              newstr2.ProductCalc = elem.ProductCalc.Name.ToString();
              Calculation.Add(newstr2);
            } 
          }
          ContractSt.CalculationBaseSberDev = Calculation;
        
          ContractSt.CalculationFlagBaseSberDev = Doc.CalculationFlagBaseSberDev != null ? Doc.CalculationFlagBaseSberDev.Value.ToString() : "";
          ContractSt.CalculationAmountBaseSberDev = Doc.CalculationAmountBaseSberDev != null ? Doc.CalculationAmountBaseSberDev.Value.ToString() : "";
          ContractSt.CalculationDistributeBaseSberDev = Doc.CalculationDistributeBaseSberDev != null ? Doc.CalculationDistributeBaseSberDev.Value.ToString() : "";
          ContractSt.CalculationResidualAmountBaseSberDev = Doc.CalculationResidualAmountBaseSberDev != null ? Doc.CalculationResidualAmountBaseSberDev.Value.ToString() : "";
          ContractSt.NoticeSendBaseSberDev = Doc.NoticeSendBaseSberDev != null ? Doc.NoticeSendBaseSberDev.Value.ToString() : "";
          ContractSt.BudItemBaseSberDev = Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Name.ToString() : "";
          ContractSt.MarketingSberDev = Doc.MarketingSberDev != null ? Doc.MarketingSberDev.Name.ToString() : "";
          ContractSt.MarketingIDSberDev = Doc.MarketingIDSberDev != null ? Doc.MarketingIDSberDev.ToString() : "";
          ContractSt.ModifiedSberDev = Doc.ModifiedSberDev != null ? Doc.ModifiedSberDev.Value.ToString() : "";
          ContractSt.EstPaymentDateSberDev = Doc.EstPaymentDateSberDev != null ? Doc.EstPaymentDateSberDev.ToString() : "";
          ContractSt.Author = Doc.Author != null ? Doc.Author.Name.ToString() : "";
          ContractSt.Created = Doc.Created != null ? Doc.Created.Value.ToString() : "";
          ContractSt.Modified = Doc.Modified != null ? Doc.Modified.Value.ToString() : "";
          ContractSt.RegistrationNumber = Doc.RegistrationNumber != null ? Doc.RegistrationNumber.ToString() : "";
          ContractSt.Index = Doc.Index != null ? Doc.Index.Value.ToString() : "";
          ContractSt.RegistrationDate = Doc.RegistrationDate != null ? Doc.RegistrationDate.Value.ToString() : "";
          ContractSt.DocumentRegister = Doc.DocumentRegister != null ? Doc.DocumentRegister.ToString() : "";
          ContractSt.DeliveryMethod = Doc.DeliveryMethod != null ? Doc.DeliveryMethod.Name.ToString() : "";
          ContractSt.IsReturnRequired = Doc.IsReturnRequired != null ? Doc.IsReturnRequired.Value.ToString() : "";
          ContractSt.IsHeldByCounterParty = Doc.IsHeldByCounterParty != null ? Doc.IsHeldByCounterParty.Value.ToString() : "";
          ContractSt.DeliveredTo = Doc.DeliveredTo != null ? Doc.DeliveredTo.Name.ToString() : "";
          ContractSt.ReturnDeadline = Doc.ReturnDeadline != null ? Doc.ReturnDeadline.Value.ToString() : "";
          ContractSt.ReturnDate = Doc.ReturnDate != null ? Doc.ReturnDate.Value.ToString() : "";
          ContractSt.LifeCycleState = Doc.LifeCycleState != null ? Doc.LifeCycleState.Value.ToString() : "";
          ContractSt.RegistrationState = Doc.RegistrationState != null ? Doc.RegistrationState.Value.ToString() : "";
          ContractSt.InternalApprovalState = Doc.InternalApprovalState != null ? Doc.InternalApprovalState.Value.ToString() : "";
          ContractSt.ExternalApprovalState = Doc.ExternalApprovalState != null ? Doc.ExternalApprovalState.Value.ToString() : "";
          ContractSt.ExecutionState = Doc.ExecutionState != null ? Doc.ExecutionState.Value.ToString() : "";
          ContractSt.ControlExecutionState = Doc.ControlExecutionState != null ? Doc.ControlExecutionState.Value.ToString() : "";
          ContractSt.LocationState = Doc.LocationState != null ? Doc.LocationState.ToString() : "";
          ContractSt.ResponsibleForReturnEmployee = Doc.ResponsibleForReturnEmployee != null ? Doc.ResponsibleForReturnEmployee.Name.ToString() : "";
          ContractSt.ScheduledReturnDateFromCounterparty = Doc.ScheduledReturnDateFromCounterparty != null ? Doc.ScheduledReturnDateFromCounterparty.Value.ToString() : "";
          ContractSt.Note = Doc.Note != null ? Doc.Note.ToString() : "";
          ContractSt.Assignee = Doc.Assignee != null ? Doc.Assignee.Name.ToString() : "";
          ContractSt.PreparedBy = Doc.PreparedBy != null ? Doc.PreparedBy.Name.ToString() : "";
          ContractSt.TotalAmount = Doc.TotalAmount != null ? Doc.TotalAmount.Value.ToString() : "";
          ContractSt.VatRate = Doc.VatRate != null ? Doc.VatRate.Name.ToString() : "";
          ContractSt.VatAmount = Doc.VatAmount != null ? Doc.VatAmount.Value.ToString() : "";
          ContractSt.NetAmount = Doc.NetAmount != null ? Doc.NetAmount.Value.ToString() : "";
          ContractSt.Currency = Doc.Currency != null ? Doc.Currency.Name.ToString() : "";
          
          RequestElem.Add(ContractSt);
        }
        return RequestElem;
      }
      else
      {
        
        return RequestElem;
      }
    }

    /// <summary>
    /// Функция передачи допсоглашений
    /// </summary>
    [Public(WebApiRequestType = RequestType.Get)]
    public List<Structures.Module.IStructSupAgreement> SupAgreements(int iddoc)
    {
      var SupAgreements = sberdev.SBContracts.SupAgreements.GetAll();
      List<Structures.Module.IStructSupAgreement> RequestElem = new List<Sungero.POST.Structures.Module.IStructSupAgreement>(); //Structures.Module.StructContractList.Create();
      if (iddoc == 0)
      {
        foreach (var Doc in SupAgreements)
        {
          var StrSupAgreement = Structures.Module.StructSupAgreement.Create();
          StrSupAgreement.Id =  Doc.Id.ToString();
          StrSupAgreement.Name =  Doc.Name != null ? Doc.Name.ToString()  : "";
          StrSupAgreement.Created =  Doc.Created != null ? Doc.Created.Value.ToString()  : "";
          StrSupAgreement.TotalAmount =  Doc.TotalAmount != null ? Doc.TotalAmount.Value.ToString()  : "";
          StrSupAgreement.ValidFrom =  Doc.ValidFrom != null ? Doc.ValidFrom.Value.ToString()  : "";
          StrSupAgreement.ValidTill =  Doc.ValidTill != null ? Doc.ValidTill.Value.ToString()  : "";
          StrSupAgreement.SDSFSberDevSDev =  Doc.SDSFSberDev != null ? Doc.SDSFSberDev.ToString()  : "";
          StrSupAgreement.SRSberDevSDev =  Doc.SRSberDev != null ? Doc.SRSberDev.ToString()  : "";
          StrSupAgreement.GoogleDocsLinkSberDevSDev =  Doc.GoogleDocsLinkSberDev != null ? Doc.GoogleDocsLinkSberDev.ToString()  : "";
          StrSupAgreement.SubjectSpecificationSberDevSDev =  Doc.SubjectSpecificationSberDev != null ? Doc.SubjectSpecificationSberDev.ToString()  : "";
          StrSupAgreement.AccArtExBaseSberDev =  Doc.AccArtExBaseSberDev != null ? Doc.AccArtExBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrBaseSberDev =  Doc.AccArtPrBaseSberDev != null ? Doc.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.Assignee =  Doc.Assignee != null ? Doc.Assignee.Id.ToString()  : "";
          StrSupAgreement.AssociatedApplication =  Doc.AssociatedApplication != null ? Doc.AssociatedApplication.Id.ToString()  : "";
          StrSupAgreement.Author =  Doc.Author != null ? Doc.Author.Id.ToString()  : "";
          StrSupAgreement.BudItemBaseSberDev =  Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.BusinessUnit =  Doc.BusinessUnit != null ? Doc.BusinessUnit.Id.ToString()  : "";
          StrSupAgreement.CalculationBaseSberDev = "";
          StrSupAgreement.CaseFile =  Doc.CaseFile != null ? Doc.CaseFile.Id.ToString()  : "";
          StrSupAgreement.Contact =  Doc.Contact != null ? Doc.Contact.Id.ToString()  : "";
          StrSupAgreement.Counterparty =  Doc.Counterparty != null ? Doc.Counterparty.Id.ToString()  : "";
          StrSupAgreement.CounterpartySignatory =  Doc.CounterpartySignatory != null ? Doc.CounterpartySignatory.Id.ToString()  : "";
          StrSupAgreement.Currency =  Doc.Currency != null ? Doc.Currency.Id.ToString()  : "";
          StrSupAgreement.DeliveredTo =  Doc.DeliveredTo != null ? Doc.DeliveredTo.Id.ToString()  : "";
          StrSupAgreement.DeliveryMethod =  Doc.DeliveryMethod != null ? Doc.DeliveryMethod.Id.ToString()  : "";
          StrSupAgreement.Department =  Doc.Department != null ? Doc.Department.Id.ToString()  : "";
          StrSupAgreement.DirectionMVZ = "";
          StrSupAgreement.DocumentGroup =  Doc.DocumentGroup != null ? Doc.DocumentGroup.Id.ToString()  : "";
          StrSupAgreement.DocumentKind =  Doc.DocumentKind != null ? Doc.DocumentKind.Id.ToString()  : "";
          StrSupAgreement.DocumentRegister =  Doc.DocumentRegister != null ? Doc.DocumentRegister.Id.ToString()  : "";
          StrSupAgreement.LeadingDocument =  Doc.LeadingDocument != null ? Doc.LeadingDocument.Id.ToString()  : "";
          StrSupAgreement.MarketDirectSberDev =  Doc.MarketDirectSberDev != null ? Doc.MarketDirectSberDev.Id.ToString()  : "";
          StrSupAgreement.Milestones = "";
          StrSupAgreement.MVPBaseSberDev =  Doc.MVPBaseSberDev != null ? Doc.MVPBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZBaseSberDev =  Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.OurSignatory =  Doc.OurSignatory != null ? Doc.OurSignatory.Id.ToString()  : "";
          StrSupAgreement.OurSigningReason =  Doc.OurSigningReason != null ? Doc.OurSigningReason.Id.ToString()  : "";
          StrSupAgreement.Parameters = "";
          StrSupAgreement.PreparedBy =  Doc.PreparedBy != null ? Doc.PreparedBy.Id.ToString()  : "";
          StrSupAgreement.ProdCollectionExBaseSberDev =  "";
          StrSupAgreement.ProdCollectionPrBaseSberDev =  "";
          StrSupAgreement.ProdSberDevCollection =  "";
          StrSupAgreement.Project =  Doc.Project != null ? Doc.Project.Id.ToString()  : "";
          StrSupAgreement.ResponsibleEmployee =  Doc.ResponsibleEmployee != null ? Doc.ResponsibleEmployee.Id.ToString()  : "";
          StrSupAgreement.ResponsibleForReturnEmployee =  Doc.ResponsibleForReturnEmployee != null ? Doc.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrSupAgreement.Subtopic =  Doc.Subtopic != null ? Doc.Subtopic.Id.ToString()  : "";
          StrSupAgreement.Topic =  Doc.Topic != null ? Doc.Topic.Id.ToString()  : "";
          StrSupAgreement.Tracking = "";
          StrSupAgreement.VatRate =  Doc.VatRate != null ? Doc.VatRate.Id.ToString()  : "";
          StrSupAgreement.Versions =  "";
          StrSupAgreement.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/265f2c57-6a8a-4a15-833b-ca00e8047fa5/" + Doc.Id.ToString();
          
          RequestElem.Add(StrSupAgreement);
        }
        return RequestElem;
      }
      else
      {
        var Doc = sberdev.SBContracts.SupAgreements.Get(long.Parse(iddoc.ToString()));
        var StrSupAgreement = Structures.Module.StructSupAgreement.Create();
        StrSupAgreement.Id =  Doc.Id.ToString();
          StrSupAgreement.Name =  Doc.Name != null ? Doc.Name.ToString()  : "";
          StrSupAgreement.Created =  Doc.Created != null ? Doc.Created.Value.ToString()  : "";
          StrSupAgreement.TotalAmount =  Doc.TotalAmount != null ? Doc.TotalAmount.Value.ToString()  : "";
          StrSupAgreement.ValidFrom =  Doc.ValidFrom != null ? Doc.ValidFrom.Value.ToString()  : "";
          StrSupAgreement.ValidTill =  Doc.ValidTill != null ? Doc.ValidTill.Value.ToString()  : "";
          StrSupAgreement.SDSFSberDevSDev =  Doc.SDSFSberDev != null ? Doc.SDSFSberDev.ToString()  : "";
          StrSupAgreement.SRSberDevSDev =  Doc.SRSberDev != null ? Doc.SRSberDev.ToString()  : "";
          StrSupAgreement.GoogleDocsLinkSberDevSDev =  Doc.GoogleDocsLinkSberDev != null ? Doc.GoogleDocsLinkSberDev.ToString()  : "";
          StrSupAgreement.SubjectSpecificationSberDevSDev =  Doc.SubjectSpecificationSberDev != null ? Doc.SubjectSpecificationSberDev.ToString()  : "";
          StrSupAgreement.AccArtExBaseSberDev =  Doc.AccArtExBaseSberDev != null ? Doc.AccArtExBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrBaseSberDev =  Doc.AccArtPrBaseSberDev != null ? Doc.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.Assignee =  Doc.Assignee != null ? Doc.Assignee.Id.ToString()  : "";
          StrSupAgreement.AssociatedApplication =  Doc.AssociatedApplication != null ? Doc.AssociatedApplication.Id.ToString()  : "";
          StrSupAgreement.Author =  Doc.Author != null ? Doc.Author.Id.ToString()  : "";
          StrSupAgreement.BudItemBaseSberDev =  Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.BusinessUnit =  Doc.BusinessUnit != null ? Doc.BusinessUnit.Id.ToString()  : "";
          StrSupAgreement.CalculationBaseSberDev = "";
          StrSupAgreement.CaseFile =  Doc.CaseFile != null ? Doc.CaseFile.Id.ToString()  : "";
          StrSupAgreement.Contact =  Doc.Contact != null ? Doc.Contact.Id.ToString()  : "";
          StrSupAgreement.Counterparty =  Doc.Counterparty != null ? Doc.Counterparty.Id.ToString()  : "";
          StrSupAgreement.CounterpartySignatory =  Doc.CounterpartySignatory != null ? Doc.CounterpartySignatory.Id.ToString()  : "";
          StrSupAgreement.Currency =  Doc.Currency != null ? Doc.Currency.Id.ToString()  : "";
          StrSupAgreement.DeliveredTo =  Doc.DeliveredTo != null ? Doc.DeliveredTo.Id.ToString()  : "";
          StrSupAgreement.DeliveryMethod =  Doc.DeliveryMethod != null ? Doc.DeliveryMethod.Id.ToString()  : "";
          StrSupAgreement.Department =  Doc.Department != null ? Doc.Department.Id.ToString()  : "";
          StrSupAgreement.DirectionMVZ = "";
          StrSupAgreement.DocumentGroup =  Doc.DocumentGroup != null ? Doc.DocumentGroup.Id.ToString()  : "";
          StrSupAgreement.DocumentKind =  Doc.DocumentKind != null ? Doc.DocumentKind.Id.ToString()  : "";
          StrSupAgreement.DocumentRegister =  Doc.DocumentRegister != null ? Doc.DocumentRegister.Id.ToString()  : "";
          StrSupAgreement.LeadingDocument =  Doc.LeadingDocument != null ? Doc.LeadingDocument.Id.ToString()  : "";
          StrSupAgreement.MarketDirectSberDev =  Doc.MarketDirectSberDev != null ? Doc.MarketDirectSberDev.Id.ToString()  : "";
          StrSupAgreement.Milestones = "";
          StrSupAgreement.MVPBaseSberDev =  Doc.MVPBaseSberDev != null ? Doc.MVPBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZBaseSberDev =  Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.OurSignatory =  Doc.OurSignatory != null ? Doc.OurSignatory.Id.ToString()  : "";
          StrSupAgreement.OurSigningReason =  Doc.OurSigningReason != null ? Doc.OurSigningReason.Id.ToString()  : "";
          StrSupAgreement.Parameters = "";
          StrSupAgreement.PreparedBy =  Doc.PreparedBy != null ? Doc.PreparedBy.Id.ToString()  : "";
          StrSupAgreement.ProdCollectionExBaseSberDev =  "";
          StrSupAgreement.ProdCollectionPrBaseSberDev =  "";
          StrSupAgreement.ProdSberDevCollection =  "";
          StrSupAgreement.Project =  Doc.Project != null ? Doc.Project.Id.ToString()  : "";
          StrSupAgreement.ResponsibleEmployee =  Doc.ResponsibleEmployee != null ? Doc.ResponsibleEmployee.Id.ToString()  : "";
          StrSupAgreement.ResponsibleForReturnEmployee =  Doc.ResponsibleForReturnEmployee != null ? Doc.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrSupAgreement.Subtopic =  Doc.Subtopic != null ? Doc.Subtopic.Id.ToString()  : "";
          StrSupAgreement.Topic =  Doc.Topic != null ? Doc.Topic.Id.ToString()  : "";
          StrSupAgreement.Tracking = "";
          StrSupAgreement.VatRate =  Doc.VatRate != null ? Doc.VatRate.Id.ToString()  : "";
          StrSupAgreement.Versions =  "";
          StrSupAgreement.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/265f2c57-6a8a-4a15-833b-ca00e8047fa5/" + Doc.Id.ToString();
        RequestElem.Add(StrSupAgreement);
        return RequestElem;
      }
    }

    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
    [Public(WebApiRequestType = RequestType.Get)]
    public List<Structures.Module.IStructContract> Contracts(int iddoc) 
    {
      var Contracts = sberdev.SBContracts.Contracts.GetAll();
      List<Structures.Module.IStructContract> RequestElem = new List<Sungero.POST.Structures.Module.IStructContract>(); //Structures.Module.StructContractList.Create();
      if (iddoc == 0)
      {
        foreach (var Dog in Contracts)
        {
          var StrContract = Structures.Module.StructContract.Create();
          StrContract.ID = Dog.Id.ToString(); 
          StrContract.Name = Dog.Name; 
          StrContract.Created = Dog.Created.Value.ToString();
          StrContract.DocNum = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString()  : "";
          StrContract.DocDate = Dog.RegistrationDate != null ? Dog.RegistrationDate.Value.ToString() : "";
          StrContract.TotalAmount = Dog.TotalAmount != null ? Dog.TotalAmount.ToString() : "";
          StrContract.ValidFrom = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString() : "";
          StrContract.ValidTill = Dog.ValidTill != null ? Dog.ValidTill.Value.ToString() : "";
          StrContract.AccArtExBaseSberDev = Dog.AccArtExBaseSberDev != null ? Dog.AccArtExBaseSberDev.Id.ToString()  : "";
          StrContract.AccArtPrBaseSberDev = Dog.AccArtPrBaseSberDev != null ? Dog.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrContract.Assignee = Dog.Assignee != null ? Dog.Assignee.Id.ToString()  : "";
          StrContract.AssociatedApplication = Dog.AssociatedApplication != null ? Dog.AssociatedApplication.Id.ToString()  : "";
          StrContract.Author = Dog.Author != null ? Dog.Author.Id.ToString()  : "";
          StrContract.BudItemBaseSberDev = Dog.BudItemBaseSberDev != null ? Dog.BudItemBaseSberDev.Id.ToString()  : "";
          StrContract.BusinessUnit = Dog.BusinessUnit != null ? Dog.BusinessUnit.Id.ToString()  : "";
          StrContract.CalculationBaseSberDev = "";
          StrContract.CaseFile = Dog.CaseFile != null ? Dog.CaseFile.Id.ToString()  : "";
          StrContract.CollectionProperty = "";
          StrContract.Contact = Dog.Contact != null ? Dog.Contact.Id.ToString()  : "";
          StrContract.Counterparty = Dog.Counterparty != null ? Dog.Counterparty.Id.ToString()  : "";
          StrContract.CounterpartySignatory = Dog.CounterpartySignatory != null ? Dog.CounterpartySignatory.Id.ToString()  : "";
          StrContract.Currency = Dog.Currency != null ? Dog.Currency.Id.ToString()  : "";
          StrContract.DeliveredTo = Dog.DeliveredTo != null ? Dog.DeliveredTo.Id.ToString()  : "";
          StrContract.DeliveryMethod = Dog.DeliveryMethod != null ? Dog.DeliveryMethod.Id.ToString()  : "";
          StrContract.Department = Dog.Department != null ? Dog.Department.Id.ToString()  : "";
          StrContract.DocumentGroup = Dog.DocumentGroup != null ? Dog.DocumentGroup.Id.ToString()  : "";
          StrContract.DocumentKind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString()  : "";
          StrContract.DocumentRegister = Dog.DocumentRegister != null ? Dog.DocumentRegister.Id.ToString()  : "";
          StrContract.LeadingDocument = Dog.LeadingDocument != null ? Dog.LeadingDocument.Id.ToString()  : "";
          StrContract.MarketDirectSberDev = Dog.MarketDirectSberDev != null ? Dog.MarketDirectSberDev.Id.ToString()  : "";
          StrContract.Milestones = "";
          StrContract.MVPBaseSberDev = Dog.MVPBaseSberDev != null ? Dog.MVPBaseSberDev.Id.ToString()  : "";
          StrContract.MVZBaseSberDev = Dog.MVZBaseSberDev != null ? Dog.MVZBaseSberDev.Id.ToString()  : "";
          StrContract.OurSignatory = Dog.OurSignatory != null ? Dog.OurSignatory.Id.ToString()  : "";
          StrContract.OurSigningReason = Dog.OurSigningReason != null ? Dog.OurSigningReason.Id.ToString()  : "";
          StrContract.Parameters = "";
          StrContract.PreparedBy = Dog.PreparedBy != null ? Dog.PreparedBy.Id.ToString()  : "";
          StrContract.ProdCollectionExBaseSberDev = "";
          StrContract.ProdCollectionPrBaseSberDev = "";
          StrContract.Project = Dog.Project != null ? Dog.Project.Id.ToString()  : "";
          StrContract.ResponsibleEmployee = Dog.ResponsibleEmployee != null ? Dog.ResponsibleEmployee.Id.ToString()  : "";
          StrContract.ResponsibleForReturnEmployee = Dog.ResponsibleForReturnEmployee != null ? Dog.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrContract.Subtopic = Dog.Subtopic != null ? Dog.Subtopic.Id.ToString()  : "";
          StrContract.Topic = Dog.Topic != null ? Dog.Topic.Id.ToString()  : "";
          StrContract.Tracking = "";
          StrContract.VatRate = Dog.VatRate != null ? Dog.VatRate.Id.ToString()  : "";
          StrContract.Versions = "";
          StrContract.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/f37c7e63-b134-4446-9b5b-f8811f6c9666/" + Dog.Id.ToString();
          
          RequestElem.Add(StrContract);
        }
        return RequestElem;
      }
      else
      {
        var Dog = sberdev.SBContracts.Contracts.Get(long.Parse(iddoc.ToString()));
        var StrContract = Structures.Module.StructContract.Create();
          StrContract.ID = Dog.Id.ToString(); 
          StrContract.Name = Dog.Name; 
          StrContract.Created = Dog.Created.Value.ToString();
          StrContract.DocNum = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString()  : "";
          StrContract.DocDate = Dog.RegistrationDate != null ? Dog.RegistrationDate.Value.ToString() : "";
          StrContract.TotalAmount = Dog.TotalAmount != null ? Dog.TotalAmount.ToString() : "";
          StrContract.ValidFrom = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString() : "";
          StrContract.ValidTill = Dog.ValidTill != null ? Dog.ValidTill.Value.ToString() : "";
          StrContract.AccArtPrBaseSberDev = Dog.AccArtPrBaseSberDev != null ? Dog.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrContract.Assignee = Dog.Assignee != null ? Dog.Assignee.Id.ToString()  : "";
          StrContract.AssociatedApplication = Dog.AssociatedApplication != null ? Dog.AssociatedApplication.Id.ToString()  : "";
          StrContract.Author = Dog.Author != null ? Dog.Author.Id.ToString()  : "";
          StrContract.BudItemBaseSberDev = Dog.BudItemBaseSberDev != null ? Dog.BudItemBaseSberDev.Id.ToString()  : "";
          StrContract.BusinessUnit = Dog.BusinessUnit != null ? Dog.BusinessUnit.Id.ToString()  : "";
          StrContract.CalculationBaseSberDev = "";
          StrContract.CaseFile = Dog.CaseFile != null ? Dog.CaseFile.Id.ToString()  : "";
          StrContract.CollectionProperty = "";
          StrContract.Contact = Dog.Contact != null ? Dog.Contact.Id.ToString()  : "";
          StrContract.Counterparty = Dog.Counterparty != null ? Dog.Counterparty.Id.ToString()  : "";
          StrContract.CounterpartySignatory = Dog.CounterpartySignatory != null ? Dog.CounterpartySignatory.Id.ToString()  : "";
          StrContract.Currency = Dog.Currency != null ? Dog.Currency.Id.ToString()  : "";
          StrContract.DeliveredTo = Dog.DeliveredTo != null ? Dog.DeliveredTo.Id.ToString()  : "";
          StrContract.DeliveryMethod = Dog.DeliveryMethod != null ? Dog.DeliveryMethod.Id.ToString()  : "";
          StrContract.Department = Dog.Department != null ? Dog.Department.Id.ToString()  : "";
          StrContract.DocumentGroup = Dog.DocumentGroup != null ? Dog.DocumentGroup.Id.ToString()  : "";
          StrContract.DocumentKind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString()  : "";
          StrContract.DocumentRegister = Dog.DocumentRegister != null ? Dog.DocumentRegister.Id.ToString()  : "";
          StrContract.LeadingDocument = Dog.LeadingDocument != null ? Dog.LeadingDocument.Id.ToString()  : "";
          StrContract.MarketDirectSberDev = Dog.MarketDirectSberDev != null ? Dog.MarketDirectSberDev.Id.ToString()  : "";
          StrContract.Milestones = "";
          StrContract.MVPBaseSberDev = Dog.MVPBaseSberDev != null ? Dog.MVPBaseSberDev.Id.ToString()  : "";
          StrContract.MVZBaseSberDev = Dog.MVZBaseSberDev != null ? Dog.MVZBaseSberDev.Id.ToString()  : "";
          StrContract.OurSignatory = Dog.OurSignatory != null ? Dog.OurSignatory.Id.ToString()  : "";
          StrContract.OurSigningReason = Dog.OurSigningReason != null ? Dog.OurSigningReason.Id.ToString()  : "";
          StrContract.Parameters = "";
          StrContract.PreparedBy = Dog.PreparedBy != null ? Dog.PreparedBy.Id.ToString()  : "";
          StrContract.ProdCollectionExBaseSberDev = "";
          StrContract.ProdCollectionPrBaseSberDev = "";
          StrContract.Project = Dog.Project != null ? Dog.Project.Id.ToString()  : "";
          StrContract.ResponsibleEmployee = Dog.ResponsibleEmployee != null ? Dog.ResponsibleEmployee.Id.ToString()  : "";
          StrContract.ResponsibleForReturnEmployee = Dog.ResponsibleForReturnEmployee != null ? Dog.ResponsibleForReturnEmployee.Id.ToString()  : "";
          StrContract.Subtopic = Dog.Subtopic != null ? Dog.Subtopic.Id.ToString()  : "";
          StrContract.Topic = Dog.Topic != null ? Dog.Topic.Id.ToString()  : "";
          StrContract.Tracking = "";
          StrContract.VatRate = Dog.VatRate != null ? Dog.VatRate.Id.ToString()  : "";
          StrContract.Versions = "";
          StrContract.Link = @"https://directum.sberdevices.ru/DrxWeb/#/card/f37c7e63-b134-4446-9b5b-f8811f6c9666/" + Dog.Id.ToString();

          RequestElem.Add(StrContract);
          return RequestElem;
      }
    }

    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
   [Public(WebApiRequestType = RequestType.Post)]
    public List<Sungero.POST.Structures.Module.IMarcetingDoc> PostMarcetingDocuments(string IDDoc)
    {
      if (IDDoc != "")
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        POST.Structures.Module.IMarcetingDoc Struct = POST.Structures.Module.MarcetingDoc.Create();
        var Doc = Sungero.Custom.MarcetingDocs.GetAll(d => d.Id == int.Parse(IDDoc)).FirstOrDefault();
        if (Doc != null)
        {
          Struct.Name = Doc.Name.ToString();
          Struct.Kind = Doc.DocumentKind.ToString();
          Struct.ActionCode = Doc.ActionCode.Value.ToString();
          Struct.ActionName = Doc.ActionName.ToString();
          Struct.BudgetItem = Doc.BudgetItem.DisplayValue.ToString();
          Struct.StartDate = Doc.StartDate.Value.ToString();
          Struct.EndDate = Doc.EndDate.Value.ToString();
          Struct.MarketingKind = Doc.MarketingKind.DisplayValue.ToString();
          Struct.MarketingSubKind = Doc.MarketingSubKind.DisplayValue.ToString();
          Struct.OwnerEmployee = Doc.OwnerEmployee.DisplayValue.ToString();
          Struct.PlanKolInt = Doc.PlanKolInt.Value.ToString();
          Struct.PlannedRashod = Doc.PlannedRashod.Value.ToString();
          Struct.PlannedRashodCurrency = Doc.PlanRashCurrency != null ? Doc.PlanRashCurrency.NumericCode.ToString() : "";
          Struct.PlannedSumm = Doc.PlannedSummRub.Value.ToString();
          Struct.PlannedSummCurrency = Doc.PlanSummCurrency.NumericCode.ToString();
          Struct.FacktKolInt = Doc.FacktKolInt.HasValue ? Doc.FacktKolInt.Value.ToString() : "";
          Struct.FacktRashod = Doc.FacktRashod.HasValue ? Doc.FacktRashod.Value.ToString() : "";
          Struct.FacktRashodCurrency = Doc.FacktRashCurrency != null ? Doc.FacktRashCurrency.NumericCode.ToString() : "";
          Struct.FacktTotalSumm = Doc.FacktTotalSumm.HasValue ? Doc.FacktTotalSumm.Value.ToString() : "";
          Struct.FacktTotalSummCurrency = Doc.FacktSummCurrency != null ? Doc.FacktSummCurrency.NumericCode.ToString() : "";
          Struct.StagesOfApproval = Doc.StagesOfApproval != null ? Doc.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();
            newstr.PricePromo = elem.PricePromo.Value.ToString();
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();
            DevicesAction.Add(newstr);
          }          
          Struct.DevicesAction = DevicesAction;
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();
          foreach (var elem in Doc.Channels)
          {
            var newchan = Structures.Module.Channels.Create();
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newchan.Channel = elem.Channel.DisplayValue.ToString();
            newchan.Komission = elem.Komission.Value.ToString();
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();
            Channels.Add(newchan);
          }
          Struct.Channels = Channels;
          ListStruct.Add(Struct);
          return ListStruct;
        }
        else // Выдача полного списка документов
        {
          throw new WebException("Выдача полного списка документов");
        }
      }
      else
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct2 = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        var Docs = Sungero.Custom.MarcetingDocs.GetAll().ToList();
        foreach (var Doc2 in Docs)
        {
          Logger.Debug("Прошли 3-1");
          POST.Structures.Module.IMarcetingDoc Struct2 = POST.Structures.Module.MarcetingDoc.Create();
          Struct2.Name = Doc2.Name.ToString();
          Struct2.Kind = Doc2.DocumentKind.ToString();
          Struct2.ActionCode = Doc2.ActionCode.Value.ToString();
          Struct2.ActionName = Doc2.ActionName.ToString();
          Struct2.BudgetItem = Doc2.BudgetItem.DisplayValue.ToString();
          Struct2.StartDate = Doc2.StartDate.Value.ToString();
          Struct2.EndDate = Doc2.EndDate.Value.ToString();
          Struct2.MarketingKind = Doc2.MarketingKind.DisplayValue.ToString();
          Struct2.MarketingSubKind = Doc2.MarketingSubKind.DisplayValue.ToString();
          Struct2.OwnerEmployee = Doc2.OwnerEmployee.DisplayValue.ToString();
          Struct2.PlanKolInt = Doc2.PlanKolInt.Value.ToString();
          Struct2.PlannedRashod = Doc2.PlannedRashod.Value.ToString();
          Struct2.PlannedRashodCurrency = Doc2.PlanRashCurrency != null ? Doc2.PlanRashCurrency.NumericCode.ToString() : "";
          Struct2.PlannedSumm = Doc2.PlannedSummRub.Value.ToString();
          Struct2.PlannedSummCurrency = Doc2.PlanSummCurrency.NumericCode.ToString();
          Struct2.FacktKolInt = Doc2.FacktKolInt.HasValue ? Doc2.FacktKolInt.Value.ToString() : "";
          Struct2.FacktRashod = Doc2.FacktRashod.HasValue ? Doc2.FacktRashod.Value.ToString() : "";
          Struct2.FacktRashodCurrency = Doc2.FacktRashCurrency != null ? Doc2.FacktRashCurrency.NumericCode.ToString() : "";
          Struct2.FacktTotalSumm = Doc2.FacktTotalSumm.HasValue ? Doc2.FacktTotalSumm.Value.ToString() : "";
          Struct2.FacktTotalSummCurrency = Doc2.FacktSummCurrency != null ? Doc2.FacktSummCurrency.NumericCode.ToString() : "";
          Struct2.StagesOfApproval = Doc2.StagesOfApproval != null ? Doc2.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc2.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();                        
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();           
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();                   
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();                    
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();   
            newstr.PricePromo = elem.PricePromo.Value.ToString();                           
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();      
            DevicesAction.Add(newstr);                                                  
          }          
          Struct2.DevicesAction = DevicesAction;                                           
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();     
          foreach (var elem in Doc2.Channels)
          {
                                                                                          
            var newchan = Structures.Module.Channels.Create();                             
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();          
            newchan.Channel = elem.Channel.DisplayValue.ToString();                  
            newchan.Komission = elem.Komission.Value.ToString();                     
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();  
            Channels.Add(newchan);                                                         
          }                                                                                                                                                                        
          Struct2.Channels = Channels;                                                  
          ListStruct2.Add(Struct2);                                                     
        }
        return ListStruct2;
      } 
    }
    
    
    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
   [Public(WebApiRequestType = RequestType.Get)]
    public string GetClosedDocs()
    {
      var listDoc = sberdev.SBContracts.AccountingDocumentBases.GetAll(d => d.MarketingSberDev != null).ToList();
      string spisok = "";
      if (listDoc.Count > 0)
      {
        foreach (var str in listDoc)
        {
          spisok += str.Id.ToString() + ";";
        }
      }
      return spisok;
    }
    
    
    /// <summary>
    /// Получение Маркетингового документа
    /// </summary>
    /// <param name="IDDoc">Тестовая строка</param>
   [Public(WebApiRequestType = RequestType.Get)]
    public List<Sungero.POST.Structures.Module.IMarcetingDoc> GetMarcetingDocuments(int IDDoc)
    {
      if (IDDoc != 0)
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        POST.Structures.Module.IMarcetingDoc Struct = POST.Structures.Module.MarcetingDoc.Create();
        int idd = IDDoc;
        var Doc = Sungero.Custom.MarcetingDocs.GetAll(d => d.Id == idd).FirstOrDefault();                
        if (Doc != null)
        {
          Struct.Name = Doc.Name.ToString();
          Struct.Kind = Doc.DocumentKind.ToString();
          Struct.ActionCode = Doc.ActionCode.Value.ToString();
          Struct.ActionName = Doc.ActionName.ToString();
          Struct.BudgetItem = Doc.BudgetItem.DisplayValue.ToString();
          Struct.StartDate = Doc.StartDate.Value.ToString();
          Struct.EndDate = Doc.EndDate.Value.ToString();
          Struct.MarketingKind = Doc.MarketingKind.DisplayValue.ToString();
          Struct.MarketingSubKind = Doc.MarketingSubKind.DisplayValue.ToString();
          Struct.OwnerEmployee = Doc.OwnerEmployee.DisplayValue.ToString();
          Struct.PlanKolInt = Doc.PlanKolInt.Value.ToString();
          Struct.PlannedRashod = Doc.PlannedRashod.Value.ToString();
          Struct.PlannedRashodCurrency = Doc.PlanRashCurrency != null ? Doc.PlanRashCurrency.NumericCode.ToString() : "";
          Struct.PlannedSumm = Doc.PlannedSummRub.Value.ToString();
          Struct.PlannedSummCurrency = Doc.PlanSummCurrency.NumericCode.ToString();
          Struct.FacktKolInt = Doc.FacktKolInt.HasValue ? Doc.FacktKolInt.Value.ToString() : "";
          Struct.FacktRashod = Doc.FacktRashod.HasValue ? Doc.FacktRashod.Value.ToString() : "";
          Struct.FacktRashodCurrency = Doc.FacktRashCurrency != null ? Doc.FacktRashCurrency.NumericCode.ToString() : "";
          Struct.FacktTotalSumm = Doc.FacktTotalSumm.HasValue ? Doc.FacktTotalSumm.Value.ToString() : "";
          Struct.FacktTotalSummCurrency = Doc.FacktSummCurrency != null ? Doc.FacktSummCurrency.NumericCode.ToString() : "";
          Struct.StagesOfApproval = Doc.StagesOfApproval != null ? Doc.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();
            newstr.PricePromo = elem.PricePromo.Value.ToString();
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();
            DevicesAction.Add(newstr);
          }          
          Struct.DevicesAction = DevicesAction;
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();
          foreach (var elem in Doc.Channels)
          {
            var newchan = Structures.Module.Channels.Create();
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();
            newchan.Channel = elem.Channel.DisplayValue.ToString();
            newchan.Komission = elem.Komission.Value.ToString();
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();
            Channels.Add(newchan);
          }
          Struct.Channels = Channels;
          ListStruct.Add(Struct);
          return ListStruct;
        }
        else // Выдача полного списка документов
        {
          return null;
        }
      }
      else
      {
        List<Sungero.POST.Structures.Module.IMarcetingDoc> ListStruct2 = new List<Sungero.POST.Structures.Module.IMarcetingDoc>();
        var Docs = Sungero.Custom.MarcetingDocs.GetAll().ToList();
        foreach (var Doc2 in Docs)
        {
          POST.Structures.Module.IMarcetingDoc Struct2 = POST.Structures.Module.MarcetingDoc.Create();
          Struct2.Name = Doc2.Name.ToString();
          Struct2.Kind = Doc2.DocumentKind.ToString();
          Struct2.ActionCode = Doc2.ActionCode.Value.ToString();
          Struct2.ActionName = Doc2.ActionName.ToString();
          Struct2.BudgetItem = Doc2.BudgetItem.DisplayValue.ToString();
          Struct2.StartDate = Doc2.StartDate.Value.ToString();
          Struct2.EndDate = Doc2.EndDate.Value.ToString();
          Struct2.MarketingKind = Doc2.MarketingKind.DisplayValue.ToString();
          Struct2.MarketingSubKind = Doc2.MarketingSubKind.DisplayValue.ToString();
          Struct2.OwnerEmployee = Doc2.OwnerEmployee.DisplayValue.ToString();
          Struct2.PlanKolInt = Doc2.PlanKolInt.Value.ToString();
          Struct2.PlannedRashod = Doc2.PlannedRashod.Value.ToString();
          Struct2.PlannedRashodCurrency = Doc2.PlanRashCurrency != null ? Doc2.PlanRashCurrency.NumericCode.ToString() : "";
          Struct2.PlannedSumm = Doc2.PlannedSummRub.Value.ToString();
          Struct2.PlannedSummCurrency = Doc2.PlanSummCurrency.NumericCode.ToString();
          Struct2.FacktKolInt = Doc2.FacktKolInt.HasValue ? Doc2.FacktKolInt.Value.ToString() : "";
          Struct2.FacktRashod = Doc2.FacktRashod.HasValue ? Doc2.FacktRashod.Value.ToString() : "";
          Struct2.FacktRashodCurrency = Doc2.FacktRashCurrency != null ? Doc2.FacktRashCurrency.NumericCode.ToString() : "";
          Struct2.FacktTotalSumm = Doc2.FacktTotalSumm.HasValue ? Doc2.FacktTotalSumm.Value.ToString() : "";
          Struct2.FacktTotalSummCurrency = Doc2.FacktSummCurrency != null ? Doc2.FacktSummCurrency.NumericCode.ToString() : "";
          Struct2.StagesOfApproval = Doc2.StagesOfApproval != null ? Doc2.StagesOfApproval.Value.ToString() : "";
          List<Sungero.POST.Structures.Module.IDevicesAction> DevicesAction = new List<Sungero.POST.Structures.Module.IDevicesAction>();
          foreach (var elem in Doc2.DevicesAction)
          {
            var newstr = Structures.Module.DevicesAction.Create();                        
            newstr.Product = elem.ProductsAndDevices.DisplayValue.ToString();           
            newstr.KolVoDevices = elem.KolVoDevices.Value.ToString();                   
            newstr.PriceNoPromo = elem.PriceNoPromo.Value.ToString();                    
            newstr.PriceNoPromoCurrency = elem.PriceNoPrCurrency.NumericCode.ToString();   
            newstr.PricePromo = elem.PricePromo.Value.ToString();                           
            newstr.PricePromoCurrency = elem.PromoCurrency.NumericCode.ToString();      
            DevicesAction.Add(newstr);                                                  
          }          
          Struct2.DevicesAction = DevicesAction;                                           
          List<Sungero.POST.Structures.Module.IChannels> Channels = new List<Sungero.POST.Structures.Module.IChannels>();     
          foreach (var elem in Doc2.Channels)
          {
                                                                                          
            var newchan = Structures.Module.Channels.Create();                             
            newchan.Product = elem.ProductsAndDevices.DisplayValue.ToString();          
            newchan.Channel = elem.Channel.DisplayValue.ToString();                  
            newchan.Komission = elem.Komission.Value.ToString();                     
            newchan.KomissionCurrency = elem.KommissCurrency.NumericCode.ToString();  
            Channels.Add(newchan);                                                         
          }                                                                                                                                                                        
          Struct2.Channels = Channels;                                                  
          ListStruct2.Add(Struct2);                                                     
        }
        return ListStruct2;
      } 
    }
    // <summary>
    /// Тестовая функция для проверки коннекта
    /// </summary>
    /// <param name="inputstring">Тестовая строка</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public POST.Structures.Module.IBaseStruct test(string inputstring)
    {
      POST.Structures.Module.IBaseStruct structur = POST.Structures.Module.BaseStruct.Create();
      structur.Name = "Результат";
      structur.Value = "Вы прислали строку: " + inputstring;
      return structur;
    }
    
    private Structures.Module.IBaseStruct AddPS(string name, string evalue)
    {
      var elem = Structures.Module.BaseStruct.Create();    
      elem.Name = name;
      elem.Value = evalue;
      return elem;
    }
    
    #region Выгрузка договора - пакет
    /// <summary>
    /// Выгрузка договора для 1С
    /// </summary>
    /// <param name="GUIDRX">ID Договора</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public Structures.Module.IFormatRequest loadDGD(string GUIDRX)
    {
      int IDCounter = int.Parse(GUIDRX);
      Logger.Debug("ЗАПРОС loadDGD: Начинается поиск документа с ИД = " + IDCounter.ToString() + ".");
      var Dog = Sungero.Contracts.Contracts.GetAll(d => d.Id == IDCounter).FirstOrDefault();
      if (Dog != null)
      {
        var list = new List<Structures.Module.IBaseStruct>();
        var Struct = Structures.Module.ContractRequest.Create();
            Struct.GUIDRX = Dog.Id.ToString();                       //" GUID "
            list.Add(AddPS("GUIDRX",Struct.GUIDRX));            
            Struct.Kind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString() : "Нет";            //" GUID "
            list.Add(AddPS("Kind",Struct.Kind));
            Struct.Name = Dog.Name != null ? Dog.Name.ToString() : "Нет";                       //"текст"	Наименование
            list.Add(AddPS("Name",Struct.Name));
            Struct.Counter = Dog.Counterparty != null ? Dog.Counterparty.Id.ToString() : "Нет";         //"GUID"	Контрагент
            list.Add(AddPS("Counter",Struct.Counter));
            Struct.Buyer = Dog.CounterpartySignatory != null ? Dog.CounterpartySignatory.Id.ToString() : "Нет";  //"GUID"	Покупатель
            list.Add(AddPS("Buyer",Struct.Buyer));
            Struct.NOR = Dog.BusinessUnit != null ? Dog.BusinessUnit.Id.ToString() : "Нет";             //"GUID"	Организация
            list.Add(AddPS("NOR",Struct.NOR));
            Struct.Departament = Dog.Department != null ? Dog.Department.Id.ToString() : "Нет";       //"GUID"	Подразделение
            list.Add(AddPS("Departament",Struct.Departament));
            Struct.Signatory = Dog.OurSignatory != null ? Dog.OurSignatory.Id.ToString() : "Нет";       //"GUID"	Подписал
            list.Add(AddPS("Signatory",Struct.Signatory));
            Struct.Summ = Dog.TotalAmount != null ? Dog.TotalAmount.ToString() : "0";                //"100,00"	Сумма договора фиксирована
            list.Add(AddPS("Summ",Struct.Summ));
            Struct.Currency = Dog.Currency != null ? Dog.Currency.NumericCode.ToString() : "Нет";   //"643"	Валюта
            list.Add(AddPS("Currency",Struct.Currency));
            Struct.Number = Dog.RegistrationNumber != null ? Dog.RegistrationNumber.ToString() : "Нет";          //"2023-СИМ-2233_NEW"	Номер договора
            list.Add(AddPS("Number",Struct.Number));
            Struct.Date = Dog.RegistrationDate != null ? Dog.RegistrationDate.ToString() : "Нет";            //"29.01.2022 0:00:00"	Дата заключения договора
            list.Add(AddPS("Date",Struct.Date));
            string LCS = Dog.LifeCycleState.ToString();
            if (LCS == "Active")
              LCS = "Действует";
            else
            {
              if (LCS == "Closed")
                LCS = "Закрыт";
              else
                LCS = "Не согласован";
            } 
            Struct.Status = LCS;          //"__________"	Статус договора
            list.Add(AddPS("Status",Struct.Status));
            Struct.ValidC = Dog.ValidFrom != null ? Dog.ValidFrom.ToString() : "Нет";          //"09.08.2023 0:00:00"	Действует с
            list.Add(AddPS("ValidC",Struct.ValidC));
            Struct.ValidPO = Dog.ValidTill != null ? Dog.ValidTill.ToString() : "Нет";         //"31.08.2023 0:00:00"	По
            list.Add(AddPS("ValidPO",Struct.ValidPO));
            
      
        var FormatRequest = Structures.Module.FormatRequest.Create();
        FormatRequest.Function = "Create";
        FormatRequest.Sysname = "RX";    
        FormatRequest.Entity = "Contract";       
        FormatRequest.Param = list;
        return FormatRequest;
      }
      else
      {
        Logger.Debug("ЗАПРОС loadDGD: Договор с указанным GUIDID (" + GUIDRX + ") не найден!");
        var FormatRequest = Structures.Module.FormatRequest.Create();
        FormatRequest.Function = "ERROR";
        FormatRequest.Sysname = "RX";    
        FormatRequest.Entity = "Contract"; 
        var list = new List<Structures.Module.IBaseStruct>();        
        FormatRequest.Param = list;
        list.Add(AddPS("Error","Договор с указанным GUIDID не найден!"));
        return FormatRequest;
        //throw new WebException(error.Value);
      } 
    }
    #endregion
  }
}