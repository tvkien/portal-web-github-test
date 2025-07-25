using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractUserCustom
    {
        public int DistrictId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }       
        public int SchoolId { get; set; }        
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }

        public string ListSchoolIDs { get; set; }
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string SearchBox { get; set; }
        public string ColumnSearchs { get; set; }
    }
}
