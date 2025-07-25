using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BankDistrict
    {
        public int BankDistrictId { get; set; }
        public int BankId { get; set; }
        public int DistrictId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EditedByUserId { get; set; }
        public int? BankDistrictAccessId { get; set; }
        public string BankName { get; set; }
        public int SubjectId { get; set; }
        public int CreatedByUserID { get; set; }
        public string Name { get; set; }//DistrictName
        public int StateId { get; set; }
        public string LiCode { get; set; }
        public bool Hide { get; set; }

    }
}
