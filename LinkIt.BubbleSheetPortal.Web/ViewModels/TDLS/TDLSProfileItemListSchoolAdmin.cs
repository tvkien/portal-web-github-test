using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TDLSProfileItemListSchoolAdmin
    {
        public int ProfileId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderID { get; set; }
        public string ECSName { get; set; }
        public bool? Section102IsNotRequired { get; set; }
        public int? EnrolmentYear { get; set; }
        public int? StudentID { get; set; }
        public string IsUploadedStatement { get; set; }
        public string PDFUrl { get; set; }

        public string GenderName
        {
            get
            {
                if (GenderID == 24)
                {
                    return "Female";
                }
                if (GenderID == 25)
                {
                    return "Male";
                }
                if (GenderID == 26)
                {
                    return "Unknown";
                }
                return string.Empty;
            }
        }

        public string DateOfBirthString
        {
            get { return DateOfBirth.DisplayDateWithFormat(); }
        }
        public DateTime? ECSCompledDate { get; set; }

        public int Status { get; set; }
    }
}
