using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryCustomScore
    {
        public int VirtualTestCustomScoreId { get; set; } 
        public string Name { get; set; }
        public List<ResultEntryScoreModel> ScoreInfos { get; set; }
    }
}
