namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class AddStudentToParentRequestModel
    {
        public string StudentIds { get; set; }
        public int ParentUserId { get; set; }
        public string Relationship { get; set; }
        public bool StudentDataAccess { get; set; }
    }
}
