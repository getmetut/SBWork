using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.BudgetOwnerRole;

namespace sberdev.SberContracts.Server
{
  partial class BudgetOwnerRoleFunctions
  {
    [Public]
    public override List<Sungero.Company.IEmployee> GetRolePerformers(Sungero.Docflow.IApprovalTask task)
    {
      #region BudgetOwnerMark
      if (_obj.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwnerMark)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var contract = SBContracts.ContractualDocuments.As(document);
        List<Sungero.Company.IEmployee> group = new List<Sungero.Company.IEmployee>();
        if (contract != null)
        {
          if (contract.MarketDirectSberDev != null)
          {
            var markDir = contract.MarketDirectSberDev;
            if (markDir.BudgetOwner == null)
            {
              if (contract.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable ||
                  contract.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
              {
                if (contract.ProdCollectionExBaseSberDev.FirstOrDefault().Product.Name == "General")
                {
                  foreach (var prod in contract.CalculationBaseSberDev)
                    group.Add(prod.ProductCalc.BudgetOwner);
                }
                else
                  group.Add(contract.ProdCollectionExBaseSberDev.FirstOrDefault().Product.BudgetOwner);
              }
              else
              {
                if (contract.ProdCollectionPrBaseSberDev.FirstOrDefault().Product.Name == "General")
                {
                  foreach (var prod in contract.CalculationBaseSberDev)
                    group.Add(prod.ProductCalc.BudgetOwner);
                }
                else
                  group.Add(contract.ProdCollectionPrBaseSberDev.FirstOrDefault().Product.BudgetOwner);
              }
            }
            else
              group.Add(markDir.BudgetOwner);
          }
        }
        
        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc != null)
        {
          if (acc.MarketDirectSberDev != null)
          {
            var markDir = acc.MarketDirectSberDev;
            if (markDir.BudgetOwner == null)
            {
              if (acc.ProdCollectionBaseSberDev.FirstOrDefault().Product.Name == "General")
              {
                foreach (var prod in acc.CalculationBaseSberDev)
                  group.Add(prod.ProductCalc.BudgetOwner);
              }
              else
                group.Add(acc.ProdCollectionBaseSberDev.FirstOrDefault().Product.BudgetOwner);
              
            }
            else
              group.Add(markDir.BudgetOwner);
            
          }
        }
        
        return group.Distinct().ToList();
      }
      #endregion
      
      #region BudgetOwnerPrGe
      if (_obj.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwnerPrGe)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var contract = SBContracts.ContractualDocuments.As(document);
        List<Sungero.Company.IEmployee> group = new List<Sungero.Company.IEmployee>();
        if (contract != null)
        {
          if (contract.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.Expendable ||
              contract.ContrTypeBaseSberDev == SBContracts.ContractualDocument.ContrTypeBaseSberDev.ExpendProfitSberDev)
          {
            if (contract.ProdCollectionExBaseSberDev.FirstOrDefault().Product.Name == "General")
            {
              if (contract.CalculationBaseSberDev.Count > 0)
                foreach (var prod in contract.CalculationBaseSberDev)
                  group.Add(prod.ProductCalc.BudgetOwnerGeneral);
            }
            else
              group.Add(contract.ProdCollectionExBaseSberDev.FirstOrDefault().Product.BudgetOwnerGeneral);
          }
          else
          {
            if (contract.ProdCollectionPrBaseSberDev.FirstOrDefault().Product.Name == "General")
            {
              if (contract.CalculationBaseSberDev.Count > 0)
                foreach (var prod in contract.CalculationBaseSberDev)
                  group.Add(prod.ProductCalc.BudgetOwnerGeneral);
            }
            else
              group.Add(contract.ProdCollectionPrBaseSberDev.FirstOrDefault().Product.BudgetOwnerGeneral);
          }
        }
        
        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc != null)
        {
          if (acc.ProdCollectionBaseSberDev.FirstOrDefault().Product.Name == "General")
          {
            if (acc.CalculationBaseSberDev.Count > 0)
              foreach (var prod in acc.CalculationBaseSberDev)
                group.Add(prod.ProductCalc.BudgetOwnerGeneral);
          }
          else
            group.Add(acc.ProdCollectionBaseSberDev.FirstOrDefault().Product.BudgetOwnerGeneral);
        }
        
        return group.Distinct().ToList();
      }
      
      #endregion
      
      return base.GetRolePerformers(task);
    }

    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      #region BudgetOwner
      if (_obj.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwner)
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
      if (_obj.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwnerMVP)
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
      if (_obj.Type == SberContracts.BudgetOwnerRole.Type.BudgetOwnerMVZ)
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
      
      return base.GetRolePerformer(task);
    }
  }
}