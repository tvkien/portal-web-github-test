namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentParentViewModel
    {
        public int ParentUserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public StudentParentViewModel()
        {
            Email = string.Empty;
            Phone = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
        }
    }
}