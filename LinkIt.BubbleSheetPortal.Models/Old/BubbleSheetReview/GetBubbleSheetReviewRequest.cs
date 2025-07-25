using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetReview
{
    public class GetBubbleSheetReviewRequest : PaggingInfo
    {
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool Archived { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SchoolId { get; set; }

        public string TestName { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }

        public string GeneralSearch { get; set; }
    }
}