using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BankSchool
    {
        public int BankSchoolId { get; set; }
        public int BankId { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }//School Name
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EditedByUserId { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
    }
}
