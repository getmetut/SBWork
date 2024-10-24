using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.Condition;

namespace sberdev.SBContracts.Shared
{
  partial class ConditionFunctions
  {
    #region Видимость и доступность условий
    
    public override void ChangePropertiesAccess()
    {
      base.ChangePropertiesAccess();
      
      var isPurchseAmount = _obj.ConditionType == ConditionType.PurchAmount;
      _obj.State.Properties.PurchaseAmountSberDev.IsVisible = isPurchseAmount;
      _obj.State.Properties.PurchaseAmountSberDev.IsRequired = isPurchseAmount;
      _obj.State.Properties.AmountOperator.IsVisible = isPurchseAmount;
      _obj.State.Properties.AmountOperator.IsRequired = isPurchseAmount;
      
      var isProductUnit = _obj.ConditionType == ConditionType.ProductUnit;
      _obj.State.Properties.ProductUnitSberDev.IsVisible = isProductUnit;
      _obj.State.Properties.ProductUnitSberDev.IsRequired = isProductUnit;
      
      var isEndorseFrom = _obj.ConditionType == ConditionType.EndorseFromSberDev || _obj.ConditionType == ConditionType.FCDApprBySberDev;
      _obj.State.Properties.EndorserSberDev.IsVisible = isEndorseFrom;
      _obj.State.Properties.EndorserSberDev.IsRequired = isEndorseFrom;
      
      var isMVP = _obj.ConditionType == ConditionType.MVP;
      _obj.State.Properties.MVP.IsVisible = isMVP;
      _obj.State.Properties.MVP.IsRequired = isMVP;

      var isMVZ = _obj.ConditionType == ConditionType.MVZ;
      _obj.State.Properties.MVZ.IsVisible = isMVZ;
      _obj.State.Properties.MVZ.IsRequired = isMVZ;
      
      var isAccoutnAticle = _obj.ConditionType == ConditionType.AccArts;
      _obj.State.Properties.AccountingArticles.IsVisible = isAccoutnAticle;
      _obj.State.Properties.AccountingArticles.IsRequired = isAccoutnAticle;
      
      var isBudgetItem = _obj.ConditionType == ConditionType.BudgetItem;
      _obj.State.Properties.BudgetItem.IsVisible = isBudgetItem;
      _obj.State.Properties.BudgetItem.IsRequired = isBudgetItem;
      
      var isMarketDirect = _obj.ConditionType == ConditionType.MarketDirect;
      _obj.State.Properties.MarketDirectSberDev.IsVisible = isMarketDirect;
      _obj.State.Properties.MarketDirectSberDev.IsRequired = isMarketDirect;
    }

    public override void ClearHiddenProperties()
    {
      base.ClearHiddenProperties();
      if(!_obj.State.Properties.PurchaseAmountSberDev.IsVisible)
      {
        _obj.PurchaseAmountSberDev = null;
        _obj.AmountOperator = null;
      }
      
      if(!_obj.State.Properties.ProductUnitSberDev.IsVisible)
        _obj.ProductUnitSberDev.Clear();
      
      if(!_obj.State.Properties.EndorserSberDev.IsVisible)
        _obj.EndorserSberDev = null;
      
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
      if (_obj.ConditionType == ConditionType.SameAttorney)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        bool flag = Functions.Condition.Remote.CheckSameAttorney(_obj, power);
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.InitIsAttorney)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        bool flag = task.Author == Sungero.CoreEntities.Users.As(power.IssuedTo);
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.MRP)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        var matrix = power.MatrixSDev;
        bool flag = false;
        if (matrix != null)
          flag = matrix.Mahineread.HasValue ? matrix.Mahineread.Value : false;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.EarlyProxy)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        bool flag = power.FirstOrDoubleSDev.HasValue ? power.FirstOrDoubleSDev.Value : false;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.PurchAmount)
      {
        var purch = SberContracts.Purchases.As(document);
        bool flag = false;
        switch (_obj.AmountOperator.Value.Value)
        {
          case "GreaterOrEqual":
            if (purch.PurchaseAmount >= _obj.PurchaseAmountSberDev)
              flag = true;
            break;
          case "GreaterThan":
            if (purch.PurchaseAmount > _obj.PurchaseAmountSberDev)
              flag = true;
            break;
          case "LessOrEqual":
            if (purch.PurchaseAmount <= _obj.PurchaseAmountSberDev)
              flag = true;
            break;
          case "LessThan":
            if (purch.PurchaseAmount < _obj.PurchaseAmountSberDev)
              flag = true;
            break;
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.Contrtype)
      {
        var acc = SBContracts.AccountingDocumentBases.As(document);
        bool flag = acc.ContrTypeBaseSberDev == SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.ProductUnit)
      {
        bool flag = false;
        var acc = SBContracts.AccountingDocumentBases.As(document);
        var contr = SBContracts.ContractualDocuments.As(document);
        if (acc != null)
        {
          var products = acc.CalculationBaseSberDev.Select(c => c.ProductCalc).ToList();
          products.Add(acc.ProdCollectionBaseSberDev.FirstOrDefault()?.Product);
          var units = products.Select(p => p.ProductUnit).ToList();
          foreach (var unit in _obj.ProductUnitSberDev.Select(p => p.ProductUnit).ToList())
            if (units.Any(u => u == unit))
              flag = true;
        }
        if (contr != null)
        {
          var products = contr.CalculationBaseSberDev.Select(c => c.ProductCalc).ToList();
          products.Add(contr.ProdCollectionExBaseSberDev.FirstOrDefault()?.Product);
          products.Add(contr.ProdCollectionPrBaseSberDev.FirstOrDefault()?.Product);
          var units = products.Where(p => p != null).Select(p => p.ProductUnit).ToList();
          foreach (var unit in _obj.ProductUnitSberDev.Select(p => p.ProductUnit).ToList())
            if (units.Any(u => u == unit))
              flag = true;
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.EndorseFromSberDev)
      {
        var signInfos = Signatures.Get(document.LastVersion);
        bool flag = false;
        foreach (var singInfo in signInfos)
        {
          if ((singInfo.Signatory == _obj.EndorserSberDev || singInfo.SubstitutedUser == _obj.EndorserSberDev) && singInfo.SignatureType != SignatureType.NotEndorsing)
          {
            flag = true;
            break;
          }
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.FCDApprBySberDev)
      {
        bool flag = false;
        var incInv = SBContracts.IncomingInvoices.As(document);
        var fcd = incInv.AccDocSberDev;
        if (fcd != null && fcd.HasVersions)
        {
          var signInfos = Signatures.Get(fcd.LastVersion);
          foreach (var signInfo in signInfos)
          {
            if (signInfo.IsValid && (signInfo.SubstitutedUser == _obj.EndorserSberDev
                                     || signInfo.Signatory == _obj.EndorserSberDev))
            {
              flag = true;
              break;
            }
          }
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.MarketDirect)
      {
        var acc = SBContracts.AccountingDocumentBases.As(document);
        bool flag = false;
        foreach (var dir in _obj.MarketDirectSberDev)
          if (dir.MarketDirect == acc.MarketDirectSberDev)
            flag = true;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.ContractSigned)
      {
        var lead = SberContracts.GuaranteeLetters.As(document).LeadingDocument;
        if (lead != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult
            .Create(PublicFunctions.Module.CheckSpecialGroupSignature(SBContracts.OfficialDocuments.As(lead), true), string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult
            .Create(false, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.PurchApproved)
      {
        var salut = Sungero.Company.PublicFunctions.BusinessUnit.Remote.GetBusinessUnits().Where(u => u.TIN == "7730253720").FirstOrDefault();
        var depart = SBContracts.PublicFunctions.Module.Remote.GetGroup("Закупки", salut);
        var purchase = SberContracts.GuaranteeLetters.As(document).AddendumDocument;
        if (depart != null && purchase != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult
            .Create(PublicFunctions.Module.CheckDepartmentApproval(purchase, depart), string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.DocumentChanged)
      {
        var contr = SBContracts.ContractualDocuments.As(document);
        var firstApprove = Signatures.Get(document.LastVersion).FirstOrDefault();
        if (firstApprove == null)
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(false, "Отбивка. Маршрут будет вычеслен по ходу согласования.");
        if (contr.ModifiedSberDev == null)
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(false, "Отбивка. Маршрут будет вычеслен по ходу согласования.");
        if (contr != null)
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(Calendar.Between(contr.ModifiedSberDev, firstApprove.SigningDate.AddHours(3), Calendar.Now)
                   || Calendar.Between(contr.LastVersion.Modified, firstApprove.SigningDate.AddHours(3), Calendar.Now), string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.ManuallyCheck)
      {
        var sbTask = SBContracts.ApprovalTasks.As(task); bool flagContract = false, flagStatement = false, flagTask = false;
        var invoice = SBContracts.IncomingInvoices.As(document);
        if (invoice != null)
        {
          var sbContract = invoice.LeadingDocument;
          var sbStatement = invoice.ContractStatement;
          if (sbContract != null)
          {
            var contracttList = PublicFunctions.Module.Remote.CheckPropertySignatures(sbContract);
            flagContract = contracttList[0] && contracttList[1];
          }
          if (sbStatement != null)
          {
            var statementList = PublicFunctions.Module.Remote.CheckPropertySignatures(sbStatement);
            flagStatement = statementList[0] && statementList[1];
          }
          if (sbTask.IsNeedManuallyCheckSberDev.HasValue)
            flagTask = sbTask.IsNeedManuallyCheckSberDev.Value;
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.
          Create(!flagContract && !(flagStatement && invoice.PayTypeBaseSberDev.Value == SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay) || flagTask,
                 string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.PayType )

      {
        var find = false;
        // Входящий счет
        var IncInv = SBContracts.IncomingInvoices.As(document);
        if (IncInv != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult
            .Create(IncInv.PayTypeBaseSberDev == sberdev.SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay, string.Empty);
          
        }
        
        
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Не заполнен тип оплаты.");
        }
      }

      if (_obj.ConditionType == ConditionType.Framework)
      {
        // Исходящее письмо
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(outLetter.Framework == true ,

                   string.Empty);

        }
        // Входящее письмо
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(IncLetter.Framework == true ,

                   string.Empty);

        }
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Нет документа.");
        }
        
        // Входящий счет
        var accDoc = SBContracts.AccountingDocumentBases.As(document);
        if (accDoc != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(accDoc.FrameworkBaseSberDev == true ,

                   string.Empty);
          
        }
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Нет документа.");
        }
      }

      if (_obj.ConditionType == ConditionType.MVZ)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVZ )
          {
            if (outLetter.MVZ == str.MVZ)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVZ )
          {
            if (IncLetter.MVZ == str.MVZ)
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
          foreach (var str in _obj.MVZ )
          {
            if (incInv.MVZBaseSberDev == str.MVZ)
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
            foreach (var str in _obj.MVZ )
            {
              if (mainContract.MVZBaseSberDev == str.MVZ)
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
              Create(null, "Условие не может быть вычислено. Не заполнено МВЗ документа.");
          }
        }

      }
      
      if (_obj.ConditionType == ConditionType.MVP)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVP )
          {
            if (outLetter.MVP == str.MVP)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVP )
          {
            if (IncLetter.MVP == str.MVP)
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
          foreach (var str in _obj.MVP )
          {
            if (incInv.MVPBaseSberDev == str.MVP)
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
            foreach (var str in _obj.MVP )
            {
              if (mainContract.MVPBaseSberDev == str.MVP)
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
              Create(null, "Условие не может быть вычислено. Не заполнено МВП документа.");
          }
        }

      }
      if (_obj.ConditionType == ConditionType.AccArts)
      {
        
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles )
          {
            if (outLetter.AccArt == str.AccountingArticles)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles )
          {
            if (IncLetter.AccArt == str.AccountingArticles)
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
        
        if ( find != true )
        {
          var mainContract = SBContracts.Contracts.GetAll(e=> e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            var ContrFind = false;
            foreach (var str in _obj.AccountingArticles )
            {
              if (mainContract.AccArtPrBaseSberDev == str.AccountingArticles)
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
              Create(null, "Условие не может быть вычислено. Не заполнена статья управленческого учета документа.");
          }
        }
      }
      
      if (_obj.ConditionType == ConditionType.BudgetItem)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.BudgetItem )
          {
            if (outLetter.AccArt.BudgetItem == str.BudgetItem)
            {
              ContrFind = true;
            }
          }
          return
            Sungero.Docflow.Structures.ConditionBase.ConditionResult.
            Create(ContrFind,
                   string.Empty);
        }
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.BudgetItem )
          {
            if (IncLetter.AccArt.BudgetItem == str.BudgetItem)
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
              Create(null, "Условие не может быть вычислено. Не заполнена статья бух. учета документа.");
          }
        }
      }
      if (_obj.ConditionType == ConditionType.ActExists)
      {
        // Исходящее письмо
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(outLetter.ActExists == true ,

                   string.Empty);

        }
        // Входящее письмо
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(IncLetter.ActExists == true ,

                   string.Empty);

        }
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Нет документа.");
        }
      }
      if (_obj.ConditionType == ConditionType.DeviceExists)
      {
        // Исходящее письмо
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(outLetter.DeviceExists == true ,

                   string.Empty);

        }
        // Входяще письмо
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(IncLetter.DeviceExists == true ,

                   string.Empty);

        }
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Нет документа.");
        }
      }
      if (_obj.ConditionType == ConditionType.FactOfPayment)
      {
        // Исходящее письмо
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(outLetter.FactOfPayment == true ,

                   string.Empty);

        }
        // Входящее письмо
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(IncLetter.FactOfPayment == true ,

                   string.Empty);

        }
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Нет документа.");
        }
      }
      if (_obj.ConditionType == ConditionType.PricesAgreed)
      {
        // Исходящее письмо
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(outLetter.PricesAgreed == true ,

                   string.Empty);

        }
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return

            Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(IncLetter.PricesAgreed == true ,

                   string.Empty);

        }
        if( find != true )
        {  return Sungero.Docflow.Structures.ConditionBase.ConditionResult.

            Create(null, "Условие не может быть вычислено. Нет документа.");
        }
      }
      return base.CheckCondition(document, task);

    }
    
    #endregion
    
    #region Поддерживаемые типы
    
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      var baseSupport = base.GetSupportedConditions();
      
      baseSupport["be859f9b-7a04-4f07-82bc-441352bce627"].Add(ConditionType.SameAttorney);
      baseSupport["be859f9b-7a04-4f07-82bc-441352bce627"].Add(ConditionType.InitIsAttorney);
      baseSupport["be859f9b-7a04-4f07-82bc-441352bce627"].Add(ConditionType.MRP);
      baseSupport["be859f9b-7a04-4f07-82bc-441352bce627"].Add(ConditionType.EarlyProxy); // power of attorney
      
      baseSupport["7aa8969f-f81d-462c-b0d8-761ccd59253f"].Add(ConditionType.PurchAmount); // purchase
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.Contrtype); // входящий счет
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.ProductUnit); // incoming invoice
      
      baseSupport["7aa8969f-f81d-462c-b0d8-761ccd59253f"].Add(ConditionType.AmountIsMore);
      
      baseSupport["7aa8969f-f81d-462c-b0d8-761ccd59253f"].Add(ConditionType.EndorseFromSberDev); // purchase
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.FCDApprBySberDev); // incoming invoice
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.MarketDirect); // incoming invoice
      
      baseSupport["9d7a0ce3-e5c5-45b9-956d-7b26daedfdd2"].Add(ConditionType.ContractSigned); // гарантийное письмо
      
      baseSupport["9d7a0ce3-e5c5-45b9-956d-7b26daedfdd2"].Add(ConditionType.PurchApproved); // гарантийное письмо
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.ManuallyCheck); // входящий счет
      
      baseSupport["7aa8969f-f81d-462c-b0d8-761ccd59253f"].Add(ConditionType.DocumentChanged); // закупка

      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.PayType); // входящий счет
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.Framework); // входящий счет
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.Framework); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.Framework); // Входящее письмо
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.Framework); // упд
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.Framework); // акт выполненых работ
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.Framework); // исходящий счет
      baseSupport["4e81f9ca-b95a-4fd4-bf76-ea7176c215a7"].Add(ConditionType.Framework); // накладная
      baseSupport["74c9ddd4-4bc4-42b6-8bb0-c91d5e21fb8a"].Add(ConditionType.Framework); // счет фактура полученный
      baseSupport["f50c4d8a-56bc-43ef-bac3-856f57ca70be"].Add(ConditionType.Framework); // счет фактура выставленный
      
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.MVZ); // входящий счет
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.MVZ); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.MVZ); // Входящее письмо
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.MVP); // входящий счет
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.MVP); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.MVP); // Входящее письмо
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.AccArts); // входящий счет
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.AccArts); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.AccArts); // Входящее письмо
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.BudgetItem); // входящий счет
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.BudgetItem); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.BudgetItem); // Входящее письмо

      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.ActExists); // исходящее письмо
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.DeviceExists); // исходящее письмо
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.FactOfPayment); // исходящее письмо
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.PricesAgreed); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.ActExists); // Входящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.DeviceExists); // Входящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.FactOfPayment); // Входящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.PricesAgreed); // Входящее письмо

      return baseSupport;

    }
    
    #endregion
  }
}