using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AblesReportJobViewModel
    {
        public int AblesReportJobId { get; set; }        
        public string ReportName { get; set; }
        public string LearningArea { get; set; }
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string AssessmentRound { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DownloadUrl { get; set; }
        public string CreatedDateString { get; set; }        
    }
}