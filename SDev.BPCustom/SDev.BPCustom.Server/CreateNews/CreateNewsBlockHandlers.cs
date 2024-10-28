using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using SDev.BPCustom.CreateNews;

namespace SDev.BPCustom.Server.CreateNewsBlocks
{
  partial class NewsNoticeHandlers
  {

    public virtual void NewsNoticeStartNotice(SDev.BPCustom.INewsNotice notice)
    {
      if (_obj.TypeNews != null)
        notice.TypeNews = _obj.TypeNews;
    }
  }




}