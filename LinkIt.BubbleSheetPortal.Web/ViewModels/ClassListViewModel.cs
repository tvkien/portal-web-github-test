using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ClassListViewModel
    {
        public int ID { get; set; }
        public int? UserID { get; set; }
        public string Name { get; set; }
        public string Term { get; set; }
        public string SchoolName { get; set; }
        public string Teachers { get; set; }
        public DateTime? TermStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public string PrimaryTeacher { get; set; }
        public bool Locked { get; set; }
        public string Students { get; set; }
        public int? SchoolID { get; set; }
        public string ModifiedBy { get; set; }
        public string ClassType { get; set; }
        public string Subjects { get; set; }
        public bool HasConfigClassMeta { get; set; }
    }
}
