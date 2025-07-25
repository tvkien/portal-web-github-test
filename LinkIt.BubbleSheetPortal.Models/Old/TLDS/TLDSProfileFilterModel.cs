using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSProfileFilterModel
    {
        public int ProfileID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Status { get; set; }
        public DateTime? LastStatusDate { get; set; }
        public int ? UpcommingSchoolID { get; set; }
        public string SchoolName { get; set; }
        public int? UserId { get; set; }
        public bool Viewable { get; set; }
        public bool Updateable { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ECSName { get; set; }
        public bool? Section102IsNotRequired { get; set; }
        public DateTime? ECSCompledDate { get; set; }
        public string ECSCompletingFormEducatorName { get; set; }
        public int? GenderID { get; set; }
        public int? StudentID { get; set; }
        public string IsUploadedStatement { get; set; }
        public string UploadedStatementPdfFileName { get; set; }
        public int? EnrolmentYear { get; set; }

        public int? TldsGroupID { get; set; }

        public string GroupName { get; set; }

        public bool StatusGroup { get; set; }
    }
}
