using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentTeacher
    {
        public int StudentId { get; set; }
        public int? Status { get; set; }
        public int DistrictId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string Gender { get; set; }
        public string Grade { get; set; }
        public int? ClassId { get; set; }
        public int? UserId { get; set; }
        public int? AdminSchoolId { get; set; }
    }
}
