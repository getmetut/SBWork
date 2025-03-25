using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace sberdev.SBContracts.Structures.Module
{
  /// <summary>
  /// DTO класс для сериализации/десериализации структуры AssignApprSeriesInfo.
  /// </summary>
  [Public]
  partial class AssignApprSeriesInfoDto
  {
    public int Completed { get; set; }
    public int Expired { get; set; }
  }

  /// <summary>
  /// 
  /// </summary>
  partial class  Sender
  {
    public string Name { get; set; }
    public string Message { get; set; }
  }
  
  /// <summary>
  /// Информация о значениях серии в графике TaskDeadlineChart
  /// </summary>
  [Public]
  partial class TaskDeadlineSeriesInfo
  {
    public List<double> Values { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B{ get; set; }
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
  
  /// <summary>
  /// Информация о значениях серии в графиках
  /// </summary>
  [Public]
  partial class AssignApprSeriesInfo
  {
    public double Completed { get; set; }
    public double Expired { get; set; }
  }
}