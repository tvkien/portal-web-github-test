using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportRosterData
    {
        public string LocationCode{ get; set; }

        public int SchoolID{ get; set; }

        public string SchoolName{ get; set; }

        public int UserID{ get; set; }

        public string Username{ get; set; }

        public string Term{ get; set; }
        public int ClassID { get; set; }
        public string ClassName{ get; set; }

        public DateTime? ClassCreated{ get; set; }

        public string Course{ get; set; }

        public string Section{ get; set; }

        public string StudentCode { get; set; }

        public string StudentNameCustom { get; set; }

        public string Email{ get; set; }

        public int StudentID{ get; set; }

        public DateTime? StudentCreate{ get; set; }

        public DateTime? StudentModified{ get; set; }

        public string Gender{ get; set; }

        public string Race{ get; set; }

        public string Grade{ get; set; }

        public string ParentFirstName{ get; set; }

        public string ParentLastName{ get; set; }

        public string ParentEmail{ get; set; }

        public string ParentPhone{ get; set; }

        public string Program{ get; set; } 
    }
}