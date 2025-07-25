using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentProgramManage
    {
        public int StudentProgramID { get; set; }
        public int ProgramID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DistrictID { get; set; }
        public int StudentID { get; set; }
        public string StudentCode { get; set; }
        public string AltCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderID { get; set; }
        public int? RaceID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string LoginCode { get; set; }
        public DateTime? Dateofbirth { get; set; }
        public int? PrimaryLanguageID { get; set; }
        public int? Status { get; set; }
        public int? SISID { get; set; }
        public string StateCode { get; set; }
        public string Note01 { get; set; }
        public int? CurrentGradeID { get; set; }
        public int? AdminSchoolID { get; set; }
    }
}
