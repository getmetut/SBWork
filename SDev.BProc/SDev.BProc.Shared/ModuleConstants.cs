using System;
using Sungero.Core;

namespace SDev.BProc.Constants
{
  public static class Module
  {
    /// <summary>
    /// Статусы проверки Контур Фокус.
    /// </summary>
    public static class KonturFocusCheckStatus
    {
      [Public]
      public static Enumeration Actual = new Enumeration("Actual");

      [Public]
      public static Enumeration DateOutdated = new Enumeration("DateOutdated");

      [Public]
      public static Enumeration Failed = new Enumeration("Failed");
    }
  }
}