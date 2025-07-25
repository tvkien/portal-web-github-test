namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentInClassViewModel
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Code { get; set; }
        public string Grade { get; set; }
        public string Gender { get; set; }
        public bool CanAccess { get; set; }
    }
}