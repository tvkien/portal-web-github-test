using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorConfigurationDTO
    {
        public int NavigatorConfigurationID { get; set; }

        public bool UseSchool { get; set; }
        public bool UseUser { get; set; }
        public bool UseClass { get; set; }
        public bool UseStudent { get; set; }
        public bool CanPublishDistrictAdmin { get; set; }
        public bool CanPublishSchoolAdmin { get; set; }
        public bool CanPublishTeacher { get; set; }
        public bool CanPublishStudent { get; set; }
        public bool KeywordMandatory { get; set; }
        public string ReportName { get; set; }
        public string ShortName { get; set; }
        public int NavigatorCategoryID { get; set; }
        public bool PeriodMandatory { get; set; }
        public string ReportTypePattern { get; set; }
        public string SchoolPattern { get; set; }
        public string SuffixPattern { get; set; }
    }
}
