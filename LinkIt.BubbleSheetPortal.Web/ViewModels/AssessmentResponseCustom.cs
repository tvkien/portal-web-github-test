using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AssessmentResponseCustom
    {
        public string DISTRICT_CODE { get; set; }
        public string ASSESSMENT_SCHOOL_YEAR_DATE { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string TEST_DATE { get; set; }
        public string STUDENT_ID { get; set; }
        public string ITEM_RESPONSE_DESCRIPTION { get; set; }
        public string ALPHA_VALUE { get; set; }
        public string RAW_SCORE { get; set; }
    }
}