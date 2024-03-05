using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ContractCondition;

namespace sberdev.SBContracts.Shared
{
  partial class ContractConditionFunctions
  {
    
    #region Видимость и доступность условий
    
    public override void ChangePropertiesAccess()
    {
      base.ChangePropertiesAccess();

      var isContrType = _obj.ConditionType == ConditionType.ContrCategory;
      _obj.State.Properties.ContrCategorysberdev.IsVisible = isContrType;

      _obj.State.Properties.ContrCategorysberdev.IsRequired = isContrType;
      
      
      var isMVP = _obj.ConditionType == ConditionType.MVP;
      _obj.State.Properties.MVP.IsVisible = isMVP;

      _obj.State.Properties.MVP.IsRequired = isMVP;
      
      var isMVZ = _obj.ConditionType == ConditionType.MVZ;
      _obj.State.Properties.MVZ.IsVisible = isMVZ;

      _obj.State.Properties.MVZ.IsRequired = isMVZ;
      
      var isAccoutnAticle = _obj.ConditionType == ConditionType.AccountAticles;
      _obj.State.Properties.AccountingArticles.IsVisible = isAccoutnAticle;

      _obj.State.Properties.AccountingArticles.IsRequired = isAccoutnAticle;
      
      var isBudgetItem = _obj.ConditionType == ConditionType.BudgetItem;
      _obj.State.Properties.BudgetItem.IsVisible = isBudgetItem;

      _obj.State.Properties.BudgetItem.IsRequired = isBudgetItem;
      
      var isInitiatorsDepartment = _obj.ConditionType == ConditionType.InitiatorsDepartment;
      _obj.State.Properties.InitiatorsDepartment.IsVisible = isInitiatorsDepartment;
      _obj.State.Properties.InitiatorsDepartment.IsRequired = isInitiatorsDepartment;
      
      var isMarketDirect = _obj.ConditionType == ConditionType.MarketDirect;
      _obj.State.Properties.MarketDirectSberDev.IsVisible = isMarketDirect;
      _obj.State.Properties.MarketDirectSberDev.IsRequired = isMarketDirect;
    }

    public override void ClearHiddenProperties()
    {
      base.ClearHiddenProperties();
      
      if(!_obj.State.Properties.InitiatorsDepartment.IsVisible)
        _obj.InitiatorsDepartment.Clear();
      
      if (!_obj.State.Properties.ContrCategorysberdev.IsVisible)
        _obj.ContrCategorysberdev.Clear();
      
      if (!_obj.State.Properties.MVP.IsVisible)
        _obj.MVP.Clear();
      
      if (!_obj.State.Properties.MVZ.IsVisible)
        _obj.MVZ.Clear();
      
      if (!_obj.State.Properties.AccountingArticles.IsVisible)
        _obj.AccountingArticles.Clear();
      
      if (!_obj.State.Properties.BudgetItem.IsVisible)
        _obj.BudgetItem.Clear();
      
      if (!_obj.State.Properties.MarketDirectSberDev.IsVisible)
        _obj.MarketDirectSberDev.Clear();
    }
    
    #endregion
    
    #region Механика условий
    
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.ConditionType == ConditionType.MarketDirect)
      {
        var contr = SBContracts.ContractualDocuments.As(document);
        var acc = SBContracts.AccountingDocumentBases.As(document);
        bool flag = false;
        
        if (contr != null && contr.MarketDirectSberDev != null)
        {
          foreach (var dir in _obj.MarketDirectSberDev)
            if (dir.MarketDirect == contr.MarketDirectSberDev)
              flag = true;
        }
        
        if (acc != null && acc.MarketDirectSberDev != null)
        {
          foreach (var dir in _obj.MarketDirectSberDev)
            if (dir.MarketDirect == acc.MarketDirectSberDev)
              flag = true;
        }
        
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.DocumentChanged)
      {
        var acc = SBContracts.AccountingDocumentBases.As(document);
        var contr = SBContracts.ContractualDocuments.As(document);
        var firstApprove = Signatures.Get(document.LastVersion).FirstOrDefault();
        if (firstApprove == null)
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(false, "Отбивка. Маршрут будет вычеслен по ходу согласования.");
        
        if (acc != null)
        {
          if (acc.ModifiedSberDev == null)
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(false, "Отбивка. Маршрут будет вычеслен по ходу согласования.");
          else
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(Calendar.Between(acc.ModifiedSberDev, firstApprove.SigningDate.AddHours(3), Calendar.Now)
                     || Calendar.Between(acc.LastVersion.Modified, firstApprove.SigningDate.AddHours(3), Calendar.Now), string.Empty);
        }
        if (contr != null)
        {
          if (contr.ModifiedSberDev == null)
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(false, "Отбивка. Маршрут будет вычеслен по ходу согласования.");
          else
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(Calendar.Between(contr.ModifiedSberDev, firstApprove.SigningDate.AddHours(3), Calendar.Now)
                     || Calendar.Between(contr.LastVersion.Modified, firstApprove.SigningDate.AddHours(3), Calendar.Now), string.Empty);
        }
      }
      
      if (_obj.ConditionType == ConditionType.InitiatorsDepartment)
      {
        bool flag = false;
        var initDep = Sungero.Company.Employees.As(task.Author).Department;
        foreach (var dep in _obj.InitiatorsDepartment)
          if (dep.InitiatorsDepartment == initDep)
            flag = true;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }

      if (_obj.ConditionType == ConditionType.ContrType)
      {
        var find = false;
        var contractual = SBContracts.ContractualDocuments.As(document);
        var accounting = SBContracts.AccountingDocumentBases.As(document);
        if (contractual != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(contractual.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable ,

                   string.Empty);

        }
        if (accounting != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(contractual.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable ,

                   string.Empty);

        }
        if ( find != true )
        {
          var mainContract = SBContracts.ContractualDocuments.GetAll(e=> e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            return

              Sungero.Docflow.Structures.ConditionBase.ConditionResult.

              Create(mainContract.ContrTypeBaseSberDev == sberdev.SBContracts.ContractualDocument.ContrTypeBaseSberDev.Profitable ,

                     string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

              Create(null, "Условие не может быть вычислено. Не заполнен тип договора.");
          }
        }
        
      }
      
      if (_obj.ConditionType == ConditionType.PayType )

      {

        var IncInv = SBContracts.IncomingInvoices.As(document);

        if (IncInv != null)

          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(IncInv.PayType == sberdev.SBContracts.IncomingInvoice.PayType.Postpay ,

                   string.Empty);

        else

          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Не заполнен тип оплаты.");

      }

      if (_obj.ConditionType == ConditionType.ContrCategory)

      {
        var contract = SBContracts.Contracts.As(document);
        var сontrFind = false;
        
        if (contract != null)
        {
          foreach (var str in _obj.ContrCategorysberdev)
          {
            if (contract.DocumentGroup == str.ContrCategory)
            {
              сontrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(сontrFind, string.Empty);
        }
        
        var sup = SBContracts.SupAgreements.As(document);

        if (sup != null)
        {
          contract = SBContracts.Contracts.As(sup.LeadingDocument);
          foreach (var str in _obj.ContrCategorysberdev)
          {
            if (contract.DocumentGroup == str.ContrCategory)
            {
              сontrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(сontrFind, string.Empty);
        }
        
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.
          Create(null, "Условие не может быть вычислено. Не заполнены категории договора.");
      }
      
      if (_obj.ConditionType == ConditionType.MVP )
      {
        var find = false;
        var contractual = SBContracts.ContractualDocuments.As(document);
        var accounting = SBContracts.AccountingDocumentBases.As(document);
        if (contractual != null)
        {
          foreach (var str in _obj.MVP )
          {
            if (contractual.MVPBaseSberDev == str.MVP)
            {
              find = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(find,
                   string.Empty);
        }
        if (accounting != null)
        {
          foreach (var str in _obj.MVP )
          {
            if (accounting.MVPBaseSberDev == str.MVP)
            {
              find = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(find,
                   string.Empty);
        }
        if ( find != true )
        {
          var mainContract = SBContracts.ContractualDocuments.GetAll(e=> e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            foreach (var str in _obj.MVP )
            {
              if (mainContract.MVPBaseSberDev == str.MVP)
              {
                find = true;
              }
            }
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(find,
                     string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(null, "Условие не может быть вычислено. Не заполнено МВП договора.");
          }
        }
      }
      
      if (_obj.ConditionType == ConditionType.MVZ )
      {
        var find = false;
        var contractual = SBContracts.ContractualDocuments.As(document);
        var accounting = SBContracts.AccountingDocumentBases.As(document);
        if (contractual != null)
        {
          foreach (var str in _obj.MVZ )
          {
            if (contractual.MVZBaseSberDev == str.MVZ)
            {
              find = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(find,
                   string.Empty);
        }
        if (accounting != null)
        {
          foreach (var str in _obj.MVP )
          {
            if (accounting.MVPBaseSberDev == str.MVP)
            {
              find = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(find,
                   string.Empty);
        }
        if ( find != true )
        {
          var mainContract = SBContracts.ContractualDocuments.GetAll(e=> e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            foreach (var str in _obj.MVP )
            {
              if (mainContract.MVPBaseSberDev == str.MVP)
              {
                find = true;
              }
            }
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(find,
                     string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(null, "Условие не может быть вычислено. Не заполнено МВП договора.");
          }
        }
      }
      
      if (_obj.ConditionType == ConditionType.AccountAticles )
      {
        var find = false;
        var contract = SBContracts.Contracts.As(document);
        if (contract != null)
        {
          find = true;
          var ContrFind = false;
          if (contract.ContrTypeBaseSberDev == SBContracts.Contract.ContrTypeBaseSberDev.Profitable)
          {
            foreach (var str in _obj.AccountingArticles )
            {
              if (contract.AccArtPrBaseSberDev == str.AccountingArticles)
              {
                ContrFind = true;
              }
            }
          }
          else
          {
            foreach (var str in _obj.AccountingArticles )
            {
              if (contract.AccArtExBaseSberDev == str.AccountingArticles)
              {
                ContrFind = true;
              }
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        
        var supAgr = SBContracts.SupAgreements.As(document);
        if (supAgr != null)
        {
          find = true;
          var ContrFind = false;
          if (supAgr.ContrTypeBaseSberDev == SBContracts.SupAgreement.ContrTypeBaseSberDev.Profitable)
          {
            foreach (var str in _obj.AccountingArticles )
            {
              if (supAgr.AccArtPrBaseSberDev == str.AccountingArticles)
              {
                ContrFind = true;
              }
            }
          }
          else
          {
            foreach (var str in _obj.AccountingArticles )
            {
              if (supAgr.AccArtExBaseSberDev == str.AccountingArticles)
              {
                ContrFind = true;
              }
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        
        var incInv = SBContracts.IncomingInvoices.As(document);
        if (incInv != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles )
          {
            if (incInv.AccArtBaseSberDev == str.AccountingArticles)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var contState = SBContracts.ContractStatements.As(document);
        if (contState != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles )
          {
            if (contState.AccArtBaseSberDev == str.AccountingArticles)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        if ( find != true )
        {
          var mainContract = SBContracts.Contracts.GetAll(e=> e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            var ContrFind = false;
            if (mainContract.ContrTypeBaseSberDev == SBContracts.Contract.ContrTypeBaseSberDev.Profitable)
            {
              foreach (var str in _obj.AccountingArticles )
              {
                if (mainContract.AccArtPrBaseSberDev == str.AccountingArticles)
                {
                  ContrFind = true;
                }
              }
            }
            else
            {
              foreach (var str in _obj.AccountingArticles )
              {
                if (mainContract.AccArtExBaseSberDev == str.AccountingArticles)
                {
                  ContrFind = true;
                }
              }
            }
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(ContrFind,
                     string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(null, "Условие не может быть вычислено. Не заполнена статья упр. учета договора.");
          }
        }
      }
      
      if (_obj.ConditionType == ConditionType.BudgetItem )

      {
        var find = false;
        var contract = SBContracts.Contracts.As(document);
        if (contract != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.BudgetItem )
          {
            if (contract.AccArtPrBaseSberDev.BudgetItem == str.BudgetItem)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var supAgr = SBContracts.SupAgreements.As(document);
        if (supAgr != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.BudgetItem )
          {
            if (supAgr.AccArtPrBaseSberDev.BudgetItem == str.BudgetItem)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var incInv = SBContracts.IncomingInvoices.As(document);
        if (incInv != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.BudgetItem )
          {
            if (incInv.AccArtBaseSberDev.BudgetItem == str.BudgetItem)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var contState = SBContracts.ContractStatements.As(document);
        if (contState != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.BudgetItem )
          {
            if (contState.AccArtBaseSberDev.BudgetItem == str.BudgetItem)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        if ( find != true )
        {
          var mainContract = SBContracts.Contracts.GetAll(e=> e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            var ContrFind = false;
            foreach (var str in _obj.BudgetItem )
            {
              if (mainContract.AccArtPrBaseSberDev.BudgetItem == str.BudgetItem)
              {
                ContrFind = true;
              }
            }
            return
              Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(ContrFind,
                     string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.
              Create(null, "Условие не может быть вычислено. Не заполнена статья бух. учета договора.");
          }
        }
      }
      
      if (_obj.ConditionType == ConditionType.Framework )

      {
        var find = false;
        var contract = SBContracts.ContractualDocuments.As(document);
        if (contract != null)
        {
          find = true;
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(contract.FrameworkBaseSberDev == true,
                   string.Empty);
        }
        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc != null)
        {
          find = true;
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(acc.FrameworkBaseSberDev == true,
                   string.Empty);
        }
      }
      return base.CheckCondition(document, task);
    }
    
    #endregion
    
    #region Поддерживаемые типы
    
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      var baseSupport = base.GetSupportedConditions();
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.ContrType);
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.ContrType);
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.MarketDirect); // сontract statement
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.MarketDirect); // universal transfer document
      baseSupport["4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"].Add(ConditionType.MarketDirect); // waybill
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.MarketDirect); // contract
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.MarketDirect); // sup agreement
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.DocumentChanged); // contract
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.DocumentChanged); // sup agreement
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.DocumentChanged); // сontract statement
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.DocumentChanged); // universal transfer document
      baseSupport["4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"].Add(ConditionType.DocumentChanged); // waybill
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.ContrCategory); // contract
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.ContrCategory); // sup agreement
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.ContrCategory); // Contract statement
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.MVZ); // contract
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.MVZ); // sup agreement
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.MVZ); // сontract statement
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.MVZ); // universal transfer document
      baseSupport["4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"].Add(ConditionType.MVZ); // waybill
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.MVP); // contract
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.MVP); // sup agreement
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.MVP); // сontract statement
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.MVP); // universal transfer document
      baseSupport["4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"].Add(ConditionType.MVP); // waybill
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.InitiatorsDepartment);
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.InitiatorsDepartment);
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.InitiatorsDepartment);
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.AccountAticles);
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.AccountAticles);
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.AccountAticles);
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.BudgetItem);
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.BudgetItem);
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.BudgetItem);
      
      baseSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.Framework);
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.Framework);
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.Framework);
      
      baseSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.PayType);
      
      return baseSupport;
    }
    
    #endregion
    
  }
}