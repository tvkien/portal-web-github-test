using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web
{
    [Serializable]
    public class PrintBubbleSheetDownloadModel
    {
        public DateTime GeneratedDateTime { get; set; } 
        public string TestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string GroupName { get; set; }
        public string DownloadUrl { get; set; }
    }
}