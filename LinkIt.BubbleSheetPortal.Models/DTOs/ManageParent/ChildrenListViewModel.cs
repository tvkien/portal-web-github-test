namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class ChildrenListViewModel
    {
        public string StudentFullName { get; set; }
        public int StudentId { get; set; }
        public string GradeName { get; set; }
        public string SchoolName { get; set; }
        public string Relationship { get; set; }
        public bool StudentDataAccess { get; set; }
        public int StudentParentId { get; set; }
    }
}
