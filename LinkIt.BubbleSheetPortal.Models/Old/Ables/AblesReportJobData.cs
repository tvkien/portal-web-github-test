using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesReportJobData
    {
        public int AblesReportJobId { get; set; }
        public int ReportTypeId { get; set; }
        public string ReportName { get; set; }
        public string LearningArea { get; set; }
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string AssessmentRound { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DownloadUrl { get; set; }
        public int Status { get; set; }
    }
}