using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker
{
    public class ScoreTypeDrapDropModel
    {
        public int ScoreId { get; set; }
        public string Name { get; set; }
        public string ScoreTypeName { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public bool IsFromScore { get; set; }
        public bool? IsSubSub { get; set; }
        public bool? IsReplace { get; set; }
        public string NameReplace { get; set; }
        public string ScoreTypeNameReplace { get; set; }
        public int RawIndex { get; set; }
    }
}
