using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class ReportFileResultModel
    {
        // [Report Name]_[Report Generated Time]_[Teacher Last Name]_[Class Name]_[Report #].pdf 
        public string TeacherLastName { get; set; }
        public string ClassName { get; set; }
        public List<byte[]> PdfFiles { get; set; }
    }
}