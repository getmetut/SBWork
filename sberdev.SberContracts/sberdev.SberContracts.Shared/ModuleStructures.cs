using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SberContracts.Structures.Module
{
  /// <summary>
  /// Результат отдельной проверки.
  /// </summary>
  [Public]
  partial class FocusCheckResult
  {
    public DateTime Date { get; set; }
    public string Result { get; set; }
  }

  /// <summary>
  /// Данные по проверкам Контур Фокус.
  /// </summary>
  [Public]
  partial class FocusCheckInfo
  {
    public Sungero.Core.Enumeration Status { get; set; }
    public List<sberdev.SberContracts.Structures.Module.IFocusCheckResult> Checks { get; set; }
    public List<Sungero.Content.IElectronicDocument> Files { get; set; }
  }
}