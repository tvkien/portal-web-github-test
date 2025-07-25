using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.UserBank
{

    public class LoadBankByMultipleSubjectIdsCriteria
    {
        public int? GradeId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserRole { get; set; }
        public string SubjectName { get; set; }
        public string SubjectIds { get; set; }
        public bool? IsFromMultiDate { get; set; }
        public bool UsingMultiDate { get; set; }
    }
}
