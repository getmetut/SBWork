using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using sberdev.SBContracts.ApprovalAssignment;

namespace sberdev.SBContracts
{
  partial class ApprovalAssignmentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      var approvalStage = sberdev.SBContracts.ApprovalStages.As(_obj.Stage);
      var performer = Users.As(_obj.Performer);
      
      if ( ( ! approvalStage.AmountChangesberdev.GetValueOrDefault()) || ( ! performer.IncludedIn( sberdev.SberContracts.PublicConstants.Module.KZTypeGuid )) )
      {e.HideAction(_obj.Info.Actions.AmountChangesberdev);}
      base.Showing(e);
    }

  }
}