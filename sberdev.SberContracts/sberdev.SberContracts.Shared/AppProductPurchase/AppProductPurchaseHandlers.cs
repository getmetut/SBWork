using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SberContracts.AppProductPurchase;

namespace sberdev.SberContracts
{
  partial class AppProductPurchaseComparativeCollection7SharedCollectionHandlers
  {

    public virtual void ComparativeCollection7Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var col = _obj.ComparativeCollection7;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseComparativeCollection6SharedCollectionHandlers
  {

    public virtual void ComparativeCollection6Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var col = _obj.ComparativeCollection6;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseComparativeCollection5SharedCollectionHandlers
  {

    public virtual void ComparativeCollection5Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var col = _obj.ComparativeCollection5;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseComparativeCollection4SharedCollectionHandlers
  {

    public virtual void ComparativeCollection4Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var col = _obj.ComparativeCollection4;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseComparativeCollection3SharedCollectionHandlers
  {

    public virtual void ComparativeCollection3Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      
      var col = _obj.ComparativeCollection3;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseComparativeCollection2SharedCollectionHandlers
  {

    public virtual void ComparativeCollection2Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      
      var col = _obj.ComparativeCollection2;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseComparativeCollection1SharedCollectionHandlers
  {

    public virtual void ComparativeCollection1Added(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var col = _obj.ComparativeCollection1;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchaseParticipantsCollectionSharedCollectionHandlers
  {

    public virtual void ParticipantsCollectionAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      
      var col = _obj.ParticipantsCollection;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }
  }

  partial class AppProductPurchasePurchasesCollectionSharedCollectionHandlers
  {

    public virtual void PurchasesCollectionAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var col = _obj.PurchasesCollection;
      
      if (col.Count > 0)
      {
        // Собираем все существующие номера
        var existingNumbers = col
          .Where(item => item.Number.HasValue)
          .Select(item => item.Number.Value)
          .OrderBy(num => num)
          .ToList();

        // Находим первый пропущенный номер
        int missingNumber = 1;
        foreach (var number in existingNumbers)
        {
          if (number != missingNumber)
            break;
          missingNumber++;
        }

        // Присваиваем первый пропущенный номер последнему элементу коллекции
        col.Last().Number = missingNumber;
      }
    }

  }

  partial class AppProductPurchaseSharedHandlers
  {

    public virtual void AgencyContractChanged(sberdev.SberContracts.Shared.AppProductPurchaseAgencyContractChangedEventArgs e)
    {
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddOrUpdate("Addendum", e.OldValue, e.NewValue);
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      _obj.ModifiedSberDev = Calendar.Now;
      _obj.Relations.AddFromOrUpdate("Purchase", e.OldValue, e.NewValue);
    }

    public virtual void DepositChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      _obj.Balance = 100 - e.NewValue;
    }

  }
}