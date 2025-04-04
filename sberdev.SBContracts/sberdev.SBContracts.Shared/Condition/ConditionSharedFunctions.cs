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
      
      var isProduct = _obj.ConditionType == ConditionType.Product;
      _obj.State.Properties.ProductsSberDev.IsVisible = isProduct;
      _obj.State.Properties.ProductsSberDev.IsRequired = isProduct;
      
      var isInitiatorsDepartment = _obj.ConditionType == ConditionType.InitDepart;
      _obj.State.Properties.InitiatorsDepartmentSberDev.IsVisible = isInitiatorsDepartment;
      _obj.State.Properties.InitiatorsDepartmentSberDev.IsRequired = isInitiatorsDepartment;
      
      var isPurchseAmount = _obj.ConditionType == ConditionType.PurchAmount;
      var isTotalAmount = _obj.ConditionType == ConditionType.AmountIsMore;
      _obj.State.Properties.PurchaseAmountSberDev.IsVisible = isPurchseAmount;
      _obj.State.Properties.PurchaseAmountSberDev.IsRequired = isPurchseAmount;
      _obj.State.Properties.AmountOperator.IsVisible = isPurchseAmount || isTotalAmount;
      _obj.State.Properties.AmountOperator.IsRequired = isPurchseAmount || isTotalAmount;
      
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
      
      var isMarketDirect = _obj.ConditionType == ConditionType.MarketDirect;
      _obj.State.Properties.MarketDirectSberDev.IsVisible = isMarketDirect;
      _obj.State.Properties.MarketDirectSberDev.IsRequired = isMarketDirect;
      
      var isPlMin = _obj.ConditionType == ConditionType.PlusMinus;
      _obj.State.Properties.PlusMinusSDev.IsVisible = isPlMin;
      _obj.State.Properties.PlusMinusSDev.IsRequired = isPlMin;
      _obj.State.Properties.SummPriznSDev.IsVisible = isPlMin;
      _obj.State.Properties.TotalSummSDev.IsVisible = isPlMin;
      _obj.State.Properties.SummPriznSDev.IsRequired = isPlMin;
      _obj.State.Properties.TotalSummSDev.IsRequired = isPlMin;
      
      var isINN = _obj.ConditionType == ConditionType.INNCollection;
      _obj.State.Properties.CollectionINNSDev.IsVisible = isINN;
      _obj.State.Properties.CollectionINNSDev.IsRequired = isINN;
    }

    public override void ClearHiddenProperties()
    {
      base.ClearHiddenProperties();
      
      if(!_obj.State.Properties.ProductsSberDev.IsVisible)
        _obj.ProductsSberDev.Clear();
      
      if(!_obj.State.Properties.PurchaseAmountSberDev.IsVisible)
      {
        _obj.PurchaseAmountSberDev = null;
        _obj.AmountOperator = null;
      }
      
      if(!_obj.State.Properties.InitiatorsDepartmentSberDev.IsVisible)
        _obj.InitiatorsDepartmentSberDev.Clear();
      
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
      
      if (!_obj.State.Properties.MarketDirectSberDev.IsVisible)
        _obj.MarketDirectSberDev.Clear();
      
      if (!_obj.State.Properties.SummPriznSDev.IsVisible)
        _obj.SummPriznSDev = null;
      
      if (!_obj.State.Properties.PlusMinusSDev.IsVisible)
        _obj.PlusMinusSDev = null;
      
      if (!_obj.State.Properties.CollectionINNSDev.IsVisible)
        _obj.CollectionINNSDev.Clear();
    }
    
    #endregion
    
    #region Механика условий
    
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(
      Sungero.Docflow.IOfficialDocument document,
      Sungero.Docflow.IApprovalTask task)
    {
      #region Проверка на наличие указанных продуктов (Product)
      if (_obj.ConditionType == ConditionType.Product)
      {
        bool flag = false;
        var products = SBContracts.PublicFunctions.OfficialDocument.GetDocumentProducts(SBContracts.OfficialDocuments.As(document));
        if (!products.Any())
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
        var targetProductList = _obj.ProductsSberDev.Select(p => p.Product);
        var targetProductsIds = new HashSet<long>(targetProductList.Select(p => p.Id));
        flag = products.Any(product => targetProductsIds.Contains(product.Id));
        
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion
      
      #region Проверка: МВЗ для производства (IsProdPurchase)
      if (_obj.ConditionType == ConditionType.IsProdPurchase)
      {
        bool flag = false;
        var purch = SberContracts.Purchases.As(document);
        if (purch != null)
          flag = purch.MVZBaseSberDev?.ProductionPurchase
            ?? purch.MVPBaseSberDev?.ProductionPurchase
            ?? false;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Подразделение инициатора (InitDepart)
      if (_obj.ConditionType == ConditionType.InitDepart)
      {
        bool flag = false;
        var initDep = Sungero.Company.Employees.As(task.Author).Department;
        foreach (var dep in _obj.InitiatorsDepartmentSberDev)
          if (dep.InitiatorsDepartment == initDep)
            flag = true;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Совпадает ли доверенность с какой либо (SameAttorney)
      if (_obj.ConditionType == ConditionType.SameAttorney)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        bool flag = Functions.Condition.Remote.CheckSameAttorney(_obj, power);
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Является ли автор задачи лицом, которому выдана доверенность (InitIsAttorney)
      if (_obj.ConditionType == ConditionType.InitIsAttorney)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        bool flag = task.Author == Sungero.CoreEntities.Users.As(power.IssuedTo);
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Признак MRP (MRP)
      if (_obj.ConditionType == ConditionType.MRP)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        var matrix = power.MatrixSDev;
        bool flag = false;
        if (matrix != null)
          flag = matrix.Mahineread.HasValue ? matrix.Mahineread.Value : false;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Признак "ранней" доверенности (EarlyProxy)
      if (_obj.ConditionType == ConditionType.EarlyProxy)
      {
        var power = Sungero.ATS.PowerOfAttorneys.As(document);
        bool flag = power.FirstOrDoubleSDev.HasValue ? power.FirstOrDoubleSDev.Value : false;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Сумма закупки (PurchAmount)
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
      #endregion

      #region Проверка: Тип договора (Contrtype)
      if (_obj.ConditionType == ConditionType.Contrtype)
      {
        var acc = SBContracts.AccountingDocumentBases.As(document);
        bool flag = acc.ContrTypeBaseSberDev == SBContracts.AccountingDocumentBase.ContrTypeBaseSberDev.Profitable;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Бизнес юнит (ProductUnit)
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
      #endregion

      #region Проверка: Есть ли подпись (EndorseFromSberDev)
      if (_obj.ConditionType == ConditionType.EndorseFromSberDev)
      {
        var signInfos = PublicFunctions.Module.GetSignatures(document.LastVersion);
        bool flag = false;
        foreach (var singInfo in signInfos)
        {
          if ((singInfo.Signatory == _obj.EndorserSberDev ||
               singInfo.SubstitutedUser == _obj.EndorserSberDev) &&
              singInfo.SignatureType != SignatureType.NotEndorsing)
          {
            flag = true;
            break;
          }
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Есть ли согласование на КЗД (FCDApprBySberDev)
      if (_obj.ConditionType == ConditionType.FCDApprBySberDev)
      {
        bool flag = false;
        var incInv = SBContracts.IncomingInvoices.As(document);
        var fcd = incInv.AccDocSberDev;
        if (fcd != null && fcd.HasVersions)
        {
          var signInfos = PublicFunctions.Module.GetSignatures(fcd.LastVersion);
          foreach (var signInfo in signInfos)
          {
            if (signInfo.IsValid &&
                (signInfo.SubstitutedUser == _obj.EndorserSberDev ||
                 signInfo.Signatory == _obj.EndorserSberDev))
            {
              flag = true;
              break;
            }
          }
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Направление маркетинга (MarketDirect)
      if (_obj.ConditionType == ConditionType.MarketDirect)
      {
        var acc = SBContracts.AccountingDocumentBases.As(document);
        bool flag = false;
        foreach (var dir in _obj.MarketDirectSberDev)
          if (dir.MarketDirect == acc.MarketDirectSberDev)
            flag = true;
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(flag, string.Empty);
      }
      #endregion

      #region Проверка: Подписан ли основной документ (ContractSigned)
      if (_obj.ConditionType == ConditionType.ContractSigned)
      {
        var lead = SberContracts.GuaranteeLetters.As(document).LeadingDocument;
        if (lead != null)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            PublicFunctions.Module.CheckSpecialGroupSignature(SBContracts.OfficialDocuments.As(lead), true),
            string.Empty);
        }
        else
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
        }
      }
      #endregion

      #region Проверка: Утверждено ли закупками (PurchApproved)
      if (_obj.ConditionType == ConditionType.PurchApproved)
      {
        var salut = Sungero.Company.PublicFunctions.BusinessUnit.Remote
          .GetBusinessUnits().Where(u => u.TIN == "7730253720").FirstOrDefault();
        var depart = SBContracts.PublicFunctions.Module.Remote.GetGroup("Закупки", salut);
        var purchase = SberContracts.GuaranteeLetters.As(document).AddendumDocument;

        if (depart != null && purchase != null)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            PublicFunctions.Module.CheckDepartmentApproval(purchase, depart),
            string.Empty);
        }
        else
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
        }
      }
      #endregion

      #region Проверка: Изменён ли документ после первой подписи (DocumentChanged)
      if (_obj.ConditionType == ConditionType.DocumentChanged)
      {
        var contr = SBContracts.ContractualDocuments.As(document);
        var firstApprove = PublicFunctions.Module.GetSignatures(document.LastVersion).FirstOrDefault();
        if (firstApprove == null)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            false,
            "Отбивка. Маршрут будет вычеслен по ходу согласования.");
        }

        if (contr.ModifiedSberDev == null)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            false,
            "Отбивка. Маршрут будет вычеслен по ходу согласования.");
        }

        if (contr != null)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            Calendar.Between(contr.ModifiedSberDev,
                             firstApprove.SigningDate.AddHours(3),
                             Calendar.Now)
            || Calendar.Between(contr.LastVersion.Modified,
                                firstApprove.SigningDate.AddHours(3),
                                Calendar.Now),
            string.Empty);
        }
      }
      #endregion

      #region Проверка: Необходима ли ручная проверка (ManuallyCheck)
      if (_obj.ConditionType == ConditionType.ManuallyCheck)
      {
        var sbTask = SBContracts.ApprovalTasks.As(task);
        bool flagContract = false, flagStatement = false, flagTask = false;
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

        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
          (!flagContract && !(flagStatement &&
                              invoice.PayTypeBaseSberDev.Value ==
                              SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay))
          || flagTask,
          string.Empty);
      }
      #endregion

      #region Проверка: Тип оплаты (PayType)
      if (_obj.ConditionType == ConditionType.PayType)
      {
        var find = false;
        // Входящий счёт
        var IncInv = SBContracts.IncomingInvoices.As(document);
        if (IncInv != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            IncInv.PayTypeBaseSberDev == sberdev.SBContracts.IncomingInvoice.PayTypeBaseSberDev.Postpay,
            string.Empty);
        }

        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Не заполнен тип оплаты.");
        }
      }
      #endregion

      #region Проверка: Признак “Рамочный” (Framework)
      if (_obj.ConditionType == ConditionType.Framework)
      {
        var find = false;
        // Исходящее письмо
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            outLetter.Framework == true,
            string.Empty);
        }

        // Входящее письмо
        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            IncLetter.Framework == true,
            string.Empty);
        }

        // Входящий счёт
        var accDoc = SBContracts.AccountingDocumentBases.As(document);
        if (accDoc != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            accDoc.FrameworkBaseSberDev == true,
            string.Empty);
        }

        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет документа.");
        }
      }
      #endregion

      #region Проверка: МВЗ (MVZ)
      if (_obj.ConditionType == ConditionType.MVZ)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVZ)
          {
            if (outLetter.MVZ == str.MVZ)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVZ)
          {
            if (IncLetter.MVZ == str.MVZ)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var incInv = SBContracts.IncomingInvoices.As(document);
        if (incInv != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVZ)
          {
            if (incInv.MVZBaseSberDev == str.MVZ)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        if (!find)
        {
          var mainContract = SBContracts.Contracts
            .GetAll(e => e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            var ContrFind = false;
            foreach (var str in _obj.MVZ)
            {
              if (mainContract.MVZBaseSberDev == str.MVZ)
                ContrFind = true;
            }
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
              null,
              "Условие не может быть вычислено. Не заполнено МВЗ документа.");
          }
        }
      }
      #endregion

      #region Проверка: MVP (MVP)
      if (_obj.ConditionType == ConditionType.MVP)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVP)
          {
            if (outLetter.MVP == str.MVP)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVP)
          {
            if (IncLetter.MVP == str.MVP)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var incInv = SBContracts.IncomingInvoices.As(document);
        if (incInv != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.MVP)
          {
            if (incInv.MVPBaseSberDev == str.MVP)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        if (!find)
        {
          var mainContract = SBContracts.Contracts
            .GetAll(e => e.Id == document.LeadingDocument.Id).FirstOrDefault();
          if (mainContract != null)
          {
            var ContrFind = false;
            foreach (var str in _obj.MVP)
            {
              if (mainContract.MVPBaseSberDev == str.MVP)
                ContrFind = true;
            }
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
          }
          else
          {
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
              null,
              "Условие не может быть вычислено. Не заполнено МВП документа.");
          }
        }
      }
      #endregion

      #region Проверка: Статьи управленческого учёта (AccArts)
      if (_obj.ConditionType == ConditionType.AccArts)
      {
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles)
            if (outLetter.AccArt == str.AccountingArticles)
              ContrFind = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles)
            if (IncLetter.AccArt == str.AccountingArticles)
              ContrFind = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var acc = SBContracts.AccountingDocumentBases.As(document);
        if (acc != null)
        {
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles)
            if (acc.AccArtBaseSberDev == str.AccountingArticles)
              ContrFind = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }

        var leadDoc = SBContracts.ContractualDocuments.As(document.LeadingDocument);
        if (leadDoc != null)
        {
          var ContrFind = false;
          foreach (var str in _obj.AccountingArticles)
            if (leadDoc.AccArtExBaseSberDev == str.AccountingArticles
                || leadDoc.AccArtPrBaseSberDev == str.AccountingArticles)
              ContrFind = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }
        else
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Не заполнена статья управленческого учета документа.");
        }
      }
      #endregion

      #region Проверка: Наличие акта (ActExists)
      if (_obj.ConditionType == ConditionType.ActExists)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            outLetter.ActExists == true,
            string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            IncLetter.ActExists == true,
            string.Empty);
        }

        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет документа.");
        }
      }
      #endregion

      #region Проверка: Наличие устройства (DeviceExists)
      if (_obj.ConditionType == ConditionType.DeviceExists)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            outLetter.DeviceExists == true,
            string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            IncLetter.DeviceExists == true,
            string.Empty);
        }

        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет документа.");
        }
      }
      #endregion

      #region Проверка: Факт оплаты (FactOfPayment)
      if (_obj.ConditionType == ConditionType.FactOfPayment)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            outLetter.FactOfPayment == true,
            string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            IncLetter.FactOfPayment == true,
            string.Empty);
        }

        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет документа.");
        }
      }
      #endregion

      #region Проверка: Согласованы ли цены (PricesAgreed)
      if (_obj.ConditionType == ConditionType.PricesAgreed)
      {
        var find = false;
        var outLetter = SBContracts.OutgoingLetters.As(document);
        if (outLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            outLetter.PricesAgreed == true,
            string.Empty);
        }

        var IncLetter = SBContracts.IncomingLetters.As(document);
        if (IncLetter != null)
        {
          find = true;
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            IncLetter.PricesAgreed == true,
            string.Empty);
        }

        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет документа.");
        }
      }
      #endregion
      
      #region Проверка: Контроль документа на указанный признак доходности
      if (_obj.ConditionType == ConditionType.PlusMinus)
      {
        var contra = false;
        var ContractStat = SBContracts.ContractStatements.As(document);
        if (ContractStat != null)
        {
          contra = true;
          var ContraSumm = ContractStat.ContrTypeBaseSberDev;
          
          bool ctrl = false;
          
          if (_obj.SummPriznSDev == SBContracts.ContractCondition.PlusMinusSDevSDev.Profitable)
            ctrl = ContraSumm == SBContracts.ContractStatement.ContrTypeBaseSberDev.Profitable;
          
          if (_obj.SummPriznSDev == SBContracts.ContractCondition.PlusMinusSDevSDev.Expendable)
            ctrl = ContraSumm == SBContracts.ContractStatement.ContrTypeBaseSberDev.Expendable;
          
          if (_obj.SummPriznSDev == SBContracts.ContractCondition.PlusMinusSDevSDev.ExpendProfit)
            ctrl = false;
          
          if (ctrl)
          {
            double ContraSumm2 = 0;
            if (ContractStat.TotalAmount.HasValue)
              ContraSumm2 = ContractStat.TotalAmount.Value;
            
            ctrl = false;
            
            if (_obj.SummPriznSDev == SBContracts.ContractCondition.SummPriznSDevSDev.Big)
              ctrl = ContraSumm2 > _obj.TotalSummSDev.Value;
            
            if (_obj.SummPriznSDev == SBContracts.ContractCondition.SummPriznSDevSDev.Little)
              ctrl = ContraSumm2 < _obj.TotalSummSDev.Value;
            
            if (_obj.SummPriznSDev == SBContracts.ContractCondition.SummPriznSDevSDev.Identy)
              ctrl = ContraSumm2 == _obj.TotalSummSDev.Value;
          }
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            ctrl,
            string.Empty);
        }

        var UPD = Sungero.ATS.UniversalTransferDocuments.As(document);
        if (UPD != null)
        {
          contra = true;
          var ContraSumm = UPD.ContrTypeBaseSberDev;
          
          bool ctrl = false;
          
          if (_obj.SummPriznSDev == SBContracts.ContractCondition.PlusMinusSDevSDev.Profitable)
            ctrl = ContraSumm == Sungero.ATS.UniversalTransferDocument.ContrTypeBaseSberDev.Profitable;
          
          if (_obj.SummPriznSDev == SBContracts.ContractCondition.PlusMinusSDevSDev.Expendable)
            ctrl = ContraSumm == Sungero.ATS.UniversalTransferDocument.ContrTypeBaseSberDev.Expendable;
          
          if (_obj.SummPriznSDev == SBContracts.ContractCondition.PlusMinusSDevSDev.ExpendProfit)
            ctrl = false;
          
          if (ctrl)
          {
            double ContraSumm2 = 0;
            if (UPD.TotalAmount.HasValue)
              ContraSumm2 = UPD.TotalAmount.Value;
            
            ctrl = false;
            
            if (_obj.SummPriznSDev == SBContracts.ContractCondition.SummPriznSDevSDev.Big)
              ctrl = ContraSumm2 > _obj.TotalSummSDev.Value;
            
            if (_obj.SummPriznSDev == SBContracts.ContractCondition.SummPriznSDevSDev.Little)
              ctrl = ContraSumm2 < _obj.TotalSummSDev.Value;
            
            if (_obj.SummPriznSDev == SBContracts.ContractCondition.SummPriznSDevSDev.Identy)
              ctrl = ContraSumm2 == _obj.TotalSummSDev.Value;
          }
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            ctrl,
            string.Empty);
        }
        
        if (!contra)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет необходимого документа.");
        }
      }
      #endregion
      
      #region Проверка: Коллекция ИНН (ИНН)
      if (_obj.ConditionType == ConditionType.INNCollection)
      {
        var find = false;
        var ContractStat = SBContracts.ContractStatements.As(document);
        if (ContractStat != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.CollectionINNSDev)
          {
            if (ContractStat.Counterparty.TIN == str.INN)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }
        
        var UPD = Sungero.ATS.UniversalTransferDocuments.As(document);
        if (UPD != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.CollectionINNSDev)
          {
            if (UPD.Counterparty.TIN == str.INN)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }
       
        var Purchas = SberContracts.Purchases.As(document);
        if (Purchas != null)
        {
          find = true;
          var ContrFind = false;
          foreach (var str in _obj.CollectionINNSDev)
          {
            if (Purchas.Counterparty.TIN == str.INN)
              ContrFind = true;
          }
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(ContrFind, string.Empty);
        }
       
        if (!find)
        {
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(
            null,
            "Условие не может быть вычислено. Нет подходящего документа с КА и его ИНН.");
        }
      }
      #endregion

      // Если ни одно из условий не выполнилось, возвращаем базовую реализацию:
      return base.CheckCondition(document, task);
    }
    
    #endregion
    
    #region Поддерживаемые типы
    
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      var baseSupport = base.GetSupportedConditions();
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.Product); // входящий счет
      
      baseSupport["7aa8969f-f81d-462c-b0d8-761ccd59253f"].Add(ConditionType.IsProdPurchase); // purchase
      
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.InitDepart); // входящий счет
      
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
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.Framework); // исходящий счет
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
      baseSupport["a523a263-bc00-40f9-810d-f582bae2205d"].Add(ConditionType.AccArts); // исходящий счет

      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.ActExists); // исходящее письмо
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.DeviceExists); // исходящее письмо
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.FactOfPayment); // исходящее письмо
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.PricesAgreed); // исходящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.ActExists); // Входящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.DeviceExists); // Входящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.FactOfPayment); // Входящее письмо
      baseSupport["8dd00491-8fd0-4a7a-9cf3-8b6dc2e6455d"].Add(ConditionType.PricesAgreed); // Входящее письмо
      
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.PlusMinus); // Sungero.FinancialArchive.UniversalTransferDocument
      baseSupport["58986e23-2b0a-4082-af37-bd1991bc6f7e"].Add(ConditionType.INNCollection); // Sungero.FinancialArchive.UniversalTransferDocument
      
      baseSupport["14a59623-89a2-4ea8-b6e9-2ad4365f358c"].Add(ConditionType.INNCollection);
      baseSupport["14a59623-89a2-4ea8-b6e9-2ad4365f358c"].Add(ConditionType.PlusMinus);
      
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.PlusMinus); // Sungero.FinancialArchive.ContractStatement
      baseSupport["f2f5774d-5ca3-4725-b31d-ac618f6b8850"].Add(ConditionType.INNCollection); // Sungero.FinancialArchive.ContractStatement
      
      baseSupport["7aa8969f-f81d-462c-b0d8-761ccd59253f"].Add(ConditionType.INNCollection); // purchase

      return baseSupport;
    }
    
    #endregion
  }
}