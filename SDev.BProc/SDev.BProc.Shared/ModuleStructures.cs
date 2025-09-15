using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Content;

namespace SDev.BProc.Structures.Module
{
  /// <summary>
  /// Результат отдельной проверки.
  /// </summary>
  [Public]
  partial class KonturFocusCheckResult
  {
    public DateTime Date { get; set; }
    public string Result { get; set; }
  }

  /// <summary>
  /// Данные по проверкам Контур Фокус.
  /// </summary>
  [Public]
  partial class KonturFocusCheck
  {
    public Enumeration Status { get; set; }
    public List<KonturFocusCheckResult> Checks { get; set; }
    public List<IElectronicDocument> Files { get; set; }
  }
}