using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalSimpleAssignment;

namespace sberdev.SBContracts.Client
{
  partial class ApprovalSimpleAssignmentFunctions
  {

    [Public]
    public string ShowCheckUCNDialog()
    {
      var dialog = Dialogs.CreateInputDialog(sberdev.SBContracts.ApprovalSimpleAssignments.Resources.CheckUCNDialogTitle);
      CommonLibrary.IStringDialogValue ucn = dialog.AddString(sberdev.SBContracts.ApprovalSimpleAssignments.Resources.CheckUCNDialogProp, true)
        .WithPlaceholder("00000000/0000/0000/0/0").WithLabel(sberdev.SBContracts.ApprovalSimpleAssignments.Resources.CheckUCNDialogLabel);
      ucn.SetOnValueChanged(u => {
                              if (u.NewValue == null || u.NewValue == u.OldValue)
                                return;
                              
                              ucn.Value = CheckUCNProperty(u.NewValue);
                            });

      if (dialog.Show() == DialogButtons.Ok)
        return ucn.Value;
      else
        return null;
    }
    
    public string CheckUCNProperty(string input)
    {
      // Проверяем, если входная строка состоит из 18 цифр
      if (input.Length == 18 && long.TryParse(input, out _))
      {
        // Формируем строку по формату "00000000/0000/0000/0/0"
        return $"{input.Substring(0, 8)}/{input.Substring(8, 4)}/{input.Substring(12, 4)}/{input.Substring(16, 1)}/{input.Substring(17, 1)}";
      }
      // Проверяем, если строка уже в нужном формате
      else if (input.Length == 22 &&
               input[8] == '/' && input[13] == '/' &&
               input[18] == '/' && input[20] == '/')
      {
        return input;
      }
      // Если строка не соответствует ни одному из форматов, возвращаем null
      else
      {
        return null;
      }
    }

  }
}