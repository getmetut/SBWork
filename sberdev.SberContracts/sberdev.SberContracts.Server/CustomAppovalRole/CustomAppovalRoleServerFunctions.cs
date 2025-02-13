using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.CustomAppovalRole;

namespace sberdev.SberContracts.Server
{
  partial class CustomAppovalRoleFunctions
  {
    [Public]
    public override List<Sungero.Company.IEmployee> GetRolePerformers(Sungero.Docflow.IApprovalTask task)
    {
      #region BudgetOwnerMark
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerMark)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var resultGroup = new List<Sungero.Company.IEmployee>();
        var products = SBContracts.PublicFunctions.OfficialDocument.GetDocumentProducts(SBContracts.OfficialDocuments.As(document));
        
        var contract = SBContracts.ContractualDocuments.As(document);
        if (contract?.MarketDirectSberDev != null)
        {
          var markDir = contract.MarketDirectSberDev;
          if (markDir.BudgetOwner == null)
          {
            foreach (var product in products)
              if (product != null && product.BudgetOwnerGeneral != null)
                resultGroup.Add(product.BudgetOwnerGeneral);
          }
          else
            resultGroup.Add(markDir.BudgetOwner);
        }
        
        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc?.MarketDirectSberDev != null)
        {
          var markDir = acc.MarketDirectSberDev;
          if (markDir.BudgetOwner == null)
          {
            foreach (var product in products)
              if (product != null && product.BudgetOwnerGeneral != null)
                resultGroup.Add(product.BudgetOwnerGeneral);
          }
          else
            resultGroup.Add(markDir.BudgetOwner);
        }
        
        return resultGroup.Distinct().ToList();
      }
      #endregion
      
      #region BudgetOwnerPrGe
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerPrGe)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var resultGroup = new List<Sungero.Company.IEmployee>();
        var products = SBContracts.PublicFunctions.OfficialDocument.GetDocumentProducts(SBContracts.OfficialDocuments.As(document));
        foreach (var product in products)
        {
          if (product != null && product.BudgetOwnerGeneral != null)
            resultGroup.Add(product.BudgetOwnerGeneral);
        }
        return resultGroup.Distinct().ToList();
      }

      
      #endregion
      
      return base.GetRolePerformers(task);
    }

    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      #region BudgetOwner
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwner)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var contract = SBContracts.ContractualDocuments.As(document);
        if (contract != null)
        {
          if (contract.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable ||
              contract.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
          {
            var mvz = contract.MVZBaseSberDev;
            if (mvz.BudgetOwner != null)
            {
              return mvz.BudgetOwner;
            }
            else
            {
              if (mvz.MainMVZ != null)
              {
                mvz = mvz.MainMVZ;
                if (mvz.BudgetOwner != null)
                {
                  return mvz.BudgetOwner;
                }
              }
            }
          }
          else
          {
            var mvp = contract.MVPBaseSberDev;
            if (mvp.BudgetOwner != null)
            {
              return mvp.BudgetOwner;
            }
            else
            {
              if (mvp.MainMVZ != null)
              {
                mvp = mvp.MainMVZ;
                if (mvp.BudgetOwner != null)
                {
                  return mvp.BudgetOwner;
                }
              }
            }
          }
        }
        else
          return null;
      }
      #endregion
      
      #region BudgetOwnerMVP
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerMVP)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var contract = SBContracts.ContractualDocuments.As(document);
        if (contract != null)
        {
          if (contract.MVPBaseSberDev != null)
          {
            var mvp = contract.MVPBaseSberDev;
            if (mvp.MainMVZ != null)
            {
              mvp = mvp.MainMVZ;
            }
            return mvp.BudgetOwner ;
          }
          else
          {
            return null;
          }
        }
        
        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc != null)
        {
          if (acc.MVPBaseSberDev != null)
          {
            var mvp = acc.MVPBaseSberDev;
            if (mvp.MainMVZ != null)
            {
              mvp = mvp.MainMVZ;
            }
            return mvp.BudgetOwner ;
          }
          else
          {
            return null;
          }
        }
        
        return null;
      }
      #endregion
      
      #region BudgetOwnerMVZ
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerMVZ)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var contract = SBContracts.ContractualDocuments.As(document);
        if (contract != null)
        {
          if (contract.MVZBaseSberDev != null)
          {
            var mvz = contract.MVZBaseSberDev;
            if (mvz.MainMVZ != null)
            {
              mvz = mvz.MainMVZ;
            }
            return mvz.BudgetOwner ;
          }
          else
          {
            return null;
          }
        }
        
        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc != null)
        {
          if (acc.MVZBaseSberDev != null)
          {
            var mvz = acc.MVZBaseSberDev;
            if (mvz.MainMVZ != null)
            {
              mvz = mvz.MainMVZ;
            }
            return mvz.BudgetOwner ;
          }
          else
          {
            return null;
          }
        }
        
        return null;
      }
      #endregion
      
      #region BudgetOwnerUnit
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerUnit)
      {
        var stage = task.ApprovalRule.Stages.Where(s => s.Number == task.StageNumber).FirstOrDefault();
        if (stage != null)
          return SBContracts.ApprovalStages.As(stage.Stage)?.ProductUnitSberDev?.Responsible;
      }
      #endregion
      
      #region Attorney
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.Attorney)
      {
        var attorney = Sungero.ATS.PowerOfAttorneys.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
        return attorney?.IssuedTo;
      }
      #endregion
      
      #region AttorneyManager
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.AttorneyManager)
      {
        var attorney = Sungero.ATS.PowerOfAttorneys.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
        return attorney?.IssuedTo?.Department?.Manager;
      }
      #endregion
      
      #region BudgetOwnerMark
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerMark)
      {
        var accounting = SBContracts.AccountingDocumentBases.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
        var contractual = SBContracts.ContractualDocuments.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
        if (accounting != null)
          return accounting.MarketDirectSberDev?.BudgetOwner;
        if (contractual != null)
          return contractual.MarketDirectSberDev?.BudgetOwner;
      }
      #endregion
      
      #region BudgetOwnerPrGe
      if (_obj.Type == SberContracts.CustomAppovalRole.Type.BudgetOwnerPrGe)
      {
        var accounting = SBContracts.AccountingDocumentBases.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
        var contractual = SBContracts.ContractualDocuments.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
        if (accounting != null)
          return accounting.ProdCollectionBaseSberDev.FirstOrDefault()?.Product.BudgetOwnerGeneral;
        if (contractual != null)
        {
          return contractual.ProdCollectionExBaseSberDev.FirstOrDefault()?.Product.BudgetOwnerGeneral;
          return contractual.ProdCollectionPrBaseSberDev.FirstOrDefault()?.Product.BudgetOwnerGeneral;
        }
      }
      
      #endregion
      
      return base.GetRolePerformer(task);
    }
  }
}