using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Structures.Module
{

  /// <summary>
  /// 
  /// </summary>
  partial class  Sender
  {
    public string Name { get; set; }
    public string Message { get; set; }
  }
  
  /// <summary>
  /// Информация о значениях серии в графике ControlFlowChart
  /// </summary>
  [Public]
  partial class ControlFlowSeriesInfo
  {
    public string ValueId { get; set; }
    public string Label { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B{ get; set; }
  }
  
  [Public]
  partial class DateRange
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }
}