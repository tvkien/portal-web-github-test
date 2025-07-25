namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SchoolTeacherListViewModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ClassName { get; set; }
        public int SchoolID { get; set; }
        public string ClassID { get; set; }
        public bool? Active { get; set; }
        public string Action { get; set; }
    }
}