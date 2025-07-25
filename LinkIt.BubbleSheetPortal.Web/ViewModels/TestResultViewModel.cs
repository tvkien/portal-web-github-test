using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestResultViewModel
    {
        public int TestResultID { get; set; }
        public DateTime ResultDate { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int DistrictTermID { get; set; }
        public string DistrictTermName { get; set; }
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int VirtualTestID { get; set; }
        public string TestName { get; set; }
    }
}