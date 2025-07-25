using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class NewACTReportData : ValidatableEntity<NewACTReportData>
    {        
        public int TestId { get; set; }
        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }        
        public int? TeacherId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? ClassId { get; set; }

        public DateTime? ResultDateFrom { get; set; }
        public DateTime? ResultDateTo { get; set; }

        public string StrTestIdList { get; set; }
        private List<string> studentIdList = new List<string>();
        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public int TimezoneOffset { get; set; }

        public string ActReportFileName { get; set; }
        

        // 1: Scores only; 2: Scores and Essays; 3: Essays only
        public int ReportContentOption { get; set; }

        public int? StateInformationId { get; set; } // Include state information into Knowsys SAT report

        public int SpecializedReportJobId { get; set; }
        public bool? isGetAllClass { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
