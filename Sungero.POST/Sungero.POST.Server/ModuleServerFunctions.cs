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
    /// 
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
          StrSupAgreement.MVZOldSberDevSDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.ToString()  : "";
          StrSupAgreement.AccArtExOldSberDevSDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.ToString()  : "";
          StrSupAgreement.MVPOldSberDevSDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.ToString()  : "";
          StrSupAgreement.AccArtPrOldSberDevSDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.ToString()  : "";
          StrSupAgreement.BudItemOldSberDevSDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.ToString()  : "";
          StrSupAgreement.ContrTypeOldSberDevSDev =  Doc.ContrTypeOldSberDev != null ? Doc.ContrTypeOldSberDev.ToString()  : "";
          StrSupAgreement.OriginalOldSberDevSDev =  Doc.OriginalOldSberDev != null ? Doc.OriginalOldSberDev.ToString()  : "";
          StrSupAgreement.SigningSDev =  Doc.Signing != null ? Doc.Signing.Value.ToString()  : "";
          StrSupAgreement.DeliveryInfoOldSberDevSDev =  Doc.DeliveryInfoOldSberDev != null ? Doc.DeliveryInfoOldSberDev.ToString()  : "";
          StrSupAgreement.NoticeSendOldSberDevSDev =  Doc.NoticeSendOldSberDev != null ? Doc.NoticeSendOldSberDev.ToString()  : "";
          StrSupAgreement.FrameworkOldSberDevSDev =  Doc.FrameworkOldSberDev != null ? Doc.FrameworkOldSberDev.ToString()  : "";
          StrSupAgreement.SDSFSberDevSDev =  Doc.SDSFSberDev != null ? Doc.SDSFSberDev.ToString()  : "";
          StrSupAgreement.SRSberDevSDev =  Doc.SRSberDev != null ? Doc.SRSberDev.ToString()  : "";
          StrSupAgreement.GoogleDocsLinkSberDevSDev =  Doc.GoogleDocsLinkSberDev != null ? Doc.GoogleDocsLinkSberDev.ToString()  : "";
          StrSupAgreement.SubjectSpecificationSberDevSDev =  Doc.SubjectSpecificationSberDev != null ? Doc.SubjectSpecificationSberDev.ToString()  : "";
          StrSupAgreement.AccArtExBaseSberDev =  Doc.AccArtExBaseSberDev != null ? Doc.AccArtExBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtExOldSberDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrBaseSberDev =  Doc.AccArtPrBaseSberDev != null ? Doc.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrOldSberDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.Id.ToString()  : "";
          StrSupAgreement.Assignee =  Doc.Assignee != null ? Doc.Assignee.Id.ToString()  : "";
          StrSupAgreement.AssociatedApplication =  Doc.AssociatedApplication != null ? Doc.AssociatedApplication.Id.ToString()  : "";
          StrSupAgreement.Author =  Doc.Author != null ? Doc.Author.Id.ToString()  : "";
          StrSupAgreement.BudItemBaseSberDev =  Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.BudItemOldSberDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.Id.ToString()  : "";
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
          StrSupAgreement.MVPOldSberDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZBaseSberDev =  Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZOldSberDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.Id.ToString()  : "";
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
          StrSupAgreement.MVZOldSberDevSDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.ToString()  : "";
          StrSupAgreement.AccArtExOldSberDevSDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.ToString()  : "";
          StrSupAgreement.MVPOldSberDevSDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.ToString()  : "";
          StrSupAgreement.AccArtPrOldSberDevSDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.ToString()  : "";
          StrSupAgreement.BudItemOldSberDevSDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.ToString()  : "";
          StrSupAgreement.ContrTypeOldSberDevSDev =  Doc.ContrTypeOldSberDev != null ? Doc.ContrTypeOldSberDev.ToString()  : "";
          StrSupAgreement.OriginalOldSberDevSDev =  Doc.OriginalOldSberDev != null ? Doc.OriginalOldSberDev.ToString()  : "";
          StrSupAgreement.SigningSDev =  Doc.Signing != null ? Doc.Signing.Value.ToString()  : "";
          StrSupAgreement.DeliveryInfoOldSberDevSDev =  Doc.DeliveryInfoOldSberDev != null ? Doc.DeliveryInfoOldSberDev.ToString()  : "";
          StrSupAgreement.NoticeSendOldSberDevSDev =  Doc.NoticeSendOldSberDev != null ? Doc.NoticeSendOldSberDev.ToString()  : "";
          StrSupAgreement.FrameworkOldSberDevSDev =  Doc.FrameworkOldSberDev != null ? Doc.FrameworkOldSberDev.ToString()  : "";
          StrSupAgreement.SDSFSberDevSDev =  Doc.SDSFSberDev != null ? Doc.SDSFSberDev.ToString()  : "";
          StrSupAgreement.SRSberDevSDev =  Doc.SRSberDev != null ? Doc.SRSberDev.ToString()  : "";
          StrSupAgreement.GoogleDocsLinkSberDevSDev =  Doc.GoogleDocsLinkSberDev != null ? Doc.GoogleDocsLinkSberDev.ToString()  : "";
          StrSupAgreement.SubjectSpecificationSberDevSDev =  Doc.SubjectSpecificationSberDev != null ? Doc.SubjectSpecificationSberDev.ToString()  : "";
          StrSupAgreement.AccArtExBaseSberDev =  Doc.AccArtExBaseSberDev != null ? Doc.AccArtExBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtExOldSberDev =  Doc.AccArtExOldSberDev != null ? Doc.AccArtExOldSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrBaseSberDev =  Doc.AccArtPrBaseSberDev != null ? Doc.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.AccArtPrOldSberDev =  Doc.AccArtPrOldSberDev != null ? Doc.AccArtPrOldSberDev.Id.ToString()  : "";
          StrSupAgreement.Assignee =  Doc.Assignee != null ? Doc.Assignee.Id.ToString()  : "";
          StrSupAgreement.AssociatedApplication =  Doc.AssociatedApplication != null ? Doc.AssociatedApplication.Id.ToString()  : "";
          StrSupAgreement.Author =  Doc.Author != null ? Doc.Author.Id.ToString()  : "";
          StrSupAgreement.BudItemBaseSberDev =  Doc.BudItemBaseSberDev != null ? Doc.BudItemBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.BudItemOldSberDev =  Doc.BudItemOldSberDev != null ? Doc.BudItemOldSberDev.Id.ToString()  : "";
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
          StrSupAgreement.MVPOldSberDev =  Doc.MVPOldSberDev != null ? Doc.MVPOldSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZBaseSberDev =  Doc.MVZBaseSberDev != null ? Doc.MVZBaseSberDev.Id.ToString()  : "";
          StrSupAgreement.MVZOldSberDev =  Doc.MVZOldSberDev != null ? Doc.MVZOldSberDev.Id.ToString()  : "";
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
          StrContract.AccArtMVZOldSberDev = Dog.AccArtMVZOldSberDev != null ? Dog.AccArtMVZOldSberDev.Id.ToString()  : "";
          StrContract.AccArtPrBaseSberDev = Dog.AccArtPrBaseSberDev != null ? Dog.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrContract.AccArtsberdevOldSberDev = Dog.AccArtsberdevOldSberDev != null ? Dog.AccArtsberdevOldSberDev.Id.ToString()  : "";
          StrContract.Assignee = Dog.Assignee != null ? Dog.Assignee.Id.ToString()  : "";
          StrContract.AssociatedApplication = Dog.AssociatedApplication != null ? Dog.AssociatedApplication.Id.ToString()  : "";
          StrContract.Author = Dog.Author != null ? Dog.Author.Id.ToString()  : "";
          StrContract.BudItemBaseSberDev = Dog.BudItemBaseSberDev != null ? Dog.BudItemBaseSberDev.Id.ToString()  : "";
          StrContract.BudItemsberdevOldSberDev = Dog.BudItemsberdevOldSberDev != null ? Dog.BudItemsberdevOldSberDev.Id.ToString()  : "";
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
          StrContract.DirectionMVZ = Dog.DirectionMVZ != null ? Dog.DirectionMVZ.ToString()  : "";
          StrContract.DocumentGroup = Dog.DocumentGroup != null ? Dog.DocumentGroup.Id.ToString()  : "";
          StrContract.DocumentKind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString()  : "";
          StrContract.DocumentRegister = Dog.DocumentRegister != null ? Dog.DocumentRegister.Id.ToString()  : "";
          StrContract.LeadingDocument = Dog.LeadingDocument != null ? Dog.LeadingDocument.Id.ToString()  : "";
          StrContract.MarketDirectSberDev = Dog.MarketDirectSberDev != null ? Dog.MarketDirectSberDev.Id.ToString()  : "";
          StrContract.Milestones = "";
          StrContract.MVPBaseSberDev = Dog.MVPBaseSberDev != null ? Dog.MVPBaseSberDev.Id.ToString()  : "";
          StrContract.MVPsberdevOldSberDev = Dog.MVPsberdevOldSberDev != null ? Dog.MVPsberdevOldSberDev.Id.ToString()  : "";
          StrContract.MVZBaseSberDev = Dog.MVZBaseSberDev != null ? Dog.MVZBaseSberDev.Id.ToString()  : "";
          StrContract.MVZsberdevOldSberDev = Dog.MVZsberdevOldSberDev != null ? Dog.MVZsberdevOldSberDev.Id.ToString()  : "";
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
          StrContract.AccArtExBaseSberDev = Dog.AccArtExBaseSberDev != null ? Dog.AccArtExBaseSberDev.Id.ToString()  : "";
          StrContract.AccArtMVZOldSberDev = Dog.AccArtMVZOldSberDev != null ? Dog.AccArtMVZOldSberDev.Id.ToString()  : "";
          StrContract.AccArtPrBaseSberDev = Dog.AccArtPrBaseSberDev != null ? Dog.AccArtPrBaseSberDev.Id.ToString()  : "";
          StrContract.AccArtsberdevOldSberDev = Dog.AccArtsberdevOldSberDev != null ? Dog.AccArtsberdevOldSberDev.Id.ToString()  : "";
          StrContract.Assignee = Dog.Assignee != null ? Dog.Assignee.Id.ToString()  : "";
          StrContract.AssociatedApplication = Dog.AssociatedApplication != null ? Dog.AssociatedApplication.Id.ToString()  : "";
          StrContract.Author = Dog.Author != null ? Dog.Author.Id.ToString()  : "";
          StrContract.BudItemBaseSberDev = Dog.BudItemBaseSberDev != null ? Dog.BudItemBaseSberDev.Id.ToString()  : "";
          StrContract.BudItemsberdevOldSberDev = Dog.BudItemsberdevOldSberDev != null ? Dog.BudItemsberdevOldSberDev.Id.ToString()  : "";
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
          StrContract.DirectionMVZ = Dog.DirectionMVZ != null ? Dog.DirectionMVZ.ToString()  : "";
          StrContract.DocumentGroup = Dog.DocumentGroup != null ? Dog.DocumentGroup.Id.ToString()  : "";
          StrContract.DocumentKind = Dog.DocumentKind != null ? Dog.DocumentKind.Id.ToString()  : "";
          StrContract.DocumentRegister = Dog.DocumentRegister != null ? Dog.DocumentRegister.Id.ToString()  : "";
          StrContract.LeadingDocument = Dog.LeadingDocument != null ? Dog.LeadingDocument.Id.ToString()  : "";
          StrContract.MarketDirectSberDev = Dog.MarketDirectSberDev != null ? Dog.MarketDirectSberDev.Id.ToString()  : "";
          StrContract.Milestones = "";
          StrContract.MVPBaseSberDev = Dog.MVPBaseSberDev != null ? Dog.MVPBaseSberDev.Id.ToString()  : "";
          StrContract.MVPsberdevOldSberDev = Dog.MVPsberdevOldSberDev != null ? Dog.MVPsberdevOldSberDev.Id.ToString()  : "";
          StrContract.MVZBaseSberDev = Dog.MVZBaseSberDev != null ? Dog.MVZBaseSberDev.Id.ToString()  : "";
          StrContract.MVZsberdevOldSberDev = Dog.MVZsberdevOldSberDev != null ? Dog.MVZsberdevOldSberDev.Id.ToString()  : "";
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