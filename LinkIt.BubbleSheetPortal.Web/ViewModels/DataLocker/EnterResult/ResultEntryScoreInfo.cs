using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryScoreInfo
    {
        public string ScoreName { get; set; }
        public string ScoreLable { get; set; }
        public int Order { get; set; }

        public VirtualTestCustomMetaModel MetaData { get; set; }
    }
}
