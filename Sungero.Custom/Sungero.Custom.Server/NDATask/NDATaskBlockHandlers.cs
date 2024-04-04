using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using Sungero.Custom.NDATask;

namespace Sungero.Custom.Server.NDATaskBlocks
{
  partial class EditLifeHandlers
  {

    public virtual void EditLifeExecute()
    {
      if (_block.LifeCycleState != null)
      {
        var Doc = _obj.BaseDocNDA.NDAs.FirstOrDefault();
        if (Doc != null)
        {
          if (_block.LifeCycleState.Value.Value == "Draft")
            Doc.LifeCycleState = Custom.NDA.LifeCycleState.Draft;
          
          if (_block.LifeCycleState.Value.Value == "Active")
            Doc.LifeCycleState = Custom.NDA.LifeCycleState.Active;
          
          if (_block.LifeCycleState.Value.Value == "Obsolete")
            Doc.LifeCycleState = Custom.NDA.LifeCycleState.Obsolete;
          
          Doc.Save();
        }
      }
    }
  }


  partial class AuthorJobHandlers
  {

    public virtual void AuthorJobStartAssignment(Sungero.Custom.IAuthorJob assignment)
    {
      if (_obj.TravelDoc == Custom.NDATask.TravelDoc.Kurier)
        assignment.TravelStr = Custom.AuthorJob.TravelStr.Paper;
      else
        assignment.TravelStr = Custom.AuthorJob.TravelStr.Electronic;
    }
  }

  partial class PlusAssignmentHandlers
  {

    public virtual void PlusAssignmentStartAssignment(Sungero.Custom.IPlusAssignment assignment)
    {
      if (_obj.TravelDoc == Custom.NDATask.TravelDoc.Kurier)
        assignment.TravelStr = Custom.PlusAssignment.TravelStr.Paper;
      else
        assignment.TravelStr = Custom.PlusAssignment.TravelStr.Electronic;
    }
  }

  partial class StandartAssignmentHandlers
  {

    public virtual void StandartAssignmentStartAssignment(Sungero.Custom.IStandartAssignment assignment)
    {
      if (_obj.TravelDoc == Custom.NDATask.TravelDoc.Kurier)
        assignment.TravelStr = Custom.StandartAssignment.TravelStr.Paper;
      else
        assignment.TravelStr = Custom.StandartAssignment.TravelStr.Electronic;
    }
  }

}